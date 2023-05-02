using Murder.Core.Graphics;

namespace LDGame.Core
{
    internal static class Palette
    {
        public readonly static Color[] Colors = new Color[]
        {
            Color.FromHex("21181B"),
            Color.FromHex("3B2027"),
            Color.FromHex("7D3833"),
            Color.FromHex("AB5130"),
            Color.FromHex("CF752B"), // orange
            Color.FromHex("F0B541"), // yellow
            Color.FromHex("FFEE83"), // yellow light
            Color.FromHex("C8D45D"), // light green
            Color.FromHex("63AB3F"), // green
            Color.FromHex("3B7D4F"),
            Color.FromHex("2F5753"), // blue-ish green
            Color.FromHex("283540"),
            Color.FromHex("1B1F21"),
            Color.FromHex("2B2B45"), // purple-ish
            Color.FromHex("3A3F5E"),
            Color.FromHex("4C6885"),
            Color.FromHex("4FA4B8"),
            Color.FromHex("92E8C0"),
            Color.FromHex("F5FFE8"),
            Color.FromHex("DFE0E8"), // light blue
            Color.FromHex("a3a7c2"),
            Color.FromHex("686f99"),
            Color.FromHex("404973"),
            Color.FromHex("2c354d"),
            Color.FromHex("14182e"),
            Color.FromHex("FFC2A1"), // blue ish?
            Color.FromHex("FF8933")  // orange
        };

        public static Color Black => Colors[12];
        public static Color DarkBlue => Colors[13];

        static Palette()
        {
            
        }
    }
}
