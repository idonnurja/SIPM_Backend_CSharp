// ============================================
// SIPM Frontend - Integruar me C# Backend API
// ============================================

// Konfigurimi i API URL
const API_BASE_URL = (window.location.protocol === 'https:' ? 'https://localhost:5001/api' : 'http://localhost:5000/api');

// Variablat globale
let currentRole = '';
let openConstatations = [];
let devicesCache = [];
let selectedAktId = null;
const TOTAL_DEVICES = 150;

// ============================================
// 1. Splash Screen & Login
// ============================================

document.addEventListener('DOMContentLoaded', async () => {
    const splashScreen = document.getElementById('splash-screen');
    const loginContainer = document.getElementById('login-form-container');
    const splashDuration = 3000;

    setTimeout(() => {
        splashScreen.style.opacity = '0';
        setTimeout(() => {
            splashScreen.style.display = 'none';
            loginContainer.style.display = 'flex';
        }, 500);
    }, splashDuration);

    // Ngarko të dhënat fillestare nga API
    await loadInitialData();
    await loadDevicesDatalist();
    wireDevicePreview();
});

// ============================================
// 2. Login Form
// ============================================

document.getElementById('login-form').addEventListener('submit', function(e) {
    e.preventDefault();
    const role = document.getElementById('role').value;
    currentRole = role;

    document.getElementById('login-form-container').style.display = 'none';
    document.getElementById('dashboard').style.display = 'block';
    document.getElementById('current-user-role').textContent = role.toUpperCase();

    showRoleApp(role);
});

function showRoleApp(role) {
    // Fsheh të gjitha
    document.getElementById('teknik-app').style.display = 'none';
    document.getElementById('inxhinier-app').style.display = 'none';
    document.getElementById('admin-app').style.display = 'none';

    // Shfaq sipas rolit
    if (role === 'teknik') {
        document.getElementById('teknik-app').style.display = 'block';
    } else if (role === 'inxhinier') {
        document.getElementById('inxhinier-app').style.display = 'block';
        updateInxhinierNotifications();
    } else if (role === 'administrator') {
        document.getElementById('admin-app').style.display = 'block';
        updateAdminDashboard();
    }
}

// ============================================
// 3. API Helper Functions
// ============================================

async function apiRequest(endpoint, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json',
        }
    };

    if (data) {
        options.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, options);
        const result = await response.json();

        if (!response.ok) {
            throw new Error(result.message || 'API Error');
        }

        return result;
    } catch (error) {
        console.error('API Request Error:', error);
        alert(`Gabim: ${error.message}`);
        return null;
    }
}

// ============================================
// 4. Ngarko të dhënat fillestare
// ============================================

async function loadInitialData() {
    try {
        // Ngarko aktet e hapura
        const response = await apiRequest('/aktkonstatimi/hapur');
        if (response && response.success) {
            openConstatations = response.data;
        }
    } catch (error) {
        console.error('Error loading initial data:', error);
    }
}

// Ngarko pajisjet dhe mbush datalist për Device ID
async function loadDevicesDatalist() {
    const list = document.getElementById('device_id_list');
    if (!list) return;

    const res = await apiRequest('/pajisje');
    if (!res || !res.success) return;

    devicesCache = res.data || [];
    list.innerHTML = '';

    devicesCache.forEach(p => {
        const opt = document.createElement('option');
        opt.value = p.deviceId;
        opt.label = `${p.deviceId} - ${p.emri}`;
        list.appendChild(opt);
    });
}

// Shfaq një preview të shpejtë kur zgjedh Device ID
function wireDevicePreview() {
    const inp = document.getElementById('device_id');
    const prev = document.getElementById('device-preview');
    if (!inp || !prev) return;

    const update = () => {
        const val = (inp.value || '').trim();
        if (!val) {
            prev.textContent = 'Zgjidh një pajisje nga lista (ngarkohet automatikisht).';
            return;
        }
        const found = devicesCache.find(d => (d.deviceId || '').toLowerCase() === val.toLowerCase());
        if (!found) {
            prev.textContent = 'Kujdes: Device ID nuk u gjet në listë. Kontrollo saktësinë.';
            return;
        }
        prev.textContent = `${found.emri} • ${found.kategoria || ''} • ${found.vendndodhja || ''}`.replace(/\s•\s$/,'');
    };

    inp.addEventListener('input', update);
    update();
}


