namespace Axon.Markdown.Viewer.Models;

public class VoiceInfo
{
    public string Name { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Age { get; set; }

    public string DisplayName => $"{Name} ({Gender}, {Culture})";

    public override string ToString() => DisplayName;
}
