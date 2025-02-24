using Godot;
using System;
using System.Collections.Generic;
using static SmallSurvivalGame.scripts.map.TileType;

namespace SmallSurvivalGame.scripts.map
{
    public partial class DualGridTileMap : TileMap
    {
        [Export] private TileMap displayTileMap;
        [Export] public Vector2I grassPlaceholderAtlasCoord;
        [Export] public Vector2I dirtPlaceholderAtlasCoord;
        readonly Vector2I[] NEIGHBOURS = new Vector2I[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1) };

        readonly Dictionary<Tuple<TileType, TileType, TileType, TileType>, Vector2I> neighboursToAtlasCoord = new()
        {
            { new (Grass, Grass, Grass, Grass), new Vector2I(2, 1) }, // All corners
            { new (Dirt,  Dirt,  Dirt,  Grass), new Vector2I(1, 3) }, // Outer bottom-right corner
            { new (Dirt,  Dirt,  Grass, Dirt),  new Vector2I(0, 0) }, // Outer bottom-left corner
            { new (Dirt,  Grass, Dirt,  Dirt),  new Vector2I(0, 2) }, // Outer top-right corner
            { new (Grass, Dirt,  Dirt,  Dirt),  new Vector2I(3, 3) }, // Outer top-left corner
            { new (Dirt,  Grass, Dirt,  Grass), new Vector2I(1, 0) }, // Right edge
            { new (Grass, Dirt,  Grass, Dirt),  new Vector2I(3, 2) }, // Left edge
            { new (Dirt,  Dirt,  Grass, Grass), new Vector2I(3, 0) }, // Bottom edge
            { new (Grass, Grass, Dirt,  Dirt),  new Vector2I(1, 2) }, // Top edge
            { new (Dirt,  Grass, Grass, Grass), new Vector2I(1, 1) }, // Inner bottom-right corner
            { new (Grass, Dirt,  Grass, Grass), new Vector2I(2, 0) }, // Inner bottom-left corner
            { new (Grass, Grass, Dirt,  Grass), new Vector2I(2, 2) }, // Inner top-right corner
            { new (Grass, Grass, Grass, Dirt),  new Vector2I(3, 1) }, // Inner top-left corner
            { new (Dirt,  Grass, Grass, Dirt),  new Vector2I(2, 3) }, // Bottom-left top-right corners
            { new (Grass, Dirt,  Dirt,  Grass), new Vector2I(0, 1) }, // Top-left down-right corners
            { new (Dirt,  Dirt,  Dirt,  Dirt),  new Vector2I(0, 3) }, // No corners
        };

        public override void _Ready()
        {
            foreach (Vector2I coord in GetUsedCells(0))
            {
                SetDisplayTile(coord);
            }
        }

        public void SetTile(Vector2I coords, Vector2I atlasCoords)
        {
            SetCell(0, coords, 0, atlasCoords);
            SetDisplayTile(coords);
        }

        private void SetDisplayTile(Vector2I coords)
        {
            foreach (Vector2I neighbour in NEIGHBOURS)
            {
                Vector2I newPosition = coords + neighbour;
                displayTileMap.SetCell(0, newPosition, 1, CalculateDisplayTile(newPosition));
            }
        }

        private Vector2I CalculateDisplayTile(Vector2I coords)
        {
            TileType bottomRight = GetWorldTile(coords - NEIGHBOURS[0]);
            TileType bottomLeft = GetWorldTile(coords - NEIGHBOURS[1]);
            TileType topRight = GetWorldTile(coords - NEIGHBOURS[2]);
            TileType topLeft = GetWorldTile(coords - NEIGHBOURS[3]);
            return neighboursToAtlasCoord[new (topLeft, topRight, bottomLeft, bottomRight)];
        }

        private TileType GetWorldTile(Vector2I coords)
        {
            return (GetCellAtlasCoords(0, coords) == grassPlaceholderAtlasCoord) ? Grass : Dirt;
        }
    }

    public enum TileType
    {
        None,
        Grass,
        Dirt
    }
}