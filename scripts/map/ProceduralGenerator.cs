using Godot;
using Godot.Collections;

public partial class ProceduralGenerator : Node2D
{
    [Export] private Noise moistureNoise;
    [Export] private Noise temperatureNoise;
    [Export] private Noise altitudeNoise;
    [Export] private TileMapLayer landLayer;
    [Export] private TileMapLayer waterLayer;
	
    private readonly Vector2I mapSize = new Vector2I(256, 256);
    private Vector2I halfMapSize;
    private Vector2 centrePosition;
    private float falloffMaxDistance;
	
    public override void _Ready()
    {
        halfMapSize = mapSize / 2;
        centrePosition = new Vector2(halfMapSize.X, halfMapSize.Y);
        falloffMaxDistance = centrePosition.Length() / 1.5f;
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        Array<Vector2I> cliffCells = new Array<Vector2I>();
		
        for (int x = 0; x < mapSize.X; x++)
        {
            for (int y = 0; y < mapSize.Y; y++)
            {
                Vector2I coords = new Vector2I(-halfMapSize.X + x, -halfMapSize.Y + y);
				
                float moist = ApplyFalloff(x, y, NormaliseNoise(moistureNoise.GetNoise2D(x, y))) * 10;
                float temperature = ApplyFalloff(x, y, NormaliseNoise(temperatureNoise.GetNoise2D(x, y))) * 10;
                float altitude = ApplyFalloff(x, y, NormaliseNoise(altitudeNoise.GetNoise2D(x, y))) * 15;

                Vector2I atlasCoords = new Vector2I(
                    (altitude < 2) ? 3 : (int) ((moist + 10) / 5),
                    Mathf.RoundToInt((temperature + 10) / 5)
                );
                landLayer.SetCell(coords, 1, atlasCoords);
				
                if (atlasCoords.X != 3)
                {
                    cliffCells.Add(coords);	
                }
            }
        }
		
        waterLayer.SetCellsTerrainConnect(cliffCells, 0, 0);
    }

    private float ApplyFalloff(int x, int y, float noiseValue)
    { 
        float distance = centrePosition.DistanceTo(new Vector2(x, y));
        float alpha = 1f - (distance / falloffMaxDistance);
        return alpha * noiseValue;
    }

    private static float NormaliseNoise(float noiseValue)
    {
        return Mathf.Clamp((noiseValue + 1f) / 2f, 0f, 1f);
    }
}