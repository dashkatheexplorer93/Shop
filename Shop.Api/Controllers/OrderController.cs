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
public class OrderController(ShopContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        var orders = await context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();
        
        return Ok(mapper.Map<IEnumerable<OrderDto>>(orders));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound();

        return Ok(mapper.Map<OrderDto>(order));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto createOrderDto)
    {
        if (!await context.Customers.AnyAsync(c => c.CustomerId == createOrderDto.CustomerId))
            return BadRequest("Invalid CustomerId");

        var order = mapper.Map<Order>(createOrderDto);
        
        foreach (var orderItem in order.OrderItems)
        {
            var product = await context.Products.FindAsync(orderItem.ProductId);
            if (product == null)
                return BadRequest($"Product with id {orderItem.ProductId} not found");
                
            orderItem.Price = product.Price;
        }

        order.TotalPrice = order.OrderItems.Sum(item => item.Price * item.Quantity);
        
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        await context.Entry(order)
            .Reference(o => o.Customer)
            .LoadAsync();
        await context.Entry(order)
            .Collection(o => o.OrderItems)
            .Query()
            .Include(oi => oi.Product)
            .LoadAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, 
            mapper.Map<OrderDto>(order));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrder(int id, CreateOrderDto updateOrderDto)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound();
        
        if (!await context.Customers.AnyAsync(c => c.CustomerId == updateOrderDto.CustomerId))
            return BadRequest("Invalid CustomerId");

        context.OrderItems.RemoveRange(order.OrderItems);
        mapper.Map(updateOrderDto, order);
        
        foreach (var orderItem in order.OrderItems)
        {
            var product = await context.Products.FindAsync(orderItem.ProductId);
            if (product == null)
                return BadRequest($"Product with id {orderItem.ProductId} not found");
                
            orderItem.OrderId = id;
            orderItem.Price = product.Price;
        }

        order.TotalPrice = order.OrderItems.Sum(item => item.Price * item.Quantity);
        await context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound();

        context.Orders.Remove(order);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
