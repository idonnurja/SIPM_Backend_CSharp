using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIPM_Backend.Data;
using SIPM_Backend.Models;
using SIPM_Backend.DTOs;

namespace SIPM_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AktKonstatimiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AktKonstatimiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/aktkonstatimi
        /// Merr të gjitha aktet e konstatimit
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AktKonstatimi>>>> GetAll(
            [FromQuery] string? statusi = null,
            [FromQuery] int? pajisjeId = null)
        {
            try
            {
                var query = _context.AktKonstatimi
                    .Include(a => a.Pajisje)
                    .Include(a => a.Nderhyrja)
                    .AsQueryable();

                // Filtrimi
                if (!string.IsNullOrEmpty(statusi))
                    query = query.Where(a => a.Statusi == statusi);

                if (pajisjeId.HasValue)
                    query = query.Where(a => a.PajisjeId == pajisjeId.Value);

                var aktet = await query
                    .OrderByDescending(a => a.DataKrijimit)
                    .ToListAsync();

                return Ok(ApiResponse<List<AktKonstatimi>>.SuccessResponse(
                    aktet,
                    $"U gjetën {aktet.Count} akte konstatimi"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<AktKonstatimi>>.ErrorResponse(
                    "Gabim në server",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// GET: api/aktkonstatimi/hapur
        /// Merr vetëm aktet e hapura (për Inxhinierin)
        /// </summary>
        [HttpGet("hapur")]
        public async Task<ActionResult<ApiResponse<List<AktKonstatimi>>>> GetHapur()
        {
            try
            {
                var aktetHapur = await _context.AktKonstatimi
                    .Include(a => a.Pajisje)
                    .Where(a => a.Statusi == "HAPUR")
                    .OrderBy(a => a.DataKrijimit) // Më të vjetrit më parë
                    .ToListAsync();

                return Ok(ApiResponse<List<AktKonstatimi>>.SuccessResponse(
                    aktetHapur,
                    $"Ka {aktetHapur.Count} akte të hapura"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<AktKonstatimi>>.ErrorResponse(
                    "Gabim në server",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// GET: api/aktkonstatimi/{id}
        /// Merr një akt specifik
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AktKonstatimi>>> GetById(int id)
        {
            try
            {
                var akt = await _context.AktKonstatimi
                    .Include(a => a.Pajisje)
                    .Include(a => a.Nderhyrja)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (akt == null)
                {
                    return NotFound(ApiResponse<AktKonstatimi>.ErrorResponse(
                        $"Akt Konstatimi me ID {id} nuk u gjet"));
                }

                return Ok(ApiResponse<AktKonstatimi>.SuccessResponse(akt));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AktKonstatimi>.ErrorResponse(
                    "Gabim në server",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// POST: api/aktkonstatimi
        /// Krijon akt të ri konstatimi (nga Tekniku)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AktKonstatimi>>> Create([FromBody] CreateAktKonstatimiDto dto)
        {
            try
            {
                // Kontrollo nëse pajisja ekziston
                var pajisje = await _context.Pajisje.FindAsync(dto.PajisjeId);
                if (pajisje == null)
                {
                    return NotFound(ApiResponse<AktKonstatimi>.ErrorResponse(
                        $"Pajisja me ID {dto.PajisjeId} nuk u gjet"));
                }

                var akt = new AktKonstatimi
                {
                    PajisjeId = dto.PajisjeId,
                    Pershkrimi = dto.Pershkrimi,
                    KrijuarNga = dto.KrijuarNga,
                    NiveliUrgjences = dto.NiveliUrgjences,
                    Statusi = "HAPUR",
                    DataKrijimit = DateTime.Now
                };

                _context.AktKonstatimi.Add(akt);

                // Ndrysho statusin e pajisjes në "JoFunksionale"
                pajisje.StatusiTeknik = "JoFunksionale";
                pajisje.DataPerditesimit = DateTime.Now;

                await _context.SaveChangesAsync();

                // Ngarko relacionet para se ta kthejmë
                await _context.Entry(akt).Reference(a => a.Pajisje).LoadAsync();

                return CreatedAtAction(nameof(GetById),
                    new { id = akt.Id },
                    ApiResponse<AktKonstatimi>.SuccessResponse(
                        akt,
                        $"Akt Konstatimi u krijua me sukses. Inxhinieri është njoftuar!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AktKonstatimi>.ErrorResponse(
                    "Gabim në krijimin e aktit",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// PUT: api/aktkonstatimi/{id}/mbyll
        /// Mbyll aktin e konstatimit (nga Inxhinieri)
        /// </summary>
        [HttpPut("{id}/mbyll")]
        public async Task<ActionResult<ApiResponse<AktKonstatimi>>> Mbyll(int id, [FromBody] MbyllAktKonstatimiDto dto)
        {
            try
            {
                var akt = await _context.AktKonstatimi
                    .Include(a => a.Pajisje)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (akt == null)
                {
                    return NotFound(ApiResponse<AktKonstatimi>.ErrorResponse(
                        $"Akt Konstatimi me ID {id} nuk u gjet"));
                }

                if (akt.Statusi == "MBYLLUR")
                {
                    return BadRequest(ApiResponse<AktKonstatimi>.ErrorResponse(
                        "Ky akt është mbyllur tashmë"));
                }

                // Mbyll aktin
                akt.Statusi = "MBYLLUR";
                akt.DataMbylljes = DateTime.Now;
                akt.MbyllurNga = dto.MbyllurNga;
                akt.NotaMbylljes = dto.NotaMbylljes;

                // Kthe pajisjen në "Aktive"
                if (akt.Pajisje != null)
                {
                    akt.Pajisje.StatusiTeknik = "Aktive";
                    akt.Pajisje.DataPerditesimit = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<AktKonstatimi>.SuccessResponse(
                    akt,
                    $"Akti u mbyll me sukses. Pajisja {akt.Pajisje?.DeviceId} është aktive përsëri."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AktKonstatimi>.ErrorResponse(
                    "Gabim në mbylljen e aktit",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// DELETE: api/aktkonstatimi/{id}
        /// Fshin aktin (vetëm nëse është MBYLLUR)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var akt = await _context.AktKonstatimi.FindAsync(id);

                if (akt == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(
                        $"Akt Konstatimi me ID {id} nuk u gjet"));
                }

                if (akt.Statusi == "HAPUR")
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse(
                        "Nuk mund të fshish një akt të hapur. Mbylleni fillimisht."));
                }

                _context.AktKonstatimi.Remove(akt);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Akti u fshi me sukses"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(
                    "Gabim në fshirjen e aktit",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// GET: api/aktkonstatimi/pajisje/{pajisjeId}
        /// Merr historikun e akteve për një pajisje specifike
        /// </summary>
        [HttpGet("pajisje/{pajisjeId}")]
        public async Task<ActionResult<ApiResponse<List<AktKonstatimi>>>> GetByPajisje(int pajisjeId)
        {
            try
            {
                var aktet = await _context.AktKonstatimi
                    .Include(a => a.Pajisje)
                    .Where(a => a.PajisjeId == pajisjeId)
                    .OrderByDescending(a => a.DataKrijimit)
                    .ToListAsync();

                return Ok(ApiResponse<List<AktKonstatimi>>.SuccessResponse(
                    aktet,
                    $"U gjetën {aktet.Count} akte për këtë pajisje"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<AktKonstatimi>>.ErrorResponse(
                    "Gabim në server",
                    new List<string> { ex.Message }));
            }
        }
    }
}
