using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Caching.Memory;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    public ReservationController(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    
    public async Task<IActionResult> GetReservations([FromQuery] string? filter, [FromQuery] string? sort, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var cacheKey = $"reservations:{filter}:{sort}:{page}:{pageSize}";

        if (_cache.TryGetValue(cacheKey, out object cachedData))
        {
            return Ok(cachedData);
        }

        var query = _context.Reservations
                        .Include(r => r.Suite)
                        .ThenInclude(s => s.Motel)
                        .Include(r => r.Customer)
                        .AsQueryable();

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

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _cache.Set(cacheKey, response, cacheEntryOptions);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> GetReservation(int id)
    {
        var Reservation = await _context.Reservations.FindAsync(id);
        if (Reservation == null)
        {
            return NotFound();
        }
        return Reservation;
    }

    [HttpPost]
    public async Task<ActionResult<Reservation>> PostReservation(Reservation Reservation)
    {
        _context.Reservations.Add(Reservation);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReservation), new { id = Reservation.Id }, Reservation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutReservation(int id, Reservation Reservation)
    {
        if (id != Reservation.Id)
        {
            return BadRequest();
        }
        _context.Entry(Reservation).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var Reservation = await _context.Reservations.FindAsync(id);
        if (Reservation == null)
        {
            return NotFound();
        }
        _context.Reservations.Remove(Reservation);
        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpGet("monthly-billing")]
    public async Task<IActionResult> GetMonthlyRevenue(int year, int month)
    {
        string cacheKey = $"invoicing_{year}_{month}";
        if (!_cache.TryGetValue(cacheKey, out decimal totalRevenue))
        {
            totalRevenue = await _context.Reservations
                .Where(r => r.CheckIn.Year == year && r.CheckIn.Month == month)
                .SumAsync(r => r.Total);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            
            _cache.Set(cacheKey, totalRevenue, cacheOptions);
        }

        return Ok(new { year, month, revenue = totalRevenue });
    }
}
