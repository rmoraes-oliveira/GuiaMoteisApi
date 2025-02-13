using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Dynamic.Core;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class SuiteController : ControllerBase
{
    private readonly AppDbContext _context;

    public SuiteController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetSuites([FromQuery] string? filter, [FromQuery] string? sort, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var query = _context.Suites.Include(s => s.Motel).Include(s => s.TypeSuite).AsQueryable();

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
    public async Task<ActionResult<Suite>> GetSuite(int id)
    {
        var suite = await _context.Suites
            .Include(s => s.Motel)
            .Include(s => s.TypeSuite)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (suite == null)
        {
            return NotFound();
        }

        var result = new
        {
            suite.Id,
            suite.Name,
            suite.Capacity,
            suite.Price,
            suite.MotelId,
            Motel = suite.Motel == null ? null : new
            {
                suite.Motel.Id,
                suite.Motel.Name,
                suite.Motel.Address,
                suite.Motel.Phone
            },
            suite.TypeSuiteId,
            TypeSuite = suite.TypeSuite == null ? null : new
            {
                suite.TypeSuite.Id,
                suite.TypeSuite.Description,
                suite.TypeSuite.Price
            }
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Suite>> PostSuite(Suite suite)
    {
        _context.Suites.Add(suite);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSuite), new { id = suite.Id }, suite);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSuite(int id, Suite suite)
    {
        if (id != suite.Id)
        {
            return BadRequest();
        }
        _context.Entry(suite).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSuite(int id)
    {
        var suite = await _context.Suites.FindAsync(id);
        if (suite == null)
        {
            return NotFound();
        }
        _context.Suites.Remove(suite);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
