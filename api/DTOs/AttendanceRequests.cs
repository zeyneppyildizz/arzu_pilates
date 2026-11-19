namespace api.DTOs;

public record ScanQrRequest(string Token);

public record QrPayload
{
    public int Id { get; init; }
    public long Timestamp { get; init; }
}