using Godot;
using System;

namespace SmallSurvivalGame.scripts.map
{
    public class GameMap
    {
        public Tile[] tiles { get; private set; }

        public GameMap(Vector2I MapSize)
        {
            tiles = new Tile[MapSize.X * MapSize.Y];
        }

        public void AddTile(Tile tile)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] == null)
                {
                    tiles[i] = tile;
                    break;
                }
            }
        }
    }
}
