using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get api/favorites
        [HttpGet, Authorize]
        public IActionResult Get()
        {
            try
            {
                // Get ID from JWT token
                string userId = User.FindFirstValue("id");

                // Check for unauthenticated users
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Get all of the user's favorites
                var favorites = _context.Favorites.Where(f => f.UserId.Equals(userId));

                return StatusCode(200, favorites);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        //Post api/favorites
        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Favorite favorite)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                if(string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                favorite.UserId = userId;

                _context.Favorites.Add(favorite);
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                _context.SaveChanges();

                return StatusCode(201, favorite);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        //Post api/favorites/l3tSzQEACAAJ
        [HttpDelete("{bookId}"), Authorize]
        public IActionResult Delete(string bookId)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }


                var favorite = _context.Favorites.Where(f => f.BookId == bookId && f.UserId == userId).SingleOrDefault();
                if (favorite != null)
                {
                    _context.Favorites.Remove(favorite);
                    _context.SaveChanges();

                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
