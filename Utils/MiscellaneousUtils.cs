using TFGMaui.Models;

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
