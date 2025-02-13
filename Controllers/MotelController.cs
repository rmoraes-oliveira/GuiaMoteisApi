using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Dynamic.Core;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class MotelController : ControllerBase
{
    private readonly AppDbContext _context;

    public MotelController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMotels([FromQuery] string? filter, [FromQuery] string? sort, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var query = _context.Motels.Include(s => s.Suites).AsQueryable();

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(sort))
        {
            query = query.OrderBy(sort);
        }

        int pageNumber = page ?? 1;
        int size = pageSize ?? 10;
        int totalRecords = await query.CountAsync();

        var totalPages = (int)Math.Ceiling((double)totalRecords / size);
        var paginatedData = await query.Skip((pageNumber - 1) * size).Take(size).ToListAsync();

        var response = new
        {
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = size,
            TotalPages = totalPages,
            Data = paginatedData
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Motel>> GetMotel(int id)
    {
        var Motel = await _context.Motels
            .Include(s => s.Suites)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (Motel == null)
        {
            return NotFound();
        }
        return Motel;
    }

    [HttpPost]
    public async Task<ActionResult<Motel>> PostMotel(Motel Motel)
    {
        _context.Motels.Add(Motel);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMotel), new { id = Motel.Id }, Motel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutMotel(int id, Motel Motel)
    {
        if (id != Motel.Id)
        {
            return BadRequest();
        }
        _context.Entry(Motel).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMotel(int id)
    {
        var Motel = await _context.Motels.FindAsync(id);
        if (Motel == null)
        {
            return NotFound();
        }
        _context.Motels.Remove(Motel);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
