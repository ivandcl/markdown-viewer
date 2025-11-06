namespace Axon.Markdown.Viewer.Services;

public interface IMarkdownService
{
    string ConvertMarkdownToHtml(string markdownContent);
    Task<string> LoadMarkdownFileAsync(string filePath);
    string ExtractPlainText(string markdownContent);
}
