using SWCPaint.Core.Models;

namespace SWCPaint.Core.Converters;

public static class ColorConverter
{
    public static Color FromHsv(double h, double s, double v)
    {
        double r = 0, g = 0, b = 0;

        if (s == 0)
        {
            r = g = b = v;
        }
        else
        {
            double sector = h / 60.0;
            int sectorIndex = (int)Math.Floor(sector);
            double fractionalSector = sector - sectorIndex;

            double lowValue = v * (1 - s);
            double fallingValue = v * (1 - s * fractionalSector);
            double risingValue = v * (1 - s * (1 - fractionalSector));

            switch (sectorIndex % 6)
            {
                case 0: r = v; g = risingValue; b = lowValue; break;
                case 1: r = fallingValue; g = v; b = lowValue; break;
                case 2: r = lowValue; g = v; b = risingValue; break;
                case 3: r = lowValue; g = fallingValue; b = v; break;
                case 4: r = risingValue; g = lowValue; b = v; break;
                case 5: r = v; g = lowValue; b = fallingValue; break;
            }
        }

        return new Color(
            (byte)(r * 255),
            (byte)(g * 255),
            (byte)(b * 255)
        );
    }
}