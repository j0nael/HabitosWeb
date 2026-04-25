const API = '';
const FREQ = ['Diario', 'Semanal', 'Mensual'];
let dashFecha = new Date();
let chartMensual = null;


const $ = id => document.getElementById(id);
const fmt = d => new Date(d).toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' });
const fmtISO = d => { const dt = new Date(d); const y = dt.getFullYear(); const m = String(dt.getMonth()+1).padStart(2,'0'); const day = String(dt.getDate()).padStart(2,'0'); return `${y}-${m}-${day}`; };

function toast(msg, type='success') {
    const t = $('toast');
    t.textContent = msg;
    t.className = `alert-toast ${type}`;
    t.style.display = 'block';
    setTimeout(() => t.style.display = 'none', 3000);
}

async function api(method, url, body=null) {
    const opts = { method, headers: { 'Content-Type': 'application/json' } };
    if (body) opts.body = JSON.stringify(body);
    const res = await fetch(API + url, opts);
    if (!res.ok) {
        const err = await res.text();
        throw new Error(err || `Error ${res.status}`);
    }
    if (res.status === 204) return null;
    return res.json();
}


function nav(sec) {
    document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
    document.querySelectorAll('#sidebar nav a').forEach(a => a.classList.remove('active'));
    $(`sec-${sec}`).classList.add('active');
    $(`nav-${sec}`).classList.add('active');
    const titles = { dashboard:'Dashboard', usuarios:'Usuarios', categorias:'Categorías', habitos:'Hábitos', registros:'Registros', metas:'Metas', estadisticas:'Estadísticas' };
    $('page-title').textContent = titles[sec];
    if (sec === 'dashboard') loadDashboard();
    if (sec === 'usuarios') loadUsuarios();
    if (sec === 'categorias') loadCategorias();
    if (sec === 'habitos') loadHabitos();
    if (sec === 'registros') loadRegistros();
    if (sec === 'metas') loadMetas();
    if (sec === 'estadisticas') initEstadisticas();
    return false;
}


function cambiarFecha(delta) {
    dashFecha.setDate(dashFecha.getDate() + delta);
    loadDashboard();
}

async function loadDashboard() {
    $('fecha-display').textContent = dashFecha.toLocaleDateString('es-ES', { weekday:'short', day:'2-digit', month:'short' });

    const usuId = $('dash-usuario').value;
    if (!usuId) {
        $('dash-habitos-list').innerHTML = '<p style="color:#64748b;font-size:13px;text-align:center;padding:16px;">Selecciona un usuario para ver sus hábitos</p>';
        resetDashStats();
        return;
    }

    try {
        const [stats, habitos, metas] = await Promise.all([
            api('GET', `/api/estadisticas/usuario/${usuId}`),
            api('GET', `/api/registro/usuario/${usuId}/fecha/${fmtISO(dashFecha)}`),
            api('GET', `/api/meta/usuario/${usuId}`)
        ]);

        $('dash-habitos-activos').textContent = stats.habitosActivos;
        $('dash-completados-hoy').textContent = stats.registrosHoy;
        $('dash-racha').textContent = `${stats.rachaActual} días`;
        $('dash-tasa').textContent = `${stats.tasaCumplimientoMes}%`;

        renderHabitosHoy(habitos);
        renderMetasDash(metas);
    } catch(e) {
        toast(e.message, 'error');
    }
}

function resetDashStats() {
    ['dash-habitos-activos','dash-completados-hoy','dash-racha','dash-tasa'].forEach(id => $(id).textContent = '—');
    $('dash-metas-list').innerHTML = '';
}

