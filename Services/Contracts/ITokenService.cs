using System.Security.Claims;
using Authentication.Models;

namespace Authentication.Services.Contracts;
public interface ITokenService
{
    string CreateToken(User user);
    
}