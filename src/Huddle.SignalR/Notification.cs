namespace Huddle.SignalR;

public class Notification<R>
{
    public R Entity {get; init;}
    public string? InitiatorSessionId {get; init;}

    public Notification(R entity, string? sessionId = null)
    {
        Entity = entity;
        InitiatorSessionId = sessionId;
    }
}