function renderHabitosHoy(habitos) {
    const c = $('dash-habitos-list');
    if (!habitos.length) { c.innerHTML = '<p style="color:#64748b;font-size:13px;text-align:center;padding:8px;">No hay hábitos activos</p>'; return; }
    c.innerHTML = habitos.map(h => `
        <div class="habit-item" id="hitem-${h.id}">
            <div class="habit-check ${h.registro?.completado ? 'done' : ''}" onclick="toggleHabito(${h.id}, ${h.registro?.id || 0}, ${!!(h.registro?.completado)})">
                ${h.registro?.completado ? '<i class="bi bi-check-lg" style="font-size:13px;"></i>' : ''}
            </div>
            <div class="habit-dot" style="background:${h.color || '#6366f1'};"></div>
            <div style="flex:1;">
                <div style="font-weight:600;font-size:14px;">${h.icono || ''} ${h.nombre}</div>
                <div style="font-size:12px;color:#64748b;">${FREQ[h.frecuencia] || 'Diario'}</div>
            </div>
            ${h.registro ? `<span class="badge-${h.registro.completado ? 'completado' : 'pendiente'}">${h.registro.completado ? 'Hecho' : 'Pendiente'}</span>` : '<span class="badge-pendiente">Sin registro</span>'}
        </div>
    `).join('');
}

function renderMetasDash(metas) {
    const c = $('dash-metas-list');
    const activas = metas.filter(m => !m.completada).slice(0, 5);
    if (!activas.length) { c.innerHTML = '<p style="color:#64748b;font-size:13px;text-align:center;">No hay metas activas</p>'; return; }
    c.innerHTML = activas.map(m => {
        const pct = Math.min(100, Math.round((m.progreso / m.objetivoDias) * 100));
        return `
        <div>
            <div style="display:flex;justify-content:space-between;margin-bottom:4px;">
                <span style="font-size:13px;font-weight:600;">${m.nombre}</span>
                <span style="font-size:12px;color:#64748b;">${m.progreso}/${m.objetivoDias} días</span>
            </div>
            <div class="progress-bar-custom"><div class="progress-bar-fill" style="width:${pct}%"></div></div>
            <div style="font-size:11px;color:#64748b;margin-top:3px;">${m.habitoNombre} · ${pct}%</div>
        </div>`;
    }).join('');
}

async function toggleHabito(habitoId, registroId, estaCompletado) {
    const fecha = fmtISO(dashFecha);
    try {
        if (registroId) {
            await api('PUT', `/api/registro/${registroId}`, { id: registroId, habitoId, fecha, completado: !estaCompletado, nota: null });
        } else {
            await api('POST', '/api/registro', { habitoId, fecha, completado: true, nota: null });
        }
        loadDashboard();
    } catch(e) { toast(e.message, 'error'); }
}


async function loadUsuarios() {
    try {
        const data = await api('GET', '/api/usuario');
        $('tabla-usuarios').innerHTML = data.map(u => `
            <tr>
                <td>${u.id}</td>
                <td style="font-weight:600;">${u.nombre}</td>
                <td>${u.email}</td>
                <td>${fmt(u.fechaRegistro)}</td>
                <td><span class="badge-${u.activo ? 'activo' : 'inactivo'}">${u.activo ? 'Activo' : 'Inactivo'}</span></td>
                <td>
                    <button class="btn-icon" onclick="editUsuario(${u.id})"><i class="bi bi-pencil"></i></button>
                    <button class="btn-icon danger" onclick="deleteUsuario(${u.id})" style="margin-left:4px;"><i class="bi bi-trash"></i></button>
                </td>
            </tr>`).join('');
    } catch(e) { toast(e.message, 'error'); }
}

function openModal(type) {
    if (type === 'usuario') { $('usu-id').value=''; $('usu-nombre').value=''; $('usu-email').value=''; $('usu-pass').value=''; $('usu-activo').value='true'; $('modal-usuario-title').textContent='Nuevo Usuario'; }
    if (type === 'categoria') { $('cat-id').value=''; $('cat-nombre').value=''; $('modal-categoria-title').textContent='Nueva Categoría'; }
    if (type === 'habito') { resetHabitoModal(); }
    if (type === 'registro') { resetRegistroModal(); }
    if (type === 'meta') { resetMetaModal(); }
    $(`modal-${type}`).classList.add('show');
}

function closeModal(type) { $(`modal-${type}`).classList.remove('show'); }

