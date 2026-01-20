using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.DTOs.Favorite;
using WeatherApi.Services;
using System.Security.Claims;
using WeatherApi.Helpers;

namespace WeatherApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/favorites")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _service;

        public FavoritesController(IFavoriteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            var favorites = await _service.GetFavorites(userId);
            return Ok(favorites);
        }
        

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FavoriteCreateDto dto)
        {
            var userId = GetUserId();

            var result = await _service.AddFavorite(dto, userId);

            if (result == null)
                return Conflict(new { message = "Esta cidade já está nos seus favoritos." });

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            await _service.RemoveFavorite(id, userId);
            return NoContent();
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
