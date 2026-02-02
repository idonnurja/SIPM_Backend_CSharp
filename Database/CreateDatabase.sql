-- =============================================
-- SIPM ARKIMEDO-21 - Database Creation Script
-- SQL Server 2019+
-- =============================================

-- 1. Krijo Database
USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SIPM_ARKIMEDO21')
BEGIN
    CREATE DATABASE SIPM_ARKIMEDO21;
    PRINT '✅ Database SIPM_ARKIMEDO21 u krijua me sukses!';
END
ELSE
BEGIN
    PRINT '⚠️ Database SIPM_ARKIMEDO21 ekziston tashmë!';
END
GO

USE SIPM_ARKIMEDO21;
GO

-- =============================================
-- 2. Krijo Tabelat
-- =============================================

-- Tabela: Pajisje
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pajisje]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Pajisje](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [DeviceID] NVARCHAR(50) NOT NULL UNIQUE,
        [Emri] NVARCHAR(200) NOT NULL,
        [Kategoria] NVARCHAR(100) NULL,
        [Prodhues] NVARCHAR(100) NULL,
        [Modeli] NVARCHAR(100) NULL,
        [NumriSerial] NVARCHAR(100) NULL,
        [VleraBlerjes] DECIMAL(18, 2) NULL,
        [DataBlerjes] DATE NULL,
        [DataFillimitPerdorimit] DATE NULL,
        [Vendndodhja] NVARCHAR(100) NULL,
        [Sherbimi] NVARCHAR(100) NULL,
        [Godina] NVARCHAR(100) NULL,
        [StatusiTeknik] NVARCHAR(20) NOT NULL DEFAULT 'Aktive',
        [NumriInventarMSHMS] NVARCHAR(100) NULL,
        [Pershkrimi] NVARCHAR(500) NULL,
        [DataKrijimit] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [DataPerditesimit] DATETIME2 NULL,
        [PerdoruesiPergjegjës] NVARCHAR(100) NULL,
        [EshteAktive] BIT NOT NULL DEFAULT 1,
        [QRCode] NVARCHAR(500) NULL,
        [VleraMbetur] DECIMAL(18, 2) NULL,
        [AmortizimAkumuluar] DECIMAL(18, 2) NULL,
        [ViteJetese] INT NULL,
        
        -- Constraints
        CONSTRAINT CK_Pajisje_StatusiTeknik 
            CHECK (StatusiTeknik IN ('Aktive', 'JoFunksionale', 'JashtëPërdorimit'))
    );
    
    PRINT '✅ Tabela Pajisje u krijua';
END
GO

-- Tabela: Distributor
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Distributor]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Distributor](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Emri] NVARCHAR(200) NOT NULL,
        [NIPT] NVARCHAR(50) NULL,
        [Adresa] NVARCHAR(500) NULL,
        [NumriTelefonit] NVARCHAR(50) NULL,
        [Email] NVARCHAR(100) NULL,
        [Website] NVARCHAR(100) NULL,
        [PersoniKontaktues] NVARCHAR(200) NULL,
        [EshteAktiv] BIT NOT NULL DEFAULT 1,
        [DataRegjistrimit] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [Shënime] NVARCHAR(1000) NULL
    );
    
    PRINT '✅ Tabela Distributor u krijua';
END
GO

-- Tabela: DistributorInxhinier
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DistributorInxhinier]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DistributorInxhinier](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [DistributorId] INT NOT NULL,
        [Emri] NVARCHAR(200) NOT NULL,
        [Email] NVARCHAR(100) NULL,
        [Telefoni] NVARCHAR(50) NULL,
        [Pozicioni] NVARCHAR(100) NULL,
        [EshteKontaktiKryesor] BIT NOT NULL DEFAULT 0,
        [Pranojnjoftime] BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT FK_DistributorInxhinier_Distributor 
            FOREIGN KEY ([DistributorId]) REFERENCES [dbo].[Distributor]([Id]) 
            ON DELETE CASCADE
    );
    
    PRINT '✅ Tabela DistributorInxhinier u krijua';
