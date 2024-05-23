using System.Drawing;
using TFGMaui.Models;
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

        /// <summary>
        /// Obtiene los primeros n elementos de la lista
        /// </summary>
        /// <param name="results"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static List<MovieModel> GetNelements(List<MovieModel> results, int v2)
        {
            List<MovieModel> list = [];

            for (int i = 0; i < v2; i++)
            {
                list.Add(results[i]);
            }

            return list;
        }
    }
}