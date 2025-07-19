using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EquipeController : BaseController<EquipeService>
{
    [HttpGet]
    public IActionResult Get() => Ok(_service.Listar());

    [HttpPost]
    public IActionResult Post([FromBody] Equipe equipe)
    {
        _service.Adicionar(equipe);
        return Created("", equipe);
    }

    [HttpPatch("{id}")]
    public IActionResult Patch(int id, [FromBody] EquipeUpdateDto dto)
    {
        _service.Atualizar(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Deletar(id);
        return NoContent();
    }
}