END
GO

-- Tabela: AktKonstatimi
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AktKonstatimi]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AktKonstatimi](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [PajisjeId] INT NOT NULL,
        [Pershkrimi] NVARCHAR(1000) NOT NULL,
        [Statusi] NVARCHAR(20) NOT NULL DEFAULT 'HAPUR',
        [KrijuarNga] NVARCHAR(100) NOT NULL,
        [DataKrijimit] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [DataMbylljes] DATETIME2 NULL,
        [MbyllurNga] NVARCHAR(100) NULL,
        [NotaMbylljes] NVARCHAR(2000) NULL,
        [NiveliUrgjences] NVARCHAR(50) NULL,
        
        CONSTRAINT FK_AktKonstatimi_Pajisje 
            FOREIGN KEY ([PajisjeId]) REFERENCES [dbo].[Pajisje]([Id]),
        
        CONSTRAINT CK_AktKonstatimi_Statusi 
            CHECK (Statusi IN ('HAPUR', 'MBYLLUR'))
    );
    
    PRINT '✅ Tabela AktKonstatimi u krijua';
END
GO

-- Tabela: Nderhyrje
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Nderhyrje]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Nderhyrje](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [PajisjeId] INT NOT NULL,
        [AktKonstatimiId] INT NULL,
        [Titulli] NVARCHAR(200) NOT NULL,
        [Pershkrimi] NVARCHAR(2000) NOT NULL,
        [Lloji] NVARCHAR(50) NOT NULL DEFAULT 'Riparim',
        [Statusi] NVARCHAR(50) NOT NULL DEFAULT 'Hapur',
        [DataHapjes] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [DataPlanifikuar] DATE NULL,
        [DataFillimit] DATETIME2 NULL,
        [DataPerfundimit] DATETIME2 NULL,
        [InxhinieriPergjegjës] NVARCHAR(100) NULL,
        [Kostoja] DECIMAL(18, 2) NULL,
        [NotaPerfundimit] NVARCHAR(2000) NULL,
        [Dokumentacioni] NVARCHAR(500) NULL,
        [PjesëzëKëmbyera] INT NULL,
        [MaterialetPërdorura] NVARCHAR(1000) NULL,
        [KërkonAprovim] BIT NOT NULL DEFAULT 0,
        [AprovuarNga] NVARCHAR(100) NULL,
        [DataAprovimit] DATETIME2 NULL,
        
        CONSTRAINT FK_Nderhyrje_Pajisje 
            FOREIGN KEY ([PajisjeId]) REFERENCES [dbo].[Pajisje]([Id]),
        
        CONSTRAINT FK_Nderhyrje_AktKonstatimi 
            FOREIGN KEY ([AktKonstatimiId]) REFERENCES [dbo].[AktKonstatimi]([Id])
            ON DELETE SET NULL,
        
        CONSTRAINT CK_Nderhyrje_Lloji 
            CHECK (Lloji IN ('Riparim', 'Mirëmbajtje Preventive', 'Kalibrim', 'Kolaudim')),
        
        CONSTRAINT CK_Nderhyrje_Statusi 
            CHECK (Statusi IN ('Hapur', 'Në Proces', 'Përfunduar', 'Refuzuar'))
    );
    
    PRINT '✅ Tabela Nderhyrje u krijua';
END
GO

-- =============================================
-- 3. Krijo Indexes për Performance
-- =============================================

-- Pajisje Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pajisje_DeviceID')
    CREATE UNIQUE INDEX IX_Pajisje_DeviceID ON Pajisje(DeviceID);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pajisje_StatusiTeknik')
    CREATE INDEX IX_Pajisje_StatusiTeknik ON Pajisje(StatusiTeknik);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pajisje_Sherbimi')
    CREATE INDEX IX_Pajisje_Sherbimi ON Pajisje(Sherbimi);

