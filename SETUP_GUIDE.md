# 🚀 UDHËZUES I PLOTË - SIPM Backend Setup

## Hapat për të Startuar Projektin

### Hapi 1: Instalo .NET 8.0 SDK

1. Shko te: https://dotnet.microsoft.com/download/dotnet/8.0
2. Shkarko dhe instalo ".NET 8.0 SDK" për sistemin tënd
3. Verifiko instalimin:
```bash
dotnet --version
```
Duhet të tregojë: `8.0.x`

---

### Hapi 2: Instalo SQL Server

**Opsioni 1: SQL Server Express (Recommended për Development)**
- Shkarko: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- Instalo SQL Server 2022 Express
- Gjatë instalimit, zgjedh "Windows Authentication"

**Opsioni 2: SQL Server LocalDB (Më i lehtë)**
- Vjen automatikisht me Visual Studio
- Ose shkarko: https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

**Opsioni 3: SQL Server në Docker**
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 --name sql_server_2022 \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

**Verifiko SQL Server:**
- Hap SQL Server Management Studio (SSMS) ose Azure Data Studio
- Lidhu me: `localhost` (ose `(localdb)\MSSQLLocalDB`)

---

### Hapi 3: Klono/Krijo Projektin

```bash
# Nëse ke Git
git clone <repository-url>
cd SIPM_Backend_CSharp

# Ose krijo manualisht dosjen dhe kopjo të gjitha files
```

---

### Hapi 4: Konfiguro Connection String

Hap `appsettings.json` dhe përditëso connection string:

**Për Windows Authentication (Default):**
```json
"DefaultConnection": "Server=localhost;Database=SIPM_ARKIMEDO21;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

**Për SQL Server me Username/Password:**
```json
"DefaultConnection": "Server=localhost;Database=SIPM_ARKIMEDO21;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

**Për LocalDB:**
```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SIPM_ARKIMEDO21;Trusted_Connection=True;MultipleActiveResultSets=true"
```

---

### Hapi 5: Instalo NuGet Packages

```bash
cd SIPM_Backend_CSharp
dotnet restore
```

Kjo do të shkarkojë:
- Entity Framework Core 8.0
- SQL Server Provider
- Swashbuckle (Swagger)

---

### Hapi 6: Krijo Database (2 Mënyra)

**Mënyra 1: Automatike (EnsureCreated - Recommended për fillim)**
Kjo bëhet automatikisht kur starto API-në për herë të parë!
```bash
dotnet run
```

**Mënyra 2: Me Migrations (Professional Way)**
```bash
# Krijo migration
dotnet ef migrations add InitialCreate

# Apliko në database
dotnet ef database update
```

**Nëse nuk ke EF Tools:**
```bash
dotnet tool install --global dotnet-ef
```

---

### Hapi 7: Starto API-në

```bash
dotnet run
```

**Output-i duhet të jetë:**
```
🚀 SIPM ARKIMEDO-21 Backend API po fillon...
📍 API URL: https://localhost:5001
📖 Swagger UI: https://localhost:5001/swagger
🔗 Frontend: http://localhost:5500 (Live Server)
✅ Database u krijua ose ekziston tashmë
```

---

### Hapi 8: Testo API-në

**Në Browser:**
- Hap: https://localhost:5001/swagger
- Do të shohësh Swagger UI me të gjitha endpoints

**Test me Postman/Insomnia:**
1. GET Pajisjet:
```
GET https://localhost:5001/api/pajisje
```

2. POST Pajisje të re:
```
POST https://localhost:5001/api/pajisje
Content-Type: application/json

{
  "deviceId": "QSUT-TEST-001",
  "emri": "Test Pajisje",
  "kategoria": "Diagnostikë",
  "prodhues": "Test Manufacturer",
  "statusiTeknik": "Aktive"
}
```

3. GET Statistika:
```
GET https://localhost:5001/api/pajisje/stats
```

---

### Hapi 9: Integro me Frontend

1. **Starto Frontend-in (Live Server në VSCode)**
   - Hap `index.html` në VSCode
   - Kliko "Go Live" (Live Server extension)
   - Do të hapet në: http://localhost:5500 ose http://127.0.0.1:5500

2. **Zëvendëso script.js me script_updated.js**
```bash
cp Frontend_Integration/script_updated.js ../YourFrontendFolder/script.js
```

3. **Testo integrimin:**
   - Hap Frontend: http://localhost:5500
   - Bëj login si "teknik"
   - Krijo një Akt Konstatimi
   - Login si "inxhinier" dhe mbyllë aktin
   - Verifiko në Admin dashboard

