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
    [Export] int delay = 30;
    [Export] Printer grid;
    [Export] AudioStreamPlayer2D audio;
    [Export] Godot.Label CurrentBrushSize;
    [Export] Godot.Label CurrentPosition;
    [Export] ColorRect colorRect;
    public override void _Ready()
    {
        Scope.Init();
        SizeEdit.Text = Scope.Size + "";
        colorRect.Color = CheckColor(Scope.BrushColor);
        CurrentBrushSize.Text = $"Brush Size: {Scope.BrushSize}  ";
        CurrentPosition.Text = $"Position: {Scope.Position}";

        Analysis();
    }
    public override void _PhysicsProcess(double delta)
    {
        if (Scope.flag == true)
        {
            colorRect.Color = CheckColor(Scope.BrushColor);
            CurrentBrushSize.Text = $"Brush Size: {Scope.BrushSize}  ";
            CurrentPosition.Text = $"Position: {Scope.Position}";
            grid.QueueRedraw();
            Scope.flag = false;
        }
    }

    #region Buttons
    void PressPlay()
    {
        Execute();
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
        Scope.Picture = new BrushBot.Color[Scope.Size, Scope.Size];
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
            Scope.Size = size;
            Scope.Picture = new BrushBot.Color[size, size];
        }
        else
        {
            SizeEdit.Text = Scope.Size + "";
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
        Scope.Replay();
        string code = edit.Text;
        
        Lexer lexer = new Lexer(code);
        var (tokens, LexerErrors) = lexer.GetTokens();
        
        Parser parser = new Parser(tokens);
        var (nodes, ParseErrors, result) = parser.Parse();

        Semanter semanter = new Semanter(nodes);
        var (checknodes, SemantErrors) = semanter.Semant();
        
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
        Handle.delay = delay;
        Scope.Replay();
        string code = edit.Text;
        
        Lexer lexer = new Lexer(code);
        var (tokens, LexerErrors) = lexer.GetTokens();
        
        Parser parser = new Parser(tokens);
        var (nodes, ParseErrors, result) = parser.Parse();
        
        Semanter semanter = new Semanter(nodes);
        var (checknodes, SemantErrors) = semanter.Semant();

        if (!LexerErrors.Any() && !ParseErrors.Any() && !SemantErrors.Any())
        {
            Evaluateer Evaluateer = new Evaluateer(checknodes);
            await Evaluateer.Evaluate();
        }
        else
        {
            Terminal.Text += "Error: No se puede ejecutar sin resolver los errores" + "\r\n";
        }
    }
    public Godot.Color CheckColor(BrushBot.Color color)
    {
        switch (color)
        {
            case  BrushBot.Color.Transparent: return new Godot.Color(255, 255, 255, 0);
            case  BrushBot.Color.Red: return new Godot.Color(255, 0, 0);
            case  BrushBot.Color.Blue: return new Godot.Color(0, 0, 255);
            case  BrushBot.Color.Green: return new Godot.Color(0, 255, 0);
            case  BrushBot.Color.Yellow: return new Godot.Color(255, 255, 0);
            case  BrushBot.Color.Orange: return new Godot.Color(255, 165, 0);
            case  BrushBot.Color.Purple: return new Godot.Color(160, 32, 240);
            case  BrushBot.Color.Black: return new Godot.Color(0, 0, 0);
            case  BrushBot.Color.White: return new Godot.Color(255, 255, 255);
            case  BrushBot.Color.Pink : return new Godot.Color(255, 80, 220);

            default: return new Godot.Color(255, 255, 255, 0);
        }
    }
    #endregion
}