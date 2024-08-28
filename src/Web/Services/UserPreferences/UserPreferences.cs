﻿using System.Globalization;

namespace Web.Services.UserPreferences;

public class UserPreferences
{
    public static readonly List<string> PrimaryColors = new()
    {
        "#2d4275",
        "#6A1B9A",
        "#4CAF50" ,
        "#FF9800",
        "#F44336",
        "#FF69B4" 
    };

    public static readonly List<string> DarkPrimaryColors = new()
    {
        "#0077b6",
        "#a541be",
        "#388E3C",
        "#FB8C00",
        "#ca322d",
        "#cf2d86",
    };

    /// <summary>
    ///     Set the direction layout of the docs to RTL or LTR. If true RTL is used
    /// </summary>
    public bool RightToLeft { get; set; }

    /// <summary>
    ///     If true DarkTheme is used. LightTheme otherwise
    /// </summary>
    public bool IsDarkMode { get; set; }

    public string PrimaryColor { get; set; } = "#2d4275";

    public string DarkPrimaryColor { get; set; } = "#8b9ac6";

    public string PrimaryDarken => AdjustBrightness(PrimaryColor, 0.8);

    public string PrimaryLighten => AdjustBrightness(PrimaryColor, 0.7);

    public string SecondaryColor { get; set; } = "#ff4081ff";

    public double BorderRadius { get; set; } = 4;
    public double DefaultFontSize { get; set; } = 0.8125;
    public double LineHeight => Math.Min(1.7, Math.Max(1.3, 1.5 * (DefaultFontSize / 0.875)));
    public double LetterSpacing => 0.00938 * (DefaultFontSize / 0.875);

    public double Body1FontSize => DefaultFontSize;
    public double Body1LineHeight => LineHeight;
    public double Body1LetterSpacing => LetterSpacing;
    public double Body2FontSize => DefaultFontSize - 0.0625;
    public double Body2LineHeight => LineHeight;
    public double Body2LetterSpacing => LetterSpacing;
    public double ButtonFontSize => DefaultFontSize;
    public double ButtonLineHeight => 1.75 * (DefaultFontSize / 0.875);
    public double CaptionFontSize => DefaultFontSize + 0.0625;
    public double CaptionLineHeight => Math.Min(1.8, Math.Max(1.4, 1.66 * (DefaultFontSize / 0.75)));
    public double OverlineFontSize => DefaultFontSize - 0.0625;
    public double Subtitle1FontSize => DefaultFontSize + 0.125;
    public double Subtitle2FontSize => DefaultFontSize;
    public DarkLightMode DarkLightTheme { get; set; }


    private string AdjustBrightness(string hexColor, double factor)
    {
        if (hexColor.StartsWith("#")) hexColor = hexColor.Substring(1);

        if (hexColor.Length != 6) throw new ArgumentException("Invalid hex color code. It must be 6 character long");

        var r = int.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);

        var g = int.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);

        var b = int.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);

        var newR = (int)Math.Clamp(r * factor, 0, 255);

        var newG = (int)Math.Clamp(g * factor, 0, 255);

        var newB = (int)Math.Clamp(b * factor, 0, 255);

        return $"#{newR:X2}{newG:X2}{newB:X2}";
    }
}

public enum DarkLightMode
{
    System = 0,
    Light = 1,
    Dark = 2
}
