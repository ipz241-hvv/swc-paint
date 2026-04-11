using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Core.Services;

namespace SWCPaint.Core.Tools;

public record ToolContext(
    Project Project, 
    DrawingSettings Settings
);