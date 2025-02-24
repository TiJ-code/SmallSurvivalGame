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

        public struct TileCombination
        {
            public TileType topLeft, topRight, bottomLeft, bottomRight;

            public TileCombination(TileType topLeft, TileType topRight, TileType bottomLeft, TileType bottomRight)
            {
                this.topLeft = topLeft;
                this.topRight = topRight;
                this.bottomLeft = bottomLeft;
                this.bottomRight = bottomRight;
            }

            public override bool Equals(object obj)
            {
                if (obj is TileCombination other)
                {
                    return topLeft == other.topLeft && topRight == other.topRight && bottomLeft == other.bottomLeft && bottomRight == other.bottomRight;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(topLeft, topRight, bottomLeft, bottomRight);
            }
        }

        readonly Dictionary<TileCombination, Vector2I> neighboursToAtlasCoord = new()
        {
            { new TileCombination(Grass, Grass, Grass, Grass), new Vector2I(2, 2) }, // All corners
            { new TileCombination(Dirt,  Dirt,  Dirt,  Grass), new Vector2I(1, 4) }, // Outer bottom-right corner
            { new TileCombination(Dirt,  Dirt,  Grass, Dirt),  new Vector2I(0, 1) }, // Outer bottom-left corner
            { new TileCombination(Dirt,  Grass, Dirt,  Dirt),  new Vector2I(0, 3) }, // Outer top-right corner
            { new TileCombination(Grass, Dirt,  Dirt,  Dirt),  new Vector2I(3, 4) }, // Outer top-left corner
            { new TileCombination(Dirt,  Grass, Dirt,  Grass), new Vector2I(1, 1) }, // Right edge
            { new TileCombination(Grass, Dirt,  Grass, Dirt),  new Vector2I(3, 3) }, // Left edge
            { new TileCombination(Dirt,  Dirt,  Grass, Grass), new Vector2I(3, 1) }, // Bottom edge
            { new TileCombination(Grass, Grass, Dirt,  Dirt),  new Vector2I(1, 3) }, // Top edge
            { new TileCombination(Dirt,  Grass, Grass, Grass), new Vector2I(1, 2) }, // Inner bottom-right corner
            { new TileCombination(Grass, Dirt,  Grass, Grass), new Vector2I(2, 1) }, // Inner bottom-left corner
            { new TileCombination(Grass, Grass, Dirt,  Grass), new Vector2I(2, 3) }, // Inner top-right corner
            { new TileCombination(Grass, Grass, Grass, Dirt),  new Vector2I(3, 2) }, // Inner top-left corner
            { new TileCombination(Dirt,  Grass, Grass, Dirt),  new Vector2I(2, 4) }, // Bottom-left top-right corners
            { new TileCombination(Grass, Dirt,  Dirt,  Grass), new Vector2I(0, 2) }, // Top-left down-right corners
            { new TileCombination(Dirt,  Dirt,  Dirt,  Dirt),  new Vector2I(0, 4) }, // No corners
        };

        public override void _Ready()
        {
            /*foreach (Vector2I coord in GetUsedCells(0))
            {
                SetDisplayTile(coord);
            }*/
        }

        public void SetTile(Vector2I coords, Vector2I atlasCoords)
        {
            SetCell(0, coords, 0, atlasCoords);
            SetDisplayTile(coords);
        }

        void SetDisplayTile(Vector2I coords)
        {
            for (int i = 0; i < NEIGHBOURS.Length; i++)
            {
                Vector2I newPos = coords + NEIGHBOURS[i];
                displayTileMap.SetCell(0, newPos, 1, CalculateDisplayTile(newPos));
            }
        }

        Vector2I CalculateDisplayTile(Vector2I coords)
        {
            TileType bottomRight = GetWorldTile(coords - NEIGHBOURS[0]);
            TileType bottomLeft = GetWorldTile(coords - NEIGHBOURS[1]);
            TileType topRight = GetWorldTile(coords - NEIGHBOURS[2]);
            TileType topLeft = GetWorldTile(coords - NEIGHBOURS[3]);
            return neighboursToAtlasCoord[new TileCombination(topLeft, topRight, bottomLeft, bottomRight)];
        }

        TileType GetWorldTile(Vector2I coords)
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