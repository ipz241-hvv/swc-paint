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
    public class LineFactory : ElementFactoryBase
    {
        public LineFactory(JsonSerializerOptions options) : base(options) { }

        public override LayerElement? Create(JsonElement json)
        {
            var dto = DeserializeDto<LineDto>(json);
            var line = new Line(
                new Point(dto.Start.X, dto.Start.Y),
                new Point(dto.End.X, dto.End.Y));
            line.StrokeColor = MapColor(dto.StrokeColor);
            line.Thickness = dto.Thickness;
            return line;
        }
    }
}
