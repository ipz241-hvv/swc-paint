using SWCPaint.Core.Models;

namespace SWCPaint.Core.Interfaces;

public interface IDialogService
{
    string? ShowInputBox(string title, string message, string defaultValue = "");
    (int width, int height, Color bgColor)? ShowNewProjectDialog();
    string? SaveFileDialog(string filter, string defaultFileName = "New Project", string defaultExt = ".png");
    string OpenFileDialog(string filter);
    Color? ShowColorPickerDialog(Color initialColor);
}