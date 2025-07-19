using Microsoft.AspNetCore.Mvc;

public abstract class BaseController<TService> : ControllerBase
    where TService : class
{
    protected readonly TService _service;

    public BaseController()
    {
        var connectionString = "Data Source=Database/sobrasa_banco_de_dados.db";

        // Cria o service com o construtor que aceita a string
        _service = (TService)Activator.CreateInstance(typeof(TService), connectionString)!;
    }
}
