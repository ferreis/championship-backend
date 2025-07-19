// Controllers/CompeticaoController.cs
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CompeticaoController : ControllerBase
{
    private readonly CompeticaoService _service;
    private const string ConnectionString = "Data Source=Database/sobrasa_banco_de_dados.db"; // Sua string de conexão

    public CompeticaoController()
    {
        _service = new CompeticaoService(ConnectionString);
    }

    [HttpGet]
    public ActionResult<List<Competicao>> Get()
    {
        var competicoes = _service.Listar();
        if (competicoes == null || competicoes.Count == 0)
        {
            return NotFound("Nenhuma competição encontrada.");
        }
        return Ok(competicoes);
    }

    [HttpGet("{id}")]
    public ActionResult<Competicao> GetById(int id)
    {
        var competicao = _service.ObterPorId(id);
        if (competicao == null)
        {
            return NotFound($"Competição com ID {id} não encontrada.");
        }
        return Ok(competicao);
    }

    [HttpPost]
    public IActionResult Post([FromBody] CompeticaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Adicionar validação para verificar se PaisId, EstadoId e CidadeId existem
            // TODO: Validação para data_inicio <= data_fim

            var newId = _service.Adicionar(dto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, _service.ObterPorId(newId));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno ao adicionar competição.", detalhes = ex.Message });
        }
    }

    [HttpPatch]
    public IActionResult Patch([FromBody] CompeticaoUpdateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existente = _service.ObterPorId(dto.Id);
            if (existente == null)
            {
                return NotFound($"Competição com ID {dto.Id} não encontrada.");
            }

            // TODO: Adicionar validação para verificar se PaisId, EstadoId e CidadeId existem (se forem atualizados)

            _service.Atualizar(dto);
            return Ok(_service.ObterPorId(dto.Id));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno ao atualizar competição.", detalhes = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var competicao = _service.ObterPorId(id);
            if (competicao == null)
            {
                return NotFound($"Competição com ID {id} não encontrada.");
            }

            _service.Deletar(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno ao deletar competição.", detalhes = ex.Message });
        }
    }
    [HttpGet("{id}/detalhes")]
    public ActionResult<CompeticaoDetalhesDto> GetDetalhes(int id)
    {
        var detalhes = _service.ObterDetalhesDaCompeticao(id);
        if (detalhes == null)
        {
            return NotFound($"Competição com ID {id} não encontrada.");
        }
        return Ok(detalhes);
    }
}