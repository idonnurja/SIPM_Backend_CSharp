# SIPM ARKIMEDO-21 - Backend API (C# / ASP.NET Core)

## Projekti: Sistemi i Informacionit të Pajisjeve Mjekësore

### Teknologjitë e Përdorura
- **ASP.NET Core 8.0** Web API
- **Entity Framework Core 8.0** (ORM)
- **SQL Server** (Database)
- **CRUD Operations** të plota
- **RESTful API** Architecture

---

## Struktura e Projektit

```
SIPM_Backend/
├── Controllers/          # API Endpoints
├── Models/              # Entity Models (Database Tables)
├── Data/                # DbContext & Migrations
├── DTOs/                # Data Transfer Objects
├── Services/            # Business Logic
├── Program.cs           # Konfigurimi kryesor
└── appsettings.json     # Connection strings & configs
```

---

## Database Schema

### Tabelat Kryesore:
1. **Pajisje** - Regjistri qendror i pajisjeve
2. **AktKonstatimi** - Aktet e konstatimit (nga teknikët)
3. **Nderhyrje** - Riparimi/Mirëmbajtja
4. **Distributor** - Operatorët ekonomikë
5. **Vendor** - Furnitorët

---

## API Endpoints

### Pajisje (Devices)
- `GET /api/pajisje` - Merr të gjitha pajisjet
- `GET /api/pajisje/{id}` - Merr një pajisje specifike
- `POST /api/pajisje` - Krijon pajisje të re
- `PUT /api/pajisje/{id}` - Përditëson pajisjen
- `DELETE /api/pajisje/{id}` - Fshin pajisjen

### Akt Konstatimi
- `GET /api/aktkonstatimi` - Merr të gjitha aktet
- `GET /api/aktkonstatimi/hapur` - Merr aktet e hapura
- `POST /api/aktkonstatimi` - Krijon akt të ri
- `PUT /api/aktkonstatimi/{id}/mbyll` - Mbyll aktin

### Ndërhyrje (Interventions)
- `GET /api/nderhyrje` - Merr të gjitha ndërhyrjet
- `POST /api/nderhyrje` - Krijon ndërhyrje të re
- `PUT /api/nderhyrje/{id}` - Përditëson ndërhyrjen

---

## Si të fillosh projektin?

### 1. Instalo .NET 8.0 SDK
```bash
https://dotnet.microsoft.com/download/dotnet/8.0
```

### 2. Klono projektin dhe instaloj paketët
```bash
cd SIPM_Backend
dotnet restore
```

### 3. Përditëso Connection String
Në `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=SIPM_DB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 4. Krijo Database (Migrations)
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run API
```bash
dotnet run
```
API do të jetë në: `https://localhost:5001`

---

## Integrimi me Frontend

Në `script.js`, zëvendëso localStorage me API calls:

```javascript
// Para (localStorage):
localStorage.setItem('constatations', JSON.stringify(data));

// Tani (API):
fetch('https://localhost:5001/api/aktkonstatimi', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
});
```

---

## Testimi i API-së

### Me Postman ose Insomnia:
1. POST pajisje të re:
```json
POST https://localhost:5001/api/pajisje
{
  "deviceId": "QSUT-EKG-001",
  "emri": "Elektrokardiograf",
  "prodhues": "GE Healthcare",
  "statusi": "Aktive"
}
```

2. GET të gjitha pajisjet:
```
GET https://localhost:5001/api/pajisje
```

---

## Konfigurimi CORS (për frontend)

Në `Program.cs`, CORS është konfiguruar për të lejuar frontend-in tuaj:
```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", 
        policy => policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
                       .AllowAnyMethod()
                       .AllowAnyHeader());
});
```

---

## Autor
**KRIZA** - Computer Engineering Student @ Epoka University
Internship: ONI sh.p.k. - SIPM ARKIMEDO-21 Project

---

## Licensë
© 2025 QKTB / ONI sh.p.k.
