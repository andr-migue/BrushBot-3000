using System;
using System.Collections.Generic;
using Godot;
using Interpreter;
public partial class GraphicalUi : Control
{
    [Export] FileDialog saveDialog;
    [Export] FileDialog loadDialog;
    [Export] CodeEdit edit;
    [Export] TextEdit text;
    [Export] LineEdit SizeEdit;
    [Export] GridGenerator grid;
    [Export] AudioStreamPlayer2D audio;
    public override void _Ready()
    {
        SizeEdit.Text = GlobalData.Size + "";
        text.Text = "\0";
        string code = edit.Text;
        Lexer lexer = new Lexer(code);
        List<Token> tokens = lexer.GetTokens();
        
        foreach (var token in tokens)
        {
            text.Text += token.ToString() + '\n';
        }
    }
    void PressPlay(){}
    void TextChanged()
    {
        text.Text = "\0";
        string code = edit.Text;
        Lexer lexer = new Lexer(code);
        List<Token> tokens = lexer.GetTokens();
        
        foreach (var token in tokens)
        {
            text.Text += token.ToString() + '\n';
        }
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
    void ShowLexer()
    {
        text.Visible = !text.Visible;
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