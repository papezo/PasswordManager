using System;

namespace WebApp.Dto;

public class ChangePasswordDto
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public DateTime DateOfChange {get; set; } = DateTime.Now;
    public bool HasBeenChangedThisMonth { get; set; } = false;
}
