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

    public User User { get; private set; }

    public AuditLog(string id,User user, string actionType,
        string entityType, string entityId, DateTimeOffset timestamp, JsonDocument metadata)
    {
        ValidateCommon(id, actionType, entityType, entityId);
        ValidateIdentity(user);
        Id = id;
        ActorId = user.Id;
        ActorRole = user.UserRole.ToString();
        ActionType = actionType;
        EntityType = entityType;
        EntityId = entityId;
        Timestamp = DateTimeOffset.Now;
        Metadata = metadata;
        User = user;
    }

    private static void ValidateIdentity(User user)
    {
        if (user.Id != user.UserRole.ToString())
        {
            throw new ArgumentException("User Id is not matching User role");
        }

    }

    private static void ValidateCommon(string id, string actionType, string entityType, string entityId)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length <6 || id.Length > 100)
        {
            throw new ArgumentException("Invalid ID");
        }

        if (string.IsNullOrWhiteSpace(actionType) || actionType.Length < 6 || actionType.Length > 100)
        {
            throw new ArgumentException("Invalid action type");
        }
        if (string.IsNullOrWhiteSpace(entityType) || entityType.Length < 6 || entityType.Length > 100)
        {
            throw new ArgumentException("Invalid entity type");
        }
        if (string.IsNullOrWhiteSpace(entityId) || entityId.Length < 6 || entityId.Length > 100)
        {
            throw new ArgumentException("Invalid entity ID");
        }
    }
}