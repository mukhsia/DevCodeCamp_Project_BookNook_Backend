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
                if (string.IsNullOrEmpty(userId))
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

        // PUT api/reviews/1
        [HttpPut("{id}"), Authorize]
        public IActionResult Put(int id, [FromBody] Review review)
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

                var myReview = _context.Reviews.Where(r => r.Id == id && userId == r.UserId).SingleOrDefault();
                if (myReview != null)
                {
                    myReview.Id = id;
                    myReview.BookId = review.BookId;
                    myReview.Text = review.Text;
                    myReview.Rating = review.Rating;

                    _context.Reviews.Update(myReview);
                    _context.SaveChanges();

                    return Ok(myReview);
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


        // DELETE api/reviews/1
        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }


                var review = _context.Reviews.Where(f => f.Id == id && f.UserId == userId).SingleOrDefault();
                if (review != null)
                {
                    _context.Reviews.Remove(review);
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
