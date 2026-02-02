# 🔗 SI TA INTEGROSH ME FRONTEND-IN TËND

## Hapi 1: Starto Backend API

```bash
cd SIPM_Backend_CSharp
dotnet restore
dotnet run
```

✅ API do të jetë në: **https://localhost:5001**  
✅ Swagger UI: **https://localhost:5001/swagger**

---

## Hapi 2: Zëvendëso `script.js`

Ke 2 opsione:

### Opsioni A: Zëvendëso Plotësisht
```bash
# Kopjo script_updated.js në projektin tënd
cp Frontend_Integration/script_updated.js ../YourFrontendFolder/script.js
```

### Opsioni B: Integro Manualisht (REKOMANDOHET)

Hap `script.js` aktual dhe **SHTO** këto në fillim:

```javascript
// ============================================
// API Configuration
// ============================================
const API_BASE_URL = 'https://localhost:5001/api';

// ============================================
// API Helper Function
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
```

---

## Hapi 3: Përditëso Funksionet Ekzistuese

### 3.1 Ngarko të Dhënat Fillestare

**PARA:**
```javascript
const saved = localStorage.getItem('constatations');
if (saved) {
    openConstatations = JSON.parse(saved);
}
```

**TANI:**
```javascript
async function loadInitialData() {
    try {
        const response = await apiRequest('/aktkonstatimi/hapur');
        if (response && response.success) {
            openConstatations = response.data;
        }
    } catch (error) {
        console.error('Error loading data:', error);
    }
}

// Thirre në DOMContentLoaded
document.addEventListener('DOMContentLoaded', async () => {
    await loadInitialData();
    // ... rest of code
});
```

---

### 3.2 Krijoni Akt Konstatimi (Tekniku)

**PARA:**
```javascript
function submitConstatation() {
    // ... krijonte në localStorage
    openConstatations.push(newConst);
    localStorage.setItem('constatations', JSON.stringify(openConstatations));
}
```

**TANI:**
```javascript
async function submitConstatation() {
    const deviceId = document.getElementById('device_id').value;
    const notes = document.getElementById('constatation_notes').value;
    const teknik = document.getElementById('username').value;

    if (!notes) {
        alert("Ju lutemi shkruani përshkrimin!");
        return;
    }

    // STEP 1: Merr pajisjen nga DeviceID
    const pajisjeResponse = await apiRequest(`/pajisje/device/${deviceId}`);
    if (!pajisjeResponse || !pajisjeResponse.success) {
        alert(`Pajisja "${deviceId}" nuk u gjet!`);
        return;
    }

    const pajisjeId = pajisjeResponse.data.id;

    // STEP 2: Krijo Akt Konstatimi
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
        
        // Përditëso
        await loadInitialData();
        updateInxhinierNotifications();
        updateAdminDashboard();
    }
}
```

---

### 3.3 Shfaq Njoftimet (Inxhinieri)

**PARA:**
```javascript
function updateInxhinierNotifications() {
    const openOnes = openConstatations.filter(c => c.status === 'HAPUR');
    // ... shfaqte nga localStorage
}
```

**TANI:**
```javascript
async function updateInxhinierNotifications() {
    const notificationsDiv = document.getElementById('inxhinier-notifications');
    notificationsDiv.innerHTML = '';

    // Merr nga API
    const response = await apiRequest('/aktkonstatimi/hapur');
    
    if (!response || !response.success) {
        notificationsDiv.innerHTML = '<p style="color: red;">Gabim në ngarkimin e të dhënave</p>';
        return;
    }

    const openOnes = response.data;

    if (openOnes.length === 0) {
        notificationsDiv.innerHTML = '<p style="color: green;">✅ Nuk ka akte të hapura</p>';
        return;
    }

    notificationsDiv.innerHTML = `<h3>⚠️ ${openOnes.length} DETYRA AKTIVE!</h3>`;

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
                    ✓ Kryej Riparimin
                </button>
            </div>
        `;
    });
}
```

---

### 3.4 Mbyll Aktin (Inxhinieri)

**PARA:**
```javascript
function completeRepair(constatationId) {
    openConstatations[index].status = 'MBYLLUR';
    localStorage.setItem('constatations', JSON.stringify(openConstatations));
}
```

**TANI:**
```javascript
async function completeRepair(aktId) {
    const inxhinier = prompt("Shkruani emrin tuaj (Inxhinier):");
    if (!inxhinier) return;

    const nota = prompt("Shkruani shënim për riparimin (optional):");

    const data = {
        mbyllurNga: inxhinier,
        notaMbylljes: nota || "Riparim i suksesshëm"
    };

    const response = await apiRequest(`/aktkonstatimi/${aktId}/mbyll`, 'PUT', data);

    if (response && response.success) {
        alert(`✅ ${response.message}`);
        
        // Përditëso
        await loadInitialData();
        updateInxhinierNotifications();
        updateAdminDashboard();
    }
}
```

---

### 3.5 Dashboard Statistikat (Admin)

**PARA:**
```javascript
function updateAdminDashboard() {
    const malfunctionCount = openConstatations.filter(c => c.status === 'HAPUR').length;
    const activeCount = TOTAL_DEVICES - malfunctionCount;
    // ...
}
```

**TANI:**
```javascript
async function updateAdminDashboard() {
    try {
        const response = await apiRequest('/pajisje/stats');
        
        if (!response || !response.success) {
            console.error('Error loading stats');
            return;
        }

        const stats = response.data;

        // Përditëso UI
        const activeSpan = document.getElementById('devices-active');
        const malSpan = document.getElementById('devices-malfunction');

        if (activeSpan) activeSpan.textContent = stats.pajisjeAktive;
        if (malSpan) malSpan.textContent = stats.pajisjeJoFunksionale;

        console.log('Dashboard Stats:', stats);
    } catch (error) {
        console.error('Error updating dashboard:', error);
    }
}
```

---

## Hapi 4: FSHI localStorage (Nuk të duhet më!)

**Fshi këto rreshta nga script.js i vjetër:**

```javascript
// ❌ FSHI këto
localStorage.setItem('constatations', JSON.stringify(openConstatations));
const saved = localStorage.getItem('constatations');
localStorage.removeItem('constatations');
```

---

## Hapi 5: Testo Integrimin

### Test 1: Login si Teknik
1. Hap frontend: `http://localhost:5500`
2. Login si **Teknik**
3. Krijo një Akt Konstatimi
4. Verifiko në Swagger: `GET /api/aktkonstatimi/hapur`

