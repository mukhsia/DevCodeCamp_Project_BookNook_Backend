using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST api/reviews
        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Review review)
        {
            try
            {
                // Get User Id from JWT Token
                string userId = User.FindFirstValue("id");

                // Authentication check
                if (userId == null)
                {
                    return Unauthorized();
                }

                review.UserId = userId;

                _context.Reviews.Add(review);
                // Invalid model state check
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _context.SaveChanges();

                return StatusCode(201, review);

            } 
            catch (Exception e) 
            { 
                return StatusCode(500, e.Message);
            }

        }
    }
}