// ============================================
// 5. Teknik - Krijon Akt Konstatimi
// ============================================

async function submitConstatation() {
    const deviceId = document.getElementById('device_id').value;
    const notes = document.getElementById('constatation_notes').value;
    const teknik = document.getElementById('username').value;

    if (!notes) {
        alert("Ju lutemi shkruani përshkrimin e dëmtimit!");
        return;
    }

    // Merr pajisjen nga DeviceID
    const pajisjeResponse = await apiRequest(`/pajisje/device/${deviceId}`);
    if (!pajisjeResponse || !pajisjeResponse.success) {
        alert(`Pajisja me Device ID "${deviceId}" nuk u gjet në sistem!`);
        return;
    }

    const pajisjeId = pajisjeResponse.data.id;

    // Krijo Akt Konstatimi
    const aktData = {
        pajisjeId: pajisjeId,
        pershkrimi: notes,
        krijuarNga: teknik,
        niveliUrgjences: "Mesatar"
    };

    const response = await apiRequest('/aktkonstatimi', 'POST', aktData);

    if (response && response.success) {
        alert(`✅ ${response.message}`);
        document.getElementById('constatation_notes').value = '';
        
        // Përditëso listen
        await loadInitialData();
    await loadDevicesDatalist();
    wireDevicePreview();
        updateInxhinierNotifications();
        updateAdminDashboard();
    }
}

// ============================================
// 6. Inxhinier - Shfaq Njoftimet
// ============================================

async function updateInxhinierNotifications() {
    const notificationsDiv = document.getElementById('inxhinier-notifications');
    notificationsDiv.innerHTML = '';

    // Ngarko aktet e hapura nga API
    const response = await apiRequest('/aktkonstatimi/hapur');
    
    if (!response || !response.success) {
        notificationsDiv.innerHTML = '<p style="color: red;">Gabim në ngarkimin e të dhënave</p>';
        return;
    }

    const openOnes = response.data;

    if (openOnes.length === 0) {
        notificationsDiv.innerHTML = '<p style="color: var(--success);"><i class="fas fa-check-circle"></i> Nuk ka Akt Konstatimi të hapur për momentin.</p>';
        return;
    }

    notificationsDiv.innerHTML = `<h3><i class="fas fa-exclamation-triangle"></i> ${openOnes.length} DETYRA AKTIVE!</h3>`;

    openOnes.forEach(akt => {
        notificationsDiv.innerHTML += `
            <div class="notification-item">
                <div>
                    <strong>Pajisja:</strong> ${akt.pajisje?.deviceId} - ${akt.pajisje?.emri}<br>
                    <strong>Konstatimi:</strong> ${akt.pershkrimi}<br>
                    <strong>Hapur nga:</strong> ${akt.krijuarNga}<br>
                    <strong>Data:</strong> ${new Date(akt.dataKrijimit).toLocaleDateString('sq-AL')}
                </div>
                <button class="btn-success" onclick="completeRepair(${akt.id})">
                    ✓ Kryej Riparimin / Mbyll Detyrën
                </button>
            </div>
        `;
    });
}

// ============================================
// 7. Inxhinier - Mbyll Aktin
// ============================================

function completeRepair(aktId) {
    openRepairModal(aktId);
}

function openRepairModal(aktId) {
    selectedAktId = aktId;
    const modal = document.getElementById('modal-repair');
    const name = document.getElementById('repair_inxhinier');
    const note = document.getElementById('repair_note');

    if (name) name.value = '';
    if (note) note.value = '';

    if (!modal) {
        // fallback
        const inxhinier = prompt("Shkruani emrin tuaj (Inxhinier):");
        if (!inxhinier) return;
        const nota = prompt("Shkruani një shënim për riparimin e kryer (optional):");
        return submitRepair(aktId, inxhinier, nota || "Riparim i suksesshëm");
    }

    modal.classList.remove('hidden');
    modal.setAttribute('aria-hidden', 'false');
    setTimeout(() => name && name.focus(), 30);
}

