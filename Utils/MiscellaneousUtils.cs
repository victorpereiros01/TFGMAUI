﻿using System.Text.RegularExpressions;
using Color = Microsoft.Maui.Graphics.Color;

namespace TFGMaui.Utils
{
    public static class MiscellaneousUtils
    {
        /// <summary>
        /// Cambia el tipo del color
        /// </summary>
        /// <param name="systemColor"></param>
        /// <returns>Maui.graphics.color</returns>
        public static Color ConvertFromSystemDrawingColor(System.Drawing.Color systemColor)
        {
            // Convert RGB values
            float red = systemColor.R / 255f;
            float green = systemColor.G / 255f;
            float blue = systemColor.B / 255f;

            // Convert alpha value
            float alpha = systemColor.A / 255f;

            return new Color(red, green, blue, alpha);
        }

        public static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }

        public static Color GetColorHobbie(string hType)
        {
            return hType switch
            {
                "Movie" => Color.FromArgb("#7078AF"),
                "Serie" => Color.FromArgb("#47b17f"),
                "Manga" => Color.FromArgb("#EBAE83"),
                "Anime" => Color.FromArgb("#cb4a4a"),
                "Game" => Color.FromArgb("#9B70AF"),
                "Book" => Color.FromArgb("#e1e97b"),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Return a color that is eye-good looking with the color passed as parameter
        /// </summary>
        /// <param name="bgColor"></param>
        /// <returns></returns>
        public static Color ColorIsDarkOrLight(Color bgColor)
        {
            // Calculate luminance
            double rLinear = (bgColor.Red <= 0.03928) ? (bgColor.Red / 12.92) : Math.Pow((bgColor.Red + 0.055) / 1.055, 2.4);
            double gLinear = (bgColor.Green <= 0.03928) ? (bgColor.Green / 12.92) : Math.Pow((bgColor.Green + 0.055) / 1.055, 2.4);
            double bLinear = (bgColor.Blue <= 0.03928) ? (bgColor.Blue / 12.92) : Math.Pow((bgColor.Blue + 0.055) / 1.055, 2.4);

            double L = 0.2126 * rLinear + 0.7152 * gLinear + 0.0722 * bLinear;

            // If luminance is greater than 0.179, use black text; otherwise, use white text
            return L > 0.179
                ? ConvertFromSystemDrawingColor(System.Drawing.Color.Black)
                : ConvertFromSystemDrawingColor(System.Drawing.Color.White);
        }

        public static List<T> GetNelements<T>(List<T> results, int v)
        {
            List<T> list = [];

            for (int i = 0; i < v; i++)
            {
                list.Add(results[i]);
            }

            return list;
        }
    }
}