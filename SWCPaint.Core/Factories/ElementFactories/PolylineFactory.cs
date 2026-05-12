using SWCPaint.Core.Dtos.Shapes;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SWCPaint.Core.Factories.ElementFactories
{
    public class PolylineFactory : ElementFactoryBase
    {
        public PolylineFactory(JsonSerializerOptions options) : base(options) { }

        public override LayerElement? Create(JsonElement json)
        {
            var dto = DeserializeDto<PolylineDto>(json);
            var polyline = new Polyline(MapPoints(dto.Points));
            polyline.StrokeColor = MapColor(dto.StrokeColor);
            polyline.Thickness = dto.Thickness;
            polyline.IsSmooth = dto.IsSmooth;
            return polyline;
        }
    }
}
