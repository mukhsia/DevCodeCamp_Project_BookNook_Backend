using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FullStackAuth_WebAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // api/bookdetails/l3tSzQEACAAJ
        [HttpGet("{bookId}")]
        public IActionResult GetBookDetails(string bookId)
        {
            try
            {
                // For Favorited by logged-in user
                string userId = User.FindFirstValue("id");
                var isFavorite = false;

                if (userId != null)
                {
                    isFavorite = _context.Favorites.Any(f => f.BookId == bookId && f.UserId == userId);
                }

                var reviews = _context.Reviews.Where(r => r.BookId == bookId);

                // If reviews exists
                if (!reviews.IsNullOrEmpty())
                {
                    var bookDetails = new BookDetailsDto
                    {
                        Reviews = reviews.Select(r => new ReviewWithUserDto
                        {
                            Id = r.Id,
                            BookId = r.BookId,
                            Text = r.Text,
                            Rating = r.Rating,
                            Owner = new UserForDisplayDto
                            {
                                Id = r.User.Id,
                                FirstName = r.User.FirstName,
                                LastName = r.User.LastName,
                                UserName = r.User.UserName,
                            },

                        }).ToList(),
                        Average = reviews.Average(r => r.Rating),
                        Favorited = isFavorite,
                    };

                    return StatusCode(200, bookDetails);
                }
                else
                {
                    var bookDetails = new BookDetailsDto
                    {
                        Reviews = new List<ReviewWithUserDto>(),
                        Average = 0,
                        Favorited = isFavorite,
                    };

                    return StatusCode(200, bookDetails);
                }

            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }
    }
}