-- AktKonstatimi Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AktKonstatimi_Statusi')
    CREATE INDEX IX_AktKonstatimi_Statusi ON AktKonstatimi(Statusi);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AktKonstatimi_PajisjeId')
    CREATE INDEX IX_AktKonstatimi_PajisjeId ON AktKonstatimi(PajisjeId);

-- Nderhyrje Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Nderhyrje_Statusi')
    CREATE INDEX IX_Nderhyrje_Statusi ON Nderhyrje(Statusi);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Nderhyrje_PajisjeId')
    CREATE INDEX IX_Nderhyrje_PajisjeId ON Nderhyrje(PajisjeId);

PRINT '✅ Indexes u krijuan';
GO

-- =============================================
-- 4. Të Dhëna Fillestare (Seed Data)
-- =============================================

-- Pajisje Fillestare
IF NOT EXISTS (SELECT * FROM Pajisje WHERE DeviceID = 'QSUT-EKG-6500-001')
BEGIN
    INSERT INTO Pajisje (DeviceID, Emri, Kategoria, Prodhues, Modeli, NumriSerial, 
                        VleraBlerjes, DataBlerjes, DataFillimitPerdorimit, 
                        Vendndodhja, Sherbimi, Godina, StatusiTeknik, ViteJetese)
    VALUES 
    ('QSUT-EKG-6500-001', 'Elektrokardiograf GE MAC 5500', 'Diagnostikë Kardiologjike', 
     'GE Healthcare', 'MAC 5500', 'SN-GE-001-2024', 
     12500.00, '2024-01-15', '2024-02-01', 
     'Kardiologji - Dhoma 302', 'Kardiologji', 'Godina Kryesore', 'Aktive', 10),
    
    ('QSUT-XRY-8800-002', 'Aparat Rreze-X Mobil', 'Imazheri Radiologjike', 
     'Siemens Healthineers', 'Mobilett Mira', 'SN-SIE-002-2024', 
     45000.00, '2023-11-20', '2023-12-05', 
     'Radiologji - Njësia Mobile', 'Radiologji', 'Godina Kryesore', 'Aktive', 12),
    
    ('QSUT-VNT-2100-003', 'Ventilator Intensiv', 'Suport Jetësor', 
     'Dräger', 'Evita V300', 'SN-DRG-003-2024', 
     28000.00, '2024-03-10', '2024-03-25', 
     'ICU - Dhoma 105', 'Terapia Intensive', 'Godina Kryesore', 'Aktive', 10);
    
    PRINT '✅ Pajisje fillestare u shtuan';
END
GO

-- Distributor Fillestar
IF NOT EXISTS (SELECT * FROM Distributor WHERE NIPT = 'K12345678L')
BEGIN
    INSERT INTO Distributor (Emri, NIPT, Adresa, NumriTelefonit, Email, Website, PersoniKontaktues)
    VALUES 
    ('Med-Tech Solutions Albania', 'K12345678L', 'Rruga Qemal Stafa, Tiranë', 
     '+355 4 2234567', 'info@medtech.al', 'www.medtech.al', 'Arben Hoxha');
    
    DECLARE @DistributorId INT = SCOPE_IDENTITY();
    
    -- Inxhinierët e Distributor-it
    INSERT INTO DistributorInxhinier (DistributorId, Emri, Email, Telefoni, Pozicioni, EshteKontaktiKryesor)
    VALUES 
    (@DistributorId, 'Petrit Kola', 'petrit.kola@medtech.al', '+355 69 1234567', 
     'Senior Biomedical Engineer', 1),
    (@DistributorId, 'Elona Gjika', 'elona.gjika@medtech.al', '+355 69 7654321', 
     'Field Service Engineer', 0);
    
    PRINT '✅ Distributor dhe Inxhinierë u shtuan';
END
GO

-- =============================================
-- 5. Views për Raportim
-- =============================================

