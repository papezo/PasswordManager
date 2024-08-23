using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApp.Models;  
using WebApp.Data;    

namespace WebApp.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordDetailsController : ControllerBase
    {
        private readonly AccountDetailsContext _context;

        public PasswordDetailsController(AccountDetailsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostPasswordDetails([FromBody] PasswordDetails passwordDetails)
        {
            if (passwordDetails == null)
            {
                Console.WriteLine("Received null data.");
                return BadRequest("Invalid data.");
            }

            Console.WriteLine($"Received data: Name={passwordDetails.Name}, Email={passwordDetails.Email}, Password={passwordDetails.Password}, Category={passwordDetails.Category}, Description={passwordDetails.Description}");

            try
            {
                _context.PasswordDetails.Add(passwordDetails);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
