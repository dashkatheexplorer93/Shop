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
public class CategoryController(ShopContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var categories = await context.Categories
            .Include(c => c.Products)
            .ToListAsync();
    
        return Ok(mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null)
            return NotFound();

        return Ok(mapper.Map<CategoryDto>(category));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Category>> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        var category = mapper.Map<Category>(createCategoryDto);
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var categoryDto = mapper.Map<CategoryDto>(category);
        return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, categoryDto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(int id, CreateCategoryDto updateCategoryDto)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        mapper.Map(updateCategoryDto, category);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null)
            return NotFound();

        if (category.Products.Any())
            return BadRequest("Cannot delete category with associated products");

        context.Categories.Remove(category);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
