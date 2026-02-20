using SWCPaint.Core.Models;

namespace SWCPaint.Core.Interfaces.Serialization;

internal interface IProjectSerializer
{
    string Serialize(Project project);
    Project Deserialize(string projectData);
}
