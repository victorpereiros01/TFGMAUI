using Microsoft.Maui.Graphics;

public static class ColorConverterUtil
{
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
}
