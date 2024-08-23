using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data;
using WebApp.Models;
using System.Threading.Tasks;

namespace WebApp.CreateAccount;

public class CreatePassword : PageModel
{
    private readonly AccountDetailsContext _context;

        public CreatePassword(AccountDetailsContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PasswordDetails userPassword { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.PasswordDetails.Add(userPassword);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
}
