using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

public class UserChanges
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int logId { get; set; }
    public int UserId { get; set; }
    public string? ChangeType { get; set; }   
    public DateTime ChangedAt {get; set;} = DateTime.Now;
    public string? Details {get; set;}
}
