using SWCPaint.Core.Dtos;
using SWCPaint.Core.Models;

namespace SWCPaint.Core.Services.Serialization;

public static class ColorMapper
{
    public static ColorDto ToDto(Color color)
    {
        return new ColorDto
        {
            Red = color.Red,
            Green = color.Green,
            Blue = color.Blue,
            Alpha = color.Alpha
        };
    }

    public static Color FromDto(ColorDto dto)
    {
        return new Color(dto.Red, dto.Green, dto.Blue, dto.Alpha);
    }
}