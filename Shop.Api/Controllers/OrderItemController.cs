using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Data.Entities;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemController(ShopContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
    {
        return await context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.Order)
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
    {
        var orderItem = await context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.Order)
            .FirstOrDefaultAsync(oi => oi.OrderItemId == id);

        return orderItem == null ? NotFound() : orderItem;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderItem), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderItem>> CreateOrderItem(OrderItem orderItem)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate that referenced Product and Order exist
        if (!await context.Products.AnyAsync(p => p.ProductId == orderItem.ProductId))
            return BadRequest("Invalid ProductId");

        if (!await context.Orders.AnyAsync(o => o.OrderId == orderItem.OrderId))
            return BadRequest("Invalid OrderId");

        context.OrderItems.Add(orderItem);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.OrderItemId }, orderItem);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderItem(int id, OrderItem orderItem)
    {
        if (id != orderItem.OrderItemId)
            return BadRequest();

        // Validate that referenced Product and Order exist
        if (!await context.Products.AnyAsync(p => p.ProductId == orderItem.ProductId))
            return BadRequest("Invalid ProductId");

        if (!await context.Orders.AnyAsync(o => o.OrderId == orderItem.OrderId))
            return BadRequest("Invalid OrderId");

        context.Entry(orderItem).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await context.OrderItems.AnyAsync(oi => oi.OrderItemId == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrderItem(int id)
    {
        var orderItem = await context.OrderItems.FindAsync(id);
        if (orderItem == null)
            return NotFound();

        context.OrderItems.Remove(orderItem);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
