using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class PasswordTrend
{
    public string Date {get; set;}
    public int SecureCount {get; set;}
    public int WeakCount {get; set;}
}
