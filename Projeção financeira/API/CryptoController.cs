using Application.UseCases;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/crypto")]
public class CryptoController : ControllerBase
{
    private readonly MonitorCryptoUseCase _monitorCryptoUseCase;

    public CryptoController(MonitorCryptoUseCase monitorCryptoUseCase)
    {
        _monitorCryptoUseCase = monitorCryptoUseCase;
    }

    [HttpPost("start")]
    public IActionResult StartMonitoring()
    {
        _ = _monitorCryptoUseCase.StartMonitoring(async crypto =>
        {
            Console.WriteLine($"Criptomoeda: {crypto.Symbol}, Preço: {crypto.Price}");
            await Task.CompletedTask;
        });

        return Ok("Monitoramento iniciado.");
    }

    [HttpPost("stop")]
    public async Task<IActionResult> StopMonitoring()
    {
        await _monitorCryptoUseCase.StopMonitoring();
        return Ok("Monitoramento encerrado.");
    }
}
