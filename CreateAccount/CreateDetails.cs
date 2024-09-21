using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data;
using WebApp.Models;
using System.Threading.Tasks;

namespace WebApp.CreateAccount;

public class CreateDetails : PageModel
{
        private readonly ApplicationDbContext _context;

        public CreateDetails(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AccountDetails UserAccount { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.AccountDetails.Add(UserAccount);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }

