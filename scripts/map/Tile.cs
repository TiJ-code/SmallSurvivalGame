using Godot;
using System;

namespace SmallSurvivalGame.scripts.map
{
    public class Tile
    {
        public Vector2I location { get; private set; }
        public TileType tileType { get; private set; }

        public Tile(Vector2I location, TileType tileType)
        {
            this.location = location;
            this.tileType = tileType;
        }

        public int GetMovementCost()
        {
            return (int) tileType;
        }
    }

    public enum TileType
    {
        Grass = 0, High_Grass = 1, Water = 2, Deep_Water = 3, Rock = 4, Stone = 5, Snow = 6, Path = 7
    }
}