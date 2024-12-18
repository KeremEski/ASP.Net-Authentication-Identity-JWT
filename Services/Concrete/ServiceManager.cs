using Authentication.Services.Contracts;

namespace Authentication.Services.Concrete;

public class ServiceManager : IServiceManager
{   
    private readonly ITokenService _tokenService;
    public ServiceManager(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public ITokenService TokenService => _tokenService;
}