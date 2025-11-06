using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Axon.Markdown.Viewer.Services;
using Axon.Markdown.Viewer.Models;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;

namespace Axon.Markdown.Viewer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IMarkdownService _markdownService;
    private readonly ITtsService _ttsService;
    private string _currentMarkdownContent = string.Empty;

    [ObservableProperty]
    private string _htmlContent = string.Empty;

    [ObservableProperty]
    private string _currentFilePath = string.Empty;

    [ObservableProperty]
    private string _currentFileName = "Sin archivo";

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private bool _isReading = false;

    [ObservableProperty]
    private bool _isPaused = false;

    [ObservableProperty]
    private int _readingProgress = 0;

    [ObservableProperty]
    private int _speechRate = 0;

    [ObservableProperty]
    private int _speechVolume = 100;

    [ObservableProperty]
    private ObservableCollection<string> _availableLanguages = new();

    [ObservableProperty]
    private string _selectedLanguage = "es";

    [ObservableProperty]
    private ObservableCollection<VoiceInfo> _availableVoices = new();

    [ObservableProperty]
    private VoiceInfo? _selectedVoice;

    public event EventHandler<int>? ScrollRequested;

    public MainViewModel(IMarkdownService markdownService, ITtsService ttsService)
    {
        _markdownService = markdownService;
        _ttsService = ttsService;

        // Suscribirse a eventos del servicio TTS
        _ttsService.ProgressChanged += OnTtsProgressChanged;
        _ttsService.SpeakCompleted += OnTtsSpeakCompleted;

        // Configurar TTS inicial
        _ttsService.SetRate(_speechRate);
        _ttsService.SetVolume(_speechVolume);

        // Inicializar idiomas y voces
        InitializeLanguagesAndVoices();

        // Mostrar mensaje de bienvenida inicial
        _currentMarkdownContent = @"# Bienvenido a Axon Markdown Viewer

## Cómo usar esta aplicación

1. Haz clic en **Archivo > Abrir** o presiona **Ctrl+O** para abrir un archivo Markdown
2. También puedes abrir archivos arrastrándolos a la ventana
3. O iniciar la aplicación con el archivo como argumento: `Axon.Markdown.Viewer.exe archivo.md`

### Características soportadas

Esta aplicación soporta **GitHub Flavored Markdown** con:

- ✅ Encabezados y formato de texto
- ✅ Listas ordenadas y desordenadas
- ✅ Listas de tareas
- ✅ Tablas
- ✅ Bloques de código con syntax highlighting
- ✅ Enlaces e imágenes
- ✅ Citas
- ✅ Lectura en voz alta con desplazamiento automático
- ✅ Y mucho más...

---

**Diseñado para revisar documentación generada por Claude**";

        HtmlContent = _markdownService.ConvertMarkdownToHtml(_currentMarkdownContent);
    }

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Archivos Markdown (*.md)|*.md|Todos los archivos (*.*)|*.*",
            Title = "Abrir archivo Markdown"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            await LoadFileAsync(openFileDialog.FileName);
        }
    }

    [RelayCommand]
    private void Exit()
    {
        _ttsService.Stop();
        System.Windows.Application.Current.Shutdown();
    }

    public async Task LoadFileAsync(string filePath)
    {
        try
        {
            IsLoading = true;

            // Detener lectura si está en progreso
            if (IsReading)
            {
                StopReading();
            }

            if (!File.Exists(filePath))
            {
                ShowError("El archivo no existe.");
                return;
            }

            _currentMarkdownContent = await _markdownService.LoadMarkdownFileAsync(filePath);
            HtmlContent = _markdownService.ConvertMarkdownToHtml(_currentMarkdownContent);

            CurrentFilePath = filePath;
            CurrentFileName = Path.GetFileName(filePath);

            // Notificar que el comando puede ejecutarse ahora
            StartReadingCommand.NotifyCanExecuteChanged();
        }
        catch (Exception ex)
        {
            ShowError($"Error al cargar el archivo: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ReloadFileAsync()
    {
        if (!string.IsNullOrEmpty(CurrentFilePath) && File.Exists(CurrentFilePath))
        {
            await LoadFileAsync(CurrentFilePath);
        }
    }

    [RelayCommand(CanExecute = nameof(CanStartReading))]
    private void StartReading()
    {
        if (string.IsNullOrWhiteSpace(_currentMarkdownContent))
            return;

        var plainText = _markdownService.ExtractPlainText(_currentMarkdownContent);

        if (string.IsNullOrWhiteSpace(plainText))
            return;

        IsReading = true;
        IsPaused = false;
        ReadingProgress = 0;

        _ttsService.Speak(plainText);

        // Actualizar estado de comandos
        StartReadingCommand.NotifyCanExecuteChanged();
        PauseReadingCommand.NotifyCanExecuteChanged();
        ResumeReadingCommand.NotifyCanExecuteChanged();
        StopReadingCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanPauseReading))]
    private void PauseReading()
    {
        if (IsReading && !IsPaused)
        {
            _ttsService.Pause();
            IsPaused = true;

            PauseReadingCommand.NotifyCanExecuteChanged();
            ResumeReadingCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand(CanExecute = nameof(CanResumeReading))]
    private void ResumeReading()
    {
        if (IsReading && IsPaused)
        {
            _ttsService.Resume();
            IsPaused = false;

            PauseReadingCommand.NotifyCanExecuteChanged();
            ResumeReadingCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand(CanExecute = nameof(CanStopReading))]
    private void StopReading()
    {
        _ttsService.Stop();
        IsReading = false;
        IsPaused = false;
        ReadingProgress = 0;

        // Actualizar estado de comandos
        StartReadingCommand.NotifyCanExecuteChanged();
        PauseReadingCommand.NotifyCanExecuteChanged();
        ResumeReadingCommand.NotifyCanExecuteChanged();
        StopReadingCommand.NotifyCanExecuteChanged();
    }

    private bool CanStartReading() => !IsReading && !string.IsNullOrEmpty(_currentMarkdownContent);
    private bool CanPauseReading() => IsReading && !IsPaused;
    private bool CanResumeReading() => IsReading && IsPaused;
    private bool CanStopReading() => IsReading;

    partial void OnSpeechRateChanged(int value)
    {
        _ttsService.SetRate(value);
    }

    partial void OnSpeechVolumeChanged(int value)
    {
        _ttsService.SetVolume(value);
    }

    partial void OnSelectedLanguageChanged(string value)
    {
        LoadVoicesForLanguage(value);
    }

    partial void OnSelectedVoiceChanged(VoiceInfo? value)
    {
        if (value != null)
        {
            _ttsService.SelectVoice(value.Name);
        }
    }

    private void InitializeLanguagesAndVoices()
    {
        // Agregar idiomas soportados
        AvailableLanguages.Add("es"); // Español
        AvailableLanguages.Add("en"); // Inglés

        // Cargar voces para el idioma por defecto (español)
        LoadVoicesForLanguage(SelectedLanguage);
    }

    private void LoadVoicesForLanguage(string languageCode)
    {
        AvailableVoices.Clear();

        var voices = _ttsService.GetVoicesByLanguage(languageCode);

        foreach (var voice in voices)
        {
            AvailableVoices.Add(voice);
        }

        // Seleccionar la primera voz disponible si existe
        if (AvailableVoices.Count > 0)
        {
            SelectedVoice = AvailableVoices[0];
        }
        else
        {
            // Si no hay voces para este idioma, usar cualquier voz disponible
            var allVoices = _ttsService.GetAvailableVoices();
            if (allVoices.Count > 0)
            {
                var fallbackVoice = allVoices[0];
                AvailableVoices.Add(fallbackVoice);
                SelectedVoice = fallbackVoice;
            }
        }
    }

    private void OnTtsProgressChanged(object? sender, int progress)
    {
        ReadingProgress = progress;

        // Solicitar desplazamiento basado en el progreso
        ScrollRequested?.Invoke(this, progress);
    }

    private void OnTtsSpeakCompleted(object? sender, EventArgs e)
    {
        IsReading = false;
        IsPaused = false;
        ReadingProgress = 0;

        // Actualizar estado de comandos
        StartReadingCommand.NotifyCanExecuteChanged();
        PauseReadingCommand.NotifyCanExecuteChanged();
        ResumeReadingCommand.NotifyCanExecuteChanged();
        StopReadingCommand.NotifyCanExecuteChanged();
    }

    private void ShowError(string message)
    {
        HtmlContent = _markdownService.ConvertMarkdownToHtml($@"# ❌ Error

{message}");
    }
}
