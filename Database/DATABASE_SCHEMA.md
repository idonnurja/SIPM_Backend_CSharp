# 📊 SIPM Database Schema Documentation

## Database Overview

**Database Name:** SIPM_ARKIMEDO21  
**DBMS:** SQL Server 2019+  
**Character Set:** Unicode (NVARCHAR)  
**Collation:** SQL_Latin1_General_CP1_CI_AS

---

## 📋 Tabelat (Tables)

### 1. Pajisje (Medical Devices)
Tabela kryesore që ruan të gjitha pajisjet mjekësore.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| **Id** | INT (PK) | NO | Primary Key (Auto-increment) |
| **DeviceID** | NVARCHAR(50) | NO | ID unik i pajisjes (UNIQUE) |
| Emri | NVARCHAR(200) | NO | Emri i pajisjes |
| Kategoria | NVARCHAR(100) | YES | Kategoria (p.sh. Diagnostikë) |
| Prodhues | NVARCHAR(100) | YES | Prodhues/Manufacturer |
| Modeli | NVARCHAR(100) | YES | Modeli i pajisjes |
| NumriSerial | NVARCHAR(100) | YES | Serial Number |
| VleraBlerjes | DECIMAL(18,2) | YES | Vlera e blerjes (€) |
| DataBlerjes | DATE | YES | Data e blerjes |
| DataFillimitPerdorimit | DATE | YES | Data fillimit përdorimit |
| Vendndodhja | NVARCHAR(100) | YES | Lokacioni aktual |
| Sherbimi | NVARCHAR(100) | YES | Shërbimi/Departamenti |
| Godina | NVARCHAR(100) | YES | Godina |
| **StatusiTeknik** | NVARCHAR(20) | NO | Status: Aktive / JoFunksionale / JashtëPërdorimit |
| NumriInventarMSHMS | NVARCHAR(100) | YES | Nr. inventarit MSHMS |
| Pershkrimi | NVARCHAR(500) | YES | Përshkrim i pajisjes |
| DataKrijimit | DATETIME2 | NO | Data e krijimit në sistem |
| DataPerditesimit | DATETIME2 | YES | Data e përditësimit |
| PerdoruesiPergjegjës | NVARCHAR(100) | YES | Përdorues përgjegjës |
| **EshteAktive** | BIT | NO | Active flag (soft delete) |
| QRCode | NVARCHAR(500) | YES | QR Code për pajisjen |
| VleraMbetur | DECIMAL(18,2) | YES | Vlera e mbetur (Amortizim) |
| AmortizimAkumuluar | DECIMAL(18,2) | YES | Amortizimi total |
| ViteJetese | INT | YES | Jetëgjatësia në vite |

**Constraints:**
- `CK_Pajisje_StatusiTeknik`: StatusiTeknik IN ('Aktive', 'JoFunksionale', 'JashtëPërdorimit')

**Indexes:**
- `IX_Pajisje_DeviceID` (UNIQUE)
- `IX_Pajisje_StatusiTeknik`
- `IX_Pajisje_Sherbimi`

---

### 2. AktKonstatimi (Inspection Reports)
Aktet e konstatimit të dëmtimeve - krijohen nga teknikët.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| **Id** | INT (PK) | NO | Primary Key |
| **PajisjeId** | INT (FK) | NO | Foreign Key → Pajisje |
| Pershkrimi | NVARCHAR(1000) | NO | Përshkrimi i dëmtimit |
| **Statusi** | NVARCHAR(20) | NO | HAPUR / MBYLLUR |
| KrijuarNga | NVARCHAR(100) | NO | Emri i teknikut |
| DataKrijimit | DATETIME2 | NO | Data e krijimit |
| DataMbylljes | DATETIME2 | YES | Data e mbylljes |
| MbyllurNga | NVARCHAR(100) | YES | Emri i inxhinierit |
| NotaMbylljes | NVARCHAR(2000) | YES | Nota e riparimit |
| NiveliUrgjences | NVARCHAR(50) | YES | I lartë / Mesatar / I ulët |

