using SWCPaint.Core.Dtos.Shapes;
using SWCPaint.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SWCPaint.Core.Factories.ElementFactories
{
    public class EraserFactory : ElementFactoryBase
    {
        public EraserFactory(JsonSerializerOptions options) : base(options) { }

        public override LayerElement? Create(JsonElement json)
        {
            var dto = DeserializeDto<EraserPathDto>(json);
            var eraser = new EraserPath(dto.Thickness);
            foreach (var p in MapPoints(dto.Points))
                eraser.AddPoint(p);
            return eraser;
        }
    }
}
