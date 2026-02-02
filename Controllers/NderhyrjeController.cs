using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIPM_Backend.Data;
using SIPM_Backend.Models;
using SIPM_Backend.DTOs;

namespace SIPM_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NderhyrjeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NderhyrjeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/nderhyrje
        /// Merr të gjitha ndërhyrjet
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Nderhyrje>>>> GetAll(
            [FromQuery] string? statusi = null,
            [FromQuery] string? lloji = null,
            [FromQuery] int? pajisjeId = null)
        {
            try
            {
                var query = _context.Nderhyrje
                    .Include(n => n.Pajisje)
                    .Include(n => n.AktKonstatimi)
                    .AsQueryable();

                // Filtrimi
                if (!string.IsNullOrEmpty(statusi))
                    query = query.Where(n => n.Statusi == statusi);

                if (!string.IsNullOrEmpty(lloji))
                    query = query.Where(n => n.Lloji == lloji);

                if (pajisjeId.HasValue)
                    query = query.Where(n => n.PajisjeId == pajisjeId.Value);

                var nderhyrjet = await query
                    .OrderByDescending(n => n.DataHapjes)
                    .ToListAsync();

                return Ok(ApiResponse<List<Nderhyrje>>.SuccessResponse(
                    nderhyrjet,
                    $"U gjetën {nderhyrjet.Count} ndërhyrje"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Nderhyrje>>.ErrorResponse(
                    "Gabim në server",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// GET: api/nderhyrje/aktive
        /// Merr vetëm ndërhyrjet aktive
        /// </summary>
        [HttpGet("aktive")]
        public async Task<ActionResult<ApiResponse<List<Nderhyrje>>>> GetAktive()
        {
            try
            {
                var nderhyrjetAktive = await _context.Nderhyrje
                    .Include(n => n.Pajisje)
                    .Include(n => n.AktKonstatimi)
                    .Where(n => n.Statusi != "Përfunduar" && n.Statusi != "Refuzuar")
                    .OrderBy(n => n.DataHapjes)
                    .ToListAsync();

                return Ok(ApiResponse<List<Nderhyrje>>.SuccessResponse(
                    nderhyrjetAktive,
                    $"Ka {nderhyrjetAktive.Count} ndërhyrje aktive"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Nderhyrje>>.ErrorResponse(
                    "Gabim në server",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// GET: api/nderhyrje/{id}
        /// Merr një ndërhyrje specifike
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Nderhyrje>>> GetById(int id)
        {
            try
            {
                var nderhyrje = await _context.Nderhyrje
                    .Include(n => n.Pajisje)
                    .Include(n => n.AktKonstatimi)
                    .FirstOrDefaultAsync(n => n.Id == id);

                if (nderhyrje == null)
                {
                    return NotFound(ApiResponse<Nderhyrje>.ErrorResponse(
                        $"Ndërhyrja me ID {id} nuk u gjet"));
                }

                return Ok(ApiResponse<Nderhyrje>.SuccessResponse(nderhyrje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Nderhyrje>.ErrorResponse(
                    "Gabim në server",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// POST: api/nderhyrje
        /// Krijon ndërhyrje të re
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Nderhyrje>>> Create([FromBody] CreateNderhyrjeDto dto)
        {
            try
            {
                // Kontrollo nëse pajisja ekziston
                var pajisje = await _context.Pajisje.FindAsync(dto.PajisjeId);
                if (pajisje == null)
                {
                    return NotFound(ApiResponse<Nderhyrje>.ErrorResponse(
                        $"Pajisja me ID {dto.PajisjeId} nuk u gjet"));
                }

                // Kontrollo aktin e konstatimit (nëse specifikohet)
                if (dto.AktKonstatimiId.HasValue)
                {
                    var akt = await _context.AktKonstatimi.FindAsync(dto.AktKonstatimiId.Value);
                    if (akt == null)
                    {
                        return NotFound(ApiResponse<Nderhyrje>.ErrorResponse(
                            $"Akt Konstatimi me ID {dto.AktKonstatimiId} nuk u gjet"));
                    }
                }

                var nderhyrje = new Nderhyrje
                {
                    PajisjeId = dto.PajisjeId,
                    AktKonstatimiId = dto.AktKonstatimiId,
                    Titulli = dto.Titulli,
                    Pershkrimi = dto.Pershkrimi,
                    Lloji = dto.Lloji,
                    DataHapjes = DateTime.Now,
                    DataPlanifikuar = dto.DataPlanifikuar,
                    InxhinieriPergjegjës = dto.InxhinieriPergjegjës,
                    Kostoja = dto.Kostoja,
                    KërkonAprovim = dto.KërkonAprovim,
                    Statusi = "Hapur"
                };

                _context.Nderhyrje.Add(nderhyrje);
                await _context.SaveChangesAsync();

                // Ngarko relacionet
                await _context.Entry(nderhyrje).Reference(n => n.Pajisje).LoadAsync();
                if (nderhyrje.AktKonstatimiId.HasValue)
                {
                    await _context.Entry(nderhyrje).Reference(n => n.AktKonstatimi).LoadAsync();
                }

                return CreatedAtAction(nameof(GetById),
                    new { id = nderhyrje.Id },
                    ApiResponse<Nderhyrje>.SuccessResponse(
                        nderhyrje,
                        "Ndërhyrja u krijua me sukses"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Nderhyrje>.ErrorResponse(
                    "Gabim në krijimin e ndërhyrjes",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// PUT: api/nderhyrje/{id}
        /// Përditëson ndërhyrjen
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Nderhyrje>>> Update(int id, [FromBody] UpdateNderhyrjeDto dto)
        {
            try
            {
                var nderhyrje = await _context.Nderhyrje
                    .Include(n => n.Pajisje)
                    .FirstOrDefaultAsync(n => n.Id == id);

                if (nderhyrje == null)
                {
                    return NotFound(ApiResponse<Nderhyrje>.ErrorResponse(
                        $"Ndërhyrja me ID {id} nuk u gjet"));
                }

                // Përditëso fushat që janë specifikuar
                if (!string.IsNullOrEmpty(dto.Statusi))
                    nderhyrje.Statusi = dto.Statusi;

                if (dto.DataFillimit.HasValue)
                    nderhyrje.DataFillimit = dto.DataFillimit.Value;

                if (dto.DataPerfundimit.HasValue)
                {
                    nderhyrje.DataPerfundimit = dto.DataPerfundimit.Value;
                    if (string.IsNullOrEmpty(dto.Statusi))
                    {
                        nderhyrje.Statusi = "Përfunduar";
                    }
                }

                if (!string.IsNullOrEmpty(dto.NotaPerfundimit))
                    nderhyrje.NotaPerfundimit = dto.NotaPerfundimit;

                if (!string.IsNullOrEmpty(dto.InxhinieriPergjegjës))
                    nderhyrje.InxhinieriPergjegjës = dto.InxhinieriPergjegjës;

                if (dto.Kostoja.HasValue)
                    nderhyrje.Kostoja = dto.Kostoja.Value;

                if (!string.IsNullOrEmpty(dto.MaterialetPërdorura))
                    nderhyrje.MaterialetPërdorura = dto.MaterialetPërdorura;

                if (!string.IsNullOrEmpty(dto.AprovuarNga))
                {
                    nderhyrje.AprovuarNga = dto.AprovuarNga;
                    nderhyrje.DataAprovimit = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<Nderhyrje>.SuccessResponse(
                    nderhyrje,
                    "Ndërhyrja u përditësua me sukses"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Nderhyrje>.ErrorResponse(
                    "Gabim në përditësimin e ndërhyrjes",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// PUT: api/nderhyrje/{id}/perfundo
        /// Përfundon ndërhyrjen
        /// </summary>
        [HttpPut("{id}/perfundo")]
        public async Task<ActionResult<ApiResponse<Nderhyrje>>> Perfundo(int id, [FromBody] UpdateNderhyrjeDto dto)
        {
            try
            {
                var nderhyrje = await _context.Nderhyrje
                    .Include(n => n.Pajisje)
                    .FirstOrDefaultAsync(n => n.Id == id);

                if (nderhyrje == null)
                {
                    return NotFound(ApiResponse<Nderhyrje>.ErrorResponse(
                        $"Ndërhyrja me ID {id} nuk u gjet"));
                }

                if (nderhyrje.Statusi == "Përfunduar")
                {
                    return BadRequest(ApiResponse<Nderhyrje>.ErrorResponse(
                        "Kjo ndërhyrje është përfunduar tashmë"));
                }

                nderhyrje.Statusi = "Përfunduar";
                nderhyrje.DataPerfundimit = DateTime.Now;
                nderhyrje.NotaPerfundimit = dto.NotaPerfundimit;
                nderhyrje.Kostoja = dto.Kostoja;
                nderhyrje.MaterialetPërdorura = dto.MaterialetPërdorura;

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<Nderhyrje>.SuccessResponse(
                    nderhyrje,
                    $"Ndërhyrja për pajisjen {nderhyrje.Pajisje?.DeviceId} u përfundua me sukses"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Nderhyrje>.ErrorResponse(
                    "Gabim në përfundimin e ndërhyrjes",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// DELETE: api/nderhyrje/{id}
        /// Fshin ndërhyrjen
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var nderhyrje = await _context.Nderhyrje.FindAsync(id);

                if (nderhyrje == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(
                        $"Ndërhyrja me ID {id} nuk u gjet"));
                }

                _context.Nderhyrje.Remove(nderhyrje);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Ndërhyrja u fshi me sukses"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(
                    "Gabim në fshirjen e ndërhyrjes",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// GET: api/nderhyrje/pajisje/{pajisjeId}
        /// Merr historikun e ndërhyrjeve për një pajisje specifike
        /// </summary>
        [HttpGet("pajisje/{pajisjeId}")]
        public async Task<ActionResult<ApiResponse<List<Nderhyrje>>>> GetByPajisje(int pajisjeId)
        {
            try
            {
                var nderhyrjet = await _context.Nderhyrje
                    .Include(n => n.Pajisje)
                    .Include(n => n.AktKonstatimi)
                    .Where(n => n.PajisjeId == pajisjeId)
                    .OrderByDescending(n => n.DataHapjes)
                    .ToListAsync();

                return Ok(ApiResponse<List<Nderhyrje>>.SuccessResponse(
                    nderhyrjet,
                    $"U gjetën {nderhyrjet.Count} ndërhyrje për këtë pajisje"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Nderhyrje>>.ErrorResponse(
                    "Gabim në server",
                    new List<string> { ex.Message }));
            }
        }
    }
}
