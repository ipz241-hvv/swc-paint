using SWCPaint.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SWCPaint.Core.Factories
{
    public interface IElementFactory
    {
        LayerElement? Create(JsonElement json);
    }
}
