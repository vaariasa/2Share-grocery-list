using System.Security.Claims;
using GroceryList.Application.DTOs;
using GroceryList.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace GroceryList.Api.Controllers;

/// <summary>
/// Grocery list endpoints (user stories 3 to 7).
/// [Authorize] means every request needs a valid token, otherwise the API returns 401 (user story 9).
/// </summary>
[ApiController]
[Authorize]
[Route("api/grocery-items")]
public class GroceryItemsController(GroceryItemService groceryItemService) : ControllerBase
{
    // The user id always comes from the validated token, never from the request body or URL.
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    public async Task<ActionResult<List<GroceryItemResponse>>> GetAll()
    {
        return Ok(await groceryItemService.GetAllAsync(CurrentUserId));
    }

    [HttpPost]
    public async Task<ActionResult<GroceryItemResponse>> Add(CreateGroceryItemRequest request)
    {
        var item = await groceryItemService.AddAsync(CurrentUserId, request);
        return StatusCode(StatusCodes.Status201Created, item);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await groceryItemService.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }

    /// <summary>Tag (isInBasket = true) or un-tag (isInBasket = false) an item.</summary>
    [HttpPatch("{id:guid}/basket")]
    public async Task<ActionResult<GroceryItemResponse>> UpdateBasket(Guid id, UpdateBasketRequest request)
    {
        return Ok(await groceryItemService.SetInBasketAsync(CurrentUserId, id, request.IsInBasket));
    }
}
