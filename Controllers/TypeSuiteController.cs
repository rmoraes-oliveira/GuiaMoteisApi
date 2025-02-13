using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Dynamic.Core;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class TypeSuiteController : ControllerBase
{
    private readonly AppDbContext _context;

    public TypeSuiteController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTypeSuites([FromQuery] string? filter, [FromQuery] string? sort, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var query = _context.TypeSuites.AsQueryable();

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
    public async Task<ActionResult<TypeSuite>> GetTypeSuite(int id)
    {
        var Typesuite = await _context.TypeSuites.FindAsync(id);
        if (Typesuite == null)
        {
            return NotFound();
        }
        return Typesuite;
    }

    [HttpPost]
    public async Task<ActionResult<TypeSuite>> PostTypeSuite(TypeSuite Typesuite)
    {
        _context.TypeSuites.Add(Typesuite);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTypeSuite), new { id = Typesuite.Id }, Typesuite);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTypeSuite(int id, TypeSuite Typesuite)
    {
        if (id != Typesuite.Id)
        {
            return BadRequest();
        }
        _context.Entry(Typesuite).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTypeSuite(int id)
    {
        var Typesuite = await _context.TypeSuites.FindAsync(id);
        if (Typesuite == null)
        {
            return NotFound();
        }
        _context.TypeSuites.Remove(Typesuite);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
