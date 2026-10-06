using System.ComponentModel.DataAnnotations;

namespace MiniWalletSystemApi.Payloads.Requests;

public class NewUserRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Valid email address is required.")]
    public string Email { get; set; } = string.Empty;

}