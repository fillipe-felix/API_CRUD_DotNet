using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers;

[Route("api/")]
public class AuthController : MainController
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(INotificador notificador, 
                          SignInManager<IdentityUser> signInManager, 
                          UserManager<IdentityUser> userManager) : base(notificador)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("nova-conta")]
    public async Task<IActionResult> Registrar([FromBody] RegisterUserViewModel registerUser)
    {
        if (!ModelState.IsValid)
        {
            return CustomReponse(ModelState);
        }

        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true // ja garante a confirmação de email
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return CustomReponse(registerUser);
        }

        foreach (var error in result.Errors)
        {
            NotificarErro(error.Description);
        }
        
        return CustomReponse(registerUser);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserViewModel loginUser)
    {
        if (!ModelState.IsValid)
        {
            return CustomReponse(ModelState);
        }

        var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

        if (result.Succeeded)
        {
            return CustomReponse(loginUser);
        }
        
        if (result.IsLockedOut)
        {
            NotificarErro("Usuário temporiariamente bloqueado por tentativas inválidas");
            return CustomReponse(loginUser);
        }
        
        NotificarErro("Usuário ou senha incorretos");
        return CustomReponse(loginUser);
    }
}
