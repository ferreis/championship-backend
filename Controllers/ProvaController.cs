// Controllers/ProvaController.cs
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")] // A rota base será /api/Prova
public class ProvaController : ControllerBase
{
    private readonly ProvaService _service;
    private const string ConnectionString = "Data Source=Database/sobrasa_banco_de_dados.db"; // Sua string de conexão

    // Construtor do controlador
    public ProvaController()
    {
        _service = new ProvaService(ConnectionString);
    }

    /// <summary>
    /// Retorna uma lista de todas as provas.
    /// GET /api/Prova
    /// </summary>
    [HttpGet]
    public ActionResult<List<Prova>> Get()
    {
        var provas = _service.Listar();
        if (provas == null || provas.Count == 0)
        {
            return NotFound("Nenhuma prova encontrada.");
        }
        return Ok(provas);
    }

    /// <summary>
    /// Obtém uma prova específica pelo seu ID.
    /// GET /api/Prova/{id}
    /// </summary>
    /// <param name="id">O ID da prova.</param>
    [HttpGet("{id}")]
    public ActionResult<Prova> GetById(int id)
    {
        var prova = _service.ObterPorId(id);
        if (prova == null)
        {
            return NotFound($"Prova com ID {id} não encontrada.");
        }
        return Ok(prova);
    }

    /// <summary>
    /// Adiciona uma nova prova.
    /// POST /api/Prova
    /// </summary>
    /// <param name="provaDto">Os dados da prova a ser adicionada.</param>
    [HttpPost]
    public IActionResult Post([FromBody] ProvaDto provaDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var novaProva = new Prova // Cria uma instância do modelo a partir do DTO
        {
            Nome = provaDto.Nome,
            Tipo = provaDto.Tipo,
            Modalidade = provaDto.Modalidade,
            TempoOuColocacao = provaDto.TempoOuColocacao,
            Genero = provaDto.Genero,
            CategoriaEtaria = provaDto.CategoriaEtaria
        };

        try
        {
            // O serviço agora retorna o ID da prova recém-criada
            int newId = _service.Adicionar(novaProva); 
            
            // Obtém o objeto completo da prova com o ID preenchido para a resposta CreatedAtAction
            var provaCriada = _service.ObterPorId(newId);
            if (provaCriada == null)
            {
                // Isso indicaria um erro inesperado: a inserção funcionou, mas a recuperação falhou
                return StatusCode(500, "Erro interno: Prova foi adicionada, mas não pôde ser recuperada.");
            }

            return CreatedAtAction(nameof(GetById), new { id = newId }, provaCriada);
        }
        catch (Exception ex)
        {
            // Captura exceções como violações de CHECK CONSTRAINTS do SQLite
            return StatusCode(500, new { erro = "Erro interno ao adicionar prova.", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza parcialmente os dados de uma prova existente.
    /// PATCH /api/Prova
    /// </summary>
    /// <param name="provaDto">Os dados da prova a serem atualizados.</param>
    [HttpPatch]
    public IActionResult Patch([FromBody] ProvaUpdateDto provaDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existente = _service.ObterPorId(provaDto.Id);
        if (existente == null)
        {
            return NotFound($"Prova com ID {provaDto.Id} não encontrada.");
        }

        // Aplica as atualizações apenas se o campo não for nulo no DTO
        if (provaDto.Nome != null) existente.Nome = provaDto.Nome;
        if (provaDto.Tipo != null) existente.Tipo = provaDto.Tipo;
        if (provaDto.Modalidade != null) existente.Modalidade = provaDto.Modalidade;
        if (provaDto.TempoOuColocacao != null) existente.TempoOuColocacao = provaDto.TempoOuColocacao;
        if (provaDto.Genero != null) existente.Genero = provaDto.Genero;
        if (provaDto.CategoriaEtaria != null) existente.CategoriaEtaria = provaDto.CategoriaEtaria;

        try
        {
            _service.Atualizar(existente);
            // Retorna o objeto atualizado do DB para garantir que reflete o estado salvo
            return Ok(_service.ObterPorId(existente.Id)); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno ao atualizar prova.", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Deleta uma prova pelo seu ID.
    /// DELETE /api/Prova/{id}
    /// </summary>
    /// <param name="id">O ID da prova a ser deletada.</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var prova = _service.ObterPorId(id);
        if (prova == null)
        {
            return NotFound($"Prova com ID {id} não encontrada para exclusão.");
        }

        try
        {
            _service.Deletar(id);
            return NoContent(); // Retorna 204 No Content para sucesso sem retorno de corpo
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno ao deletar prova.", detalhes = ex.Message });
        }
    }
}