function closeRepairModal() {
    const modal = document.getElementById('modal-repair');
    if (!modal) return;
    modal.classList.add('hidden');
    modal.setAttribute('aria-hidden', 'true');
    selectedAktId = null;
}

async function submitRepairModal() {
    if (!selectedAktId) return;

    const inxhinier = (document.getElementById('repair_inxhinier')?.value || '').trim();
    const nota = (document.getElementById('repair_note')?.value || '').trim();

    if (!inxhinier) {
        showToast("Ju lutemi shkruani emrin e inxhinierit.", false);
        return;
    }

    await submitRepair(selectedAktId, inxhinier, nota || "Riparim i suksesshëm");
    closeRepairModal();
}

async function submitRepair(aktId, inxhinier, nota) {
    const data = { mbyllurNga: inxhinier, notaMbylljes: nota };

    const response = await apiRequest(`/aktkonstatimi/${aktId}/mbyll`, 'PUT', data);

    if (response && response.success) {
        showToast(response.message || "U mbyll me sukses.", true);

        // Përditëso
        await loadInitialData();
        updateInxhinierNotifications();
        updateAdminDashboard();
    }
}

// ============================================
// 8. Admin - Dashboard me Statistika
// ============================================

async function updateAdminDashboard() {
    try {
        // Merr statistikat nga API
        const response = await apiRequest('/pajisje/stats');
        
        if (!response || !response.success) {
            console.error('Error loading stats');
            return;
        }

        const stats = response.data;

        // Përditëso elementet
        const activeSpan = document.getElementById('devices-active');
        const malSpan = document.getElementById('devices-malfunction');

        if (activeSpan) activeSpan.textContent = stats.pajisjeAktive;
        if (malSpan) malSpan.textContent = stats.pajisjeJoFunksionale;

        // Mund të shtojmë më shumë statistika këtu
        console.log('Dashboard Stats:', stats);
    } catch (error) {
        console.error('Error updating admin dashboard:', error);
    }
}

// ============================================
// 9. Çkyçja
// ============================================

function logout() {
    currentRole = '';
    document.getElementById('dashboard').style.display = 'none';
    document.getElementById('login-form-container').style.display = 'flex';
}

// ============================================
// 10. Testing & Debug Functions
// ============================================

// Funksion për të testuar API-në
async function testAPI() {
    console.log('🧪 Duke testuar API...');
    
    // Test GET Pajisje
    const pajisjet = await apiRequest('/pajisje');
    console.log('Pajisjet:', pajisjet);
    
    // Test GET Stats
    const stats = await apiRequest('/pajisje/stats');
    console.log('Stats:', stats);
    
    // Test GET Akte Hapura
    const aktet = await apiRequest('/aktkonstatimi/hapur');
    console.log('Akte Hapura:', aktet);
}

// Thirre testAPI() në console për të testuar: testAPI()


// ============================================
// 11. UI Helpers (Toast + Loading)
// ============================================

function showToast(msg, ok = true) {
    const t = document.getElementById('toast');
    if (!t) { console.log(msg); return; }

    t.classList.remove('hidden');
    t.textContent = String(msg);
    t.style.borderColor = ok ? 'rgba(52,211,153,.55)' : 'rgba(255,91,107,.55)';

    clearTimeout(window.__toastTimer);
    window.__toastTimer = setTimeout(() => t.classList.add('hidden'), 2600);
}

function setLoading(on) {
    const l = document.getElementById('loading');
    if (!l) return;
    l.classList.toggle('hidden', !on);
}

// zëvendëso alert me toast (mos ndrysho logjikën e kodit)
const __nativeAlert = window.alert;
window.alert = (msg) => showToast(msg, !String(msg).toLowerCase().startsWith('gabim'));

// Wrap API request me loading
const __apiRequestOriginal = apiRequest;
apiRequest = async function(endpoint, method = 'GET', data = null) {
    setLoading(true);
    try {
        return await __apiRequestOriginal(endpoint, method, data);
    } finally {
        setLoading(false);
    }
};

// Mbylle modal-in me ESC
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') closeRepairModal();
});