async function saveUsuario() {
    const id = $('usu-id').value;
    const body = { id: id ? +id : 0, nombre: $('usu-nombre').value, email: $('usu-email').value, passwordHash: $('usu-pass').value, activo: $('usu-activo').value === 'true' };
    try {
        if (id) await api('PUT', `/api/usuario/${id}`, body);
        else await api('POST', '/api/usuario', body);
        closeModal('usuario');
        loadUsuarios();
        populateSelects();
        toast(id ? 'Usuario actualizado' : 'Usuario creado');
    } catch(e) { toast(e.message, 'error'); }
}

async function editUsuario(id) {
    try {
        const u = await api('GET', `/api/usuario/${id}`);
        $('usu-id').value = u.id;
        $('usu-nombre').value = u.nombre;
        $('usu-email').value = u.email;
        $('usu-pass').value = u.passwordHash;
        $('usu-activo').value = String(u.activo);
        $('modal-usuario-title').textContent = 'Editar Usuario';
        $('modal-usuario').classList.add('show');
    } catch(e) { toast(e.message, 'error'); }
}

async function deleteUsuario(id) {
    if (!confirm('¿Eliminar este usuario?')) return;
    try { await api('DELETE', `/api/usuario/${id}`); loadUsuarios(); populateSelects(); toast('Usuario eliminado'); }
    catch(e) { toast(e.message, 'error'); }
}


async function loadCategorias() {
    try {
        const [cats, habitos] = await Promise.all([api('GET', '/api/categoriahabito'), api('GET', '/api/habito')]);
        $('tabla-categorias').innerHTML = cats.map(c => {
            const count = habitos.filter(h => h.categoriaHabitoId === c.id).length;
            return `<tr>
                <td>${c.id}</td>
                <td style="font-weight:600;">${c.nombre}</td>
                <td>${count} hábito${count !== 1 ? 's' : ''}</td>
                <td>
                    <button class="btn-icon" onclick="editCategoria(${c.id})"><i class="bi bi-pencil"></i></button>
                    <button class="btn-icon danger" onclick="deleteCategoria(${c.id})" style="margin-left:4px;"><i class="bi bi-trash"></i></button>
                </td>
            </tr>`;
        }).join('');
    } catch(e) { toast(e.message, 'error'); }
}

async function saveCategoria() {
    const id = $('cat-id').value;
    const body = { id: id ? +id : 0, nombre: $('cat-nombre').value };
    try {
        if (id) await api('PUT', `/api/categoriahabito/${id}`, body);
        else await api('POST', '/api/categoriahabito', body);
        closeModal('categoria');
        loadCategorias();
        populateSelects();
        toast(id ? 'Categoría actualizada' : 'Categoría creada');
    } catch(e) { toast(e.message, 'error'); }
}

async function editCategoria(id) {
    try {
        const c = await api('GET', `/api/categoriahabito/${id}`);
        $('cat-id').value = c.id;
        $('cat-nombre').value = c.nombre;
        $('modal-categoria-title').textContent = 'Editar Categoría';
        $('modal-categoria').classList.add('show');
    } catch(e) { toast(e.message, 'error'); }
}

async function deleteCategoria(id) {
    if (!confirm('¿Eliminar esta categoría?')) return;
    try { await api('DELETE', `/api/categoriahabito/${id}`); loadCategorias(); populateSelects(); toast('Categoría eliminada'); }
    catch(e) { toast(e.message, 'error'); }
}


