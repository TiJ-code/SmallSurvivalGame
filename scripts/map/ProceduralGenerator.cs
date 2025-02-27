using Godot;
using System;

public partial class ProceduralGenerator : Node2D
{
	[Export] public NoiseTexture2D noiseTexture2D;
	[Export] public Sprite2D noiseTexture;
	[Export] public Sprite2D noiseFalloffMapTexture;
	private TileMapLayer tileMapLayer;
	private Noise noise;
	private int width = 255;
	private int height = 255;
	
	public override void _Ready()
	{
		noise = noiseTexture2D.Noise;
		tileMapLayer = GetNode<TileMapLayer>("World Layer");
		GenerateWorld();
	}

	private void GenerateWorld()
	{
		/*
		Image noiseMapImage = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
		Image falloffMapImage = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
		*/
		
		float[,] noiseMap = new float[width, height];
		
		float minValue = float.MaxValue;
		float maxValue = float.MinValue;
		
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				noiseMap[x, y] = noise.GetNoise2D(x, y);
				if (noiseMap[x, y] < minValue) minValue = noiseMap[x, y];
				if (noiseMap[x, y] > maxValue) maxValue = noiseMap[x, y];
			}
		}

		float range = maxValue - minValue;
		
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				noiseMap[x, y] = (noiseMap[x, y] - minValue) / Math.Abs(range);
				/*noiseMapImage.SetPixel(x, y, new Color(noiseMap[x, y], noiseMap[x, y], noiseMap[x, y]));*/
				noiseMap[x, y] = ApplyFalloff(x, y, noiseMap[x, y]);
				/*falloffMapImage.SetPixel(x, y, new Color(noiseMap[x, y], noiseMap[x, y], noiseMap[x, y]));*/
				PlaceTile(x, y, noiseMap[x, y]);
			}
		}
		
		/*
		 ImageTexture noiseImgTexture = new ImageTexture();
		noiseImgTexture.SetImage(noiseMapImage);
		noiseTexture.SetTexture(noiseImgTexture);
		
		ImageTexture falloffNoiseTexture = new ImageTexture();
		falloffNoiseTexture.SetImage(falloffMapImage);
		noiseFalloffMapTexture.Texture = falloffNoiseTexture;
		*/
	}

	private float ApplyFalloff(int x, int y, float noiseValue)
	{
		float halfWidth = width / 2f;
		float halfHeight = height / 2f;
		
		Vector2 center = new Vector2(halfWidth, halfHeight);
		float maxDistance = center.Length() / 1.5f;
		
		Vector2 pixelPosition = new Vector2(x, y);
		float distance = center.DistanceTo(pixelPosition);
		float alpha = Mathf.Clamp(1 - (distance / maxDistance), 0, 2);
		
		return alpha * noiseValue;
	}

	private void PlaceTile(int x, int y, float noiseValue)
	{
		Vector2I atlasCoord;
		
		if (noiseValue < 0.2)
		{
			atlasCoord = new Vector2I(3, 0);
		}
		else if (noiseValue < 0.3)
		{
			atlasCoord = new Vector2I(2, 0);
		}
		else
		{
			atlasCoord = Vector2I.Zero;
		}
		
		tileMapLayer.SetCell(new Vector2I(x, y), 0, atlasCoord);
	}
}