-- View: Pajisje me Statistika
IF OBJECT_ID('vw_PajisjeStatistika', 'V') IS NOT NULL
    DROP VIEW vw_PajisjeStatistika;
GO

CREATE VIEW vw_PajisjeStatistika AS
SELECT 
    p.Id,
    p.DeviceID,
    p.Emri,
    p.Kategoria,
    p.Prodhues,
    p.StatusiTeknik,
    p.Sherbimi,
    p.VleraBlerjes,
    p.VleraMbetur,
    p.DataBlerjes,
    COUNT(DISTINCT ak.Id) AS NumriAkteve,
    COUNT(DISTINCT CASE WHEN ak.Statusi = 'HAPUR' THEN ak.Id END) AS AkteHapur,
    COUNT(DISTINCT n.Id) AS NumriNderhyrjeve,
    COUNT(DISTINCT CASE WHEN n.Statusi != 'Përfunduar' THEN n.Id END) AS NderhyrjeAktive
FROM Pajisje p
LEFT JOIN AktKonstatimi ak ON p.Id = ak.PajisjeId
LEFT JOIN Nderhyrje n ON p.Id = n.PajisjeId
WHERE p.EshteAktive = 1
GROUP BY p.Id, p.DeviceID, p.Emri, p.Kategoria, p.Prodhues, 
         p.StatusiTeknik, p.Sherbimi, p.VleraBlerjes, p.VleraMbetur, p.DataBlerjes;
GO

PRINT '✅ View vw_PajisjeStatistika u krijua';

-- =============================================
-- 6. Stored Procedures
-- =============================================

-- SP: Merr Statistika Dashboard
IF OBJECT_ID('sp_GetDashboardStats', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetDashboardStats;
GO

CREATE PROCEDURE sp_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(CASE WHEN EshteAktive = 1 THEN 1 END) AS TotalePajisje,
        COUNT(CASE WHEN StatusiTeknik = 'Aktive' AND EshteAktive = 1 THEN 1 END) AS PajisjeAktive,
        COUNT(CASE WHEN StatusiTeknik = 'JoFunksionale' AND EshteAktive = 1 THEN 1 END) AS PajisjeJoFunksionale,
        COUNT(CASE WHEN StatusiTeknik = 'JashtëPërdorimit' AND EshteAktive = 1 THEN 1 END) AS PajisjeJashtePerdorimit,
        ISNULL(SUM(VleraBlerjes), 0) AS VleraTotal,
        ISNULL(SUM(VleraMbetur), 0) AS VleraMbeturTotal
    FROM Pajisje;
    
    SELECT 
        COUNT(CASE WHEN Statusi = 'HAPUR' THEN 1 END) AS AkteHapur,
        COUNT(CASE WHEN Statusi = 'MBYLLUR' THEN 1 END) AS AkteMbyllur
    FROM AktKonstatimi;
    
    SELECT 
        COUNT(CASE WHEN Statusi NOT IN ('Përfunduar', 'Refuzuar') THEN 1 END) AS NderhyrjeAktive,
        COUNT(CASE WHEN Statusi = 'Përfunduar' THEN 1 END) AS NderhyrjePerfunduar
    FROM Nderhyrje;
END
GO

PRINT '✅ Stored Procedure sp_GetDashboardStats u krijua';

-- =============================================
-- Script Completed!
-- =============================================

PRINT '';
PRINT '========================================';
PRINT '✅ SIPM Database u krijua me sukses!';
PRINT '========================================';
PRINT '📊 Tabelat: Pajisje, AktKonstatimi, Nderhyrje, Distributor';
PRINT '📈 Views: vw_PajisjeStatistika';
PRINT '⚙️ Stored Procedures: sp_GetDashboardStats';
PRINT '🔍 Indexes: Performance optimized';
PRINT '📝 Seed Data: 3 pajisje + 1 distributor';
PRINT '';

-- Test Query
SELECT 'Database Test:' AS Info, COUNT(*) AS NumriPajisjeve FROM Pajisje;
GO
