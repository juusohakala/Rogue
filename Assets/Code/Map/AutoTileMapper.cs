using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AutoTileMapper : MonoBehaviour
{
    [Header("TerrainMap:")]
    public TerrainMap TerrainMap;

    [Header("Tilemaps:")]
    public Tilemap Over;
    public Tilemap Under;

    private Dictionary<TerrainMap.Terrain, AutoTileset> Tilesets;


    void SetTile(Tilemap tilemap, int x, int y, int corner, TerrainMap.Terrain? terrain, AutoTileset.TilePosition tilePos)
    {
        if (terrain == null) return;
        if (!Tilesets.TryGetValue((TerrainMap.Terrain)terrain, out var tileset)) return;
        if (!tileset.Tiles.TryGetValue(tilePos, out var tiles)) return;
        if (tiles.Length == 0) return;

        var tile = tiles[0];
        if (tiles.Length > 1) tile = tiles[UnityEngine.Random.Range(0, tiles.Length - 1)];

        var tileX = x * 2;
        var tileY = y * 2;

        var finalTilePosition =
            (corner == 1) ? new Vector3Int(tileX + 1, tileY + 1, 0)
            : (corner == 2) ? new Vector3Int(tileX, tileY + 1, 0)
            : (corner == 3) ? new Vector3Int(tileX, tileY, 0)
            : (corner == 4) ? new Vector3Int(tileX + 1, tileY, 0)
            : new Vector3Int();

        tilemap.SetTile(finalTilePosition, tile);
    }


    void SetOver(int x, int y, int corner, TerrainMap.Terrain? terrain, AutoTileset.TilePosition tilePos)
    {
        SetTile(Over, x, y, corner, terrain, tilePos);
    }

    void SetUnder(int x, int y, int corner, TerrainMap.Terrain? terrain)
    {
        SetTile(Under, x, y, corner, terrain, AutoTileset.TilePosition.Middle);
    }


    /// <summary>
    /// Sets tiles by it's terrain and surrounding terrains.
    /// </summary>
    void SetTiles(int x, int y)
    {
        if (TerrainMap.GetTerrain(x, y) == null)
        {
            Debug.LogWarning($"Tried to set tiles but terrain not found {(x, y)}");
            return;
        }
        else
        {


            var top = TerrainMap.GetTerrain(x, y + 1);
            var right = TerrainMap.GetTerrain(x + 1, y);
            var bottom = TerrainMap.GetTerrain(x, y - 1);
            var left = TerrainMap.GetTerrain(x - 1, y);

            var topRight = TerrainMap.GetTerrain(x + 1, y + 1);
            var bottomRight = TerrainMap.GetTerrain(x + 1, y - 1);
            var bottomLeft = TerrainMap.GetTerrain(x - 1, y - 1);
            var topLeft = TerrainMap.GetTerrain(x - 1, y + 1);

            // Terrains:
            foreach (TerrainMap.Terrain terrain in Enum.GetValues(typeof(TerrainMap.Terrain)))
            {


                if (TerrainMap.GetTerrain(x, y) == terrain)
                {

                    // 1. Top Right tile

                    var tilepos1 =
                        (right >= terrain && top >= terrain)
                            ? (topRight >= terrain)
                                ? AutoTileset.TilePosition.Middle
                                : AutoTileset.TilePosition.InsideTopRight
                            : (right >= terrain)
                                ? AutoTileset.TilePosition.Top
                                : (top >= terrain)
                                    ? AutoTileset.TilePosition.Right
                                    : AutoTileset.TilePosition.TopRight;

                    SetOver(x, y, 1, terrain, tilepos1);

                    if (tilepos1 == AutoTileset.TilePosition.Top)
                        SetUnder(x, y, 1, TerrainMap.GetTerrain(x, y + 1));
                    if (tilepos1 == AutoTileset.TilePosition.Right)
                        SetUnder(x, y, 1, TerrainMap.GetTerrain(x + 1, y));
                    if (tilepos1 == AutoTileset.TilePosition.InsideTopRight)
                        SetUnder(x, y, 1, TerrainMap.GetTerrain(x + 1, y + 1));
                    if (tilepos1 == AutoTileset.TilePosition.TopRight)
                    {
                        var t1 = (int)TerrainMap.GetTerrain(x, y + 1);
                        var t2 = (int)TerrainMap.GetTerrain(x + 1, y);
                        SetUnder(x, y, 1, (TerrainMap.Terrain)Math.Max(t1, t2));
                    }




                    // 2. Top left tile
                    var tilepos2 =
                        (left >= terrain && top >= terrain)
                            ? (topLeft >= terrain)
                                ? AutoTileset.TilePosition.Middle
                                : AutoTileset.TilePosition.InsideTopLeft
                            : (left >= terrain)
                                ? AutoTileset.TilePosition.Top
                                : (top >= terrain)
                                    ? AutoTileset.TilePosition.Left
                                    : AutoTileset.TilePosition.TopLeft;

                    SetOver(x, y, 2, terrain, tilepos2);

                    if (tilepos2 == AutoTileset.TilePosition.Top)
                        SetUnder(x, y, 2, TerrainMap.GetTerrain(x, y + 1));
                    if (tilepos2 == AutoTileset.TilePosition.Left)
                        SetUnder(x, y, 2, TerrainMap.GetTerrain(x - 1, y));
                    if (tilepos2 == AutoTileset.TilePosition.InsideTopLeft)
                        SetUnder(x, y, 2, TerrainMap.GetTerrain(x - 1, y + 1));
                    if (tilepos2 == AutoTileset.TilePosition.TopLeft)
                    {
                        var t1 = (int)TerrainMap.GetTerrain(x, y + 1);
                        var t2 = (int)TerrainMap.GetTerrain(x - 1, y);
                        SetUnder(x, y, 2, (TerrainMap.Terrain)Math.Max(t1, t2));
                    }


                    // 3. Bottom Left tile
                    var tilepos3 =
                        (left >= terrain && bottom >= terrain)
                            ? (bottomLeft >= terrain)
                                ? AutoTileset.TilePosition.Middle
                                : AutoTileset.TilePosition.InsideBottomLeft
                            : (left >= terrain)
                                ? AutoTileset.TilePosition.Bottom
                                : (bottom >= terrain)
                                    ? AutoTileset.TilePosition.Left
                                    : AutoTileset.TilePosition.BottomLeft;

                    SetOver(x, y, 3, terrain, tilepos3);

                    if (tilepos3 == AutoTileset.TilePosition.Bottom)
                        SetUnder(x, y, 3, TerrainMap.GetTerrain(x, y - 1));
                    if (tilepos3 == AutoTileset.TilePosition.Left)
                        SetUnder(x, y, 3, TerrainMap.GetTerrain(x - 1, y));
                    if (tilepos3 == AutoTileset.TilePosition.InsideBottomLeft)
                        SetUnder(x, y, 3, TerrainMap.GetTerrain(x - 1, y - 1));
                    if (tilepos3 == AutoTileset.TilePosition.BottomLeft)
                    {
                        var t1 = (int)TerrainMap.GetTerrain(x, y - 1);
                        var t2 = (int)TerrainMap.GetTerrain(x - 1, y);
                        SetUnder(x, y, 3, (TerrainMap.Terrain)Math.Max(t1, t2));
                    }


                    // 4. Bottom Right tile
                    var tilepos4 =
                        (right >= terrain && bottom >= terrain)
                            ? (bottomRight >= terrain)
                                ? AutoTileset.TilePosition.Middle
                                : AutoTileset.TilePosition.InsideBottomRight
                            : (right >= terrain)
                                ? AutoTileset.TilePosition.Bottom
                                : (bottom >= terrain)
                                    ? AutoTileset.TilePosition.Right
                                    : AutoTileset.TilePosition.BottomRight;

                    SetOver(x, y, 4, terrain, tilepos4);

                    if (tilepos4 == AutoTileset.TilePosition.Bottom)
                        SetUnder(x, y, 4, TerrainMap.GetTerrain(x, y - 1));
                    if (tilepos4 == AutoTileset.TilePosition.Right)
                        SetUnder(x, y, 4, TerrainMap.GetTerrain(x + 1, y));
                    if (tilepos4 == AutoTileset.TilePosition.InsideBottomRight)
                        SetUnder(x, y, 4, TerrainMap.GetTerrain(x + 1, y - 1));
                    if (tilepos4 == AutoTileset.TilePosition.BottomRight)
                    {
                        var t1 = (int)TerrainMap.GetTerrain(x, y - 1);
                        var t2 = (int)TerrainMap.GetTerrain(x + 1, y);
                        SetUnder(x, y, 4, (TerrainMap.Terrain)Math.Max(t1, t2));
                    }


                }
            }
        }



    }

    // Start is called before the first frame update
    void Start()
    {

        Tilesets = new Dictionary<TerrainMap.Terrain, AutoTileset>();

        foreach (Transform c in transform)
        {
            var c1 = c.GetComponent<AutoTileset>();
            var c2 = c.gameObject.GetComponent<AutoTileset>();

            var name = c.gameObject.name;

            Enum.TryParse(typeof(TerrainMap.Terrain), name, out var key);

            Tilesets.Add((TerrainMap.Terrain)key, c2);
        }

        // tee sama tarkistus kaikilel yksittäisille Tileille (tileposition) !
        // check all terrains has tileset:
        foreach (TerrainMap.Terrain terrain in Enum.GetValues(typeof(TerrainMap.Terrain)))
        {
            if (!Tilesets.ContainsKey(terrain))
            {
                Debug.LogError($"AutoTileset not found for terrain \"{terrain}\"");

            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        var tilesDrawnByFrame = 30;

        for (var i = 0; i < tilesDrawnByFrame; i++)
        {
            var key = TerrainMap.TakeReadyKey();

            if (key != null)
            {
                var x = key.Value.Item1;
                var y = key.Value.Item2;

                if (TerrainMap.GetTerrain(x, y) != null) SetTiles(x, y);
            }
        }



    }
}
