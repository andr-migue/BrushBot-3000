using System;
using System.Collections.Generic;
using Godot;
using BrushBot;
public partial class Main : Control
{
    [Export] FileDialog saveDialog;
    [Export] FileDialog loadDialog;
    [Export] CodeEdit edit;
    [Export] TextEdit Terminal;
    [Export] LineEdit SizeEdit;
    [Export] GridGenerator grid;
    [Export] AudioStreamPlayer2D audio;
    public override void _Ready()
    {
        SizeEdit.Text = Interpreter.Size + "";
        Handle.size = Interpreter.Size;
    }
    #region Buttons
    void PressPlay()
    {
        string code = edit.Text;
        
        Lexer lexer = new Lexer(code);
        List<Token> tokens = lexer.GetTokens();
        
        Parser parser = new Parser(tokens);
        var (nodes, errors, result) = parser.Parse();

        Interpreter.Init(nodes);
        Interpreter.Interpret();
        
        foreach (var error in Interpreter.Errors)
        {
            Terminal.Text += error.Message + "\r\n";
            continue;
        }

        grid.QueueRedraw();
    }
    void TextChanged()
    {
        string code = edit.Text;
        
        Lexer lexer = new Lexer(code);
        List<Token> tokens = lexer.GetTokens();
        
        Parser parser = new Parser(tokens);
        var (nodes, errors, result) = parser.Parse();
        
        Terminal.Text = "\0";
        foreach (var token in tokens)
        {
            if (token.Type == TokenType.Unknown)
            {
                Terminal.Text += "Error: " + token + "\r\n";
                continue;
            }
        }
        foreach (var error in errors)
        {
            Terminal.Text += error.Message + "\r\n";
            continue;
        }
    }
    void PressReset()
    {
        Interpreter.Picture = new BrushBot.Color[Interpreter.Size, Interpreter.Size];
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
    }
    void ChangeSize(string text) {
        if (IsNumber(text))
        {
            int size = int.Parse(text);
            Interpreter.Size = size;
            Interpreter.Picture = new BrushBot.Color[size, size];
            Handle.size = size;
        }
        else
        {
            SizeEdit.Text = Interpreter.Size + "";
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
    #endregion
}