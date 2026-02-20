namespace SWCPaint.Core.Interfaces.Persistence;

internal interface IFileManager
{
    void Save(string path, byte[] content);
    byte[] Load(string path);
}
