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
[Route("api/V{version:apiVersion}/Order/{orderId:int}/items")]
public class OrderItemController(ShopContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int orderId)
    {
        var orderItems = await context.OrderItems
            .Include(oi => oi.Product)
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();

        return Ok(mapper.Map<IEnumerable<OrderItemDto>>(orderItems));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderItemDto>> GetOrderItem(int orderId, int id)
    {
        var orderItem = await context.OrderItems
            .Include(oi => oi.Product)
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.OrderItemId == id);

        if (orderItem == null)
            return NotFound();

        return Ok(mapper.Map<OrderItemDto>(orderItem));
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderItem), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderItemDto>> CreateOrderItem(int orderId, CreateOrderItemDto createOrderItemDto)
    {
        var order = await context.Orders.FindAsync(orderId);
        if (order == null)
            return BadRequest("Invalid OrderId");

        var product = await context.Products.FindAsync(createOrderItemDto.ProductId);
        if (product == null)
            return BadRequest("Invalid ProductId");

        var orderItem = mapper.Map<OrderItem>(createOrderItemDto);
        orderItem.OrderId = orderId;
        orderItem.Price = product.Price;

        context.OrderItems.Add(orderItem);
        
        order.TotalPrice += orderItem.Price * orderItem.Quantity;

        await context.SaveChangesAsync();
        
        await context.Entry(orderItem)
            .Reference(oi => oi.Product)
            .LoadAsync();

        return CreatedAtAction(nameof(GetOrderItem), 
            new { orderId, id = orderItem.OrderItemId }, 
            mapper.Map<OrderItemDto>(orderItem));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderItem(int orderId, int id, CreateOrderItemDto updateOrderItemDto)
    {
        var orderItem = await context.OrderItems
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.OrderItemId == id);
        if (orderItem == null)
            return NotFound();

        var product = await context.Products.FindAsync(updateOrderItemDto.ProductId);
        if (product == null)
            return BadRequest("Invalid ProductId");

        var oldTotal = orderItem.Price * orderItem.Quantity;
        mapper.Map(updateOrderItemDto, orderItem);
        orderItem.Price = product.Price;

        var order = await context.Orders.FindAsync(orderId);
        order!.TotalPrice = order.TotalPrice - oldTotal + (orderItem.Price * orderItem.Quantity);

        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrderItem(int orderId, int id)
    {
        var orderItem = await context.OrderItems
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.OrderItemId == id);
        if (orderItem == null)
            return NotFound();

        var order = await context.Orders.FindAsync(orderId);
        order!.TotalPrice -= orderItem.Price * orderItem.Quantity;

        context.OrderItems.Remove(orderItem);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
