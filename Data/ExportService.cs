using System;
using System.Net.Http.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace WebApp.Data;

public class ExportService
{
    private readonly ApplicationDbContext _context;

    public ExportService(ApplicationDbContext context)
    {
        _context = context;
    }

public async Task<string> ExportDataAsync(int userId)
    {
        // Načte data z databáze
        var userData = await _context.PasswordDetails
            .Where(e => e.Id == userId)
            .ToListAsync();

        // Serializuj data do JSON
        var jsonData = JsonConvert.SerializeObject(userData, Formatting.Indented);

        // Vrať JSON jako string
        return jsonData;
    }

}
