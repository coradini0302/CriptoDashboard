using System.Text.Json.Serialization;

public class BinanceTradeMessage
{
    [JsonPropertyName("s")]
    public string Symbol { get; set; } // "BTCUSDT"

    [JsonPropertyName("p")]
    public string Price { get; set; } // "50000.12"

    [JsonPropertyName("T")]
    public long Timestamp { get; set; } // 123456785
}