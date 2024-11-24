using Core.Entities;

namespace Core.Interfaces;

public interface ICryptoWebSocketService
{
    Task ConnectAsync();
    Task ListenAsync(Func<Crypto, Task> onMessage);
    Task CloseAsync();
}