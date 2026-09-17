using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniWalletSystemApi.Entities;

public class UserEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    [Required]
    public Guid UserReference { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public int RoleID { get; set; } = 1;

    [Required]
    public bool IsActive { get; set; } = true;

    public string? Notes { get; set; }
}