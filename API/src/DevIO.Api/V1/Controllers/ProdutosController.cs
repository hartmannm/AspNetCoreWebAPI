using AutoMapper;
using DevIO.Api.Controllers;
using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Interfaces;
using DevIO.Business.Interfaces.Repositories;
using DevIO.Business.Interfaces.Services;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DevIO.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/produtos")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutosController(INotificador notificador, IProdutoRepository produtoRepository, IProdutoService produtoService, IMapper mapper, IUser appUser)
            : base(notificador, appUser)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            var produto = await ObterProduto(id);
            if (produto == null) return NotFound();
            return produto;
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            var imagemNome = $"{Guid.NewGuid()}_{produtoViewModel.Imagem}";
            if (!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
            {
                return CustomResponse(produtoViewModel);
            }
            produtoViewModel.Imagem = imagemNome;
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));
            return CustomResponse(produtoViewModel);
        }

        [HttpPost("adicionar")]
        [RequestSizeLimit(4000000)]
        public async Task<ActionResult<ProdutoViewModel>> AdicionarAlternativo([ModelBinder(BinderType = typeof(ProdutoModelBinder))] ProdutoImagemViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            var imgPrefix = $"{Guid.NewGuid()}_";
            if (!await UploadAlternativo(produtoViewModel.ImagemUpload, imgPrefix))
            {
                return CustomResponse(produtoViewModel);
            }
            produtoViewModel.Imagem = imgPrefix + produtoViewModel.ImagemUpload.FileName;
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));
            return CustomResponse(produtoViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Atualizar(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id) return NotFound();
            var produtoAtualizacao = await ObterProduto(id);
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            if (produtoViewModel.Imagem != null)
            {
                var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
                if (!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome)) return CustomResponse(ModelState);
                produtoAtualizacao.Imagem = imagemNome;
            }
            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;
            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));
            return CustomResponse(produtoViewModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {
            var produto = await ObterProduto(id);
            if (produto == null) return NotFound();
            await _produtoService.Remover(id);
            return CustomResponse(_mapper.Map<ProdutoViewModel>(produto));
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
        }

        private bool UploadArquivo(string arquivo, string imgNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }
            var imageDataByteArray = Convert.FromBase64String(arquivo);
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);
            if (System.IO.File.Exists(filepath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }
            System.IO.File.WriteAllBytes(filepath, imageDataByteArray);
            return true;
        }

        private async Task<bool> UploadAlternativo(IFormFile arquivo, string imgPrefix)
        {
            if (arquivo == null || arquivo.Length <= 0)
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefix + arquivo.FileName);
            if (System.IO.File.Exists(path))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }
            return true;
        }
    }
}
