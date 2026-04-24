namespace SWCPaint.Core.Interfaces.Persistence;

public interface IFileManager
{
    void Save(string path, byte[] content);
    void SaveText(string path, string content);
    byte[] Load(string path);
    string LoadText(string path);
}
