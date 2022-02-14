using AutoMapper;

using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;

using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class ProdutosController : MainController
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IProdutoService _produtoService;
    private readonly IMapper _mapper;

    public ProdutosController(INotificador notificador, 
                              IProdutoRepository produtoRepository, 
                              IProdutoService produtoService,
                              IMapper mapper) : base(notificador)
    {
        _produtoRepository = produtoRepository;
        _produtoService = produtoService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var produtos = await _produtoRepository.ObterProdutosFornecedores();

        var produtosViewModel = _mapper.Map<IEnumerable<ProdutoViewModel>>(produtos);

        return Ok(produtosViewModel);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId([FromRoute(Name = "id")] Guid id)
    {
        var produtoViewModel = await ObterProduto(id);

        if (produtoViewModel == null)
        {
            return NotFound();
        }

        return Ok(produtoViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Adicionar([FromBody] ProdutoViewModel produtoViewModel)
    {
        if (!ModelState.IsValid)
        {
            return CustomReponse(ModelState);
        }

        var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Image;

        if (!UploadArquivo(produtoViewModel.ImageUpload, imagemNome))
        {
            return CustomReponse(produtoViewModel);
        }

        produtoViewModel.Image = imagemNome;
        var produto = _mapper.Map<Produto>(produtoViewModel);

        await _produtoService.Adicionar(produto);

        return CustomReponse(produtoViewModel);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir([FromRoute(Name = "id")] Guid id)
    {
        var produto = await ObterProduto(id);

        if (produto == null)
        {
            return NotFound();
        }

        await _produtoService.Remover(id);

        return CustomReponse(produto);
    }

    private async Task<ProdutoViewModel> ObterProduto(Guid id)
    {
        var produto = await _produtoRepository.ObterProdutoFornecedor(id);
        var produtoViewModel = _mapper.Map<ProdutoViewModel>(produto);

        return produtoViewModel;
    }

    private bool UploadArquivo(string arquivo, string imgNome)
    {
        if (string.IsNullOrEmpty(arquivo))
        {
            NotificarErro("Forneça uma imagem para este produto!");
            return false;
        }
        
        var imageDataByteArray = Convert.FromBase64String(arquivo);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

        if (System.IO.File.Exists(filePath))
        {
            NotificarErro("Já existe um arquivo com este nome!");
        }
        
        System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

        return true;
    }
}
