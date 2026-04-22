namespace SWCPaint.Core.Interfaces;

public interface IDialogService
{
    string? ShowInputBox(string title, string message, string defaultValue = "");
    (int w, int h, SWCPaint.Core.Models.Color color)? ShowNewProjectDialog();
}