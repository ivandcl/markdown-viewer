# Script para asociar archivos .md con Axon Markdown Viewer
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
Write-Host "  Axon Markdown Viewer - Registro de Asociación de Archivos" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# Obtener la ruta del directorio del script
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$exePath = Join-Path $scriptPath "Axon.Markdown.Viewer.exe"
$iconPath = Join-Path $scriptPath "Axon.Markdown.Viewer\app.ico"

# Verificar que el ejecutable existe
if (-not (Test-Path $exePath)) {
    Write-Host "ERROR: No se encontró Axon.Markdown.Viewer.exe" -ForegroundColor Red
    Write-Host "Ruta buscada: $exePath" -ForegroundColor Yellow
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "[1/5] Verificando archivos..." -ForegroundColor Green
Write-Host "      Ejecutable: $exePath" -ForegroundColor Gray

# Registrar el ProgID (Identificador de Programa)
$progId = "AxonMarkdownViewer.Document"
$appName = "Axon Markdown Viewer"

Write-Host "[2/5] Registrando aplicación en el sistema..." -ForegroundColor Green

# Crear clave del ProgID
$regPathProgId = "Registry::HKEY_CLASSES_ROOT\$progId"
New-Item -Path $regPathProgId -Force | Out-Null
Set-ItemProperty -Path $regPathProgId -Name "(Default)" -Value "Markdown Document"
Set-ItemProperty -Path $regPathProgId -Name "FriendlyTypeName" -Value "$appName Document"

# Crear clave de DefaultIcon
$regPathIcon = "$regPathProgId\DefaultIcon"
New-Item -Path $regPathIcon -Force | Out-Null
if (Test-Path $iconPath) {
    Set-ItemProperty -Path $regPathIcon -Name "(Default)" -Value "`"$iconPath`",0"
} else {
    Set-ItemProperty -Path $regPathIcon -Name "(Default)" -Value "`"$exePath`",0"
}

# Crear clave de shell\open\command
$regPathCommand = "$regPathProgId\shell\open\command"
New-Item -Path $regPathCommand -Force | Out-Null
Set-ItemProperty -Path $regPathCommand -Name "(Default)" -Value "`"$exePath`" `"%1`""

Write-Host "[3/5] Asociando extensión .md..." -ForegroundColor Green

# Asociar la extensión .md con el ProgID
$regPathExt = "Registry::HKEY_CLASSES_ROOT\.md"
New-Item -Path $regPathExt -Force | Out-Null
Set-ItemProperty -Path $regPathExt -Name "(Default)" -Value $progId

# Agregar a la lista de programas sugeridos
$regPathOpenWith = "$regPathExt\OpenWithProgids"
New-Item -Path $regPathOpenWith -Force | Out-Null
Set-ItemProperty -Path $regPathOpenWith -Name $progId -Value ([byte[]]@()) -Type Binary

Write-Host "[4/5] Registrando en 'Abrir con...'..." -ForegroundColor Green

# Registrar la aplicación en la lista de aplicaciones
$regPathApps = "Registry::HKEY_CLASSES_ROOT\Applications\Axon.Markdown.Viewer.exe"
New-Item -Path $regPathApps -Force | Out-Null

$regPathAppsCommand = "$regPathApps\shell\open\command"
New-Item -Path $regPathAppsCommand -Force | Out-Null
Set-ItemProperty -Path $regPathAppsCommand -Name "(Default)" -Value "`"$exePath`" `"%1`""

# Registrar en Current User para mayor compatibilidad
$regPathUserChoice = "Registry::HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.md\UserChoice"
if (Test-Path $regPathUserChoice) {
    Remove-Item -Path $regPathUserChoice -Force -Recurse -ErrorAction SilentlyContinue
}

Write-Host "[5/5] Actualizando explorador de archivos..." -ForegroundColor Green

# Notificar al sistema sobre los cambios
$code = @"
[System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
public static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
"@
Add-Type -MemberDefinition $code -Namespace Win32 -Name Shell32

# SHCNE_ASSOCCHANGED = 0x08000000, SHCNF_IDLIST = 0x0000
[Win32.Shell32]::SHChangeNotify(0x08000000, 0x0000, [IntPtr]::Zero, [IntPtr]::Zero)

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host "  ✓ Asociación completada exitosamente" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host ""
Write-Host "Ahora puedes:" -ForegroundColor Cyan
Write-Host "  • Hacer doble clic en archivos .md para abrirlos" -ForegroundColor White
Write-Host "  • Click derecho > 'Abrir con' > Axon Markdown Viewer" -ForegroundColor White
Write-Host ""
Write-Host "NOTA: Si Windows ya tenía una asociación previa," -ForegroundColor Yellow
Write-Host "es posible que necesites:" -ForegroundColor Yellow
Write-Host "  1. Click derecho en un archivo .md" -ForegroundColor Cyan
Write-Host "  2. 'Abrir con' > 'Elegir otra aplicación'" -ForegroundColor Cyan
Write-Host "  3. Seleccionar 'Axon Markdown Viewer'" -ForegroundColor Cyan
Write-Host "  4. Marcar 'Usar siempre esta aplicación'" -ForegroundColor Cyan
Write-Host ""
Read-Host "Presiona Enter para salir"
