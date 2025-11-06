using System.Speech.Synthesis;
using System.Windows.Threading;
using Axon.Markdown.Viewer.Models;

namespace Axon.Markdown.Viewer.Services;

public class TtsService : ITtsService, IDisposable
{
    private readonly SpeechSynthesizer _synthesizer;
    private readonly Dispatcher _dispatcher;
    private string _currentText = string.Empty;
    private int _currentCharPosition = 0;
    private bool _isPaused = false;

    public event EventHandler<int>? ProgressChanged;
    public event EventHandler? SpeakCompleted;

    public bool IsSpeaking { get; private set; }
    public bool IsPaused => _isPaused;

    public TtsService()
    {
        _dispatcher = Dispatcher.CurrentDispatcher;
        _synthesizer = new SpeechSynthesizer();
        _synthesizer.SetOutputToDefaultAudioDevice();

        // Suscribirse a eventos
        _synthesizer.SpeakProgress += OnSpeakProgress;
        _synthesizer.SpeakCompleted += OnSpeakCompleted;
        _synthesizer.StateChanged += OnStateChanged;
    }

    public void Speak(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        Stop();

        _currentText = text;
        _currentCharPosition = 0;
        IsSpeaking = true;
        _isPaused = false;

        // Hablar de forma asíncrona
        _synthesizer.SpeakAsync(text);
    }

    public void Pause()
    {
        if (IsSpeaking && !_isPaused)
        {
            _synthesizer.Pause();
            _isPaused = true;
        }
    }

    public void Resume()
    {
        if (IsSpeaking && _isPaused)
        {
            _synthesizer.Resume();
            _isPaused = false;
        }
    }

    public void Stop()
    {
        if (IsSpeaking)
        {
            _synthesizer.SpeakAsyncCancelAll();
            IsSpeaking = false;
            _isPaused = false;
            _currentCharPosition = 0;
        }
    }

    public int GetProgress()
    {
        if (string.IsNullOrEmpty(_currentText))
            return 0;

        return (int)((_currentCharPosition / (double)_currentText.Length) * 100);
    }

    public void SetRate(int rate)
    {
        // Rate: -10 (muy lento) a 10 (muy rápido), 0 es normal
        _synthesizer.Rate = Math.Clamp(rate, -10, 10);
    }

    public void SetVolume(int volume)
    {
        // Volume: 0 a 100
        _synthesizer.Volume = Math.Clamp(volume, 0, 100);
    }

    private void OnSpeakProgress(object? sender, SpeakProgressEventArgs e)
    {
        _currentCharPosition = e.CharacterPosition;

        _dispatcher.Invoke(() =>
        {
            ProgressChanged?.Invoke(this, GetProgress());
        });
    }

    private void OnSpeakCompleted(object? sender, SpeakCompletedEventArgs e)
    {
        IsSpeaking = false;
        _isPaused = false;
        _currentCharPosition = 0;

        _dispatcher.Invoke(() =>
        {
            SpeakCompleted?.Invoke(this, EventArgs.Empty);
        });
    }

    private void OnStateChanged(object? sender, StateChangedEventArgs e)
    {
        // Manejar cambios de estado si es necesario
    }

    public List<Models.VoiceInfo> GetAvailableVoices()
    {
        var voices = new List<Models.VoiceInfo>();

        foreach (var voice in _synthesizer.GetInstalledVoices())
        {
            if (voice.Enabled)
            {
                var info = voice.VoiceInfo;
                voices.Add(new Models.VoiceInfo
                {
                    Name = info.Name,
                    Culture = info.Culture.Name,
                    Gender = info.Gender.ToString(),
                    Age = (int)info.Age
                });
            }
        }

        return voices;
    }

    public List<Models.VoiceInfo> GetVoicesByLanguage(string languageCode)
    {
        var allVoices = GetAvailableVoices();

        // Filtrar por código de idioma (ej: "es" para español, "en" para inglés)
        return allVoices
            .Where(v => v.Culture.StartsWith(languageCode, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void SelectVoice(string voiceName)
    {
        try
        {
            _synthesizer.SelectVoice(voiceName);
        }
        catch (ArgumentException)
        {
            // La voz no está disponible, mantener la voz actual
        }
    }

    public Models.VoiceInfo? GetCurrentVoice()
    {
        var currentVoice = _synthesizer.Voice;
        if (currentVoice == null)
            return null;

        return new Models.VoiceInfo
        {
            Name = currentVoice.Name,
            Culture = currentVoice.Culture.Name,
            Gender = currentVoice.Gender.ToString(),
            Age = (int)currentVoice.Age
        };
    }

    public void Dispose()
    {
        Stop();
        _synthesizer.Dispose();
    }
}