**Relationships:**
- `FK_AktKonstatimi_Pajisje`: PajisjeId → Pajisje(Id)
- One-to-Many: Një pajisje mund të ketë shumë akte

**Constraints:**
- `CK_AktKonstatimi_Statusi`: Statusi IN ('HAPUR', 'MBYLLUR')

**Indexes:**
- `IX_AktKonstatimi_Statusi`
- `IX_AktKonstatimi_PajisjeId`

---

### 3. Nderhyrje (Interventions/Repairs)
Ndërhyrjet teknike - riparime dhe mirëmbajtje.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| **Id** | INT (PK) | NO | Primary Key |
| **PajisjeId** | INT (FK) | NO | Foreign Key → Pajisje |
| AktKonstatimiId | INT (FK) | YES | Foreign Key → AktKonstatimi |
| Titulli | NVARCHAR(200) | NO | Titulli i ndërhyrjes |
| Pershkrimi | NVARCHAR(2000) | NO | Përshkrim i detajuar |
| **Lloji** | NVARCHAR(50) | NO | Riparim / Mirëmbajtje Preventive / Kalibrim / Kolaudim |
| **Statusi** | NVARCHAR(50) | NO | Hapur / Në Proces / Përfunduar / Refuzuar |
| DataHapjes | DATETIME2 | NO | Data e hapjes |
| DataPlanifikuar | DATE | YES | Data e planifikuar |
| DataFillimit | DATETIME2 | YES | Data e fillimit |
| DataPerfundimit | DATETIME2 | YES | Data e përfundimit |
| InxhinieriPergjegjës | NVARCHAR(100) | YES | Emri i inxhinierit |
| Kostoja | DECIMAL(18,2) | YES | Kostoja totale |
| NotaPerfundimit | NVARCHAR(2000) | YES | Nota finale |
| Dokumentacioni | NVARCHAR(500) | YES | Path to documents |
| PjesëzëKëmbyera | INT | YES | Numri i pjesëve |
| MaterialetPërdorura | NVARCHAR(1000) | YES | Lista e materialeve |
| KërkonAprovim | BIT | NO | Nëse kërkon aprovim |
| AprovuarNga | NVARCHAR(100) | YES | Kush e aprovoi |
| DataAprovimit | DATETIME2 | YES | Data e aprovimit |

**Relationships:**
- `FK_Nderhyrje_Pajisje`: PajisjeId → Pajisje(Id)
- `FK_Nderhyrje_AktKonstatimi`: AktKonstatimiId → AktKonstatimi(Id) (SET NULL on delete)
- One-to-One (optional): Një akt mund të ketë një ndërhyrje

**Constraints:**
- `CK_Nderhyrje_Lloji`: Lloji IN ('Riparim', 'Mirëmbajtje Preventive', 'Kalibrim', 'Kolaudim')
- `CK_Nderhyrje_Statusi`: Statusi IN ('Hapur', 'Në Proces', 'Përfunduar', 'Refuzuar')

**Indexes:**
- `IX_Nderhyrje_Statusi`
- `IX_Nderhyrje_PajisjeId`

---

### 4. Distributor (Suppliers/Operators)
Operatorët ekonomikë dhe distributorët.

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| **Id** | INT (PK) | NO | Primary Key |
| Emri | NVARCHAR(200) | NO | Emri i kompanisë |
| NIPT | NVARCHAR(50) | YES | NIPT (Unique Tax ID) |
| Adresa | NVARCHAR(500) | YES | Adresa |
| NumriTelefonit | NVARCHAR(50) | YES | Telefon |
| Email | NVARCHAR(100) | YES | Email |
| Website | NVARCHAR(100) | YES | Website |
| PersoniKontaktues | NVARCHAR(200) | YES | Personi kontaktues |
| EshteAktiv | BIT | NO | Active flag |
| DataRegjistrimit | DATETIME2 | NO | Data e regjistrimit |
| Shënime | NVARCHAR(1000) | YES | Shënime |

---

