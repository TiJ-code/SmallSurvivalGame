using Godot;
using System;

namespace SmallSurvivalGame.scripts.map.generation
{
    public partial class WorldGenerator : TileMap
    {
        private Vector2I WorldSize {get; set;}
        public GameMap WorldMap { get; private set;}

        public override void _Ready()
        {
            WorldSize = new Vector2I(128, 64);
            WorldMap = GenerateWorld();
        }

        private GameMap GenerateWorld()
        {
            Clear();
            PerlinNoise perlinNoise = new PerlinNoise(WorldSize.X, WorldSize.Y, (int) new RandomNumberGenerator().Randi());
            double[,] noiseMap = perlinNoise.GenerateNoiseMap(8, 0.5d, 1d, Vector2.Zero);

            GameMap gameMap = new GameMap(WorldSize);

            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                for (int y = 0; y < noiseMap.GetLength(1); y++)
                {
                    Vector2I location = new Vector2I(x, y);

                    Tile tile = new Tile(location, GetTileTypeByValue(noiseMap[x, y]));
                    gameMap.AddTile(tile);
                    
                    SetCell(0, location, 0, new Vector2I((int)tile.tileType, 0));
                }
            }

            return gameMap;
        }

        private TileType GetTileTypeByValue(double value)
        {
            TileType returnValue = TileType.Grass;
            /*
            if (value < 0.15d)
            {
                returnValue = TileType.Deep_Water;
            } 
            else if (value < 0.25d)
            {
                returnValue = TileType.Water;
            }
            else if (value < 0.6d)
            {
                returnValue = TileType.Grass;
            }
            else if (value < 0.75d)
            {
                returnValue = TileType.High_Grass;
            }
            else if (value < 0.80d)
            {
                returnValue = TileType.Rock;
            }
            else if (value < 0.9d)
            {
                returnValue = TileType.Stone;
            }
            else
            {
                returnValue = TileType.Snow;
            }
            */
            return returnValue;
        }
    }
}