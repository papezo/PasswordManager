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
    }
}
