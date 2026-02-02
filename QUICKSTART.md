# ⚡ QUICK START - 5 Minuta Setup

## Hapat e Shpejtë për të Filluar

### 1️⃣ Instalo Requirements (Nëse nuk i ke)
```bash
# Verifiko .NET
dotnet --version
# Duhet: 8.0.x

# Nëse nuk ke, shkarko:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

---

### 2️⃣ Konfiguro SQL Server

**MËNYRA MË E LEHTË - SQL Server Express:**

1. Shkarko dhe instalo: https://go.microsoft.com/fwlink/p/?linkid=2216019
2. Gjatë instalimit, zgjedh "Basic"
3. Prit derisa të përfundojë (5-10 min)
4. ✅ Gati!

**Connection String:** (tashmë i konfiguruar në `appsettings.json`)
```
Server=localhost;Database=SIPM_ARKIMEDO21;Trusted_Connection=True;TrustServerCertificate=True;
```

---

### 3️⃣ Instalo Projektin

```bash
cd SIPM_Backend_CSharp
dotnet restore
```

---

### 4️⃣ RUN! 🚀

```bash
dotnet run
```

**Output:**
```
🚀 SIPM ARKIMEDO-21 Backend API po fillon...
📍 API URL: https://localhost:5001
📖 Swagger UI: https://localhost:5001/swagger
✅ Database u krijua ose ekziston tashmë
```

---

### 5️⃣ Testo API-në

**Në Browser:**
Hap: https://localhost:5001/swagger

**Test me curl:**
```bash
curl https://localhost:5001/api/pajisje
```

**Test me Postman:**
```
GET https://localhost:5001/api/pajisje
GET https://localhost:5001/api/pajisje/stats
GET https://localhost:5001/api/aktkonstatimi/hapur
```

---

## ✅ Gati! Tani integro me Frontend

### Përditëso `script.js`:

1. Kopjo kodin nga `Frontend_Integration/script_updated.js`
2. Zëvendëso në frontend-in tënd
3. Starto Live Server (VSCode): http://localhost:5500
4. Login dhe testo!

---

## 🧪 Test Endpoints

### CREATE Pajisje të Re
```bash
POST https://localhost:5001/api/pajisje
Content-Type: application/json

{
  "deviceId": "QSUT-TEST-999",
  "emri": "Test Equipment",
  "kategoria": "Diagnostikë",
  "prodhues": "Test Inc",
  "statusiTeknik": "Aktive"
}
```

### CREATE Akt Konstatimi
```bash
POST https://localhost:5001/api/aktkonstatimi
Content-Type: application/json

{
  "pajisjeId": 1,
  "pershkrimi": "Dëmtim në ekran",
  "krijuarNga": "Teknik Test",
  "niveliUrgjences": "I lartë"
}
```

### GET Statistika
```bash
GET https://localhost:5001/api/pajisje/stats
```

---

## 🔥 Pro Tips

1. **CORS Error?** 
   - Sigurohu që frontend-i të jetë në `http://localhost:5500`
   
2. **Database nuk po krijohet?**
   ```bash
   # Manual create:
   dotnet ef database update
   ```

3. **Port conflict?**
   - Ndrysho portin në `Properties/launchSettings.json`

4. **Swagger nuk po hapet?**
   - Provo: https://localhost:5001/ (direktpërdrejt)

---

## 📱 Next Steps

1. ✅ Testo të gjithë CRUD operations
2. ✅ Integro me frontend
3. ✅ Shtoni më shumë pajisje test
4. ✅ Krijo akt konstatimi dhe mbylleni
5. ✅ Verifiko dashboard statistikat

---

## 🆘 Ndihmë?

```bash
# Nëse ka problem me database:
dotnet ef migrations add InitialCreate
dotnet ef database update

# Nëse duhet të rifillosh nga 0:
dotnet ef database drop
dotnet run
```

---

## 🎯 Rezultati Final

✅ Backend API funksionon në `https://localhost:5001`  
✅ Database është krijuar me 3 pajisje test  
✅ Swagger UI disponueshme për testing  
✅ CORS konfiguruar për frontend  
✅ CRUD operations 100% funksionale  

---

**That's it! Enjoy coding! 🎉**
