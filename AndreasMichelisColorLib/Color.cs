using System;
using System.Globalization;
using System.Text.RegularExpressions;

// I'm a change,
// notice me Senpai


namespace AndreasMichelis.ColorUtils
{
    public class Color
    {
        protected static readonly Regex ValidColorString = new(@"^(#)?[0-9a-fA-F]{6}([0-9a-fA-F]{2})?$");

        protected sbyte _Red;
        protected sbyte _Green;
        protected sbyte _Blue;

        protected double _Cyan;
        protected double _Magenta;
        protected double _Yellow;
        protected double _Blackness;

        protected double _Hue;
        protected double _Saturation;
        protected double _Lightness;

        public double Alpha { get; set; }


        public sbyte R
        {
            get => _Red;
            set
            {
                _Red = value;
                CalculateFromRgb();
            }
        }
        public double Red { get => R / 255d; set => R = (sbyte)(value * 255d); }

        public sbyte G
        {
            get => _Green;
            set
            {
                _Green = value;
                CalculateFromRgb();
            }
        }
        public double Green { get => G / 255d; set => G = (sbyte)(value * 255d); }

        public sbyte B
        {
            get => _Blue;
            set
            {
                _Blue = value;
                CalculateFromRgb();
            }
        }
        public double Blue { get => B / 255d; set => B = (sbyte)(value * 255d); }


        public double C
        {
            get => _Cyan;
            set
            {
                _Cyan = value;
                CalculateFromCmyk();
            }
        }
        public double M
        {
            get => _Magenta;
            set
            {
                _Magenta = value;
                CalculateFromCmyk();
            }
        }
        public double Y
        {
            get => _Yellow;
            set
            {
                _Yellow = value;
                CalculateFromCmyk();
            }
        }
        public double K
        {
            get => _Blackness;
            set
            {
                _Blackness = value;
                CalculateFromCmyk();
            }
        }


        public double H
        {
            get => _Hue;
            set
            {
                _Hue = value;
                CalculateFromHsl();
            }
        }
        public double S
        {
            get => _Saturation;
            set
            {
                _Saturation = value;
                CalculateFromHsl();
            }
        }
        public double L
        {
            get => _Lightness;
            set
            {
                _Lightness = value;
                CalculateFromHsl();
            }
        }


        protected Color()
        {
            Alpha = 1d;
            _Red = _Green = _Blue = 0;
            CalculateFromRgb();
        }

        protected Color(sbyte r, sbyte g, sbyte b, double a)
        {
            Alpha = a;
            _Red = r;
            _Green = g;
            _Blue = b;
            CalculateFromRgb();
        }

        protected Color(double h, double s, double l, double a)
        {
            Alpha = a;
            _Hue = h;
            _Saturation = s;
            _Lightness = l;
            CalculateFromHsl();
        }

        protected Color(double c, double m, double y, double k, double a)
        {
            Alpha = a;
            _Cyan = c;
            _Magenta = m;
            _Yellow = y;
            _Blackness = k;
            CalculateFromCmyk();
        }

        public static Color FromRgb(sbyte r, sbyte g, sbyte b, double a = 1d) => new(r, g, b, a);
        public static Color FromHsl(double h, double s, double l, double a = 1d) => new(h, s, l, a);
        public static Color FromCmyk(double c, double m, double y, double k, double a = 1d) => new(c, m, y, k, a);

        public static implicit operator Color(string s)
        {
            if (!ValidColorString.IsMatch(s))
                throw new FormatException(
                    "The format of a color string has to consist of 6(RGB) or 8(RGBA) hex digits and can optionally start with a '#' symbol");
            s = s.TrimStart('#');
            var r = sbyte.Parse(s[..1], NumberStyles.HexNumber);
            var g = sbyte.Parse(s[2..3], NumberStyles.HexNumber);
            var b = sbyte.Parse(s[4..5], NumberStyles.HexNumber);
            var a = 1d;
            if (s.Length == 8)
            {
                a = sbyte.Parse(s[6..], NumberStyles.HexNumber) / 255d;
            }
            return new Color(r, g, b, a);
        }

        public override string ToString() => ToString("#RGBA");

