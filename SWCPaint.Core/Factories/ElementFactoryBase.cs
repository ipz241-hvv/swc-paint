using SWCPaint.Core.Dtos;
using SWCPaint.Core.Interfaces.Shapes;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SWCPaint.Core.Factories
{
    public abstract class ElementFactoryBase : IElementFactory
    {
        protected readonly JsonSerializerOptions _options;

        protected ElementFactoryBase(JsonSerializerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public abstract LayerElement? Create(JsonElement json);

        protected static List<Point> MapPoints(IEnumerable<PointDto> dtos)
            => dtos.Select(p => new Point(p.X, p.Y)).ToList();

        protected static Color MapColor(ColorDto dto)
            => new Color(dto.Red, dto.Green, dto.Blue, dto.Alpha);

        protected void ApplyCommonProperties(BoxBoundedShape shape, BoxBoundedShapeDto dto)
        {
            shape.StrokeColor = MapColor(dto.StrokeColor);
            shape.Thickness = dto.Thickness;

            if (shape is IFillable fillable && dto is IFillableDto fillableDto)
                fillable.FillColor = fillableDto.FillColor != null
                    ? MapColor(fillableDto.FillColor)
                    : null;
        }

        protected T DeserializeDto<T>(JsonElement json)
            => json.Deserialize<T>(_options)
               ?? throw new JsonException($"Failed to deserialize {typeof(T).Name}");
    }
}
