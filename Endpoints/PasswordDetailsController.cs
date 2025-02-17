using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApp.Models;
using WebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using System.Text;

namespace WebApp.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ExportService _exportService;
        private List<string> userChanges = new List<string>(); // for tracking changes to display in Dashboard

        public PasswordDetailsController(ApplicationDbContext context, ExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        [HttpPost]
        public async Task<IActionResult> PostPasswordDetails(int id, [FromBody] PasswordDetails passwordDetails)
        {
            // Kontrola, zda data nejsou null
            if (passwordDetails == null)
            {
                return BadRequest("Invalid data.");
            }

            // Získání AccountDetails na základě AccountDetailsId
            var accountDetails = await _context.AccountDetails.FindAsync(passwordDetails.AccountDetailsId);
            
            // Kontrola existence AccountDetails
            if (accountDetails == null)
            {
                return BadRequest("Invalid AccountDetailsId. The related AccountDetails record does not exist.");
            }

            try
            {
                // Přidání PasswordDetails do databáze
                _context.PasswordDetails.Add(passwordDetails);

                foreach (var change in userChanges)
                {
                    var userChanges = new UserChanges
                    {
                        UserId = id,
                        ChangeType = "Created",
                        ChangedAt = DateTime.UtcNow,
                        Details = change // Text popisující změnu
                    };

                    _context.UserChanges.Add(userChanges);
                }


                await _context.SaveChangesAsync();
                return Ok(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{accountId}")]
        public async Task<ActionResult<List<PasswordDetails>>> GetPasswords(int accountId)
        {
            try
            {
                Console.WriteLine($"Fetching passwords for accountId: {accountId}");

                var passwords = await _context.PasswordDetails
                    .Where(p => p.AccountDetailsId == accountId)
                    .ToListAsync();

                Console.WriteLine($"Passwords retrieved: {passwords.Count}");
                return Ok(passwords);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching passwords: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePasswordDetails(int id)
        {
            try
            {
                var password = await _context.PasswordDetails.FindAsync(id);
                if (password == null)
                {
                    return NotFound();
                }

                _context.PasswordDetails.Remove(password);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteLog/{logId}")]
        public async Task<IActionResult> DeletePasswordLog(int logId)
        {
            try
            {
                var log = await _context.PasswordChanges.FindAsync(logId);
                
                if (log == null)
                {
                    return NotFound();
                }

                _context.PasswordChanges.Remove(log);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting log with Id {logId}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteAllLogs")]
        public async Task<IActionResult> DeleteAllLogs()
        {
            try
            {
                Console.WriteLine("Request received to delete all logs.");
                _context.PasswordChanges.RemoveRange(_context.PasswordChanges);
                await _context.SaveChangesAsync();
                Console.WriteLine("All logs deleted successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting all logs: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] PasswordDetails updatedPassword)
        {
            List<string> changes = new List<string>();
            

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPassword = await _context.PasswordDetails.FindAsync(id);

            if (existingPassword == null)
            {
                return NotFound();
            }

            // Sledování změn před přiřazením nových hodnot
            if (existingPassword.Name != updatedPassword.Name)
            {
                changes.Add($"Name changed from '{existingPassword.Name}' to '{updatedPassword.Name}'");
                existingPassword.Name = updatedPassword.Name;
            }
            if (existingPassword.Email != updatedPassword.Email)
            {
                changes.Add($"Email changed from '{existingPassword.Email}' to '{updatedPassword.Email}'");
                existingPassword.Email = updatedPassword.Email;
            }
            if (existingPassword.Password != updatedPassword.Password)
            {
                changes.Add("Password was updated.");
                existingPassword.Password = updatedPassword.Password; // Heslo neukládej jako text do historie.
            }
            if (existingPassword.Category != updatedPassword.Category)
            {
                changes.Add($"Category changed from '{existingPassword.Category}' to '{updatedPassword.Category}'");
                existingPassword.Category = updatedPassword.Category;
            }
            if (existingPassword.Notes != updatedPassword.Notes)
            {
                changes.Add($"Notes changed from '{existingPassword.Notes}' to '{updatedPassword.Notes}'");
                existingPassword.Notes = updatedPassword.Notes;
            }
            if (existingPassword.CreatedAt != updatedPassword.CreatedAt)
            {
                changes.Add($"CreatedAt changed from '{existingPassword.CreatedAt}' to '{updatedPassword.CreatedAt}'");
                existingPassword.CreatedAt = updatedPassword.CreatedAt;
            }

            // Uložení změn do historie
            foreach (var change in changes)
            {
                var passwordChange = new PasswordChanges
                {
                    PasswordId = id,
                    ActionType = "Edit",
                    ChangedBy = "Admin", // Změna provedena API nebo jiným uživatelem
                    ChangedAt = DateTime.UtcNow,
                    Details = change // Text popisující změnu
                };

                _context.PasswordChanges.Add(passwordChange);
            }

            await _context.SaveChangesAsync();

            return NoContent();
            }

            [HttpGet("{passwordId}/history")]
            public IActionResult GetPasswordHistory(int passwordId)
            {
                var history = _context.PasswordChanges
                    .Where(p => p.PasswordId == passwordId)
                    .OrderByDescending(p => p.ChangedAt)
                    .ToList();

                // Vrátí prázdný seznam, pokud historie není nalezena
                return Ok(history);
            }

        [HttpPut("UpdateOldPasswords")]
        public async Task<IActionResult> UpdateOldPasswords([FromBody] List<PasswordDetails> passwords)
        {
            foreach (var password in passwords)
            {
                var dbPassword = await _context.PasswordDetails.FindAsync(password.Id);
                if (dbPassword != null)
                {
                    dbPassword.IsPasswordOld = password.IsPasswordOld;
                }
            }
            
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/calculate-score")]
        public async Task<IActionResult> CalculatePasswordScore([FromRoute] int id, [FromBody] PasswordDetails passwordDetails)
        {
            if (passwordDetails == null || string.IsNullOrWhiteSpace(passwordDetails.Password))
            {
                return BadRequest("Password is required.");
            }


            int score = 0;

            if (passwordDetails.Password.Length >= 8)
            {
                Console.WriteLine("Adding +25 to score for password length.");
                score += 25;
            }
            if (passwordDetails.Password.Any(char.IsUpper))
            {
                Console.WriteLine("Adding +25 to score for upper case.");
                score += 25;
            }
            if (passwordDetails.Password.Any(char.IsLower))
            {
                Console.WriteLine("Adding +25 to score for lower case.");
                score += 25;
            }
            if (passwordDetails.Password.Any(char.IsDigit))
            {
                Console.WriteLine("Adding +10 to score for password digits.");
                score += 10;
            }
            if (passwordDetails.Password.Any(char.IsPunctuation))
            {
                Console.WriteLine("Adding +15 to score for symbols.");
                score += 15;
            }
            bool isSecure = score >= 50;
            
            var existingPassword = await _context.PasswordDetails.FindAsync(id);


            if (existingPassword == null)
            {
                return NotFound("Password record not found.");
            }

            existingPassword.PasswordScore = score;
            existingPassword.PasswordSecure = isSecure;

            await _context.SaveChangesAsync();
            
            return Ok(score);
        }

        [HttpPut("{id}/check-duplicate")]
        public async Task<IActionResult> CheckDuplicatePassword([FromRoute] int id, [FromBody] PasswordDetails passwordDetails)
        {
            if (passwordDetails == null || string.IsNullOrWhiteSpace(passwordDetails.Password))
            {
                return BadRequest("Password is required.");
            }

            var existingPassword = await _context.PasswordDetails.FindAsync(id);

            if (existingPassword == null)
            {
                return NotFound("Password record not found.");
            }

            var duplicatePassword = await _context.PasswordDetails.FirstOrDefaultAsync(p => p.Password == passwordDetails.Password && p.Id != id);

            if (duplicatePassword != null)
            {
                existingPassword.DuplicatedPassword = true;
            }
            else
            {
                existingPassword.DuplicatedPassword = false;
            }

            await _context.SaveChangesAsync();
            
            return Ok(existingPassword.DuplicatedPassword);
        }

        [HttpGet("Export/{userId}")]
        public async Task<IActionResult> ExportJSONData(int userId)
        {
            try
            {
                Console.WriteLine($"Exporting data for userId: {userId}");
                var jsonData = await _exportService.ExportDataAsync(userId);

                if (string.IsNullOrEmpty(jsonData))
                {
                    return NotFound("No data found for this user.");
                }

                var fileName = $"user_data_{userId}.json";
                var bytes = Encoding.UTF8.GetBytes(jsonData);

                return File(bytes, "application/json", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during export for userId {userId}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("SetFavorite/{id}")]
        public async Task<IActionResult> SetAsFavorite(int id)
        {
            
            try
            {
                var password = await _context.PasswordDetails.FindAsync(id);
                if (password == null)
                {
                    Console.WriteLine("Password not found.");
                    return NotFound();
                }
                Console.WriteLine($"Found password: {password.Name}, IsFavorite: {password.IsFavorite}");

               

                password.IsFavorite = !password.IsFavorite;
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