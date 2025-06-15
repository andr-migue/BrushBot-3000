# 🖌️ BrushBot-3000 🎨

![image](https://github.com/user-attachments/assets/64e68678-b950-4044-99cd-11437b3f4a91)

*"Arte abstracto dirán algunos"* 😜

## 🌟 Descripción Breve
BrushBot-3000 es una aplicación desarrollada en **Godot Engine** que te permite programar un simpático bot para crear dibujos y arte pixelado. ¡Todo esto mediante un lenguaje de scripting personalizado! Es una herramienta fantástica y educativa para sumergirse en los conceptos básicos de la programación y desatar tu creatividad gráfica. 🚀

## ✨ Características Principales
- **📜 Lenguaje de Scripting Propio**: Un lenguaje simple e intuitivo, ¡hecho a medida para dibujar!
- **🤖 Control del Pincel**: Comandos para mover el bot (`Spawn`, `Respawn`, `GoTo`), cambiar el color (`Color`) y el tamaño del pincel (`Size`).
- **🎨 Operaciones de Dibujo**:
    - `DrawLine`: Dibuja líneas entre dos puntos.
    - `DrawCircle`: Crea círculos perfectos.
    - `DrawRectangle`: Forma rectángulos y cuadrados.
    - `Fill`: ¡Rellena áreas cerradas con el color que elijas!
- **🧠 Lógica de Programación**: Soporte para variables, asignaciones, y expresiones aritméticas y lógicas.
- **↪️ Control de Flujo**: Usa etiquetas (`Label`) y saltos (`Jump`) para dirigir la aventura de tu bot.
- **🔧 Funciones Integradas**: Un arsenal de funciones como `GetActualX()`, `GetActualY()`, `GetCanvasSize()`, `RGBA()`, y más.
- **👁️‍🗨️ Interfaz Gráfica en Godot**: Observa en tiempo real cómo tu bot da vida a tus scripts en un lienzo digital.
- **🐞 Reporte de Errores**: Un sistema amigable que te ayuda a encontrar y corregir errores léxicos, sintácticos y semánticos.
- **🌈 Manejo de Colores**: Elige entre colores predefinidos (ej. "Red", "Blue") o crea los tuyos con `RGBA`. ¡El color "White" es tu borrador mágico!

## ⚙️ ¿Cómo Funciona? Un Vistazo Técnico Profundo

El corazón de BrushBot-3000 es su intérprete, que procesa tus scripts `.pw` en varias etapas para convertir tus comandos en arte visual:

1.  **📜 Análisis Léxico (Lexer - `Lexer.cs`)** 🧐
    *   **Entrada**: Tu código BrushScript como una cadena de texto.
    *   **Proceso**: El `Lexer` (o tokenizador) recorre el texto carácter por carácter. Identifica secuencias de caracteres que forman unidades lógicas, como palabras clave (`Color`, `Spawn`), números (`100`, `2.5`), cadenas de texto (`"Blue"`), operadores (`<-`, `+`), delimitadores (`(`, `)`), etc.
    *   **Salida**: Una secuencia (o "stream") de **Tokens**. Cada token es un objeto que representa una de estas unidades, guardando su tipo (ej. `TokenType.Keyword`, `TokenType.Number`) y su valor (ej. `"Color"`, `"100"`), además de su posición en el código fuente para el reporte de errores.
    *   *Ejemplo*: La línea `Color("Red")` se convertiría en tokens como: `Keyword("Color")`, `Delimiter("(")` , `Color("Red")`, `Delimiter(")")`.
    *   
2.  **🌳 Análisis Sintáctico (Parser - `Parser.cs`)** 🧩
    *   **Entrada**: La secuencia de Tokens generada por el Lexer.
    *   **Proceso**: El `Parser` toma estos tokens y verifica si siguen las reglas gramaticales del lenguaje BrushScript. Construye una estructura jerárquica llamada **Árbol de Sintaxis Abstracta (AST)**. El AST representa la estructura gramatical del código de una manera que es más fácil de procesar para las etapas posteriores. Cada nodo en el árbol representa una construcción del lenguaje (como una asignación, una llamada a función, una expresión, etc.).
    *   **Salida**: Un AST. Si el código no sigue las reglas sintácticas (ej. un paréntesis sin cerrar), el Parser reportará errores sintácticos.
    *   *Ejemplo*: La secuencia de tokens de `Color("Red")` se transformaría en un nodo de tipo "Instruction" (o similar) en el AST, con "Color" como el nombre de la instrucción y un nodo "Literal Color" con valor "Red" como su argumento.

3.  **💡 Análisis Semántico (Semanter - `Semanter.cs`)** ✅
    *   **Entrada**: El AST generado por el Parser.
    *   **Proceso**: El `Semanter` recorre el AST para verificar la coherencia y el significado del código. Realiza comprobaciones como:
        *   ¿Las variables se usan después de ser declaradas?
        *   ¿Los tipos de datos son compatibles en las operaciones (ej. no intentar sumar un número con una cadena de texto directamente sin conversión)?
        *   ¿Las funciones o instrucciones se llaman con el número y tipo correcto de argumentos?
        *   ¿Las etiquetas a las que se salta existen?
    *   **Salida**: Un AST validado (posiblemente anotado con información adicional, como tipos resueltos) y una lista de errores semánticos si se encuentran problemas.
    *   *Ejemplo*: Si tienes `Color(123)`, el Semanter podría marcar un error si la instrucción `Color` espera una cadena de texto o un objeto Color, no un número.

4.  **▶️ Interpretación (Interpreter - `Interpreter.cs`)** 🚀
    *   **Entrada**: El AST validado (y libre de errores graves) por el Semanter.
    *   **Proceso**: El `Interpreter` recorre el AST y ejecuta las acciones correspondientes a cada nodo.
        *   Para un nodo de asignación, calcula el valor de la expresión y lo guarda en la variable correspondiente (gestionada en un `Scope`).
        *   Para un nodo de instrucción de dibujo (como `DrawLine` o `Color`), invoca la lógica correspondiente para realizar esa acción.
        *   Para nodos de control de flujo (`Jump`), altera el orden de ejecución.
    *   **Salida**: Los efectos de los comandos ejecutados (dibujos en el lienzo, salida en la consola, etc.).

## ✨ Conexión con Godot: La Magia Visual 🎬

La forma en que el intérprete se conecta con la parte visual en Godot es crucial:

1.  **Orquestación (`Main.cs`)**: Un script principal en Godot (probablemente `Main.cs` o similar en `EngineScripts/`) maneja la interfaz de usuario (UI). Permite al usuario cargar archivos `.pw`, iniciar la ejecución, y ver el lienzo.
2.  **Disparador**: Cuando el usuario ejecuta un script, `Main.cs` crea instancias del `Lexer`, `Parser`, `Semanter`, e `Interpreter`.
3.  **Ejecución de Comandos (`Interpreter.cs` -> `Handle.cs`)**:
    *   Cuando el `Interpreter` encuentra un nodo en el AST que representa una acción de dibujo (ej. `DrawLine(10,10,50,50)`) o una manipulación del estado del pincel (ej. `Color("Blue")`), no dibuja directamente.
    *   En lugar de eso, delega estas tareas a una clase especializada, como `Handle.cs`. El `Interpreter` llama a métodos en `Handle.cs` (ej. `Handle.DrawLine(params...)`, `Handle.SetColor(params...)`).
4.  **Estado Centralizado (`Context.cs`)**:
    *   La clase `Handle.cs` interactúa con `Context.cs`. El `Context` es como el cerebro del bot: almacena el estado actual del pincel (posición, color, tamaño), las dimensiones del lienzo, y lo más importante, la representación de la imagen que se está dibujando (por ejemplo, un array 2D de objetos `Color` que representa cada píxel del lienzo: `Context.Picture`).
5.  **Actualización del Lienzo en Godot**:
    *   Cuando `Handle.cs` modifica `Context.Picture` (ej. al pintar píxeles para una línea), estos cambios son solo en la memoria de C#.
    *   Para que se vean en Godot, `Handle.cs` (o `Context.cs`, o incluso `Main.cs` al final de cada "paso" de dibujo) necesita actualizar el nodo visual en Godot que representa el lienzo.
    *   Esto se hace típicamente con un nodo `TextureRect` en Godot. La información de `Context.Picture` se usa para crear o actualizar un objeto `ImageTexture` en Godot, que luego se asigna a la propiedad `Texture` del `TextureRect`.
    *   Godot se encarga de renderizar ese `TextureRect` en la pantalla.
6.  **Visualización del Pincel**: De manera similar, la posición y apariencia del nodo del pincel en la escena de Godot (`brush.tscn`) se actualizan basándose en el estado mantenido en `Context.cs` (ej. `Context.BrushPosition`, `Context.BrushColor`).

En resumen: **Script `.pw` -> Lexer -> Parser -> Semanter -> Interpreter -> `Handle.cs` (lógica de dibujo) -> `Context.cs` (estado) -> Actualización de Nodos de Godot (ej. `TextureRect`) -> ¡Magia Visual!** ✨

## ⚠️ Control de Errores y Feedback al Usuario

BrushBot-3000 se esfuerza por proporcionar información clara cuando algo no va según lo planeado en tus scripts. Los errores se detectan y reportan en diferentes etapas del procesamiento:

### 1. Errores Léxicos (Detectados por el `Lexer`)
Estos errores ocurren cuando el script contiene secuencias de caracteres que no forman tokens válidos.
- **Ejemplos Comunes**: Símbolos desconocidos (ej. `@`, `#` si no son parte de la sintaxis), cadenas de texto sin cerrar comillas.
- **Feedback**: El `Lexer` identificará el carácter o secuencia problemática y su ubicación (línea y columna), reportando algo como: `Error Léxico: Carácter inesperado '?' en la línea 5, columna 10.`

### 2. Errores Sintácticos (Detectados por el `Parser`)
Estos errores surgen cuando la secuencia de tokens, aunque individualmente válidos, no sigue las reglas gramaticales del lenguaje BrushScript.
- **Ejemplos Comunes**: Paréntesis desbalanceados, falta de argumentos en una instrucción, una instrucción en un lugar inesperado.
- **Feedback**: El `Parser` indicará dónde se rompió la estructura gramatical, por ejemplo: `Error Sintáctico: Se esperaba ')' pero se encontró ';' en la línea 12, columna 20.`

### 3. Errores Semánticos (Detectados por el `Semanter`)
Estos errores ocurren cuando el script es sintácticamente correcto, pero no tiene sentido lógico o viola las reglas de tipo del lenguaje.
- **Ejemplos Comunes**: Usar una variable no declarada, intentar asignar un tipo de dato incorrecto a una variable (ej. un string a una variable que espera un número sin conversión explícita), llamar a una instrucción con tipos de argumentos incorrectos (ej. `Size("grande")` en lugar de `Size(10)`).
- **Feedback**: El `Semanter` proporcionará detalles sobre la incoherencia, como: `Error Semántico: La variable 'miColorFavorito' no ha sido declarada en la línea 8, columna 5.` o `Error Semántico: La instrucción 'Color' espera un argumento de tipo Color o String, pero recibió un Número en la línea 9, columna 7.`

### 4. Errores en Tiempo de Ejecución (Detectados por el `Interpreter`)
Estos errores aparecen durante la ejecución del script, cuando una operación, aunque sintáctica y semánticamente válida en su forma, resulta imposible de realizar bajo las condiciones actuales.
- **Ejemplos Comunes**: División por cero, intentar dibujar fuera de los límites del lienzo, un salto a una etiqueta que no existe (aunque el Semanter intenta atrapar esto, algunos casos dinámicos podrían pasar).
- **Feedback**: El `Interpreter` detendrá la ejecución o manejará el error según esté programado, mostrando un mensaje como: `Error de Ejecución: Intento de dibujar fuera de los límites del lienzo en la instrucción DrawLine en la línea 25, columna 3.`

### 📢 Feedback Visual en Tiempo Real
BrushBot-3000 muestra estos errores directamente en la interfaz de usuario, usualmente en un panel o consola dedicada. Esto te permite identificar y corregir problemas rápidamente.

<!-- Aquí puedes insertar tu imagen de ejemplo -->
<!-- ![Ejemplo de Feedback de Errores en BrushBot-3000](URL_DE_TU_IMAGEN_AQUI) -->
```
[Aquí puedes describir brevemente qué muestra la imagen, por ejemplo: "La imagen de arriba muestra cómo se resaltan los errores en el editor de scripts y se listan en la consola de errores."]
```

![image](https://github.com/user-attachments/assets/830b8a44-1532-4687-a334-4c514c78496f)

## 🛠️ Tecnologías Utilizadas
- **🎮 Motor de Juego**: Godot Engine (versión 4.x recomendada)
- **💻 Lenguaje Principal**: C# (para la lógica del bot, el intérprete y la integración con Godot)
- **📜 Lenguaje de Scripting**: "BrushScript" (archivos `.pw`) - ¡Tu propio lenguaje!

## 🚀 Requisitos Previos
- **Godot Engine**: Versión 4.x o superior. Descárgalo desde [godotengine.org](https://godotengine.org/).
- **.NET SDK**: Versión compatible con tu Godot (usualmente .NET 6+).
- **IDE (Opcional pero ¡muy útil!)**: VS Code, JetBrains Rider, o Visual Studio con extensiones para C# y Godot.

## 📦 Instrucciones de Instalación
1.  **Clonar el Repositorio**:
    ```bash
    git clone https://github.com/andr-migue/BrushBot-3000
    cd BrushBot-3000
    ```
2.  **Abrir en Godot**:
    - Lanza Godot Engine.
    - Clic en "Importar" o "Abrir Proyecto".
    - Navega a la carpeta `BrushBot-3000` y selecciona `project.godot`.
3.  **Construir el Proyecto**:
    - Godot debería detectar que es un proyecto C# y pedirte construir las soluciones .NET. ¡Dale que sí!

## 🧑‍🎨 Uso
1.  **Escribir Scripts**:
    - Crea o edita archivos `.pw` (ej. `mi_obra_maestra.pw`) en `Templates/` o donde prefieras.
    - ¡Usa BrushScript para dar instrucciones a tu bot!

    **Ejemplo de Script (`spiral.pw`):**
    ```brushscript
    Spawn(100, 100)
    Color("Yellow")
    Size(3)

    n <- 0
    Miguel
    n <- n + 10
    Print("Hola BrushBot")
    DrawCircle(0, 0, n)
    GoTo [Miguel] (n < 50)

    Aqua <- RGBA(0,125,125,254)
    Moradito <- RGBA(100, 30, 100, 254)
    Color(Moradito)
    DrawCircle(0,0,n)

    Fill()
    ```
2.  **Ejecutar Scripts**:
    - Inicia el proyecto desde Godot (F5 o el botón de "Play" ▶️).
    - Usa la UI de BrushBot-3000 para cargar y correr tus scripts.
    - ¡Mira cómo tu bot se convierte en un artista! 🖼️

## 📁 Estructura del Proyecto (¡Un Mapa del Tesoro!)
- `Assets/`: Imágenes 🖼️, fuentes ✒️, y otros tesoros visuales.
- `Scenes/`: Escenas de Godot (`.tscn`), como la UI (`graphical_ui.tscn`) y el pincel (`brush.tscn`).
- `Scripts/`: ¡El cerebro del proyecto! 🧠
    - `BrushBot/`: El núcleo del BrushBot.
        - `Lexical Analyzer/`: `Lexer.cs`, `Token.cs`
        - `Sintactical Analyzer/`: `Parser.cs`, Estructuras AST
        - `Semantical Analyser/`: `Semanter.cs`
        - `Interpreter/`: `Interpreter.cs`
        - `Core/`: `Context.cs`, `Handle.cs`, `Color.cs`, `Scope.cs`
        - `Errors/`: Clases para errores personalizados ⚠️
    - `EngineScripts/`: Scripts de Godot para la UI y la escena (`Main.cs`, `Printer.cs`).
- `Soundtrack/`: ¡Música para inspirarte! 🎶
- `Templates/`: Ejemplos de scripts `.pw` para empezar.
- `README.md`: ¡Este archivo que estás leyendo! 📖
- `project.godot`: El archivo principal del proyecto Godot.

¡Gracias por explorar BrushBot-3000! ¡Diviértete programando y dibujando! 🎉

## Para información mas detallada
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/andr-migue/BrushBot-3000)