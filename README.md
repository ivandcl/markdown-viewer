# Axon Markdown Viewer

Una aplicación WPF moderna y minimalista para visualizar archivos Markdown con soporte extendido de GitHub Flavored Markdown.

## Características

- ✨ **Tema oscuro moderno** - Interfaz elegante y cómoda para la vista
- 🚀 **Renderizado rápido** - Utiliza Markdig y WebView2 para un renderizado eficiente
- 📝 **GitHub Flavored Markdown** - Soporte completo para tablas, listas de tareas, código, etc.
- 🎨 **Syntax highlighting** - Resaltado de sintaxis para bloques de código
- 🖱️ **Múltiples formas de abrir archivos**:
  - Desde el menú (Archivo > Abrir)
  - Por línea de comandos
  - Arrastrando y soltando archivos
- 🔍 **Control de zoom** - Acerca, aleja o restablece el zoom del contenido
- ⌨️ **Atajos de teclado** - Accesos rápidos para operaciones comunes

## Requisitos

- Windows 10/11
- WebView2 Runtime (generalmente pre-instalado en Windows 11)

**Nota**: El ejecutable incluido (`Axon.Markdown.Viewer.exe`) es autocontenido y NO requiere tener .NET instalado.

## Inicio Rápido

### ⚡ Configuración Recomendada (Una sola vez)

Para **asociar archivos `.md`** con Axon Markdown Viewer y poder abrir archivos con doble clic:

1. Ejecuta **`Registrar-AsociacionMD.bat`** (requiere permisos de Administrador)
2. Acepta el UAC (Control de Cuentas de Usuario)
3. ¡Listo! Ahora puedes hacer **doble clic en cualquier archivo `.md`** para abrirlo

> 📘 Ver **`ASOCIACION-ARCHIVOS.txt`** para instrucciones detalladas
>
> 💡 Para revertir: Ejecuta `Desregistrar-AsociacionMD.bat`

### Opción 1: Doble clic (Más fácil)

1. **Doble clic en `Axon.Markdown.Viewer.exe`** para abrir la aplicación
2. Usa el menú `Archivo > Abrir` para seleccionar un archivo `.md`
3. O arrastra y suelta un archivo `.md` en la ventana

### Opción 2: Usando el script run.bat

```bash
# Ejecuta la aplicación con el archivo de ejemplo
run.bat
```

### Opción 3: Línea de comandos

```bash
# Sin argumentos (muestra pantalla de bienvenida)
Axon.Markdown.Viewer.exe

# Con archivo como argumento
Axon.Markdown.Viewer.exe ejemplo.md

# Con ruta completa
Axon.Markdown.Viewer.exe "C:\ruta\a\tu\archivo.md"
```

## Compilación desde Código Fuente

Solo si deseas compilar desde el código fuente:

```bash
cd Axon.Markdown.Viewer
dotnet build

# Para generar el ejecutable autocontenido:
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Uso

### Abrir archivos

1. **Desde el menú**: `Archivo > Abrir` o presiona `Ctrl+O`
2. **Línea de comandos**: `Axon.Markdown.Viewer.exe archivo.md`
3. **Arrastrar y soltar**: Arrastra un archivo `.md` a la ventana de la aplicación

### Atajos de teclado

| Atajo | Acción |
|-------|--------|
| `Ctrl + O` | Abrir archivo |
| `F5` | Recargar archivo actual |
| `Ctrl + +` | Acercar zoom |
| `Ctrl + -` | Alejar zoom |
| `Ctrl + 0` | Restablecer zoom |
| `Alt + F4` | Salir de la aplicación |

## Estructura del Proyecto

```
Axon.Markdown.Viewer/
├── Models/              # Modelos de datos
├── ViewModels/          # ViewModels (MVVM)
│   ├── ViewModelBase.cs
│   └── MainViewModel.cs
├── Views/               # Vistas XAML
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
├── Services/            # Servicios
│   ├── IMarkdownService.cs
│   └── MarkdownService.cs
└── Resources/           # Recursos (estilos, imágenes, etc.)
```

## Patrón MVVM

La aplicación sigue el patrón **Model-View-ViewModel (MVVM)**:

- **Model**: Representación de datos (actualmente no se necesitan modelos complejos)
- **View**: Interfaces de usuario en XAML (MainWindow.xaml)
- **ViewModel**: Lógica de presentación (MainViewModel.cs)
- **Services**: Lógica de negocio (MarkdownService.cs)

## Tecnologías Utilizadas

- **.NET 9.0** - Framework de aplicación
- **WPF (Windows Presentation Foundation)** - Framework de UI
- **Markdig** - Procesador de Markdown de alto rendimiento
- **WebView2** - Control de navegador basado en Chromium para renderizar HTML
- **CommunityToolkit.Mvvm** - Helpers para implementar MVVM

## Características Markdown Soportadas

### Formato básico
- Encabezados (H1-H6)
- Negrita, cursiva, tachado
- Listas ordenadas y desordenadas
- Enlaces e imágenes
- Citas
- Líneas horizontales

### Características extendidas (GFM)
- ✅ Tablas
- ✅ Listas de tareas (checkboxes)
- ✅ Bloques de código con syntax highlighting
- ✅ Código inline
- ✅ Autolinks
- ✅ Strikethrough

## Casos de Uso Ideales

Esta aplicación está especialmente diseñada para:

1. **Revisar documentación generada por Claude**
   - Especificaciones de requerimientos
   - Documentación técnica
   - Presentaciones y reportes

2. **Visualizar archivos README**
   - De repositorios GitHub
   - De proyectos locales
   - Documentación de APIs

3. **Leer y revisar contenido Markdown**
   - Artículos técnicos
   - Notas de desarrollo
   - Documentación de proyectos

## Personalización

Los estilos CSS del tema oscuro se encuentran en `Services/MarkdownService.cs` en el método `GetDarkThemeCss()`. Puedes modificar los colores y estilos según tus preferencias.

## Licencia

© 2025 Axon Group

## Ejemplo

Para probar la aplicación, incluimos un archivo de ejemplo:

```bash
dotnet run -- ejemplo.md
```

Este archivo demuestra todas las características soportadas del renderizado Markdown.

---

**Desarrollado con ❤️ usando .NET y WPF**
