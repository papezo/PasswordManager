using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Models;

namespace WebApp.Models;

public class PasswordChanges
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int PasswordId {get; set;}
    public string? ActionType {get; set;}
    public string? OldValue {get; set;}
    public string? NewValue {get; set;}
    public string? ChangedBy {get; set;}
    public DateTime ChangedAt {get; set;} = DateTime.Now;


}
