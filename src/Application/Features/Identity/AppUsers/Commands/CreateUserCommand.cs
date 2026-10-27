using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.AppUsers.Commands;
public record CreateUserCommand(
    string UserName,
    string Password,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string PhotoUrl,
    bool IsActive,
    List<string>? Roles)
{
}
