using Godot;
using System;

namespace SmallSurvivalGame.scripts.map.generation
{
    public class PerlinNoise
    {
        private readonly int[] _p = { 151, 160, 137,  91,  90,  15, 131,  13, 201,  95,  96,  53, 194, 233,   7, 225,
			140,  36, 103,  30,  69, 142,   8,  99,  37, 240,  21,  10,  23, 190,   6, 148,
			247, 120, 234,  75,   0,  26, 197,  62,  94, 252, 219, 203, 117,  35,  11,  32,
			57, 177,  33,  88, 237, 149,  56,  87, 174,  20, 125, 136, 171, 168,  68, 175,
			74, 165,  71, 134, 139,  48,  27, 166,  77, 146, 158, 231,  83, 111, 229, 122,
			60, 211, 133, 230, 220, 105,  92,  41,  55,  46, 245,  40, 244, 102, 143,  54,
			65,  25,  63, 161,   1, 216,  80,  73, 209,  76, 132, 187, 208,  89,  18, 169,
			200, 196, 135, 130, 116, 188, 159,  86, 164, 100, 109, 198, 173, 186,   3,  64,
			52, 217, 226, 250, 124, 123,   5, 202,  38, 147, 118, 126, 255,  82,  85, 212,
			207, 206,  59, 227,  47,  16,  58,  17, 182, 189,  28,  42, 223, 183, 170, 213,
			119, 248, 152,   2,  44, 154, 163,  70, 221, 153, 101, 155, 167,  43, 172,   9,
			129,  22,  39, 253,  19,  98, 108, 110,  79, 113, 224, 232, 178, 185, 112, 104,
			218, 246,  97, 228, 251,  34, 242, 193, 238, 210, 144,  12, 191, 179, 162, 241,
			81,  51, 145, 235, 249,  14, 239, 107,  49, 192, 214,  31, 181, 199, 106, 157,
			184,  84, 204, 176, 115, 121,  50,  45, 127,   4, 150, 254, 138, 236, 205,  93,
			222, 114,  67,  29,  24,  72, 243, 141, 128, 195,  78,  66, 215,  61, 156, 180,
			151, 160, 137,  91,  90,  15, 131,  13, 201,  95,  96,  53, 194, 233,   7, 225,
			140,  36, 103,  30,  69, 142,   8,  99,  37, 240,  21,  10,  23, 190,   6, 148,
			247, 120, 234,  75,   0,  26, 197,  62,  94, 252, 219, 203, 117,  35,  11,  32,
			57, 177,  33,  88, 237, 149,  56,  87, 174,  20, 125, 136, 171, 168,  68, 175,
			74, 165,  71, 134, 139,  48,  27, 166,  77, 146, 158, 231,  83, 111, 229, 122,
			60, 211, 133, 230, 220, 105,  92,  41,  55,  46, 245,  40, 244, 102, 143,  54,
			65,  25,  63, 161,   1, 216,  80,  73, 209,  76, 132, 187, 208,  89,  18, 169,
			200, 196, 135, 130, 116, 188, 159,  86, 164, 100, 109, 198, 173, 186,   3,  64,
			52, 217, 226, 250, 124, 123,   5, 202,  38, 147, 118, 126, 255,  82,  85, 212,
			207, 206,  59, 227,  47,  16,  58,  17, 182, 189,  28,  42, 223, 183, 170, 213,
			119, 248, 152,   2,  44, 154, 163,  70, 221, 153, 101, 155, 167,  43, 172,   9,
			129,  22,  39, 253,  19,  98, 108, 110,  79, 113, 224, 232, 178, 185, 112, 104,
			218, 246,  97, 228, 251,  34, 242, 193, 238, 210, 144,  12, 191, 179, 162, 241,
			81,  51, 145, 235, 249,  14, 239, 107,  49, 192, 214,  31, 181, 199, 106, 157,
			184,  84, 204, 176, 115, 121,  50,  45, 127,   4, 150, 254, 138, 236, 205,  93,
			222, 114,  67,  29,  24,  72, 243, 141, 128, 195,  78,  66, 215,  61, 156, 180 };

