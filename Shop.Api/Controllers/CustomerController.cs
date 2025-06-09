using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Contract.Requests;
using Shop.Contract.Responses;
using Shop.Data;
using Shop.Data.Entities;

namespace Shop.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/V{version:apiVersion}/[controller]")]
public class CustomerController(ShopContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Customer>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        var customers = await context.Customers.ToListAsync();
        return Ok(mapper.Map<IEnumerable<CustomerDto>>(customers));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
        if (customer == null)
            return NotFound();
        
        return Ok(mapper.Map<CustomerDto>(customer));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Customer>> CreateCustomer(CreateCustomerDto createCustomerDto)
    {
        var customer = mapper.Map<Customer>(createCustomerDto);
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var customerDto = mapper.Map<CustomerDto>(customer);
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customerDto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCustomer(int id, CreateCustomerDto updateCustomerDto)
    {
        var customer = await context.Customers.FindAsync(id);
        if (customer == null)
            return NotFound();

        mapper.Map(updateCustomerDto, customer);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await context.Customers.FindAsync(id);
        
        if (customer == null)
            return NotFound();

        context.Customers.Remove(customer);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
