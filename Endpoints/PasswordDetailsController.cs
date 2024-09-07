using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApp.Models;
using WebApp.Data;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<ActionResult<List<PasswordDetails>>> GetPasswords()
        {
            return await _context.PasswordDetails.ToListAsync();
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
        public async Task<IActionResult> UpdatePassword(int id, PasswordDetails updatedPassword)
        {
            var existingPassword = await _context.PasswordDetails.FindAsync(id);
            if (existingPassword == null)
            {
                return NotFound();
            }

            // Aktualizujte data hesla
            existingPassword.Name = updatedPassword.Name;
            existingPassword.Email = updatedPassword.Email;
            existingPassword.Password = updatedPassword.Password;
            existingPassword.Category = updatedPassword.Category;
            existingPassword.Description = updatedPassword.Description;

            await _context.SaveChangesAsync(); // Uložení změn do databáze

            return NoContent();
        }

        private bool PasswordExists(int id)
        {
            return _context.PasswordDetails.Any(e => e.Id == id);
        }
    }
}