### 5. DistributorInxhinier (Distributor Engineers)
Inxhinierët e distributor-it (Child Table).

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| **Id** | INT (PK) | NO | Primary Key |
| **DistributorId** | INT (FK) | NO | Foreign Key → Distributor |
| Emri | NVARCHAR(200) | NO | Emri i inxhinierit |
| Email | NVARCHAR(100) | YES | Email |
| Telefoni | NVARCHAR(50) | YES | Telefon |
| Pozicioni | NVARCHAR(100) | YES | Pozicioni/Roli |
| EshteKontaktiKryesor | BIT | NO | A është kontakti kryesor? |
| Pranojnjoftime | BIT | NO | A pranon njoftime? |

**Relationships:**
- `FK_DistributorInxhinier_Distributor`: DistributorId → Distributor(Id) (CASCADE delete)
- One-to-Many: Një distributor mund të ketë shumë inxhinierë

---

## 🔗 Entity Relationship Diagram (ERD)

```
┌─────────────────────┐
│     Distributor     │
│  (Operatorët)       │
├─────────────────────┤
│ Id (PK)             │
│ Emri                │
│ NIPT                │
│ ...                 │
└─────────────────────┘
          │
          │ 1:N
          ▼
┌─────────────────────┐
│DistributorInxhinier │
├─────────────────────┤
│ Id (PK)             │
│ DistributorId (FK)  │
│ Emri                │
│ ...                 │
└─────────────────────┘


┌─────────────────────┐
│      Pajisje        │
│  (Pajisjet)         │
├─────────────────────┤
│ Id (PK)             │
│ DeviceID (UNIQUE)   │
│ Emri                │
│ StatusiTeknik       │
│ ...                 │
└─────────────────────┘
          │
          │ 1:N
          ├──────────────────┐
          │                  │
          ▼                  ▼
┌─────────────────────┐  ┌─────────────────────┐
│  AktKonstatimi      │  │    Nderhyrje        │
│  (Aktet)            │  │  (Riparime)         │
├─────────────────────┤  ├─────────────────────┤
│ Id (PK)             │  │ Id (PK)             │
│ PajisjeId (FK)      │  │ PajisjeId (FK)      │
│ Statusi             │  │ AktKonstatimiId (FK)│◄──┐
│ ...                 │  │ Statusi             │   │
└─────────────────────┘  │ ...                 │   │
          │              └─────────────────────┘   │
          │ 1:1 (optional)                         │
          └────────────────────────────────────────┘
```

---

## 📈 Views & Stored Procedures

### View: vw_PajisjeStatistika
Pajisjet me statistika të detajuara (akte, ndërhyrje).

**Columns:**
- Të gjitha kolonat e Pajisje
- NumriAkteve
- AkteHapur
- NumriNderhyrjeve
- NderhyrjeAktive

**Usage:**
```sql
SELECT * FROM vw_PajisjeStatistika 
WHERE StatusiTeknik = 'Aktive'
ORDER BY AkteHapur DESC;
```

---

### Stored Procedure: sp_GetDashboardStats
Merr statistikat e përgjithshme për dashboard.

**Returns:**
- TotalePajisje
- PajisjeAktive
- PajisjeJoFunksionale
- PajisjeJashtePerdorimit
- VleraTotal
- VleraMbeturTotal
- AkteHapur
- AkteMbyllur
- NderhyrjeAktive
- NderhyrjePerfunduar

**Usage:**
```sql
EXEC sp_GetDashboardStats;
```

---

## 🔍 Important Queries

### 1. Pajisjet me akte të hapura
```sql
SELECT p.DeviceID, p.Emri, COUNT(ak.Id) AS NumriAkteve
FROM Pajisje p
INNER JOIN AktKonstatimi ak ON p.Id = ak.PajisjeId
WHERE ak.Statusi = 'HAPUR'
GROUP BY p.DeviceID, p.Emri
ORDER BY NumriAkteve DESC;
```

### 2. Ndërhyrjet aktive për një pajisje
```sql
SELECT n.*, p.DeviceID, p.Emri
FROM Nderhyrje n
INNER JOIN Pajisje p ON n.PajisjeId = p.Id
WHERE n.Statusi IN ('Hapur', 'Në Proces')
AND p.DeviceID = 'QSUT-EKG-6500-001';
```

