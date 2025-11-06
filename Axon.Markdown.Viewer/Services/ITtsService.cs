using Axon.Markdown.Viewer.Models;

namespace Axon.Markdown.Viewer.Services;

public interface ITtsService
{
    event EventHandler<int>? ProgressChanged;
    event EventHandler? SpeakCompleted;

    void Speak(string text);
    void Pause();
    void Resume();
    void Stop();
    bool IsSpeaking { get; }
    bool IsPaused { get; }
    int GetProgress();
    void SetRate(int rate);
    void SetVolume(int volume);

    // Métodos para voces
    List<VoiceInfo> GetAvailableVoices();
    List<VoiceInfo> GetVoicesByLanguage(string languageCode);
    void SelectVoice(string voiceName);
    VoiceInfo? GetCurrentVoice();
}