        public string ToString(string format)
        {
            if (string.IsNullOrWhiteSpace(format)) return "";
            var ret = format
                    .Replace("{", "{{")
                    .Replace("}", "}}")
                    .Replace("A", "{0}")
                    .Replace("a", "{0}")
                    .Replace("RD", "{4}")
                    .Replace("GD", "{5}")
                    .Replace("BD", "{6}")
                    .Replace("Rd", "{4}")
                    .Replace("Gd", "{5}")
                    .Replace("Bd", "{6}")
                    .Replace("R", "{1}")
                    .Replace("G", "{2}")
                    .Replace("B", "{3}")
                    .Replace("rd", "{4}")
                    .Replace("gd", "{5}")
                    .Replace("bd", "{6}")
                    .Replace("rD", "{4}")
                    .Replace("gD", "{5}")
                    .Replace("bD", "{6}")
                    .Replace("r", "{7}")
                    .Replace("g", "{8}")
                    .Replace("b", "{9}")
                    .Replace("H", "{10}")
                    .Replace("S", "{11}")
                    .Replace("L", "{12}")
                    .Replace("h", "{10}")
                    .Replace("s", "{11}")
                    .Replace("l", "{12}")
                    .Replace("C", "{13}")
                    .Replace("M", "{14}")
                    .Replace("Y", "{15}")
                    .Replace("K", "{16}")
                    .Replace("c", "{13}")
                    .Replace("m", "{14}")
                    .Replace("y", "{15}")
                    .Replace("k", "{16}")
                ;

            return string.Format(ret, Alpha, R.ToString("X2"), G.ToString("X2"), B.ToString("X2"), R, G, B, Red, Green, Blue, H, S, L, C, M, Y, K);
        }

        protected void CalculateFromRgb()
        {
            var r = Red;
            var g = Green;
            var b = Blue;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));
            var delta = max - min;

            // HSL //////////////////////////////////////////////////////////////////////

            _Lightness = (max + min) / 2;
            _Hue = 0;
            _Saturation = 0;
            if (delta != 0)
            {
                _Hue = 60;
                if (Math.Abs(max - r) < 0.00001) _Hue *= ((g - b) / delta) % 6d;
                else if (Math.Abs(max - g) < 0.00001) _Hue *= ((b - r) / delta) + 2d;
                else _Hue *= ((r - g) / delta) + 4d;
                _Saturation = delta / (1d - Math.Abs(2 * _Lightness - 1));
            }

            // CMYK /////////////////////////////////////////////////////////////////////


            _Blackness = 1 - max;
            _Cyan = (1 - r - _Blackness) / (1 - _Blackness);
            _Magenta = (1 - g - _Blackness) / (1 - _Blackness);
            _Yellow = (1 - b - _Blackness) / (1 - _Blackness);
        }

        protected void CalculateFromHsl()
        {
            // RGB //////////////////////////////////////////////////////////////////////

            var c = (1d - Math.Abs(2 * _Lightness - 1)) * _Saturation;
            var x = c * (1 - Math.Abs((_Hue / 60d) % 2d - 1));
            var m = _Lightness - c / 2d;

            var (r, g, b) = (_Hue % 360d) switch
            {
                >= 0d and < 60d => (c, x, 0d),
                >= 60d and < 120d => (x, c, 0d),
                >= 120d and < 180d => (0d, c, x),
                >= 180d and < 240d => (0d, x, c),
                >= 240d and < 300d => (x, 0d, c),
                >= 300d and < 360d => (c, 0d, x),
                _ => throw new ArgumentOutOfRangeException()
            };

            _Red = (sbyte)((r + m) * 255d);
            _Green = (sbyte)((g + m) * 255d);
            _Blue = (sbyte)((b + m) * 255d);

            var max = Math.Max(r, Math.Max(g, b));

            // CMYK /////////////////////////////////////////////////////////////////////


            _Blackness = 1 - max;
            _Cyan = (1 - r - _Blackness) / (1 - _Blackness);
            _Magenta = (1 - g - _Blackness) / (1 - _Blackness);
            _Yellow = (1 - b - _Blackness) / (1 - _Blackness);
        }

        protected void CalculateFromCmyk()
        {
            // RGB //////////////////////////////////////////////////////////////////////

            var r = (1 - _Cyan) * (1 - _Blackness);
            var g = (1 - _Magenta) * (1 - _Blackness);
            var b = (1 - _Yellow) * (1 - _Blackness);

            (_Red, _Green, _Blue) = ((sbyte)(r * 255d), (sbyte)(g * 255d), (sbyte)(b * 255d));

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));
            var delta = max - min;



            // HSL //////////////////////////////////////////////////////////////////////

            _Lightness = (max + min) / 2;
            _Hue = 0;
            _Saturation = 0;
            if (delta != 0)
            {
                _Hue = 60;
                if (Math.Abs(max - r) < 0.00001) _Hue *= ((g - b) / delta) % 6d;
                else if (Math.Abs(max - g) < 0.00001) _Hue *= ((b - r) / delta) + 2d;
                else _Hue *= ((r - g) / delta) + 4d;
                _Saturation = delta / (1d - Math.Abs(2 * _Lightness - 1));
            }
        }
    }
}
