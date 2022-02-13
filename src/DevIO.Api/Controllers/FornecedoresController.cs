using AutoMapper;

using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;

using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers;

[Route("api/[controller]")]
public class FornecedoresController : MainController
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IFornecedorService _fornecedorService;
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly IMapper _mapper;

    protected FornecedoresController(IFornecedorRepository fornecedorRepository, 
                                     IMapper mapper, IFornecedorService fornecedorService,
                                     INotificador notificador,
                                     IEnderecoRepository enderecoRepository) : base(notificador)
    {
        _fornecedorRepository = fornecedorRepository;
        _mapper = mapper;
        _fornecedorService = fornecedorService;
        _enderecoRepository = enderecoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var fornecedor = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

        return Ok(fornecedor);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId([FromRoute(Name = "id")] Guid id)
    {
        var fornecedor = await ObterFornecedorProdutosEndereco(id);

        if (fornecedor == null)
        {
            return NotFound();
        }

        return Ok(fornecedor);
    }

    [HttpPost]
    public async Task<IActionResult> Adicionar([FromBody] FornecedorViewModel fornecedorViewModel)
    {
        if (!ModelState.IsValid)
        {
            return CustomReponse(ModelState);
        }

        var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
        await _fornecedorService.Adicionar(fornecedor);

        return CustomReponse(fornecedorViewModel);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar([FromRoute(Name = "id")] Guid id, 
                                               [FromBody] FornecedorViewModel fornecedorViewModel)
    {
        if (id != fornecedorViewModel.Id)
        {
            NotificarErro("O id informado não é o mesmo que foi passado na query");
            return CustomReponse(fornecedorViewModel);
        }
        
        if (!ModelState.IsValid)
        {
            return CustomReponse(ModelState);
        }

        var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
        await _fornecedorService.Atualizar(fornecedor);
        
        return CustomReponse(fornecedorViewModel);;
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir([FromRoute(Name = "id")] Guid id)
    {
        var fornecedorViewModel = await ObterFornecedoEndereco(id);

        if (fornecedorViewModel == null)
        {
            return NotFound();
        }

        await _fornecedorService.Remover(id);

        return CustomReponse();
    }
    
    [HttpGet("obter-endereco/{id:guid}")]
    public async Task<IActionResult> ObterEnderecoPorId([FromRoute(Name = "id")]Guid id)
    {
        var enderecoViewModel = _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));
        return Ok(enderecoViewModel);
    }
    
    [HttpPut("atualizar-endereco/{id:guid}")]
    public async Task<IActionResult> AtualizarEndereco([FromRoute(Name = "id")] Guid id, 
                                               [FromBody] EnderecoViewModel enderecoViewModel)
    {
        if (id != enderecoViewModel.Id)
        {
            NotificarErro("O id informado não é o mesmo que foi passado na query");
            return CustomReponse(enderecoViewModel);
        }
        
        if (!ModelState.IsValid)
        {
            return CustomReponse(ModelState);
        }

        var fornecedor = _mapper.Map<Endereco>(enderecoViewModel);
        await _fornecedorService.AtualizarEndereco(fornecedor);
        
        return CustomReponse(enderecoViewModel);;
    }

    public async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
    {
        return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
    }
    
    public async Task<FornecedorViewModel> ObterFornecedoEndereco(Guid id)
    {
        return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
    }
}