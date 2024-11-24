using Core.Entities;
using Core.Interfaces;

namespace Application.UseCases;

public class MonitorCryptoUseCase
{
    private readonly ICryptoWebSocketService _webSocketService;

    public MonitorCryptoUseCase(ICryptoWebSocketService webSocketService)
    {
        _webSocketService = webSocketService;
    }

    public async Task StartMonitoring(Func<Crypto, Task> onMessage)
    {
        await _webSocketService.ConnectAsync();
        await _webSocketService.ListenAsync(onMessage);
    }

    public async Task StopMonitoring()
    {
        await _webSocketService.CloseAsync();
    }
}
