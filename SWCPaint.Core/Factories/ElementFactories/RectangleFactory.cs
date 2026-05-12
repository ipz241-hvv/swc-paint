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
    public class RectangleFactory : ElementFactoryBase
    {
        public RectangleFactory(JsonSerializerOptions options) : base(options) { }

        public override LayerElement? Create(JsonElement json)
        {
            var dto = DeserializeDto<RectangleDto>(json);
            var rect = new Rectangle(new Point(dto.X, dto.Y), dto.Width, dto.Height);
            ApplyCommonProperties(rect, dto);
            return rect;
        }
    }
}
