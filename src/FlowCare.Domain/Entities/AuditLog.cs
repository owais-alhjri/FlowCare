using System.Text.Json;

namespace FlowCare.Domain.Entities;

public class AuditLog
{
    public string Id { get; private set; }
    // User ID.
    public string ActorId { get; private set; }
    // User Role
    public string ActorRole { get; private set; }
    public string ActionType { get; private set; }
    public string EntityType{ get; private set; }
    public string EntityId{ get; private set; }

    // DateTimeOffset used for the time zone offset
    public DateTimeOffset Timestamp  { get; private set; }
    public JsonDocument Metadata { get; private set; }

    protected AuditLog(){}

    public AuditLog(string id, string actorId, string actorRole, string actionType,
        string entityType, string entityId, DateTimeOffset timestamp, JsonDocument metadata)
    {
        Id = id;
        ActorId = actorId;
        ActorRole = actorRole;
        ActionType = actionType;
        EntityType = entityType;
        EntityId = entityId;
        Timestamp = DateTimeOffset.Now;
        Metadata = metadata;
    }
}