async function loadHabitos() {
    try {
        const data = await api('GET', '/api/habito');
        $('tabla-habitos').innerHTML = data.map(h => `
            <tr>
                <td>${h.id}</td>
                <td>
                    <div style="display:flex;align-items:center;gap:8px;">
                        <span style="width:10px;height:10px;border-radius:50%;background:${h.color||'#6366f1'};display:inline-block;"></span>
                        <strong>${h.icono || ''} ${h.nombre}</strong>
                    </div>
                </td>
                <td><span class="freq-badge freq-${h.frecuencia}">${FREQ[h.frecuencia]}</span></td>
                <td>${h.categoriaNombre || '—'}</td>
                <td>${h.usuarioNombre || '—'}</td>
                <td>${h.horaRecordatorio || '—'}</td>
                <td><span class="badge-${h.activo ? 'activo' : 'inactivo'}">${h.activo ? 'Activo' : 'Inactivo'}</span></td>
                <td>
                    <button class="btn-icon" onclick="editHabito(${h.id})"><i class="bi bi-pencil"></i></button>
                    <button class="btn-icon danger" onclick="deleteHabito(${h.id})" style="margin-left:4px;"><i class="bi bi-trash"></i></button>
                </td>
            </tr>`).join('');
    } catch(e) { toast(e.message, 'error'); }
}

async function resetHabitoModal() {
    $('hab-id').value = '';
    $('hab-nombre').value = '';
    $('hab-descripcion').value = '';
    $('hab-frecuencia').value = '0';
    $('hab-hora').value = '';
    $('hab-color').value = '#6366f1';
    $('hab-icono').value = '';
    $('hab-activo').value = 'true';
    $('modal-habito-title').textContent = 'Nuevo Hábito';
    await fillSelect('hab-usuario', '/api/usuario', 'id', 'nombre');
    await fillSelect('hab-categoria', '/api/categoriahabito', 'id', 'nombre');
}

async function saveHabito() {
    const id = $('hab-id').value;
    const body = {
        id: id ? +id : 0,
        nombre: $('hab-nombre').value,
        descripcion: $('hab-descripcion').value || null,
        frecuencia: +$('hab-frecuencia').value,
        horaRecordatorio: $('hab-hora').value || null,
        activo: $('hab-activo').value === 'true',
        color: $('hab-color').value,
        icono: $('hab-icono').value || null,
        usuarioId: +$('hab-usuario').value,
        categoriaHabitoId: +$('hab-categoria').value,
        fechaCreacion: new Date().toISOString()
    };
    try {
        if (id) await api('PUT', `/api/habito/${id}`, body);
        else await api('POST', '/api/habito', body);
        closeModal('habito');
        loadHabitos();
        populateSelects();
        toast(id ? 'Hábito actualizado' : 'Hábito creado');
    } catch(e) { toast(e.message, 'error'); }
}

async function editHabito(id) {
    try {
        const h = await api('GET', `/api/habito/${id}`);
        await resetHabitoModal();
        $('hab-id').value = h.id;
        $('hab-nombre').value = h.nombre;
        $('hab-descripcion').value = h.descripcion || '';
        $('hab-frecuencia').value = h.frecuencia;
        $('hab-hora').value = h.horaRecordatorio || '';
        $('hab-color').value = h.color || '#6366f1';
        $('hab-icono').value = h.icono || '';
        $('hab-activo').value = String(h.activo);
        $('hab-usuario').value = h.usuarioId;
        $('hab-categoria').value = h.categoriaHabitoId;
        $('modal-habito-title').textContent = 'Editar Hábito';
        $('modal-habito').classList.add('show');
    } catch(e) { toast(e.message, 'error'); }
}

async function deleteHabito(id) {
    if (!confirm('¿Eliminar este hábito?')) return;
    try { await api('DELETE', `/api/habito/${id}`); loadHabitos(); toast('Hábito eliminado'); }
    catch(e) { toast(e.message, 'error'); }
}


