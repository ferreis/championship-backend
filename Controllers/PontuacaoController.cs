// Controllers/PontuacaoController.cs
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

// using SeuProjeto.Models; // Ajuste o namespace
// using SeuProjeto.Dtos;   // Ajuste o namespace
// using SeuProjeto.Services; // Ajuste o namespace

[ApiController]
[Route("api/[controller]")] // Rota base será /api/Pontuacao
public class PontuacaoController : ControllerBase
{
    private readonly PontuacaoService _service;
    private const string ConnectionString = "Data Source=Database/sobrasa_banco_de_dados.db"; // Sua string de conexão

    public PontuacaoController()
    {
        _service = new PontuacaoService(ConnectionString); // Instância direta, como no ProvaController
    }

    /// <summary>
    /// Retorna uma lista de todas as pontuações/participações.
    /// GET /api/Pontuacao
    /// </summary>
    [HttpGet]
    public ActionResult<List<Pontuacao>> Get()
    {
        var pontuacoes = _service.Listar();
        if (pontuacoes == null || pontuacoes.Count == 0)
        {
            return NotFound("Nenhuma pontuação encontrada.");
        }
        return Ok(pontuacoes);
    }

    /// <summary>
    /// Obtém uma pontuação/participação específica pelo seu ID.
    /// GET /api/Pontuacao/{id}
    /// </summary>
    /// <param name="id">O ID da pontuação.</param>
    [HttpGet("{id}")]
    public ActionResult<Pontuacao> GetById(int id)
    {
        var pontuacao = _service.ObterPorId(id);
        if (pontuacao == null)
        {
            return NotFound($"Pontuação com ID {id} não encontrada.");
        }
        return Ok(pontuacao);
    }

    /// <summary>
    /// Adiciona uma nova pontuação/participação.
    /// POST /api/Pontuacao
    /// </summary>
    /// <param name="pontuacaoDto">Os dados da pontuação a ser adicionada.</param>
    [HttpPost]
    public IActionResult Post([FromBody] PontuacaoDto pontuacaoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var novaPontuacao = new Pontuacao
        {
            ParticipacaoProvaId = pontuacaoDto.ParticipacaoProvaId,
            Pontos = pontuacaoDto.Pontos
        };

        try
        {
            _service.Adicionar(novaPontuacao);
            // Em SQLite, o ID só é gerado após a inserção. Se precisar, use LastInsertRowId no service.
            // Por enquanto, não podemos retornar o ID exato aqui sem um método de serviço que o retorne.
            return CreatedAtAction(nameof(GetById), new { id = novaPontuacao.Id }, novaPontuacao); // novaPontuacao.Id pode ser 0 ou o valor padrão.
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao adicionar pontuação: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza parcialmente os dados de uma pontuação/participação existente.
    /// PATCH /api/Pontuacao
    /// </summary>
    /// <param name="pontuacaoDto">Os dados da pontuação a serem atualizados.</param>
    [HttpPatch]
    public IActionResult Patch([FromBody] PontuacaoUpdateDto pontuacaoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existente = _service.ObterPorId(pontuacaoDto.Id);
        if (existente == null)
        {
            return NotFound($"Pontuação com ID {pontuacaoDto.Id} não encontrada.");
        }

        if (pontuacaoDto.ParticipacaoProvaId.HasValue) existente.ParticipacaoProvaId = pontuacaoDto.ParticipacaoProvaId.Value;
        if (pontuacaoDto.Pontos.HasValue) existente.Pontos = pontuacaoDto.Pontos.Value;

        try
        {
            _service.Atualizar(existente);
            return Ok(existente);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao atualizar pontuação: {ex.Message}");
        }
    }

    /// <summary>
    /// Deleta uma pontuação/participação pelo seu ID.
    /// DELETE /api/Pontuacao/{id}
    /// </summary>
    /// <param name="id">O ID da pontuação a ser deletada.</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var pontuacao = _service.ObterPorId(id);
        if (pontuacao == null)
        {
            return NotFound($"Pontuação com ID {id} não encontrada para exclusão.");
        }

        try
        {
            _service.Deletar(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao deletar pontuação: {ex.Message}");
        }
    }
}