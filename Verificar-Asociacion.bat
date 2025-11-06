@echo off
:: Script para verificar el estado de la asociación de archivos .md

powershell -ExecutionPolicy Bypass -File "%~dp0Verificar-Asociacion.ps1"
