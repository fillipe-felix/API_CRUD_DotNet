using AutoMapper;

using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;

using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers;

[Route("api/[controller]")]
public class FornecedoresController : MainController
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IMapper _mapper;

    protected FornecedoresController(IFornecedorRepository fornecedorRepository, IMapper mapper)
    {
        _fornecedorRepository = fornecedorRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var fornecedor = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

        return Ok(fornecedor);
    }
}