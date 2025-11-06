@echo off
:: Script para ejecutar el registro de asociación como Administrador

echo ================================================================
echo   Axon Markdown Viewer - Registro de Asociacion de Archivos
echo ================================================================
echo.
echo Este script registrara la asociacion de archivos .md con
echo Axon Markdown Viewer.
echo.
echo NOTA: Se abrira una ventana de PowerShell con privilegios
echo       de Administrador. Por favor, acepta el UAC cuando aparezca.
echo.
pause

:: Ejecutar PowerShell como administrador
powershell -Command "Start-Process powershell -ArgumentList '-ExecutionPolicy Bypass -File \"%~dp0Registrar-AsociacionMD.ps1\"' -Verb RunAs"

echo.
echo El script se esta ejecutando en otra ventana...
echo Puedes cerrar esta ventana.
echo.
pause
