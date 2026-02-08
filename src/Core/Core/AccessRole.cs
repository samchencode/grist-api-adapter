using System.Text.Json;
using System.Text.Json.Serialization;

namespace GristApiAdapter.Core;

[JsonConverter(typeof(AccessRoleConverter))]
public readonly record struct AccessRole
{
    public static readonly AccessRole Owners = new("owners");
    public static readonly AccessRole Editors = new("editors");
    public static readonly AccessRole Viewers = new("viewers");
    public static readonly AccessRole Members = new("members");
    public static readonly AccessRole Guests = new("guests");

    private static readonly HashSet<string> ValidValues = [
        Owners.Value, Editors.Value, Viewers.Value, Members.Value, Guests.Value
    ];

    public string Value { get; }

    private AccessRole(string value)
    {
        Value = value;
    }

    public static AccessRole Parse(string value)
    {
        if (!ValidValues.Contains(value))
        {
            throw new ArgumentException($"Invalid access role: '{value}'. Must be one of: {string.Join(", ", ValidValues)}");
        }
        return new AccessRole(value);
    }

    public override string ToString() => Value;
}

class AccessRoleConverter : JsonConverter<AccessRole>
{
    public override AccessRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString() ?? throw new JsonException("Access role cannot be null");
        return AccessRole.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, AccessRole value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}

class NullableAccessRoleConverter : JsonConverter<AccessRole?>
{
    public override AccessRole? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        string value = reader.GetString() ?? throw new JsonException("Unexpected null token");
        return AccessRole.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, AccessRole? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value.Value);
    }
}
