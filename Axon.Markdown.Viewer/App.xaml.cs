using System.Windows;
using Axon.Markdown.Viewer.Services;
using Axon.Markdown.Viewer.ViewModels;
using Axon.Markdown.Viewer.Views;

namespace Axon.Markdown.Viewer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Configurar inyección de dependencias simple
        var markdownService = new MarkdownService();
        var ttsService = new TtsService();
        var mainViewModel = new MainViewModel(markdownService, ttsService);

        // Crear y mostrar la ventana principal
        var mainWindow = new MainWindow();
        mainWindow.DataContext = mainViewModel;
        mainWindow.AllowDrop = true;

        // Verificar si se pasó un archivo como argumento
        if (e.Args.Length > 0)
        {
            string filePathToOpen = e.Args[0];

            // Cargar el archivo después de que la ventana esté completamente inicializada
            mainWindow.Loaded += async (s, args) =>
            {
                await mainViewModel.LoadFileAsync(filePathToOpen);
            };
        }

        mainWindow.Show();
    }
}
