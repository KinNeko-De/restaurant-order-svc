namespace Restaurant.SvcOrder.Repositories.SourceEvents;

public static class UuidExtensions
{

    public static Guid? ToNullableGuid(this Uuid? uuid)
    {
        return uuid?.ToGuid();
    }

    public static bool TryParseToNullableGuid(this Uuid? uuid, out Guid? guid)
    {
        if (uuid == null)
        {
            guid = null;
            return true;
        }

        var result = uuid.TryParseToGuid(out var x);
        guid = x;
        return result;
    }
}
