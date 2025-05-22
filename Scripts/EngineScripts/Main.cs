using Godot;
using BrushBot;
using System.Linq;
using System.Threading.Tasks;
public partial class Main : Control
{
    [Export] FileDialog saveDialog;
    [Export] FileDialog loadDialog;
    [Export] CodeEdit edit;
    [Export] TextEdit Terminal;
    [Export] LineEdit SizeEdit;
    [Export] int Delay = 30;
    [Export] Printer grid;
    [Export] AudioStreamPlayer2D audio;
    [Export] Godot.Label CurrentBrushSize;
    [Export] Godot.Label CurrentPosition;
    [Export] ColorRect colorRect;
    private Context Context;
    public override void _Ready()
    {
        Context = new Context(64);

        SizeEdit.Text = Context.Size + "";
        colorRect.Color = Handle.CheckColor(Context.BrushColor);
        CurrentBrushSize.Text = $"Brush Size: {Context.BrushSize}  ";
        CurrentPosition.Text = $"Position: {Context.Position}";

        grid.Init(Context);

        Analysis();
    }
    public override void _PhysicsProcess(double delta)
    {
        if (Context.Flag == true)
        {
            colorRect.Color = Handle.CheckColor(Context.BrushColor);
            CurrentBrushSize.Text = $"Brush Size: {Context.BrushSize}  ";
            CurrentPosition.Text = $"Position: {Context.Position}";
            grid.QueueRedraw();
            Context.Flag = false;
        }
        if (Context.RuntimeError)
        {
            Terminal.Text += Context.PossibleRuntimeError.Message + "\r\n";
            Context.RuntimeError = false;
        }
    }

    #region Buttons
    void PressPlay()
    {
        Execute();
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
    void ChangeSize(string text) {
        if (IsNumber(text))
        {
            int size = int.Parse(text);
            Context.Size = size;
            Context.Picture = new BrushBot.Color[size, size];
        }
        else
        {
            SizeEdit.Text = Context.Size + "";
        }
        grid.QueueRedraw();
    }
    private bool IsNumber(string text) {
        // Verificar si el texto ingresado es un número.
        if (string.IsNullOrEmpty(text)) return false;
        for (int i = 0; i < text.Length; i++) {
            if (char.IsDigit(text[i])) return true;
        }
        return false;
    }
    void Analysis()
    {
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
            Terminal.Text += error.Message + "\r\n";
        }
        foreach (var error in ParseErrors)
        {
            Terminal.Text += error.Message + "\r\n";
        }
        foreach (var error in SemantErrors)
        {
            Terminal.Text += error.Message + "\r\n";
        }
    }
    async Task Execute()
    {
        Handle.delay = Delay;
        Context.Reset();

        string code = edit.Text;
        
        Lexer lexer = new Lexer(code);
        var (tokens, LexerErrors) = lexer.GetTokens();
        
        Parser parser = new Parser(tokens);
        var (nodes, ParseErrors, result) = parser.Parse();
        
        Semanter semanter = new Semanter(nodes, Context);
        var (checknodes, SemantErrors, context) = semanter.Semant();

        Context = context;
        Context.Scope.Variables = new();

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