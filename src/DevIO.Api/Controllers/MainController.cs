using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevIO.Api.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    private readonly INotificador _notificador;
    public readonly IUser AppUser;
    
    protected Guid UsuarioId { get; set; }
    protected bool UsuarioAutenticado { get; set; }
    
    protected MainController(INotificador notificador, IUser appUser)
    {
        _notificador = notificador;
        AppUser = appUser;

        if (appUser.IsAuthenticated())
        {
            UsuarioId = appUser.GetUserId();
            UsuarioAutenticado = true;
        }
    }

    protected bool OperacaoValida()
    {
        return !_notificador.TemNotificacao();
    }

    protected IActionResult CustomReponse(object result = null)
    {
        if (OperacaoValida())
        {
            return Ok(new
            {
                success = true,
                data = result
            });
        }

        return BadRequest(new
        {
            success = false,
            errors = _notificador.ObterNotificacoes().Select(n => n.Mensagem)
        });
    }

    protected IActionResult CustomReponse(ModelStateDictionary modelState)
    {
        if (!modelState.IsValid)
        {
            NotificarErroModelInvalida(modelState);
        }

        return CustomReponse();
    }

    protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
    {
        var erros = modelState.Values.SelectMany(e => e.Errors);

        foreach (var error in erros)
        {
            var errorMsg = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
            NotificarErro(errorMsg);
        }
    }

    protected void NotificarErro(string mengsagem)
    {
        _notificador.Handle(new Notificacao(mengsagem));
    }
}