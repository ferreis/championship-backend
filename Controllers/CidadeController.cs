using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class CidadeController : BaseController<CidadeService>
{
    [HttpGet]
    public ActionResult<List<Cidade>> Get() => Ok(_service.Listar());

    [HttpPost]
    public IActionResult Post([FromBody] Cidade cidade)
    {
        _service.Adicionar(cidade);
        return Created($"api/cidade", cidade);
    }
}