# Script para eliminar la asociación de archivos .md con Axon Markdown Viewer
# Debe ejecutarse como Administrador

# Verificar privilegios de administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Red
    Write-Host "  ERROR: Este script requiere privilegios de Administrador" -ForegroundColor Red
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor, ejecuta este script como Administrador:" -ForegroundColor Yellow
    Write-Host "1. Click derecho en el archivo" -ForegroundColor Cyan
    Write-Host "2. Selecciona 'Ejecutar como Administrador'" -ForegroundColor Cyan
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  Axon Markdown Viewer - Eliminar Asociación de Archivos" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "ADVERTENCIA: Esta acción eliminará la asociación de archivos .md" -ForegroundColor Yellow
Write-Host "con Axon Markdown Viewer." -ForegroundColor Yellow
Write-Host ""
$confirm = Read-Host "¿Deseas continuar? (S/N)"

if ($confirm -ne "S" -and $confirm -ne "s") {
    Write-Host "Operación cancelada." -ForegroundColor Yellow
    Read-Host "Presiona Enter para salir"
    exit 0
}

Write-Host ""

$progId = "AxonMarkdownViewer.Document"

Write-Host "[1/4] Eliminando ProgID de la aplicación..." -ForegroundColor Green
$regPathProgId = "Registry::HKEY_CLASSES_ROOT\$progId"
if (Test-Path $regPathProgId) {
    Remove-Item -Path $regPathProgId -Force -Recurse -ErrorAction SilentlyContinue
    Write-Host "      ✓ ProgID eliminado" -ForegroundColor Gray
} else {
    Write-Host "      ⓘ ProgID no encontrado (ya estaba eliminado)" -ForegroundColor Gray
}

Write-Host "[2/4] Eliminando asociación de extensión .md..." -ForegroundColor Green
$regPathExt = "Registry::HKEY_CLASSES_ROOT\.md"
if (Test-Path $regPathExt) {
    $currentValue = (Get-ItemProperty -Path $regPathExt -Name "(Default)" -ErrorAction SilentlyContinue)."(Default)"
    if ($currentValue -eq $progId) {
        Remove-ItemProperty -Path $regPathExt -Name "(Default)" -Force -ErrorAction SilentlyContinue
        Write-Host "      ✓ Asociación predeterminada eliminada" -ForegroundColor Gray
    } else {
        Write-Host "      ⓘ .md no estaba asociado con Axon Markdown Viewer" -ForegroundColor Gray
    }

    # Eliminar de OpenWithProgids
    $regPathOpenWith = "$regPathExt\OpenWithProgids"
    if (Test-Path $regPathOpenWith) {
        Remove-ItemProperty -Path $regPathOpenWith -Name $progId -Force -ErrorAction SilentlyContinue
        Write-Host "      ✓ Eliminado de la lista 'Abrir con'" -ForegroundColor Gray
    }
}

Write-Host "[3/4] Eliminando registro de aplicación..." -ForegroundColor Green
$regPathApps = "Registry::HKEY_CLASSES_ROOT\Applications\Axon.Markdown.Viewer.exe"
if (Test-Path $regPathApps) {
    Remove-Item -Path $regPathApps -Force -Recurse -ErrorAction SilentlyContinue
    Write-Host "      ✓ Registro de aplicación eliminado" -ForegroundColor Gray
} else {
    Write-Host "      ⓘ Registro de aplicación no encontrado" -ForegroundColor Gray
}

Write-Host "[4/4] Limpiando preferencias de usuario..." -ForegroundColor Green
$regPathUserChoice = "Registry::HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.md\UserChoice"
if (Test-Path $regPathUserChoice) {
    $userChoice = (Get-ItemProperty -Path $regPathUserChoice -Name "ProgId" -ErrorAction SilentlyContinue).ProgId
    if ($userChoice -eq $progId) {
        Remove-Item -Path $regPathUserChoice -Force -Recurse -ErrorAction SilentlyContinue
        Write-Host "      ✓ Preferencia de usuario eliminada" -ForegroundColor Gray
    } else {
        Write-Host "      ⓘ Usuario no tenía configurado Axon Markdown Viewer" -ForegroundColor Gray
    }
}

# Notificar al sistema sobre los cambios
$code = @"
[System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
public static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
"@
Add-Type -MemberDefinition $code -Namespace Win32 -Name Shell32

[Win32.Shell32]::SHChangeNotify(0x08000000, 0x0000, [IntPtr]::Zero, [IntPtr]::Zero)

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host "  ✓ Asociación eliminada exitosamente" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host ""
Write-Host "La asociación de archivos .md con Axon Markdown Viewer" -ForegroundColor Cyan
Write-Host "ha sido eliminada del sistema." -ForegroundColor Cyan
Write-Host ""
Write-Host "Nota: Windows puede volver a la asociación predeterminada" -ForegroundColor Yellow
Write-Host "anterior o pedirte que elijas un programa la próxima vez" -ForegroundColor Yellow
Write-Host "que abras un archivo .md" -ForegroundColor Yellow
Write-Host ""
Read-Host "Presiona Enter para salir"
