using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReviewClassification
{
    Good,
    Excellent,
    NeedsWork
}

public sealed class ReviewResult
{
    [JsonPropertyName("classification")]
    public ReviewClassification Classification { get; set; }

    [JsonPropertyName("feedback")]
    public string Feedback { get; set; } = string.Empty;
}

