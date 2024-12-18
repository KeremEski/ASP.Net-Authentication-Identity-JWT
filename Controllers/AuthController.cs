using System.Net.NetworkInformation;
using Authentication.Models;
using Authentication.Models.Dtos;
using Authentication.Services.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Controllers;

// Important: This is the AuthController responsible for authentication-related endpoints.
[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IServiceManager _service;
    private readonly SignInManager<User> _signInManager;

    // Important: Constructor injecting required services.
    public AuthController(IMapper mapper, UserManager<User> userManager, IServiceManager service, SignInManager<User> signInManager)
    {
        _mapper = mapper;
        _userManager = userManager;
        _service = service;
        _signInManager = signInManager;
    }

    // Important: Endpoint for user registration.
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
                // Not Important: Return bad request if the model state is invalid.
                return BadRequest(ModelState);

            // Important: Map RegisterDto to User model.
            // You can create manually.
            User user = _mapper.Map<User>(registerDto);
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                // Important: Assign the "User" role to the newly created user.
                var roleResult = await _userManager.AddToRoleAsync(user, "User");

                if (roleResult.Succeeded)
                {   
                    // Important: Return success response with user details and token.
                    return Ok(new RegisteredUserDto
                    {
                        Email = registerDto.Email,
                        UserName = registerDto.UserName,
                        Token = _service.TokenService.CreateToken(user)
                    });
                }
                // Not Important: Return error if role assignment fails.
                return StatusCode(500, roleResult.Errors);
            }
            // Not Important: Return error if user creation fails.
            return StatusCode(500, result.Errors);
        }
        catch (Exception e)
        {
            // Not Important: Return internal server error for exceptions.
            return StatusCode(500, e.Message);
        }
    }

    // Important: Endpoint for user login.
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        // Not Important: Validate the model state before processing.
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Important: Try to find the user by email or username.
        var user = await _userManager.FindByEmailAsync(loginDto.UserName);
        if (user == null)
        {
            user = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == loginDto.UserName.ToUpper());
        }

        if (user == null)
            // Important: Return unauthorized if the user is not found.
            return Unauthorized("Invalid Username or Mail");

        // Important: Check if the provided password is correct.
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded)
            // Important: Return unauthorized if the password is incorrect.
            return Unauthorized("Wrong Password");

        // Important: Return success response with user details and token.
        return Ok(new RegisteredUserDto
        {
            Email = user.Email!,
            UserName = user.UserName!,
            Token = _service.TokenService.CreateToken(user)
        });
    }
}