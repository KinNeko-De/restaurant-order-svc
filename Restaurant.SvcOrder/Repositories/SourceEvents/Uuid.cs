using Google.Protobuf;

namespace Restaurant.SvcOrder.Repositories.SourceEvents;

public partial class Uuid : ICustomDiagnosticMessage
{
    public static Uuid FromGuid(Guid guid)
    {
        var uuid = new Uuid() { Value = guid.ToString("D") };

        return uuid;
    }

    public Guid ToGuid()
    {
        return Guid.Parse(Value);
    }

    public bool TryParseToGuid(out Guid guid)
    {
        if (Guid.TryParse(Value, out var parsedGuid))
        {
            guid = parsedGuid;
            return true;
        }

        guid = Guid.Empty;
        return false;
    }

    /// <summary>
    /// This effectively overrides <see cref="ToString"/> and <see cref="object.ToString"/>
    /// for our purposes, causing those to return a clean string representation of the uuid.
    /// </summary>
    /// <returns>A readable uuid without json wrapper or quotes.</returns>
    string ICustomDiagnosticMessage.ToDiagnosticString() => Value;
}
