using Godot;
using BrushBot;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
public partial class Main : Control
{
    [Export] FileDialog saveDialog;
    [Export] FileDialog loadDialog;
    [Export] CodeEdit edit;
    [Export] TextEdit Terminal;
    [Export] LineEdit SizeEdit;
    [Export] Printer grid;
    [Export] AudioStreamPlayer2D audio;
    [Export] Godot.Label CurrentBrushSize;
    [Export] Godot.Label CurrentPosition;
    [Export] ColorRect colorRect;
    [Export] HSlider slider;
    Context Context;
    Godot.Color colorError = new Godot.Color(1.0f, 0.2f, 0.2f, 0.3f);
    Godot.Color transparent = new Godot.Color(0, 0, 0, 0);
    HashSet<int> errorLines = new();
    public override void _Ready()
    {
        Context = new Context(64);

        SizeEdit.Text = Context.Size + "";
        colorRect.Color = CheckColor.GetColor(Context.BrushColor);
        CurrentBrushSize.Text = $"Brush Size: {Context.BrushSize}  ";
        CurrentPosition.Text = $"Position: {Context.Position}";
        slider.Value = 30;

        grid.Init(Context);

        Analysis();
    }
    public override void _PhysicsProcess(double delta)
    {
        if (Context.Flag == true)
        {
            colorRect.Color = CheckColor.GetColor(Context.BrushColor);
            CurrentBrushSize.Text = $"Brush Size: {Context.BrushSize}  ";
            CurrentPosition.Text = $"Position: {Context.Position}";
            grid.QueueRedraw();
            Context.Flag = false;
        }

        if (Context.RuntimeError)
        {
            Terminal.Text += "RunTime Error: " + Context.Message + "\r\n";
            Context.RuntimeError = false;
        }

        if (Context.Print)
        {
            Terminal.Text += Context.Message + "\r\n";
            Context.Print = false;
        }
    }
    #region Buttons
    void PressPlay()
    {
        Execute();
    }
    void Slider(float value)
    {
        Handle.delay = (int)value;
    }
    void RefreshTerminal()
    {
        Terminal.Text = "";
    }
    void PressSkip()
    {
        Handle.delay = 0;
    }
    void TextChanged()
    {
        Analysis();
    }
    void PressReset()
    {
        Context.Picture = new BrushBot.Color[Context.Size, Context.Size];
        Context.InitPicture();
        grid.QueueRedraw();
    }
    void PressExit()
    {
        GetTree().Quit();
    }
    void PressSaveFile()
    {
        saveDialog.Popup();
    }
    void PressLoadFile()
    {
        loadDialog.Popup();
    }
    void PressEditCode()
    {
        edit.Visible = !edit.Visible;
    }
    void PressMute()
    {
        audio.Playing = !audio.Playing;
    }
    void SaveFile(string path)
    {
        var script = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        script.StoreString(edit.Text);
        script.Close();
    }
    void LoadFile(string path)
    {
        var script = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        edit.Text = script.GetAsText();
        script.Close();
        Analysis();
    }
    void ChangeSize(string text)
    {
        if (IsNumber(text))
        {
            int size = int.Parse(text);

            if (size >= 0 && size <= 256)
            {
                Context.Size = size;
                Context.Picture = new BrushBot.Color[size, size];
                Context.InitPicture();
            }

            else
            {
                SizeEdit.Text = Context.Size + "";
            }
        }

        else
        {
            SizeEdit.Text = Context.Size + "";
        }

        grid.QueueRedraw();
    }
    #endregion
    #region Aux
    private bool IsNumber(string text)
    {
        // Verificar si el texto ingresado es un número.
        if (string.IsNullOrEmpty(text)) return false;
        
        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsDigit(text[i])) return true;
        }

        return false;
    }
    void Analysis()
    {
        foreach (var line in errorLines)
        {
            edit.SetLineBackgroundColor(line, transparent);
        }
        errorLines.Clear();

        Context.Reset();
        string code = edit.Text;

        Lexer lexer = new Lexer(code);
        var (tokens, LexerErrors) = lexer.GetTokens();

        Parser parser = new Parser(tokens);
        var (nodes, ParseErrors, result) = parser.Parse();

        Semanter semanter = new Semanter(nodes, Context);
        var (checknodes, SemantErrors, context) = semanter.Semant();

        Terminal.Text = "\0";

        foreach (var error in LexerErrors)
        {
            edit.SetLineBackgroundColor(error.Location.Item1 - 1, colorError);
            errorLines.Add(error.Location.Item1 - 1);
            Terminal.Text += "Lexical Error: " + error.Message + "\r\n";
        }

        foreach (var error in ParseErrors)
        {
            edit.SetLineBackgroundColor(error.Location.Item1 - 1, colorError);
            errorLines.Add(error.Location.Item1 - 1);
            Terminal.Text += "Syntax Error: " + error.Message + "\r\n";
        }

        foreach (var error in SemantErrors)
        {
            edit.SetLineBackgroundColor(error.Location.Item1 - 1, colorError);
            errorLines.Add(error.Location.Item1 - 1);
            Terminal.Text += "Semantic Error: " + error.Message + "\r\n";
        }
    }
    async Task Execute()
    {
        Handle.delay = (int)slider.Value;
        Context.Reset();

        string code = edit.Text;

        Lexer lexer = new Lexer(code);
        var (tokens, LexerErrors) = lexer.GetTokens();

        Parser parser = new Parser(tokens);
        var (nodes, ParseErrors, result) = parser.Parse();

        Semanter semanter = new Semanter(nodes, Context);
        var (checknodes, SemantErrors, context) = semanter.Semant();

        Context = context;

        if (!LexerErrors.Any() && !ParseErrors.Any() && !SemantErrors.Any())
        {
            Interpreter Interpreter = new Interpreter(checknodes);
            await Interpreter.Interpret(Context);
        }

        else
        {
            Terminal.Text += "Error: Can't execute without solve the errors." + "\r\n";
        }
    }
    #endregion
}