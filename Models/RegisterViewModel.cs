using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

public class RegisterViewModel
{
    [Required]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    [Required, Compare("Password"), DataType(DataType.Password)]
    public string PasswordConfirm { get; set; }

    [Required]
    public Role Role { get; set; }
}
