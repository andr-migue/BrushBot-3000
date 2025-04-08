using System;
using System.Collections.Generic;
using Godot;
using BrushBot;
public partial class GraphicalUi : Control
{
    [Export] FileDialog saveDialog;
    [Export] FileDialog loadDialog;
    [Export] CodeEdit edit;
    [Export] TextEdit text;
    [Export] TextEdit textparser;
    [Export] LineEdit SizeEdit;
    [Export] GridGenerator grid;
    [Export] AudioStreamPlayer2D audio;
    public override void _Ready()
    {
        SizeEdit.Text = GlobalData.Size + "";
        TextChanged();
    }
    void PressPlay()
    {
        string code = edit.Text;
        Lexer lexer = new Lexer(code);
        List<Token> tokens = lexer.GetTokens();
        text.Text = "\0";
        foreach (var token in tokens)
        {
            if (token.Type == TokenType.Unknown)
            {
                text.Text += "Error: " + token.ToString() + "\r\n";
                continue;
            }
            text.Text += token.ToString() + "\r\n";
        }

        Parser parser = new Parser(tokens);
        var (nodes, errors, result) = parser.Parse();
        var printer = new AstPrinter();
        textparser.Text = "\0";
        foreach (var item in result)
        {
            if (item is BrushBot.Node node)
            {
                textparser.Text += printer.Print(node) + "\r\n";
            }
            else if (item is ParserError error)
            {
                textparser.Text += error.Message + "\r\n" + "\r\n";
            }
        }
    }
    void TextChanged(){}
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
    void ShowLexer()
    {
        text.Visible = !text.Visible;
        textparser.Visible = !textparser.Visible;
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
        if (IsNumber(text)) GlobalData.Size = int.Parse(text);
        else SizeEdit.Text = GlobalData.Size + "";
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
}