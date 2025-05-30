using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using System.Threading.Tasks;
using WebApp.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApp.Security;


namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string encryptionKeyBase64;
        private readonly IConfiguration _configuration;

        public AccountDetailsController(ApplicationDbContext context, IEmailSender emailSender, 
                                        UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration; // Přidat inicializaci
            
            encryptionKeyBase64 = _configuration["Encryption:Key"]; // Teď už to bude fungovat
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

            var user = await _context.AccountDetails.FirstOrDefaultAsync(u => u.Username == loginDetails.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok("Login successful.");
        }


       [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var user = await _context.AccountDetails
                .FirstOrDefaultAsync(u => u.Email == forgotPassword.Email);

            if (user == null)
            {
                // Předstírat, že e-mail byl odeslán, i když uživatel neexistuje
                await Task.Delay(500);
                return Ok("Reset link sent.");
            }


            // Vygeneruj a ulož token
            var token = GenerateResetToken();
            user.PasswordResetToken = token;
            user.TokenExpiry = DateTime.UtcNow.AddHours(1); // Token expiration = 1 Hour

            await _context.SaveChangesAsync();

            var param = new Dictionary<string, string?>
            {
                {"token", token},
                {"email", forgotPassword.Email}
            };

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var callback = QueryHelpers.AddQueryString($"{baseUrl}/Reset-password", param); 


            var subject = "Reset Password";
            var message =$@"
                            <html>
                            <body style='font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; padding: 20px;'>
                                <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); padding: 20px;'>
                                <h2 style='color: #4CAF50;'>Password Reset Request</h2>
                                <p>Hello,</p>
                                <p>We received a request to reset your password for your <strong>Password Manager</strong> account.</p>
                                <p>If you made this request, please click the button below to reset your password:</p>
                                <div style='text-align: center;'>
                                    <a href='{callback}' style='display: inline-block; padding: 10px 20px; background-color: #4CAF50; color: #ffffff; text-decoration: none; font-weight: bold; border-radius: 5px;'>Reset Password</a>
                                </div>
                                <p style='margin-top: 20px;'>If you didn't request a password reset, you can safely ignore this email. Your password will remain unchanged.</p>
                                <p>For security reasons, this link will expire in 1 hour.</p>
                                <hr style='border: none; border-top: 1px solid #dddddd; margin-top: 20px;'>
                                <p>If you have any questions or need further assistance, feel free to contact our support team at 
                                    <a href='mailto:support@passwordmanager.com' style='color: #4CAF50;'>support@passwordmanager.com</a>.
                                </p>
                                <p>Best regards,</p>
                                <p><strong>Password Manager Support Team</strong></p>
                                </div>
                            </body>
                            </html>
                            ";



            await _emailSender.SendEmailAsync(forgotPassword.Email, subject, message);

            return Ok("Reset link sent.");
        }

        



        private string GenerateResetToken()
        {
            return Guid.NewGuid().ToString();
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
            return Ok(accountDetails);
        }

        [HttpGet("{id}/passwords")]
        public async Task<IActionResult> GetPasswords(int id)
        {
            Console.WriteLine($"Received AccountDetailsId: {id}");

            var passwords = await _context.PasswordDetails
                .Where(p => p.AccountDetailsId == id)
                .ToListAsync();

            Console.WriteLine($"Passwords count for AccountDetailsId {id}: {passwords.Count}");

            if (!passwords.Any())
            {
                return NotFound("No passwords found for this account.");
            }

            return Ok(passwords);
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

            // Ověření starého hesla pomocí metody SecretHasher.Verify
            bool isOldPasswordValid = Security.SecretHasher.Verify(user.Password, dto.OldPassword);
            if (!isOldPasswordValid)
            {
                return BadRequest("Old password is incorrect.");
            }

            // Hashování nového hesla
            user.Password = Security.SecretHasher.Hash(dto.NewPassword);

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



        [HttpPut("{id}/update-score")]
        public async Task<IActionResult> UpdateAccountScore([FromRoute] int id, [FromBody] AccountDetails accountDetails)
        {
            var user = await _context.AccountDetails.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.AccountScore = accountDetails.AccountScore;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Score updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("CheckEmail")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var user = await _context.AccountDetails.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                return Ok(true);
            }

            return Ok(false);
        }

        [HttpPut("{id}/UpdateProfileCompletion")]
        public async Task<IActionResult> UpdateAccountCompletion([FromRoute] int id, [FromBody] AccountDetails accountDetails)
        {
            var user = await _context.AccountDetails.FindAsync(id);
            int points = 0;

            if(user == null)
            {
                return NotFound("User not found.");
            }

            if(user.Email != null)
            {
                points += 25;
            }
            if(user.Username != null)
            {
                points += 10;
            }
            if(user.Password != null)
            {
                points += 10;
            }
            if(user.DateOfBirth != null)
            {
                points += 10;
            }
            if(user.Address != null)
            {
                points += 10;
            }
            if(user.TwoFactorEnabled == true)
            {
                points += 25;
            }

            user.AccountPoints = points;
            try
            {
                await _context.SaveChangesAsync();
                return Ok("Profile completion updated successfully.");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            } 


        }

    }
}