---

## 📊 Testimi i Database-it

### Në SQL Server Management Studio (SSMS):

1. Lidhu me SQL Server
2. Zgjedh Database: `SIPM_ARKIMEDO21`
3. Ekzekuto queries:

```sql
-- Shiko tabelat
SELECT * FROM INFORMATION_SCHEMA.TABLES

-- Shiko pajisjet
SELECT * FROM Pajisje

-- Shiko aktet e hapura
SELECT * FROM AktKonstatimi WHERE Statusi = 'HAPUR'

-- Statistika
SELECT 
    StatusiTeknik,
    COUNT(*) as NumriPajisjeve,
    SUM(VleraBlerjes) as VleraTotal
FROM Pajisje
GROUP BY StatusiTeknik
```

---

## ⚠️ Troubleshooting

### Problem 1: "Unable to connect to SQL Server"
**Zgjidhje:**
1. Verifiko që SQL Server është duke punuar:
   - Services → SQL Server (MSSQLSERVER) → Start
2. Kontrollo connection string në `appsettings.json`
3. Provo: `Server=localhost;...` ose `Server=.;...`

### Problem 2: "CORS error" në browser
**Zgjidhje:**
- Verifiko që frontend URL është në `Program.cs`:
```csharp
policy.WithOrigins(
    "http://localhost:5500",
    "http://127.0.0.1:5500"
)
```

### Problem 3: "Port 5001 already in use"
**Zgjidhje:**
- Ndryshoni portin në `Properties/launchSettings.json`

### Problem 4: "Entity Framework Tools not found"
**Zgjidhje:**
```bash
dotnet tool install --global dotnet-ef --version 8.*
```

### Problem 5: Database nuk po krijohet
**Zgjidhje 1:** Provo manual:
```bash
dotnet ef database update
```

**Zgjidhje 2:** Krijo database manualisht:
```sql
CREATE DATABASE SIPM_ARKIMEDO21
```

---

## 🎯 Endpoints të Disponueshëm

### Pajisje
- `GET    /api/pajisje` - Lista e pajisjeve
- `GET    /api/pajisje/{id}` - Pajisje specifike
- `GET    /api/pajisje/device/{deviceId}` - Pajisje nga DeviceID
- `GET    /api/pajisje/stats` - Statistika
- `POST   /api/pajisje` - Krijo pajisje
- `PUT    /api/pajisje/{id}` - Përditëso
- `DELETE /api/pajisje/{id}` - Fshi

### Akt Konstatimi
- `GET    /api/aktkonstatimi` - Lista e akteve
- `GET    /api/aktkonstatimi/hapur` - Akte të hapura
- `GET    /api/aktkonstatimi/{id}` - Akt specifik
- `POST   /api/aktkonstatimi` - Krijo akt
- `PUT    /api/aktkonstatimi/{id}/mbyll` - Mbyll aktin
- `DELETE /api/aktkonstatimi/{id}` - Fshi

### Ndërhyrje
- `GET    /api/nderhyrje` - Lista e ndërhyrjeve
- `GET    /api/nderhyrje/aktive` - Ndërhyrje aktive
- `GET    /api/nderhyrje/{id}` - Ndërhyrje specifike
- `POST   /api/nderhyrje` - Krijo ndërhyrje
- `PUT    /api/nderhyrje/{id}` - Përditëso
- `PUT    /api/nderhyrje/{id}/perfundo` - Përfundo
- `DELETE /api/nderhyrje/{id}` - Fshi

---

## 📝 Shënime të Rëndësishme

1. **Siguria:** Në production, duhet të shtosh Authentication (JWT)
2. **HTTPS:** Certifikatat self-signed mund të japin warning në browser
3. **Seed Data:** Të dhënat fillestare ngarkohen automatikisht në startup
4. **Backup:** Bëj backup të database rregullisht

---

## 🎓 Për Projekt Final / Praktikë

Në raportin tënd, përfshi:

1. **Database Schema** (diagram i tabelave)
2. **API Documentation** (screenshots nga Swagger)
3. **CRUD Examples** (Postman requests/responses)
4. **Frontend Integration** (trego lidhjen me API)
5. **Code Samples** (Controllers, Models, DbContext)

---

## 📞 Kontakt

**Developer:** KRIZA  
**Company:** ONI sh.p.k.  
**Project:** SIPM ARKIMEDO-21  
**Institution:** QKTB / QSUT "Nënë Tereza"

---

**Good Luck! 🚀**
