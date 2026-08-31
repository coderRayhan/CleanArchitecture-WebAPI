using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Models;
public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    public bool IsExpired => DateTime.Now >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => !IsExpired && !IsRevoked;
    public string ApplicationUserId { get; set; } = string.Empty;
}