### Test 2: Login si Inxhinier
1. Login si **Inxhinier**
2. Duhet të shohësh aktin që krijove
3. Mbyllë aktin
4. Verifiko që zhduket nga lista

### Test 3: Login si Admin
1. Login si **Administrator**
2. Verifiko që statistikat janë të sakta
3. Kontrollo në Swagger: `GET /api/pajisje/stats`

---

## Hapi 6: Debugging (Nëse ka probleme)

### Problem 1: CORS Error
**Gabim:** `Access to fetch at 'https://localhost:5001' has been blocked by CORS policy`

**Zgjidhje:**
- Sigurohu që Backend-i është duke punuar: `dotnet run`
- Verifiko që frontend URL është në `Program.cs`:
```csharp
policy.WithOrigins(
    "http://localhost:5500",
    "http://127.0.0.1:5500"
)
```

### Problem 2: SSL Certificate Error
**Gabim:** `NET::ERR_CERT_AUTHORITY_INVALID`

**Zgjidhje 1:** Prano certifikatën në browser (unsafe por OK për development)
**Zgjidhje 2:** Instalo dev certificate:
```bash
dotnet dev-certs https --trust
```

### Problem 3: API nuk po kthen të dhëna
**Zgjidhje:**
- Hap Developer Tools (F12)
- Shiko Console për errors
- Shiko Network tab për API calls
- Verifiko që API URL është saktë: `https://localhost:5001/api`

---

## Hapi 7: Testing me Postman (Recommended)

Para se të integrosh me frontend, testo API-në në Postman:

### Test 1: GET Pajisjet
```
GET https://localhost:5001/api/pajisje
```

### Test 2: CREATE Akt Konstatimi
```
POST https://localhost:5001/api/aktkonstatimi
Content-Type: application/json

{
  "pajisjeId": 1,
  "pershkrimi": "Test dëmtim",
  "krijuarNga": "Teknik Test",
  "niveliUrgjences": "I lartë"
}
```

### Test 3: MBYLL Aktin
```
PUT https://localhost:5001/api/aktkonstatimi/1/mbyll
Content-Type: application/json

{
  "mbyllurNga": "Inxhinier Test",
  "notaMbylljes": "Riparim i suksesshëm"
}
```

---

## 🎯 Rezultati Final

Pas integrimit, do të kesh:

✅ **Frontend** (HTML/CSS/JS) → `http://localhost:5500`  
✅ **Backend API** (C#) → `https://localhost:5001`  
✅ **Database** (SQL Server) → `localhost`  

**Workflow:**
1. Teknik krijon Akt → **POST /api/aktkonstatimi**
2. Inxhinier shikon njoftimet → **GET /api/aktkonstatimi/hapur**
3. Inxhinier mbyll aktin → **PUT /api/aktkonstatimi/{id}/mbyll**
4. Admin shikon statistika → **GET /api/pajisje/stats**

---

## 🚀 Pro Tips

1. **Gjatë Zhvillimit:**
   - Mbaj Swagger UI hapur: `https://localhost:5001/swagger`
   - Përdor Browser Developer Tools (F12)
   - Testo endpoint në Postman para se në frontend

2. **Error Handling:**
   - Gjithmonë shfaq gabime në console: `console.error()`
   - Përdor try-catch në çdo async function
   - Trego mesazhe miqësore tek përdoruesi

3. **Performance:**
   - Thirr API-në vetëm kur duhet (jo në loop)
   - Ruaj rezultatet në variabla globale
   - Përdor loading indicators gjatë fetch

---

**GATI! Tani frontend dhe backend punojnë bashkë! 🎉🚀**
