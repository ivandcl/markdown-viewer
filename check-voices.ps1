Add-Type -AssemblyName System.Speech
$synth = New-Object System.Speech.Synthesis.SpeechSynthesizer
Write-Host "=== VOCES INSTALADAS EN EL SISTEMA ===" -ForegroundColor Green
Write-Host ""
$synth.GetInstalledVoices() | Where-Object { $_.Enabled } | ForEach-Object {
    $voice = $_.VoiceInfo
    Write-Host "Nombre: $($voice.Name)" -ForegroundColor Cyan
    Write-Host "  Idioma: $($voice.Culture.DisplayName) [$($voice.Culture.Name)]"
    Write-Host "  Género: $($voice.Gender)"
    Write-Host "  Edad: $($voice.Age)"
    Write-Host ""
}
