using System;

namespace SIPM_Backend.DTOs
{
    /// <summary>
    /// DTO për krijimin e një pajisje të re
    /// </summary>
    public class CreatePajisjeDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Emri { get; set; } = string.Empty;
        public string? Kategoria { get; set; }
        public string? Prodhues { get; set; }
        public string? Modeli { get; set; }
        public string? NumriSerial { get; set; }
        public decimal? VleraBlerjes { get; set; }
        public DateTime? DataBlerjes { get; set; }
        public DateTime? DataFillimitPerdorimit { get; set; }
        public string? Vendndodhja { get; set; }
        public string? Sherbimi { get; set; }
        public string? Godina { get; set; }
        public string StatusiTeknik { get; set; } = "Aktive";
        public string? NumriInventarMSHMS { get; set; }
        public string? Pershkrimi { get; set; }
        public string? PerdoruesiPergjegjës { get; set; }
        public int? ViteJetese { get; set; }
    }

    /// <summary>
    /// DTO për përditësimin e pajisjes
    /// </summary>
    public class UpdatePajisjeDto : CreatePajisjeDto
    {
        // Trashëgon të gjitha fushat nga CreatePajisjeDto
    }

    /// <summary>
    /// DTO për krijimin e Aktit të Konstatimit
    /// </summary>
    public class CreateAktKonstatimiDto
    {
        public int PajisjeId { get; set; }
        public string Pershkrimi { get; set; } = string.Empty;
        public string KrijuarNga { get; set; } = string.Empty;
        public string? NiveliUrgjences { get; set; }
    }

    /// <summary>
    /// DTO për mbylljen e Aktit të Konstatimit
    /// </summary>
    public class MbyllAktKonstatimiDto
    {
        public string MbyllurNga { get; set; } = string.Empty;
        public string? NotaMbylljes { get; set; }
    }

    /// <summary>
    /// DTO për krijimin e Ndërhyrjes
    /// </summary>
    public class CreateNderhyrjeDto
    {
        public int PajisjeId { get; set; }
        public int? AktKonstatimiId { get; set; }
        public string Titulli { get; set; } = string.Empty;
        public string Pershkrimi { get; set; } = string.Empty;
        public string Lloji { get; set; } = "Riparim";
        public DateTime? DataPlanifikuar { get; set; }
        public string? InxhinieriPergjegjës { get; set; }
        public decimal? Kostoja { get; set; }
        public bool KërkonAprovim { get; set; } = false;
    }

    /// <summary>
    /// DTO për përditësimin e Ndërhyrjes
    /// </summary>
    public class UpdateNderhyrjeDto
    {
        public string? Statusi { get; set; }
        public DateTime? DataFillimit { get; set; }
        public DateTime? DataPerfundimit { get; set; }
        public string? NotaPerfundimit { get; set; }
        public string? InxhinieriPergjegjës { get; set; }
        public decimal? Kostoja { get; set; }
        public string? MaterialetPërdorura { get; set; }
        public string? AprovuarNga { get; set; }
    }

    /// <summary>
    /// DTO për Response të Pajisjes (me statistika)
    /// </summary>
    public class PajisjeResponseDto
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string Emri { get; set; } = string.Empty;
        public string? Kategoria { get; set; }
        public string? Prodhues { get; set; }
        public string? Modeli { get; set; }
        public string StatusiTeknik { get; set; } = string.Empty;
        public string? Vendndodhja { get; set; }
        public string? Sherbimi { get; set; }
        public decimal? VleraBlerjes { get; set; }
        public decimal? VleraMbetur { get; set; }
        public DateTime? DataBlerjes { get; set; }
        public int NumriAkteveTeHapura { get; set; }
        public int NumriNderhyrjeveAktive { get; set; }
    }

    /// <summary>
    /// DTO për Response të Dashboard (Statistika)
    /// </summary>
    public class DashboardStatsDto
    {
        public int TotalePajisje { get; set; }
        public int PajisjeAktive { get; set; }
        public int PajisjeJoFunksionale { get; set; }
        public int PajisjeJashtePerdorimit { get; set; }
        public int AkteKonstatimiHapur { get; set; }
        public int AkteKonstatimiMbyllur { get; set; }
        public int NderhyrjeAktive { get; set; }
        public int NderhyrjePerfunduar { get; set; }
        public decimal VleraToталe { get; set; }
        public decimal VleraMbeturTotale { get; set; }
    }

    /// <summary>
    /// Response standard për API
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operacioni u krye me sukses")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
