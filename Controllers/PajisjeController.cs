using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIPM_Backend.Data;
using SIPM_Backend.Models;
using SIPM_Backend.DTOs;

namespace SIPM_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PajisjeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PajisjeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/pajisje
        /// Merr të gjitha pajisjet
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PajisjeResponseDto>>>> GetAll(
            [FromQuery] string? statusi = null,
            [FromQuery] string? kategoria = null,
            [FromQuery] string? sherbimi = null)
        {
            try
            {
                var query = _context.Pajisje
                    .Include(p => p.AkteKonstatimit)
                    .Include(p => p.Nderhyrjet)
                    .Where(p => p.EshteAktive);

                // Filtrimi
                if (!string.IsNullOrEmpty(statusi))
                    query = query.Where(p => p.StatusiTeknik == statusi);

                if (!string.IsNullOrEmpty(kategoria))
                    query = query.Where(p => p.Kategoria == kategoria);

                if (!string.IsNullOrEmpty(sherbimi))
                    query = query.Where(p => p.Sherbimi == sherbimi);

                var pajisjet = await query.ToListAsync();

                var result = pajisjet.Select(p => new PajisjeResponseDto
                {
                    Id = p.Id,
                    DeviceId = p.DeviceId,
                    Emri = p.Emri,
                    Kategoria = p.Kategoria,
                    Prodhues = p.Prodhues,
                    Modeli = p.Modeli,
                    StatusiTeknik = p.StatusiTeknik,
                    Vendndodhja = p.Vendndodhja,
                    Sherbimi = p.Sherbimi,
                    VleraBlerjes = p.VleraBlerjes,
                    VleraMbetur = p.VleraMbetur,
                    DataBlerjes = p.DataBlerjes,
                    NumriAkteveTeHapura = p.AkteKonstatimit?.Count(a => a.Statusi == "HAPUR") ?? 0,
                    NumriNderhyrjeveAktive = p.Nderhyrjet?.Count(n => n.Statusi != "Përfunduar") ?? 0
                }).ToList();

                return Ok(ApiResponse<List<PajisjeResponseDto>>.SuccessResponse(
                    result, 
                    $"U gjetën {result.Count} pajisje"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<PajisjeResponseDto>>.ErrorResponse(
                    "Gabim në server", 
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// GET: api/pajisje/{id}
        /// Merr një pajisje specifike sipas ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Pajisje>>> GetById(int id)
        {
            try
            {
                var pajisje = await _context.Pajisje
                    .Include(p => p.AkteKonstatimit)
                    .Include(p => p.Nderhyrjet)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pajisje == null)
                {
                    return NotFound(ApiResponse<Pajisje>.ErrorResponse(
                        $"Pajisja me ID {id} nuk u gjet"));
                }

                return Ok(ApiResponse<Pajisje>.SuccessResponse(pajisje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Pajisje>.ErrorResponse(
                    "Gabim në server", 
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// GET: api/pajisje/device/{deviceId}
        /// Merr pajisje sipas Device ID
        /// </summary>
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<ApiResponse<Pajisje>>> GetByDeviceId(string deviceId)
        {
            try
            {
                var pajisje = await _context.Pajisje
                    .Include(p => p.AkteKonstatimit)
                    .Include(p => p.Nderhyrjet)
                    .FirstOrDefaultAsync(p => p.DeviceId == deviceId);

                if (pajisje == null)
                {
                    return NotFound(ApiResponse<Pajisje>.ErrorResponse(
                        $"Pajisja me Device ID '{deviceId}' nuk u gjet"));
                }

                return Ok(ApiResponse<Pajisje>.SuccessResponse(pajisje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Pajisje>.ErrorResponse(
                    "Gabim në server", 
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// POST: api/pajisje
        /// Krijon pajisje të re
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Pajisje>>> Create([FromBody] CreatePajisjeDto dto)
        {
            try
            {
                // Kontrollo nëse ekziston Device ID
                var exists = await _context.Pajisje
                    .AnyAsync(p => p.DeviceId == dto.DeviceId);

                if (exists)
                {
                    return BadRequest(ApiResponse<Pajisje>.ErrorResponse(
                        $"Pajisja me Device ID '{dto.DeviceId}' ekziston tashmë"));
                }

                var pajisje = new Pajisje
                {
                    DeviceId = dto.DeviceId,
                    Emri = dto.Emri,
                    Kategoria = dto.Kategoria,
                    Prodhues = dto.Prodhues,
                    Modeli = dto.Modeli,
                    NumriSerial = dto.NumriSerial,
                    VleraBlerjes = dto.VleraBlerjes,
                    DataBlerjes = dto.DataBlerjes,
                    DataFillimitPerdorimit = dto.DataFillimitPerdorimit,
                    Vendndodhja = dto.Vendndodhja,
                    Sherbimi = dto.Sherbimi,
                    Godina = dto.Godina,
                    StatusiTeknik = dto.StatusiTeknik,
                    NumriInventarMSHMS = dto.NumriInventarMSHMS,
                    Pershkrimi = dto.Pershkrimi,
                    PerdoruesiPergjegjës = dto.PerdoruesiPergjegjës,
                    ViteJetese = dto.ViteJetese,
                    DataKrijimit = DateTime.Now,
                    EshteAktive = true
                };

                // Llogarit amortizimin
                if (pajisje.VleraBlerjes.HasValue && pajisje.DataFillimitPerdorimit.HasValue && pajisje.ViteJetese.HasValue)
                {
                    var viteEKaluar = (DateTime.Now - pajisje.DataFillimitPerdorimit.Value).Days / 365.0;
                    var normaAmortizimi = 100.0M / pajisje.ViteJetese.Value;
                    pajisje.AmortizimAkumuluar = pajisje.VleraBlerjes.Value * (normaAmortizimi / 100.0M) * (decimal)viteEKaluar;
                    pajisje.VleraMbetur = Math.Max(0, pajisje.VleraBlerjes.Value - pajisje.AmortizimAkumuluar.Value);
                }

                _context.Pajisje.Add(pajisje);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), 
                    new { id = pajisje.Id }, 
                    ApiResponse<Pajisje>.SuccessResponse(pajisje, "Pajisja u krijua me sukses"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Pajisje>.ErrorResponse(
                    "Gabim në krijimin e pajisjes", 
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// PUT: api/pajisje/{id}
        /// Përditëson pajisjen
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Pajisje>>> Update(int id, [FromBody] UpdatePajisjeDto dto)
        {
            try
            {
                var pajisje = await _context.Pajisje.FindAsync(id);

                if (pajisje == null)
                {
                    return NotFound(ApiResponse<Pajisje>.ErrorResponse(
                        $"Pajisja me ID {id} nuk u gjet"));
                }

                // Kontrollo Device ID dublikat (përveç kësaj pajisje)
                if (dto.DeviceId != pajisje.DeviceId)
                {
                    var exists = await _context.Pajisje
                        .AnyAsync(p => p.DeviceId == dto.DeviceId && p.Id != id);

                    if (exists)
                    {
                        return BadRequest(ApiResponse<Pajisje>.ErrorResponse(
                            $"Device ID '{dto.DeviceId}' përdoret tashmë nga pajisje tjetër"));
                    }
                }

                // Përditëso fushat
                pajisje.DeviceId = dto.DeviceId;
                pajisje.Emri = dto.Emri;
                pajisje.Kategoria = dto.Kategoria;
                pajisje.Prodhues = dto.Prodhues;
                pajisje.Modeli = dto.Modeli;
                pajisje.NumriSerial = dto.NumriSerial;
                pajisje.VleraBlerjes = dto.VleraBlerjes;
                pajisje.DataBlerjes = dto.DataBlerjes;
                pajisje.DataFillimitPerdorimit = dto.DataFillimitPerdorimit;
                pajisje.Vendndodhja = dto.Vendndodhja;
                pajisje.Sherbimi = dto.Sherbimi;
                pajisje.Godina = dto.Godina;
                pajisje.StatusiTeknik = dto.StatusiTeknik;
                pajisje.NumriInventarMSHMS = dto.NumriInventarMSHMS;
                pajisje.Pershkrimi = dto.Pershkrimi;
                pajisje.PerdoruesiPergjegjës = dto.PerdoruesiPergjegjës;
                pajisje.ViteJetese = dto.ViteJetese;
                pajisje.DataPerditesimit = DateTime.Now;

                // Rillogarit amortizimin
                if (pajisje.VleraBlerjes.HasValue && pajisje.DataFillimitPerdorimit.HasValue && pajisje.ViteJetese.HasValue)
                {
                    var viteEKaluar = (DateTime.Now - pajisje.DataFillimitPerdorimit.Value).Days / 365.0;
                    var normaAmortizimi = 100.0M / pajisje.ViteJetese.Value;
                    pajisje.AmortizimAkumuluar = pajisje.VleraBlerjes.Value * (normaAmortizimi / 100.0M) * (decimal)viteEKaluar;
                    pajisje.VleraMbetur = Math.Max(0, pajisje.VleraBlerjes.Value - pajisje.AmortizimAkumuluar.Value);
                }

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<Pajisje>.SuccessResponse(pajisje, "Pajisja u përditësua me sukses"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Pajisje>.ErrorResponse(
                    "Gabim në përditësimin e pajisjes", 
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// DELETE: api/pajisje/{id}
        /// Fshin pajisjen (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var pajisje = await _context.Pajisje.FindAsync(id);

                if (pajisje == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(
                        $"Pajisja me ID {id} nuk u gjet"));
                }

                // Soft delete
                pajisje.EshteAktive = false;
                pajisje.DataPerditesimit = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Pajisja u fshi me sukses"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(
                    "Gabim në fshirjen e pajisjes", 
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// GET: api/pajisje/stats
        /// Merr statistikat e përgjithshme
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats()
        {
            try
            {
                var stats = new DashboardStatsDto
                {
                    TotalePajisje = await _context.Pajisje.CountAsync(p => p.EshteAktive),
                    PajisjeAktive = await _context.Pajisje.CountAsync(p => p.EshteAktive && p.StatusiTeknik == "Aktive"),
                    PajisjeJoFunksionale = await _context.Pajisje.CountAsync(p => p.EshteAktive && p.StatusiTeknik == "JoFunksionale"),
                    PajisjeJashtePerdorimit = await _context.Pajisje.CountAsync(p => p.EshteAktive && p.StatusiTeknik == "JashtëPërdorimit"),
                    AkteKonstatimiHapur = await _context.AktKonstatimi.CountAsync(a => a.Statusi == "HAPUR"),
                    AkteKonstatimiMbyllur = await _context.AktKonstatimi.CountAsync(a => a.Statusi == "MBYLLUR"),
                    NderhyrjeAktive = await _context.Nderhyrje.CountAsync(n => n.Statusi != "Përfunduar" && n.Statusi != "Refuzuar"),
                    NderhyrjePerfunduar = await _context.Nderhyrje.CountAsync(n => n.Statusi == "Përfunduar"),
                    VleraToталe = await _context.Pajisje.Where(p => p.EshteAktive).SumAsync(p => p.VleraBlerjes ?? 0),
                    VleraMbeturTotale = await _context.Pajisje.Where(p => p.EshteAktive).SumAsync(p => p.VleraMbetur ?? 0)
                };

                return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(stats));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<DashboardStatsDto>.ErrorResponse(
                    "Gabim në marrjen e statistikave", 
                    new List<string> { ex.Message }));
            }
        }
    }
}
