// Controllers/ParticipacaoProvaController.cs
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ParticipacaoProvaController : ControllerBase // Ou BaseController<ParticipacaoProvaService>
{
    private readonly ParticipacaoProvaService _service;
    private const string ConnectionString = "Data Source=Database/sobrasa_banco_de_dados.db"; // Sua string de conexão

    public ParticipacaoProvaController()
    {
        _service = new ParticipacaoProvaService(ConnectionString);
    }

    [HttpGet]
    public ActionResult<List<ParticipacaoProva>> Get()
    {
        var participacoes = _service.Listar();
        if (participacoes == null || participacoes.Count == 0)
        {
            return NotFound("Nenhuma participação em prova encontrada.");
        }
        return Ok(participacoes);
    }

    [HttpGet("{id}")]
    public ActionResult<ParticipacaoProva> GetById(int id)
    {
        var participacao = _service.ObterPorId(id);
        if (participacao == null)
        {
            return NotFound($"Participação em prova com ID {id} não encontrada.");
        }
        return Ok(participacao);
    }

    [HttpPost]
    public IActionResult Post([FromBody] ParticipacaoProvaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newId = _service.Adicionar(dto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, _service.ObterPorId(newId));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno ao adicionar participação em prova.", detalhes = ex.Message });
        }
    }

    [HttpPatch]
    public IActionResult Patch([FromBody] ParticipacaoProvaUpdateDto dto)
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
                return NotFound($"Participação em prova com ID {dto.Id} não encontrada.");
            }

            // TODO: Adicionar validação se EquipeId é obrigatório para o tipo de prova, etc.

            _service.Atualizar(dto);

            // Retorna o objeto atualizado para o frontend
            var participacaoAtualizada = _service.ObterPorId(dto.Id);
            return Ok(participacaoAtualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno ao atualizar participação em prova.", detalhes = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var participacao = _service.ObterPorId(id);
            if (participacao == null)
            {
                return NotFound($"Participação em prova com ID {id} não encontrada.");
            }

            _service.Deletar(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno ao deletar participação em prova.", detalhes = ex.Message });
        }
    }
}