        private int mapWidth, mapHeight;
		private Random random;
        
        public PerlinNoise(int mapWidth, int mapHeight, int seed)
        {
	        this.mapWidth = mapWidth;
	        this.mapHeight = mapHeight;
	        random = new Random(seed);
        }

        public double[,] GenerateNoiseMap(int octaves, double persistence, double lacunarity, Vector2 offset)
        {
	        double[,] noise = new double[mapWidth, mapHeight];

	        Vector2[] octaveOffsets = new Vector2[octaves];
	        for (int i = 0; i < octaves; i++)
	        {
		        float offsetX = random.Next(-100_000, 100_000) + offset.X;
		        float offsetY = random.Next(-100_000, 100_000) + offset.Y;
		        octaveOffsets[i] = new Vector2(offsetX, offsetY);
	        }
	        
	        double maxNoiseHeight = double.MinValue;
	        double minNoiseHeight = double.MaxValue;
	        
	        double halfWidth = mapWidth / 2;
	        double halfHeight = mapHeight / 2;

	        for (int x = 0; x < mapWidth; x++)
	        {
		        for (int y = 0; y < mapHeight; y++)
		        {
			        double amplitude = 1;
			        double frequency = 1;
			        double noiseHeight = 0;

			        for (int i = 0; i < octaves; i++)
			        {
				        double sampleX = (x - halfWidth) * frequency + octaveOffsets[i].X;
				        double sampleY = (y - halfHeight) * frequency + octaveOffsets[i].Y;
				        
				        double perlinValue = Noise(sampleX, sampleY) * 2 - 1;
				        noiseHeight += perlinValue * amplitude;
				        
				        amplitude *= persistence;
				        frequency *= lacunarity;
			        }

			        if (noiseHeight > maxNoiseHeight)
			        {
				        maxNoiseHeight = noiseHeight;
			        }

			        if (noiseHeight < minNoiseHeight)
			        {
				        minNoiseHeight = noiseHeight;
			        }
			        noise[x, y] = noiseHeight;
		        }
	        }

	        for (int x = 0; x < mapWidth; x++)
	        {
		        for (int y = 0; y < mapHeight; y++)
		        {
			        noise[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noise[x, y]);
		        }
	        }
	        
	        return noise;
        }

        private double Noise(double x, double y)
        {
	        x *= 0.1d;
	        y *= 0.1d;

	        int X = (int)Math.Floor(x) & 255;
	        int Y = (int)Math.Floor(y) & 255;
	        
	        x -= Math.Floor(x);
	        y -= Math.Floor(y);

	        double u = Fade(x);
	        double v = Fade(y);

	        int aa = _p[X] + Y;
	        int ab = _p[X] + Y + 1;
	        int ba = _p[X + 1] + Y;
	        int bb = _p[X + 1] + Y + 1;

	        double x1 = Lerp(Gradient(_p[aa], x, y), Gradient(_p[ba], x - 1, y), u);
	        double x2 = Lerp(Gradient(_p[ab], x, y - 1), Gradient(_p[bb], x - 1, y - 1), u);

	        return (Lerp(x1, x2, v) + 1) / 2d;
        }

        private double Fade(double t)
        {
	        return (((6 * t) - 15) * t + 10) * t * t * t;
        }

        private double Lerp(double a, double b, double t)
        {
	        return a + t * (b - a);
        }

        private double Gradient(int hash, double x, double y)
        {
	        int h = hash & 3;
	        double u = h < 2 ? x : y;
	        double v = h < 2 ? y : x;
	        return (( h & 1) == 0 ? u : -u) + ((h & 1) == 0 ? v : -v);
        }
    }
}