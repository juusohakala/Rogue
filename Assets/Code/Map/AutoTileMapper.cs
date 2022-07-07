using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AutoTileMapper : MonoBehaviour
{
    [Header("TerrainMap:")]
    public TerrainMap TerrainMap;

    //[Header("Tilesets:")]
    //public AutoTileset[] AutoTilesets;

    [Header("Tilemaps:")]
    public Tilemap Over;
    public Tilemap Under;

    private Dictionary<TerrainMap.Terrain, AutoTileset> Tilesets;


    //public void SetUnSettedTiles()
    //{
    //    for (int i = KeysOfUnSettedTiles.Count - 1; i >= 0; i--)
    //    {
    //        var x = KeysOfUnSettedTiles[i].Item1;
    //        var y = KeysOfUnSettedTiles[i].Item2;
    //        if (
    //                  Data.ContainsKey((x, y + 1))
    //            && Data.ContainsKey((x + 1, y + 1))
    //            && Data.ContainsKey((x + 1, y))
    //            && Data.ContainsKey((x + 1, y - 1))
    //            && Data.ContainsKey((x, y - 1))
    //            && Data.ContainsKey((x - 1, y - 1))
    //            && Data.ContainsKey((x - 1, y))
    //            && Data.ContainsKey((x - 1, y + 1)))
    //        {
    //            SetTiles(x, y);
    //            KeysOfUnSettedTiles.RemoveAt(i);
    //        }
    //    }
    //}

    Vector3Int FinalTilePosition(int x, int y, int corner)
    {
        var tileX = x * 2;
        var tileY = y * 2;

        return
            (corner == 1) ? new Vector3Int(tileX + 1, tileY + 1, 0)
            : (corner == 2) ? new Vector3Int(tileX, tileY + 1, 0)
            : (corner == 3) ? new Vector3Int(tileX, tileY, 0)
            : (corner == 4) ? new Vector3Int(tileX + 1, tileY, 0)
            : new Vector3Int();
    }

    void SetOver(int x, int y, int corner, AutoTileset tileset, AutoTileset.TilePosition tilePos)
    {
        Over.SetTile(FinalTilePosition(x, y, corner), tileset.Tiles[tilePos][0]);
    }

    void SetUnder(int x, int y, int corner, AutoTileset tileset)
    {
        Under.SetTile(FinalTilePosition(x, y, corner), tileset.Tiles[AutoTileset.TilePosition.Middle][0]);
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
                if (!Tilesets.TryGetValue(terrain, out var tileset))
                {
                    Debug.LogWarning($"AutoTileset not found for terrain \"{terrain}\"");
                    continue;
                }

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

                    SetOver(x, y, 1, tileset, tilepos1);

                    //if (tilepos1 == AutoTileset.TilePosition.Left)
                    //{
                    //    Under.SetTile(new Vector3Int(tileX + 1, tileY + 1, 0), tileset2.Tiles[AutoTileset.TilePosition.Middle][0]);
                    //}




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

                    SetOver(x, y, 2, tileset, tilepos2);


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

                    SetOver(x, y, 3, tileset, tilepos3);


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

                    SetOver(x, y, 4, tileset, tilepos4);




                }
            }
        }



    }

    // Start is called before the first frame update
    void Start()
    {
        //for (var i = 0; i < AutoTilesets.Length; i++)
        //{
        //    AutoTilesets[i] = Instantiate(AutoTilesets[i]);
        //}

        Tilesets = new Dictionary<TerrainMap.Terrain, AutoTileset>();



        foreach (Transform c in transform)
        {
            var c1 = c.GetComponent<AutoTileset>();
            var c2 = c.gameObject.GetComponent<AutoTileset>();

            var name = c.gameObject.name;

            Enum.TryParse(typeof(TerrainMap.Terrain), name, out var key);

            Tilesets.Add((TerrainMap.Terrain)key, c2);
        }

        //SetUnSettedTiles();

    }

    // Update is called once per frame
    void Update()
    {
        //var newestKey = TerrainMap.TakeNewestKey();

        //if (newestKey != null)
        //{
        //    var x = newestKey.Value.Item1;
        //    var y = newestKey.Value.Item2;

        //    if (TerrainMap.GetTerrain(x, y) != TerrainMap.Terrain.Empty) SetTiles(x, y);

        //    if (TerrainMap.GetTerrain(x, y + 1) != TerrainMap.Terrain.Empty) SetTiles(x, y + 1);
        //    if (TerrainMap.GetTerrain(x + 1, y) != TerrainMap.Terrain.Empty) SetTiles(x + 1, y);
        //    if (TerrainMap.GetTerrain(x, y - 1) != TerrainMap.Terrain.Empty) SetTiles(x, y - 1);
        //    if (TerrainMap.GetTerrain(x - 1, y) != TerrainMap.Terrain.Empty) SetTiles(x - 1, y);

        //    if (TerrainMap.GetTerrain(x + 1, y + 1) != TerrainMap.Terrain.Empty) SetTiles(x + 1, y + 1);
        //    if (TerrainMap.GetTerrain(x + 1, y - 1) != TerrainMap.Terrain.Empty) SetTiles(x + 1, y - 1);
        //    if (TerrainMap.GetTerrain(x - 1, y - 1) != TerrainMap.Terrain.Empty) SetTiles(x - 1, y - 1);
        //    if (TerrainMap.GetTerrain(x - 1, y + 1) != TerrainMap.Terrain.Empty) SetTiles(x - 1, y + 1);

        //}

        for (var i = 0; i < 30; i++)
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