async function loadRegistros() {
    try {
        const data = await api('GET', '/api/registro');
        const filtroUsu = $('filtro-usuario-registro').value;
        const filtroFecha = $('filtro-fecha-registro').value;
        let filtered = data;
        if (filtroUsu) filtered = filtered.filter(r => String(r.usuarioId) === filtroUsu);
        if (filtroFecha) filtered = filtered.filter(r => fmtISO(r.fecha) === filtroFecha);
        $('tabla-registros').innerHTML = filtered.map(r => `
            <tr>
                <td>${r.id}</td>
                <td><strong>${r.habitoNombre || '—'}</strong></td>
                <td>${fmt(r.fecha)}</td>
                <td><span class="badge-${r.completado ? 'completado' : 'pendiente'}">${r.completado ? 'Completado' : 'No completado'}</span></td>
                <td style="color:#64748b;font-size:12px;">${r.nota || '—'}</td>
                <td>
                    <button class="btn-icon" onclick="editRegistro(${r.id})"><i class="bi bi-pencil"></i></button>
                    <button class="btn-icon danger" onclick="deleteRegistro(${r.id})" style="margin-left:4px;"><i class="bi bi-trash"></i></button>
                </td>
            </tr>`).join('');
    } catch(e) { toast(e.message, 'error'); }
}

async function resetRegistroModal() {
    $('reg-id').value = '';
    $('reg-fecha').value = fmtISO(new Date());
    $('reg-completado').value = 'true';
    $('reg-nota').value = '';
    $('modal-registro-title').textContent = 'Nuevo Registro';
    await fillSelect('reg-habito', '/api/habito', 'id', 'nombre');
}

async function saveRegistro() {
    const id = $('reg-id').value;
    const body = { id: id ? +id : 0, habitoId: +$('reg-habito').value, fecha: $('reg-fecha').value, completado: $('reg-completado').value === 'true', nota: $('reg-nota').value || null };
    try {
        if (id) await api('PUT', `/api/registro/${id}`, body);
        else await api('POST', '/api/registro', body);
        closeModal('registro');
        loadRegistros();
        toast(id ? 'Registro actualizado' : 'Registro creado');
    } catch(e) { toast(e.message, 'error'); }
}

async function editRegistro(id) {
    try {
        const r = await api('GET', `/api/registro/${id}`);
        await resetRegistroModal();
        $('reg-id').value = r.id;
        $('reg-habito').value = r.habitoId;
        $('reg-fecha').value = fmtISO(r.fecha);
        $('reg-completado').value = String(r.completado);
        $('reg-nota').value = r.nota || '';
        $('modal-registro-title').textContent = 'Editar Registro';
        $('modal-registro').classList.add('show');
    } catch(e) { toast(e.message, 'error'); }
}

async function deleteRegistro(id) {
    if (!confirm('¿Eliminar este registro?')) return;
    try { await api('DELETE', `/api/registro/${id}`); loadRegistros(); toast('Registro eliminado'); }
    catch(e) { toast(e.message, 'error'); }
}


async function loadMetas() {
    try {
        const data = await api('GET', '/api/meta');
        $('tabla-metas').innerHTML = data.map(m => {
            const pct = Math.min(100, Math.round((m.progreso / m.objetivoDias) * 100));
            return `<tr>
                <td>${m.id}</td>
                <td style="font-weight:600;">${m.nombre}</td>
                <td>${m.habitoNombre || '—'}</td>
                <td>${m.usuarioNombre || '—'}</td>
                 <td>${m.habitoId || '—'}</td>
                <td style="font-size:12px;color:#64748b;">${fmt(m.fechaInicio)} → ${fmt(m.fechaFin)}</td>
                <td style="text-align:center;">${m.objetivoDias} días</td>
                <td style="min-width:120px;">
                    <div class="progress-bar-custom" style="margin-bottom:3px;"><div class="progress-bar-fill" style="width:${pct}%"></div></div>
                    <div style="font-size:11px;color:#64748b;">${m.progreso}/${m.objetivoDias} · ${pct}%</div>
                </td>
                <td><span class="badge-${m.completada ? 'completado' : 'pendiente'}">${m.completada ? 'Completada' : 'En progreso'}</span></td>
                <td>
                    ${!m.completada ? `<button class="btn-icon" title="Marcar completada" onclick="completarMeta(${m.id})" style="margin-right:4px;"><i class="bi bi-check2-all"></i></button>` : ''}
                    <button class="btn-icon" onclick="editMeta(${m.id})"><i class="bi bi-pencil"></i></button>
                    <button class="btn-icon danger" onclick="deleteMeta(${m.id})" style="margin-left:4px;"><i class="bi bi-trash"></i></button>
                </td>
            </tr>`;
        }).join('');
    } catch(e) { toast(e.message, 'error'); }
}

