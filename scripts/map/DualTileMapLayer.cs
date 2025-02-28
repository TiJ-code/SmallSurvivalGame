using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using static TileType;

public partial class DualTileMapLayer : TileMapLayer
{
    [Export] private TileMapLayer SandDisplayLayer;
    [Export] private TileMapLayer GrassDisplayLayer;
    private int width = 256;
    private int height = 256;

    private readonly Vector2I[] neighbours = new Vector2I[]
    {
        new Vector2I(0, 0), new Vector2I(1, 0), new Vector2I(0, 1), new Vector2I(1, 1),
    };

    public void GenerateDisplayLayer()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2I coord = new Vector2I(x, y);
                int counter = 0;
                // top-left, top-right, bottom-left, bottom-right
                TileType[] neighbourTiles = new TileType[4];
                foreach (Vector2I neighbour in neighbours)
                {
                    Vector2I neighbourCoord = coord + neighbour;
                    neighbourTiles[counter] = GetNeighbourPlaceholderType(GetCellAtlasCoords(neighbourCoord));
                    counter++;
                }
                Tuple<TileType, TileType> tileTypesInArray = GetAllTileTypesInArray(neighbourTiles);
                PlaceTileType(tileTypesInArray.Item1, coord, GetTextureAtlasCoord(neighbourTiles, tileTypesInArray));
            }
        }
    }

    private TileType GetNeighbourPlaceholderType(Vector2I placeholderAtlasCoord)
    {
        TileType type = None;
        if (placeholderAtlasCoord.Equals(new Vector2I(0, 0)))
        {
            type = Grass;
        }
        else if (placeholderAtlasCoord.Equals(new Vector2I(2, 0)))
        {
            type = Sand;
        }
        else if (placeholderAtlasCoord.Equals(new Vector2I(3, 0)))
        {
            type = Water;
        }

        return type;
    }

    private Vector2I GetTextureAtlasCoord(TileType[] neighbourTypes, Tuple<TileType, TileType> tileTypesInArray)
    {
        TileType item1 = tileTypesInArray.Item1;
        TileType item2 = tileTypesInArray.Item2;

        Tuple<TileType, TileType, TileType, TileType> compareTuple = new(neighbourTypes[0], neighbourTypes[1],
            neighbourTypes[2], neighbourTypes[3]);

        Vector2I atlasCoord;
        if (item1 == None || item2 == None)
        {
            atlasCoord = new Vector2I(0, 3);
        }
        else
        {
            if (neighbourTypes[0] == neighbourTypes[1] && neighbourTypes[1] == neighbourTypes[2] &&
                neighbourTypes[2] == neighbourTypes[3])
            {
                if (item1 == Grass || item1 == Sand)
                {
                    atlasCoord = new Vector2I(2, 1);
                }
                else
                {
                    atlasCoord = new Vector2I(0, 3);
                }
            }
            else
            {
                atlasCoord = compareTuple switch
                {
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item1, item1, item1, item1))
                        => new Vector2I(2, 1), // All item1
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item1, item1, item1, item2))
                        => new Vector2I(1, 3), // Three item1, one item2
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item1, item1, item2, item1))
                        => new Vector2I(0, 0), // Two item1, one item2
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item1, item1, item2, item2))
                        => new Vector2I(3, 0), // Two item1, two item2
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item1, item2, item1, item1))
                        => new Vector2I(0, 2), // Two item1, one item2
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item1, item2, item1, item2))
                        => new Vector2I(1, 0), // Alternating item1 and item2
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item1, item2, item2, item1))
                        => new Vector2I(2, 3), // Two item2, one item1
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item1, item2, item2, item2))
                        => new Vector2I(1, 1), // All item2
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item2, item1, item1, item1))
                        => new Vector2I(3, 3), // All item1
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item2, item1, item1, item2))
                        => new Vector2I(0, 1), // Two item1, one item2
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item2, item1, item2, item1))
                        => new Vector2I(3, 2), // Alternating item1 and item2
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item2, item1, item2, item2))
                        => new Vector2I(2, 0), // Two item2, one item1
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item2, item2, item1, item1))
                        => new Vector2I(1, 2), // Two item1, two item2
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item2, item1, item1, item1))
                        => new Vector2I(0, 3), // Two item1, one item2
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item2, item2, item2, item1))
                        => new Vector2I(3, 1), // Three item2, one item1
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item2, item2, item1, item2))
                        => new Vector2I(2, 2), // Two item2, one item1
                    var t when t.Equals(new Tuple<TileType, TileType, TileType, TileType>(item2, item1, item1, item2))
                        => new Vector2I(1, 3), // Two item1, one item2
                    _ => new Vector2I(0, 0) // Default case for unhandled combinations
                };
            }
        }

        Vector2I deltaVec = Vector2I.Zero;
        if (item1 == Water || item2 == Water)
        {
            deltaVec = new Vector2I(0, 0);
        }
        else if (item1 == Grass || item2 == Grass)
        {
            deltaVec = new Vector2I(0, 4);
        }

        Vector2I returnVec = atlasCoord + deltaVec;

        return returnVec;
    }

    private Tuple<TileType, TileType> GetAllTileTypesInArray(TileType[] localNeighbours)
    {
        // Create a HashSet to store unique tile types
        HashSet<TileType> uniqueTileTypes = new HashSet<TileType>();

        // Iterate through the array and add tile types to the HashSet
        foreach (TileType tileType in localNeighbours)
        {
            uniqueTileTypes.Add(tileType);
            // Stop if we already have two unique types
            if (uniqueTileTypes.Count == 2) break;
        }

        // Create a list to store the prioritized tile types
        List<TileType> prioritizedTiles = new List<TileType>();

        // Add tile types based on the new priority: Water > Sand > Grass
        if (uniqueTileTypes.Contains(Water))
        {
            prioritizedTiles.Add(Water);
        }

        if (uniqueTileTypes.Contains(Sand))
        {
            prioritizedTiles.Add(Sand);
        }

        if (uniqueTileTypes.Contains(Grass))
        {
            prioritizedTiles.Add(Grass);
        }

        // Ensure we have at least one tile type
        if (prioritizedTiles.Count == 0)
        {
            return new Tuple<TileType, TileType>(TileType.None, TileType.None);
        }

        // Return the first two unique tile types, filling with the first if only one exists
        TileType firstTile = prioritizedTiles[0];
        TileType secondTile = prioritizedTiles.Count > 1 ? prioritizedTiles[1] : firstTile;

        return new Tuple<TileType, TileType>(firstTile, secondTile);
    }

    private void PlaceTileType(TileType layer, Vector2I position, Vector2I atlasCoord)
    {
        TileMapLayer placementLayer;
        if (layer == Grass)
        {
            placementLayer = GrassDisplayLayer;
        }
        else
        {
            placementLayer = SandDisplayLayer;
        }
        placementLayer.SetCell(position, 0, atlasCoord);
    }
}

public enum TileType
{
    None,
    Water,
    Sand,
    Dirt,
    Grass
}