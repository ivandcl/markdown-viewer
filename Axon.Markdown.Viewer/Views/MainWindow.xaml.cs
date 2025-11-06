using System.Windows;
using Axon.Markdown.Viewer.ViewModels;
using Microsoft.Web.WebView2.Core;
using System.ComponentModel;

namespace Axon.Markdown.Viewer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel? _viewModel;
    private double _currentZoom = 1.0;

    public MainWindow()
    {
        InitializeComponent();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            // Inicializar WebView2
            await webView.EnsureCoreWebView2Async();

            // Suscribirse a cambios en el ViewModel
            if (DataContext is MainViewModel viewModel)
            {
                _viewModel = viewModel;
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
                _viewModel.ScrollRequested += ViewModel_ScrollRequested;

                // Cargar contenido inicial
                webView.CoreWebView2.NavigateToString(_viewModel.HtmlContent);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al inicializar WebView2: {ex.Message}\n\nAsegúrate de tener WebView2 Runtime instalado.",
                "Error de inicialización",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.HtmlContent) && _viewModel != null)
        {
            // Actualizar el contenido HTML cuando cambia
            if (webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.NavigateToString(_viewModel.HtmlContent);
            }
        }
    }

    private async void ViewModel_ScrollRequested(object? sender, int progress)
    {
        if (webView.CoreWebView2 != null)
        {
            // Desplazar el contenido basado en el progreso (0-100%)
            var script = $@"
                var docHeight = document.documentElement.scrollHeight - document.documentElement.clientHeight;
                var scrollPosition = (docHeight * {progress}) / 100;
                window.scrollTo({{ top: scrollPosition, behavior: 'smooth' }});
            ";

            try
            {
                await webView.CoreWebView2.ExecuteScriptAsync(script);
            }
            catch
            {
                // Ignorar errores de script
            }
        }
    }

    private void ZoomIn_Click(object sender, RoutedEventArgs e)
    {
        _currentZoom += 0.1;
        if (_currentZoom > 3.0)
            _currentZoom = 3.0;

        if (webView.CoreWebView2 != null)
        {
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping("zoom", "", CoreWebView2HostResourceAccessKind.Allow);
            webView.ZoomFactor = _currentZoom;
        }
    }

    private void ZoomOut_Click(object sender, RoutedEventArgs e)
    {
        _currentZoom -= 0.1;
        if (_currentZoom < 0.5)
            _currentZoom = 0.5;

        if (webView.CoreWebView2 != null)
        {
            webView.ZoomFactor = _currentZoom;
        }
    }

    private void ResetZoom_Click(object sender, RoutedEventArgs e)
    {
        _currentZoom = 1.0;

        if (webView.CoreWebView2 != null)
        {
            webView.ZoomFactor = _currentZoom;
        }
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "Axon Markdown Viewer v1.0\n\n" +
            "Aplicación para visualizar archivos Markdown con soporte extendido.\n\n" +
            "Diseñada especialmente para revisar documentación generada por Claude.\n\n" +
            "Características:\n" +
            "• GitHub Flavored Markdown\n" +
            "• Tema oscuro minimalista\n" +
            "• Soporte para tablas, código, listas de tareas\n" +
            "• Syntax highlighting\n\n" +
            "© 2025 Axon Group",
            "Acerca de",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    // Event handlers para la barra de título personalizada
    private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            // Doble clic para maximizar/restaurar
            MaximizeButton_Click(sender, e);
        }
        else
        {
            // Arrastrar la ventana
            DragMove();
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
        else
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    protected override void OnDrop(DragEventArgs e)
    {
        base.OnDrop(e);

        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && _viewModel != null)
            {
                string file = files[0];
                if (file.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                {
                    _ = _viewModel.LoadFileAsync(file);
                }
            }
        }
    }

    protected override void OnDragEnter(DragEventArgs e)
    {
        base.OnDragEnter(e);

        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _viewModel.ScrollRequested -= ViewModel_ScrollRequested;
        }

        webView?.Dispose();
        base.OnClosed(e);
    }
}
