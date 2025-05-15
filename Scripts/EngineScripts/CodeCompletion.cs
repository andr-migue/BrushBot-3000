using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public partial class CodeCompletion : CodeEdit
{
    private readonly List<string> availableFunctions = new()
    {
        "Spawn", "Color", "Size", "Fill", "DrawLine", "DrawCircle", 
        "DrawRectangle", "GetActualX", "GetActualY", "GetCanvasSize",
        "GetColorCount", "IsBrushColor", "IsBrushSize", "IsCanvasColor"
    };

    private readonly List<string> availableColors = new()
    {
        "Red", "Blue", "Green", "Yellow", "Orange", 
        "Purple", "Black", "White", "Pink", "Transparent"
    };

    public override void _Ready()
    {
        InitializeCodeCompletion();
        SubscribeToEvents();
    }

    private void InitializeCodeCompletion()
    {
        CodeCompletionEnabled = true;
        CodeCompletionPrefixes = new Godot.Collections.Array<string> { 
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "."
        };
    }

    private void SubscribeToEvents()
    {
        CodeCompletionRequested += () => UpdateCompletionSuggestions(true);
        TextChanged += () => { if (ShouldShowCompletions()) UpdateCompletionSuggestions(false); };
    }

    private void UpdateCompletionSuggestions(bool forceUpdate)
    {
        string currentPrefix = GetCurrentWordPrefix();
        ClearCompletionOptions();
        List<string> suggestions = GetSuggestionsBasedOnContext(currentPrefix);

        foreach (string suggestion in suggestions.Distinct().OrderBy(s => s))
        {
            AddCodeCompletionOption(CodeCompletionKind.Constant, suggestion, suggestion);
        }

        UpdateCodeCompletionOptions(forceUpdate || suggestions.Any());
    }

    private List<string> GetSuggestionsBasedOnContext(string prefix)
    {
        return IsInsideColorQuotes() 
            ? availableColors.Where(c => string.IsNullOrEmpty(prefix) || c.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList()
            : availableFunctions.Where(f => string.IsNullOrEmpty(prefix) || f.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private bool ShouldShowCompletions()
    {
        string currentPrefix = GetCurrentWordPrefix();
        return !string.IsNullOrEmpty(currentPrefix);
    }

    private bool IsInsideColorQuotes()
    {
        string currentLine = GetLine(GetCaretLine());
        int cursorColumn = GetCaretColumn();
        string textBeforeCursor = currentLine.Substring(0, cursorColumn);
        
        int lastQuotePosition = textBeforeCursor.LastIndexOf('"');
        if (lastQuotePosition == -1) return false;
        
        string textBeforeQuote = textBeforeCursor.Substring(0, lastQuotePosition);
        return textBeforeQuote.Contains("Color(") && textBeforeCursor.Count(c => c == '"') % 2 != 0;
    }

    private string GetCurrentWordPrefix()
    {
        int cursorColumn = GetCaretColumn();
        if (cursorColumn == 0) return string.Empty;
        
        string currentLine = GetLine(GetCaretLine());
        string textBeforeCursor = currentLine.Substring(0, cursorColumn);
        Match match = Regex.Match(textBeforeCursor, @"[\w\d_-]+$");
        
        return match.Success ? match.Value : string.Empty;
    }

    private void ClearCompletionOptions() => UpdateCodeCompletionOptions(true);
}
