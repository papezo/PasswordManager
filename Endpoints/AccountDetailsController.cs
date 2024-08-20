using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountDetailsController : ControllerBase
    {
        private readonly AccountDetailsContext _context;

        public AccountDetailsController(AccountDetailsContext context)
        {
            _context = context;
        }

        // POST: api/AccountDetails
        [HttpPost]
        public async Task<IActionResult> PostAccountDetails([FromBody] AccountDetails accountDetails)
        {
            if (accountDetails == null)
            {
                Console.WriteLine("Received null data.");
                return BadRequest("Invalid data.");
            }

            Console.WriteLine($"Received data: Name={accountDetails.Name}, Email={accountDetails.Email}, Password={accountDetails.Password}");

            try
            {
                _context.AccountDetails.Add(accountDetails);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/AccountDetails/authenticate
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDetails loginDetails)
        {
            if (loginDetails == null)
            {
                return BadRequest("Invalid login details.");
            }

            var user = await _context.AccountDetails
                .FirstOrDefaultAsync(u => u.Name == loginDetails.Username && u.Password == loginDetails.Password);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            
            return Ok("Login successful.");
        }
    }
}
