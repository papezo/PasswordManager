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
        public async Task<ActionResult<IEnumerable<PasswordDetails>>> GetPasswordDetails()
        {
            try
            {
                var passwords = await _context.PasswordDetails.ToListAsync();
                return Ok(passwords);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPasswordDetails(int id, [FromBody] PasswordDetails passwordDetails)
        {
            if (id != passwordDetails.Id)
            {
                return BadRequest("ID mismatch.");
            }

            var existingPassword = await _context.PasswordDetails.FindAsync(id);
            if (existingPassword == null)
            {
                return NotFound();
            }

            existingPassword.Name = passwordDetails.Name;
            existingPassword.Email = passwordDetails.Email;
            existingPassword.Password = passwordDetails.Password;
            existingPassword.Category = passwordDetails.Category;
            existingPassword.Description = passwordDetails.Description;
            existingPassword.CreatedAt = passwordDetails.CreatedAt; 

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PasswordDetailsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        private bool PasswordDetailsExists(int id)
        {
            return _context.PasswordDetails.Any(e => e.Id == id);
        }
    }
}
