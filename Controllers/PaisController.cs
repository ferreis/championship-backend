using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class PaisController : BaseController<PaisService>
{
    [HttpGet]
    public ActionResult<List<Pais>> Get() => Ok(_service.Listar());

    [HttpPost]
    public IActionResult Post([FromBody] PaisDto dto)
    {
        _service.Adicionar(dto);
        return Ok("País adicionado com sucesso.");
    }

    [HttpPatch("{id}")]
    public IActionResult Patch(int id, [FromBody] PaisUpdateDto dto)
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
