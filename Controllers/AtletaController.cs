using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AtletaController : BaseController<AtletaService>
{
    [HttpGet]
    public IActionResult GetAtletas([FromQuery] int? competicaoId)
    {
        var lista = _service.ListarComEquipes(competicaoId);
        return Ok(lista);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Atleta atleta)
    {
        _service.Adicionar(atleta);
        return Created($"api/atleta/{atleta.Id}", atleta);
    }

    [HttpPatch]
    public IActionResult Atualizar([FromBody] AtletaUpdateDto atletaUpdateDto)
    {
        var existente = _service.ObterPorId(atletaUpdateDto.Id);
        if (existente == null)
            return NotFound($"Atleta com ID {atletaUpdateDto.Id} não encontrado.");

        existente.Nome = atletaUpdateDto.Nome ?? existente.Nome;
        existente.Cpf = atletaUpdateDto.Cpf ?? existente.Cpf;
        existente.Genero = atletaUpdateDto.Genero ?? existente.Genero;
        existente.DataNascimento = atletaUpdateDto.DataNascimento ?? existente.DataNascimento;
        existente.PaisId = atletaUpdateDto.PaisId ?? existente.PaisId;

        _service.Atualizar(existente);
        return Ok(existente);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var atleta = _service.ObterPorId(id);
        if (atleta == null)
            return NotFound($"Atleta com ID {id} não encontrado para exclusão.");

        _service.Deletar(id);
        return NoContent();
    }

    [HttpPost("vincular")]
    public IActionResult Vincular([FromBody] AtletaEquipeDto dto)
    {
        if (dto.AtletaId == null || dto.EquipeId == null || dto.CompeticaoId == null)
            return BadRequest("ID do Atleta, da Equipe e da Competição são obrigatórios.");

        _service.VincularAtletaEquipe(dto);
        return Ok("Atleta vinculado à equipe com sucesso para esta competição.");
    }
}