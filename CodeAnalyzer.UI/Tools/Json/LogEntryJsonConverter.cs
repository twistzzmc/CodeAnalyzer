using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeAnalyzer.UI.LoggerUi.Dtos;
using CodeAnalyzer.UI.LoggerUi.Enums;

namespace CodeAnalyzer.UI.Tools.Json;

internal sealed class LogEntryJsonConverter : JsonConverter<LogEntry>
{
    public override LogEntry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        string title = root.GetProperty("Title").GetString() ?? string.Empty;
        LogPriority priority = root.TryGetProperty("Priority", out JsonElement p)
            ? Enum.Parse<LogPriority>(p.GetString() ?? "Info")
            : LogPriority.Info;

        LogEntry logEntry = new(title, priority);

        if (root.TryGetProperty("Key", out var keyProp))
        {
            logEntry.Key = keyProp.GetString() ?? string.Empty;
        }

        if (root.TryGetProperty("IsSuccess", out var isSuccessProp))
        {
            logEntry.IsSuccess = isSuccessProp.GetBoolean();
        }

        if (!root.TryGetProperty("Children", out var childrenProp) || childrenProp.ValueKind != JsonValueKind.Array)
        {
            return logEntry;
        }
        
        foreach (JsonElement child in childrenProp.EnumerateArray())
        {
            LogEntry? childLog = JsonSerializer.Deserialize<LogEntry>(child.GetRawText(), options);
            if (childLog is not null)
            {
                logEntry.AddChild(childLog);
            }
        }

        return logEntry;
    }

    public override void Write(Utf8JsonWriter writer, LogEntry value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("Title", value.Title);
        writer.WriteString("Key", value.Key);
        writer.WriteString("Priority", value.Priority.ToString());
        writer.WriteBoolean("IsSuccess", value.IsSuccess);

        writer.WritePropertyName("Children");
        writer.WriteStartArray();
        foreach (LogEntry child in value.Children)
        {
            JsonSerializer.Serialize(writer, child, options);
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}