using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using System.Threading.Tasks;
using WebApp.Dto;

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

            accountDetails.CustomerId = GenerateUniqueCustomerId();
            accountDetails.AccountId = GenerateUniqueAccountId();

            _context.AccountDetails.Add(accountDetails);

            Console.WriteLine($"Received data: Username: {accountDetails.Username}, Email: {accountDetails.Email}, Password: {accountDetails.Password}, CreatedAt: {accountDetails.CreatedAt}");

            try
            {
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
                .FirstOrDefaultAsync(u => u.Username == loginDetails.Username && u.Password == loginDetails.Password);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok("Login successful.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccountDetails([FromBody] AccountDetails accountDetails)
        {
            if (accountDetails == null)
            {
                return BadRequest("Invalid data.");
            }

            var existingAccountDetails = await _context.AccountDetails.FirstOrDefaultAsync(u => u.Id == accountDetails.Id);

            if (existingAccountDetails == null)
            {
                return NotFound("Account not found.");
            }

            // Log the incoming data for debugging
            Console.WriteLine($"TwoFactorEnabled: {accountDetails.TwoFactorEnabled}");

            existingAccountDetails.Email = accountDetails.Email;
            existingAccountDetails.Username = accountDetails.Username;
            existingAccountDetails.Password = accountDetails.Password;
            existingAccountDetails.Role = accountDetails.Role;
            existingAccountDetails.DateOfBirth = accountDetails.DateOfBirth;
            existingAccountDetails.Address = accountDetails.Address;
            existingAccountDetails.TwoFactorEnabled = accountDetails.TwoFactorEnabled; // Ensure this is passed correctly
            existingAccountDetails.dev_mode = accountDetails.dev_mode;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingAccountDetails); // Return updated data for debugging
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountDetails(int id)
        {
            var accountDetails = await _context.AccountDetails.FindAsync(id);

            if (accountDetails == null)
            {
                return NotFound("Account not found.");
            }
            else
            {
                return Ok(accountDetails);
            }

        }

        private int GenerateUniqueCustomerId()
        {
            var random = new Random();
            int customerId;
            bool isUnique;

            do
            {
                customerId = random.Next(10000, 99999);
                isUnique = !_context.AccountDetails.Any(u => u.CustomerId == customerId);

            } while (!isUnique);
            return customerId;
        }

        private int GenerateUniqueAccountId()
        {
            var random = new Random();
            int accountId;
            bool isUnique;

            do
            {
                accountId = random.Next(1000, 9999);
                isUnique = !_context.AccountDetails.Any(u => u.AccountId == accountId);

            } while (!isUnique);
            return accountId;
        }

        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
        {
            var user = await _context.AccountDetails.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Validate the old password
            if (user.Password != dto.OldPassword)
            {
                return BadRequest("Old password is incorrect.");
            }

            // Update the password
            user.Password = dto.NewPassword;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
