public static class Pixelization
{
    public static float[,] PixelateNoise(float[,] noise, int width, int height, int pixelSize)
    {
        if (pixelSize < 1)
            return noise;

        float[,] pixelatedNoise = new float[width, height];

        for (int y = 0; y < height; y += pixelSize)
        {
            for (int x = 0; x < width; x += pixelSize)
            {
                float representative = noise[x, y];

                for (int dy = 0; dy < pixelSize; dy++)
                {
                    for (int dx = 0; dx < pixelSize; dx++)
                    {
                        if (x + dx < width && y + dy < height)
                            pixelatedNoise[x + dx, y + dy] = representative;
                    }
                }
            }
        }

        return pixelatedNoise;
    }
}