async function resetMetaModal() {
    $('meta-id').value = '';
    $('meta-nombre').value = '';
    $('meta-descripcion').value = '';
    $('meta-inicio').value = fmtISO(new Date());
    $('meta-fin').value = '';
    $('meta-objetivo').value = '';
    $('modal-meta-title').textContent = 'Nueva Meta';
    await fillSelect('meta-usuario', '/api/usuario', 'id', 'nombre', true);
    $('meta-habito').innerHTML = '<option value="">Selecciona un hábito...</option>';
}

async function loadHabitosPorUsuario(targetId, usuarioId) {
    if (!usuarioId) { $(targetId).innerHTML = '<option value="">Selecciona un hábito...</option>'; return; }
    try {
        const data = await api('GET', `/api/habito/usuario/${usuarioId}`);
        $(targetId).innerHTML = data.map(h => `<option value="${h.id}">${h.nombre}</option>`).join('');
    } catch(e) { $(targetId).innerHTML = '<option value="">Error al cargar</option>'; }
}

async function saveMeta() {
    const id = $('meta-id').value;
    const body = { id: id ? +id : 0, nombre: $('meta-nombre').value, descripcion: $('meta-descripcion').value || null, habitoId: +$('meta-habito').value, usuarioId: +$('meta-usuario').value, fechaInicio: $('meta-inicio').value, fechaFin: $('meta-fin').value, objetivoDias: +$('meta-objetivo').value, completada: false };
    try {
        if (id) await api('PUT', `/api/meta/${id}`, body);
        else await api('POST', '/api/meta', body);
        closeModal('meta');
        loadMetas();
        toast(id ? 'Meta actualizada' : 'Meta creada');
    } catch(e) { toast(e.message, 'error'); }
}

async function editMeta(id) {
    try {
        const m = await api('GET', `/api/meta/${id}`);
        await resetMetaModal();
        $('meta-id').value = m.id;
        $('meta-nombre').value = m.nombre;
        $('meta-descripcion').value = m.descripcion || '';
        $('meta-usuario').value = m.usuarioId;
        await loadHabitosPorUsuario('meta-habito', m.usuarioId);
        $('meta-habito').value = m.habitoId;
        $('meta-inicio').value = fmtISO(m.fechaInicio);
        $('meta-fin').value = fmtISO(m.fechaFin);
        $('meta-objetivo').value = m.objetivoDias;
        $('modal-meta-title').textContent = 'Editar Meta';
        $('modal-meta').classList.add('show');
    } catch(e) { toast(e.message, 'error'); }
}

async function completarMeta(id) {
    if (!confirm('¿Marcar esta meta como completada?')) return;
    try { await api('PUT', `/api/meta/${id}/completar`); loadMetas(); toast('Meta completada'); }
    catch(e) { toast(e.message, 'error'); }
}

async function deleteMeta(id) {
    if (!confirm('¿Eliminar esta meta?')) return;
    try { await api('DELETE', `/api/meta/${id}`); loadMetas(); toast('Meta eliminada'); }
    catch(e) { toast(e.message, 'error'); }
}


async function initEstadisticas() {
    await fillSelect('est-usuario', '/api/usuario', 'id', 'nombre', true);
}

async function loadEstadisticas() {
    const usuId = $('est-usuario').value;
    if (!usuId) { $('est-contenido').style.display = 'none'; return; }
    try {
        const stats = await api('GET', `/api/estadisticas/usuario/${usuId}`);
        $('est-contenido').style.display = 'block';
        $('est-habitos-activos').textContent = stats.habitosActivos;
        $('est-racha').textContent = `${stats.rachaActual} días`;
        $('est-tasa').textContent = `${stats.tasaCumplimientoMes}%`;
        $('est-metas').textContent = `${stats.metasCompletadas}/${stats.metasTotales}`;
        await fillSelect('est-habito-sel', `/api/habito/usuario/${usuId}`, 'id', 'nombre', true);
    } catch(e) { toast(e.message, 'error'); }
}

