using System.IO;
using SWCPaint.Core.Interfaces.Persistence;

namespace SWCPaint.Infrastructure.Persistence;

public class PhysicalFileManager : IFileManager
{
    public void Save(string path, byte[] content)
    {
        File.WriteAllBytes(path, content);
    }

    public void SaveText(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public byte[] Load(string path)
    {
        return File.ReadAllBytes(path);
    }

    public string LoadText(string path)
    {
        return File.ReadAllText(path);
    }
}