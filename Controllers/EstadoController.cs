using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class EstadoController : BaseController<EstadoService>
{
    [HttpGet]
    public ActionResult<List<Estado>> Get() => Ok(_service.Listar());

    [HttpPost]
    public IActionResult Post([FromBody] Estado estado)
    {
        _service.Adicionar(estado);
        return Created($"api/estado", estado);
    }
}