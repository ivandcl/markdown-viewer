# Axon Markdown Viewer - Documento de Prueba

## Introducción

Este es un **documento de ejemplo** para probar el _Axon Markdown Viewer_. Esta aplicación está diseñada para visualizar archivos Markdown con soporte extendido de **GitHub Flavored Markdown**.

---

## Características Soportadas

### 1. Formato de Texto

- **Texto en negrita**
- *Texto en cursiva*
- ***Texto en negrita y cursiva***
- ~~Texto tachado~~
- `Código inline`

### 2. Listas

#### Lista desordenada:
- Elemento 1
- Elemento 2
  - Sub-elemento 2.1
  - Sub-elemento 2.2
- Elemento 3

#### Lista ordenada:
1. Primer paso
2. Segundo paso
3. Tercer paso

### 3. Listas de Tareas

- [x] Tarea completada
- [x] Implementar renderizado de Markdown
- [x] Agregar tema oscuro
- [ ] Tarea pendiente
- [ ] Agregar más características

### 4. Tablas

| Característica | Estado | Prioridad |
|---------------|--------|-----------|
| Renderizado básico | ✅ Completado | Alta |
| Tema oscuro | ✅ Completado | Alta |
| Tablas | ✅ Completado | Media |
| Syntax highlighting | ✅ Completado | Media |
| Exportar a PDF | ❌ Pendiente | Baja |

### 5. Bloques de Código

```csharp
public class MarkdownViewer
{
    private readonly IMarkdownService _service;

    public MarkdownViewer(IMarkdownService service)
    {
        _service = service;
    }

    public async Task<string> RenderAsync(string markdownContent)
    {
        return await _service.ConvertMarkdownToHtmlAsync(markdownContent);
    }
}
```

```javascript
function greetUser(name) {
    console.log(`¡Hola, ${name}!`);
    return `Bienvenido/a ${name} a Axon Markdown Viewer`;
}

greetUser("Usuario");
```

```python
def fibonacci(n):
    if n <= 1:
        return n
    return fibonacci(n-1) + fibonacci(n-2)

# Calcular los primeros 10 números de Fibonacci
for i in range(10):
    print(f"F({i}) = {fibonacci(i)}")
```

### 6. Citas

> "La documentación es una carta de amor que escribes para tu futuro yo."
>
> — Damian Conway

> Múltiples niveles de citas:
> > Nivel 2
> > > Nivel 3

### 7. Enlaces e Imágenes

- [Repositorio GitHub](https://github.com)
- [Documentación Markdown](https://www.markdownguide.org/)

### 8. Líneas Horizontales

---

## Casos de Uso

Esta aplicación es ideal para:

1. **Revisar documentación generada por Claude**
   - Especificaciones de requerimientos
   - Documentación técnica
   - Presentaciones

2. **Visualizar archivos README**
   - De proyectos GitHub
   - De proyectos locales
   - Documentación de APIs

3. **Leer artículos y notas en Markdown**
   - Blogs técnicos
   - Notas personales
   - Documentación de proyectos

---

## Atajos de Teclado

| Atajo | Acción |
|-------|--------|
| `Ctrl + O` | Abrir archivo |
| `F5` | Recargar archivo actual |
| `Ctrl + +` | Acercar zoom |
| `Ctrl + -` | Alejar zoom |
| `Ctrl + 0` | Restablecer zoom |
| `Alt + F4` | Salir |

---

## Código Complejo

Aquí hay un ejemplo de código más complejo con comentarios:

```typescript
interface User {
    id: number;
    name: string;
    email: string;
    role: 'admin' | 'user' | 'guest';
}

class UserService {
    private users: Map<number, User> = new Map();

    /**
     * Agrega un nuevo usuario al sistema
     * @param user - El usuario a agregar
     * @returns El usuario agregado con ID asignado
     */
    addUser(user: Omit<User, 'id'>): User {
        const id = this.users.size + 1;
        const newUser: User = { ...user, id };
        this.users.set(id, newUser);
        return newUser;
    }

    /**
     * Obtiene un usuario por ID
     * @param id - El ID del usuario
     * @returns El usuario o undefined si no existe
     */
    getUserById(id: number): User | undefined {
        return this.users.get(id);
    }
}
```

---

## Matemáticas y Fórmulas

Aunque no es interpretado como LaTeX en este viewer básico, aquí hay ejemplos:

- La ecuación cuadrática: `ax² + bx + c = 0`
- El teorema de Pitágoras: `a² + b² = c²`
- La fórmula de Euler: `e^(iπ) + 1 = 0`

---

## Conclusión

**Axon Markdown Viewer** proporciona una experiencia de visualización moderna y minimalista para archivos Markdown, con soporte completo para GitHub Flavored Markdown.

### Características Destacadas:
- ✨ Tema oscuro elegante
- 🚀 Renderizado rápido
- 📝 Soporte completo de GFM
- 🎨 Syntax highlighting
- 🖱️ Interfaz intuitiva

---

*Documento generado el: 2025-11-05*
*Versión: 1.0*
*© 2025 Axon Group*
