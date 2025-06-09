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
public class ProductController(ShopContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await context.Products
            .Include(p => p.Category)
            .ToListAsync();
    
        return Ok(mapper.Map<IEnumerable<ProductDto>>(products));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
            return NotFound();

        return Ok(mapper.Map<ProductDto>(product));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> CreateProduct(CreateProductDto createProductDto
    )
    {
        if (!await context.Categories.AnyAsync(c => c.CategoryId == createProductDto.CategoryId))
            return BadRequest("Invalid CategoryId");

        var product = mapper.Map<Product>(createProductDto);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        await context.Entry(product)
            .Reference(p => p.Category)
            .LoadAsync();

        var productDto = mapper.Map<ProductDto>(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, productDto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, CreateProductDto updateProductDto)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        if (!await context.Categories.AnyAsync(c => c.CategoryId == updateProductDto.CategoryId))
            return BadRequest("Invalid CategoryId");

        mapper.Map(updateProductDto, product);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        context.Products.Remove(product);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
