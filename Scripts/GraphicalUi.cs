using Godot;
public partial class GraphicalUi : Control
{
    [Export] FileDialog saveDialog;
    [Export] FileDialog loadDialog;
    [Export] CodeEdit edit;
    [Export] LineEdit rows;
    [Export] LineEdit collumns;
    [Export] GridGenerator grid;
    [Export] AudioStreamPlayer2D audio;
    public override void _Ready()
    {
        rows.Text = GlobalData.Rows + "";
        collumns.Text = GlobalData.Collumns + "";
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
    void ChangeRows(string text) {
        if (IsNumber(text)) GlobalData.Rows = int.Parse(text);
        else rows.Text = GlobalData.Rows + "";
        grid.QueueRedraw();
    }
    void ChangeCollumns(string text) {
        if (IsNumber(text)) GlobalData.Collumns = int.Parse(text);
        else collumns.Text = GlobalData.Collumns + "";
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