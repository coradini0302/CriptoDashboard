using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services;

public class BinanceWebSocketService : ICryptoWebSocketService
{
    private readonly ClientWebSocket _webSocket;
    private readonly Uri _uri;

    public BinanceWebSocketService(string url)
    {
        _uri = new Uri(url);
        _webSocket = new ClientWebSocket();
    }

    public async Task ConnectAsync()
    {
        await _webSocket.ConnectAsync(_uri, CancellationToken.None);
        Console.WriteLine("Conectado ao WebSocket.");

        // Enviar a mensagem de assinatura
        var subscribeMessage = new
        {
            method = "SUBSCRIBE",
            params = new[] { "btcusdt@trade" }, // Ajuste para o par desejado
            id = 1
        };

        var message = JsonSerializer.Serialize(subscribeMessage);
        var bytes = Encoding.UTF8.GetBytes(message);

        await _webSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );

        Console.WriteLine("Mensagem de assinatura enviada.");
    }

    public async Task ListenAsync(Func<Crypto, Task> onMessage)
    {
        var buffer = new byte[1024 * 4];

        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                try
                {
                    // Deserialize o JSON da Binance
                    var tradeMessage = JsonSerializer.Deserialize<BinanceTradeMessage>(message);

                    // Converte para a entidade Crypto
                    var crypto = new Crypto
                    {
                        Symbol = tradeMessage.Symbol,
                        Price = decimal.Parse(tradeMessage.Price),
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(tradeMessage.Timestamp).UtcDateTime
                    };

                    // Processa a mensagem
                    await onMessage(crypto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                }
            }
        }
    }

    public async Task CloseAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fechando conexão", CancellationToken.None);
    }

    private Crypto ParseCryptoMessage(string message)
    {
        // Parse simples para exemplo, ajustar conforme formato da API.
        return new Crypto
        {
            Symbol = "BTCUSDT", // extrair do JSON
            Price = decimal.Parse("50000"), // extrair do JSON
            Timestamp = DateTime.UtcNow
        };
    }
}
