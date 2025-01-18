using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApp.Models;
using WebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Azure.Identity;

namespace WebApp.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PasswordDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostPasswordDetails([FromBody] PasswordDetails passwordDetails)
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
            return await _context.PasswordDetails.Where(p => p.AccountDetailsId == accountId).ToListAsync();
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] PasswordDetails updatedPassword)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState); 
            }

            var existingPassword = await _context.PasswordDetails.FindAsync(id);
            
            if (existingPassword == null)
            {
                return NotFound();
            }

            existingPassword.Name = updatedPassword.Name;
            if(existingPassword.Password != updatedPassword.Password)
            {
                
            }
            existingPassword.Email = updatedPassword.Email;
            existingPassword.Password = updatedPassword.Password;
            existingPassword.Category = updatedPassword.Category;
            existingPassword.Notes = updatedPassword.Notes;
            existingPassword.CreatedAt = updatedPassword.CreatedAt;
            existingPassword.PasswordScore = updatedPassword.PasswordScore;

            await _context.SaveChangesAsync(); // Uložení změn do databáze

            return NoContent();
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

            if (passwordDetails.Password.Length >= 8 && passwordDetails.Password.Length <= 14)
                score += 25;

            if (passwordDetails.Password.Any(char.IsUpper))
                score += 25;

            if (passwordDetails.Password.Any(char.IsLower))
                score += 25;

            if (passwordDetails.Password.Any(char.IsDigit))
                score += 10;

            if (passwordDetails.Password.Any(char.IsSymbol))
                score += 15;

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
    }

}