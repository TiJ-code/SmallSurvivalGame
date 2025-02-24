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
}