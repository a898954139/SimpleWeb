namespace SignalR.Chatroom;

public sealed record ChatMessageDto(string User, string Message)
{
    public DateTime Timestamp { get; set; } = DateTime.Now; 
}