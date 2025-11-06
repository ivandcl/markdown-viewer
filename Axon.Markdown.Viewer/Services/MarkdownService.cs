using Markdig;
using System.IO;
using System.Text;

namespace Axon.Markdown.Viewer.Services;

public class MarkdownService : IMarkdownService
{
    private readonly MarkdownPipeline _pipeline;
    private readonly string _cssStyles;

    public MarkdownService()
    {
        // Configurar Markdig con extensiones avanzadas (GitHub Flavored Markdown)
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions() // Incluye: tablas, listas de tareas, código con syntax highlighting, etc.
            .UseEmojiAndSmiley()
            .UsePipeTables()
            .UseGridTables()
            .UseListExtras()
            .UseTaskLists()
            .UseAutoLinks()
            .Build();

        _cssStyles = GetDarkThemeCss();
    }

    public string ConvertMarkdownToHtml(string markdownContent)
    {
        if (string.IsNullOrWhiteSpace(markdownContent))
            return WrapInHtmlTemplate(string.Empty);

        var htmlContent = Markdig.Markdown.ToHtml(markdownContent, _pipeline);
        return WrapInHtmlTemplate(htmlContent);
    }

    public async Task<string> LoadMarkdownFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("El archivo Markdown no existe.", filePath);

        return await File.ReadAllTextAsync(filePath, Encoding.UTF8);
    }

    public string ExtractPlainText(string markdownContent)
    {
        if (string.IsNullOrWhiteSpace(markdownContent))
            return string.Empty;

        // Convertir Markdown a HTML primero
        var htmlContent = Markdig.Markdown.ToHtml(markdownContent, _pipeline);

        // Remover etiquetas HTML para obtener texto plano
        var plainText = System.Text.RegularExpressions.Regex.Replace(htmlContent, "<[^>]*>", " ");

        // Decodificar entidades HTML
        plainText = System.Net.WebUtility.HtmlDecode(plainText);

        // Limpiar espacios múltiples y líneas vacías
        plainText = System.Text.RegularExpressions.Regex.Replace(plainText, @"\s+", " ");
        plainText = plainText.Trim();

        return plainText;
    }

    private string WrapInHtmlTemplate(string htmlContent)
    {
        return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <style>
        {_cssStyles}
    </style>
</head>
<body>
    <div class='markdown-body'>
        {htmlContent}
    </div>
</body>
</html>";
    }

    private static string GetDarkThemeCss()
    {
        return @"
            * {
                margin: 0;
                padding: 0;
                box-sizing: border-box;
            }

            body {
                background-color: #1e1e1e;
                color: #d4d4d4;
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                line-height: 1.6;
                padding: 20px;
            }

            .markdown-body {
                max-width: 900px;
                margin: 0 auto;
                padding: 30px;
                background-color: #252526;
                border-radius: 8px;
                box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
            }

            /* Encabezados */
            h1, h2, h3, h4, h5, h6 {
                color: #e0e0e0;
                margin-top: 24px;
                margin-bottom: 16px;
                font-weight: 600;
                line-height: 1.25;
            }

            h1 {
                font-size: 2em;
                border-bottom: 2px solid #3c3c3c;
                padding-bottom: 0.3em;
            }

            h2 {
                font-size: 1.5em;
                border-bottom: 1px solid #3c3c3c;
                padding-bottom: 0.3em;
            }

            h3 { font-size: 1.25em; }
            h4 { font-size: 1em; }
            h5 { font-size: 0.875em; }
            h6 { font-size: 0.85em; color: #a0a0a0; }

            /* Párrafos y texto */
            p {
                margin-bottom: 16px;
            }

            strong {
                color: #e0e0e0;
                font-weight: 600;
            }

            em {
                color: #d4d4d4;
                font-style: italic;
            }

            /* Enlaces */
            a {
                color: #4fc3f7;
                text-decoration: none;
                transition: color 0.2s ease;
            }

            a:hover {
                color: #81d4fa;
                text-decoration: underline;
            }

            /* Listas */
            ul, ol {
                margin-bottom: 16px;
                padding-left: 32px;
            }

            li {
                margin-bottom: 8px;
            }

            /* Listas de tareas */
            input[type='checkbox'] {
                margin-right: 8px;
            }

            /* Código inline */
            code {
                background-color: #1e1e1e;
                color: #ce9178;
                padding: 2px 6px;
                border-radius: 3px;
                font-family: 'Consolas', 'Courier New', monospace;
                font-size: 0.9em;
            }

            /* Bloques de código */
            pre {
                background-color: #1e1e1e;
                border: 1px solid #3c3c3c;
                border-radius: 6px;
                padding: 16px;
                overflow-x: auto;
                margin-bottom: 16px;
            }

            pre code {
                background-color: transparent;
                color: #d4d4d4;
                padding: 0;
                font-size: 0.85em;
                line-height: 1.45;
            }

            /* Citas */
            blockquote {
                border-left: 4px solid #4fc3f7;
                padding-left: 16px;
                margin: 16px 0;
                color: #a0a0a0;
                font-style: italic;
            }

            /* Tablas */
            table {
                border-collapse: collapse;
                width: 100%;
                margin-bottom: 16px;
                overflow: hidden;
                border-radius: 6px;
            }

            th, td {
                border: 1px solid #3c3c3c;
                padding: 12px 16px;
                text-align: left;
            }

            th {
                background-color: #2d2d30;
                color: #e0e0e0;
                font-weight: 600;
            }

            tr:nth-child(even) {
                background-color: #2a2a2a;
            }

            tr:hover {
                background-color: #2d2d30;
            }

            /* Líneas horizontales */
            hr {
                border: none;
                border-top: 2px solid #3c3c3c;
                margin: 24px 0;
            }

            /* Imágenes */
            img {
                max-width: 100%;
                height: auto;
                border-radius: 6px;
                margin: 16px 0;
            }

            /* Scrollbar personalizado */
            ::-webkit-scrollbar {
                width: 10px;
                height: 10px;
            }

            ::-webkit-scrollbar-track {
                background: #1e1e1e;
            }

            ::-webkit-scrollbar-thumb {
                background: #3c3c3c;
                border-radius: 5px;
            }

            ::-webkit-scrollbar-thumb:hover {
                background: #505050;
            }
        ";
    }
}
