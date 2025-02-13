using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Dynamic.Core;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomerController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] string? filter, [FromQuery] string? sort, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var query = _context.Customers.AsQueryable();

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
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var Customer = await _context.Customers.FindAsync(id);
        if (Customer == null)
        {
            return NotFound();
        }
        return Customer;
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> PostCustomer(Customer Customer)
    {
        _context.Customers.Add(Customer);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCustomer), new { id = Customer.Id }, Customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomer(int id, Customer Customer)
    {
        if (id != Customer.Id)
        {
            return BadRequest();
        }
        _context.Entry(Customer).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var Customer = await _context.Customers.FindAsync(id);
        if (Customer == null)
        {
            return NotFound();
        }
        _context.Customers.Remove(Customer);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