async function loadHabitoStats() {
    const habId = $('est-habito-sel').value;
    if (!habId) { $('est-habito-stats').innerHTML = ''; destroyChart(); return; }
    try {
        const stats = await api('GET', `/api/estadisticas/habito/${habId}`);
        $('est-habito-stats').innerHTML = `
            <div class="row g-2 mb-3">
                <div class="col-6"><div style="background:#f8fafc;border-radius:8px;padding:12px;text-align:center;"><div style="font-size:22px;font-weight:800;color:#6366f1;">${stats.completados}</div><div style="font-size:11px;color:#64748b;">Completados</div></div></div>
                <div class="col-6"><div style="background:#f8fafc;border-radius:8px;padding:12px;text-align:center;"><div style="font-size:22px;font-weight:800;color:#ef4444;">${stats.noCompletados}</div><div style="font-size:11px;color:#64748b;">No Completados</div></div></div>
                <div class="col-6"><div style="background:#f8fafc;border-radius:8px;padding:12px;text-align:center;"><div style="font-size:22px;font-weight:800;color:#ea580c;">${stats.rachaActual}</div><div style="font-size:11px;color:#64748b;">Racha Actual</div></div></div>
                <div class="col-6"><div style="background:#f8fafc;border-radius:8px;padding:12px;text-align:center;"><div style="font-size:22px;font-weight:800;color:#16a34a;">${stats.tasaCumplimiento}%</div><div style="font-size:11px;color:#64748b;">Cumplimiento</div></div></div>
            </div>`;
        renderChart(stats.historicoPorMes);
    } catch(e) { toast(e.message, 'error'); }
}

function destroyChart() { if (chartMensual) { chartMensual.destroy(); chartMensual = null; } }

function renderChart(data) {
    destroyChart();
    if (!data || !data.length) return;
    const ctx = $('chartMensual').getContext('2d');
    chartMensual = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.map(d => d.mes),
            datasets: [
                { label: 'Completados', data: data.map(d => d.completados), backgroundColor: '#6366f1', borderRadius: 4 },
                { label: 'Total', data: data.map(d => d.total), backgroundColor: '#e2e8f0', borderRadius: 4 }
            ]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            plugins: { legend: { position: 'bottom', labels: { font: { size: 11 } } } },
            scales: { x: { grid: { display: false } }, y: { beginAtZero: true, ticks: { stepSize: 1 } } }
        }
    });
}


async function fillSelect(selectId, endpoint, valueField, labelField, addEmpty=false) {
    try {
        const data = await api('GET', endpoint);
        const sel = $(selectId);
        sel.innerHTML = (addEmpty ? '<option value="">Selecciona...</option>' : '') + data.map(d => `<option value="${d[valueField]}">${d[labelField]}</option>`).join('');
    } catch(e) {  }
}

async function populateSelects() {
    const usuarios = await api('GET', '/api/usuario').catch(() => []);
    const opts = usuarios.map(u => `<option value="${u.id}">${u.nombre}</option>`).join('');
    const emptyOpt = '<option value="">Todos los usuarios</option>';
    ['dash-usuario', 'filtro-usuario-registro'].forEach(id => {
        const el = $(id);
        if (el) el.innerHTML = (id === 'filtro-usuario-registro' ? emptyOpt : '<option value="">Selecciona un usuario...</option>') + opts;
    });
}


async function init() {
    const hoy = new Date();
    $('fecha-hoy').textContent = hoy.toLocaleDateString('es-ES', { weekday:'long', year:'numeric', month:'long', day:'numeric' });
    $('fecha-display').textContent = hoy.toLocaleDateString('es-ES', { weekday:'short', day:'2-digit', month:'short' });
    $('filtro-fecha-registro').value = fmtISO(hoy);
    await populateSelects();
    loadDashboard();
}

document.addEventListener('DOMContentLoaded', init);