### 3. Historiku i plotë i pajisjes
```sql
SELECT 
    'Akt Konstatimi' AS Tipi,
    ak.DataKrijimit AS Data,
    ak.Pershkrimi AS Detajet,
    ak.Statusi
FROM AktKonstatimi ak
WHERE ak.PajisjeId = 1

UNION ALL

SELECT 
    'Ndërhyrje' AS Tipi,
    n.DataHapjes AS Data,
    n.Titulli AS Detajet,
    n.Statusi
FROM Nderhyrje n
WHERE n.PajisjeId = 1

ORDER BY Data DESC;
```

### 4. Kostoja totale e mirëmbajtjes për shërbim
```sql
SELECT 
    p.Sherbimi,
    COUNT(DISTINCT n.Id) AS NumriNderhyrjeve,
    SUM(n.Kostoja) AS KostojaTotal,
    AVG(n.Kostoja) AS KostojaMessatare
FROM Pajisje p
INNER JOIN Nderhyrje n ON p.Id = n.PajisjeId
WHERE n.Statusi = 'Përfunduar'
GROUP BY p.Sherbimi
ORDER BY KostojaTotal DESC;
```

### 5. Pajisjet që duhet amortizuar
```sql
SELECT 
    DeviceID,
    Emri,
    VleraBlerjes,
    VleraMbetur,
    AmortizimAkumuluar,
    DATEDIFF(YEAR, DataFillimitPerdorimit, GETDATE()) AS VitePerdorim,
    ViteJetese,
    CASE 
        WHEN DATEDIFF(YEAR, DataFillimitPerdorimit, GETDATE()) >= ViteJetese 
        THEN 'Amortizuar plotësisht'
        ELSE 'Në proces amortizimi'
    END AS StatusiAmortizimit
FROM Pajisje
WHERE DataFillimitPerdorimit IS NOT NULL
AND ViteJetese IS NOT NULL
ORDER BY VleraMbetur DESC;
```

---

## 🛡️ Security & Performance

### Backup Strategy
```sql
-- Full Backup
BACKUP DATABASE SIPM_ARKIMEDO21 
TO DISK = 'C:\Backups\SIPM_Full.bak' 
WITH FORMAT;

-- Differential Backup
BACKUP DATABASE SIPM_ARKIMEDO21 
TO DISK = 'C:\Backups\SIPM_Diff.bak' 
WITH DIFFERENTIAL;
```

### Maintenance
```sql
-- Rebuild Indexes
ALTER INDEX ALL ON Pajisje REBUILD;
ALTER INDEX ALL ON AktKonstatimi REBUILD;
ALTER INDEX ALL ON Nderhyrje REBUILD;

-- Update Statistics
UPDATE STATISTICS Pajisje;
UPDATE STATISTICS AktKonstatimi;
UPDATE STATISTICS Nderhyrje;
```

---

## 📊 Data Dictionary Summary

| Table | Purpose | Primary Key | Foreign Keys | Records (Initial) |
|-------|---------|-------------|--------------|-------------------|
| Pajisje | Medical devices registry | Id | - | 3 |
| AktKonstatimi | Inspection reports | Id | PajisjeId | 0 |
| Nderhyrje | Maintenance/repairs | Id | PajisjeId, AktKonstatimiId | 0 |
| Distributor | Suppliers | Id | - | 1 |
| DistributorInxhinier | Supplier engineers | Id | DistributorId | 2 |

**Total Tables:** 5  
**Total Relationships:** 4  
**Total Indexes:** 8  
**Total Views:** 1  
**Total SPs:** 1

---

## 🎯 Next Steps

1. **Add Authentication Tables** (Users, Roles, Permissions)
2. **Add Audit Trail** (Track all changes)
3. **Add File Attachments Table** (Documents, Photos)
4. **Add Notifications Table** (System notifications)
5. **Add Sinjalizim Tables** (Sensor data, alerts)

---

**Last Updated:** 2025-02-01  
**Version:** 1.0.0  
**Database:** SIPM_ARKIMEDO21
