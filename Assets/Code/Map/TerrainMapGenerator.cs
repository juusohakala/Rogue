using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainMapGenerator : MonoBehaviour
{
    enum Terrain
    {
        Empty,
        Grass,
        Sand
    }


    [Header("Camera:")]
    public Camera Camera;

    [Header("Tilesets:")]
    public AutoTileset[] AutoTilesets;

    [Header("Tilemaps:")]
    public Tilemap Over;
    public Tilemap Under;

    [Header("Generator settings:")]
    public double PerlinNoiseZoom;


    /// <summary>
    /// These values should be big enough to cover visible play area when camera is zoomed out to max.
    /// These could be calculated from camera dimensions somehow but I don't care for now.
    /// Even numbers only!
    /// </summary>
    private int MapMinWidth = 4;
    private int MapMinHeight = 4;





    private Vector3 CameraPositionInTerrainCellsWhenLastGenerate;


    private Dictionary<(int, int), Terrain> TerrainMap;
    private List<(int, int)> UnSettedTiles;


    Terrain GetTerrain(int x, int y)
    {

        if (TerrainMap.ContainsKey((x, y)))
        {
            return TerrainMap[(x, y)];
        }

        return Terrain.Empty;
    }


    /// <summary>
    /// Sets tiles by it's terrain and surrounding terrains.
    /// </summary>
    void SetTiles(int x, int y)
    {
        if (!TerrainMap.ContainsKey((x, y)))
        {
            Debug.LogWarning($"Tried to set tiles but terrain not found {(x, y)}");
            return;
        }
        else
        {

            var tileX = x * 2;
            var tileY = y * 2;



            

            var top = GetTerrain(x, y + 1);
            var right = GetTerrain(x + 1, y);
            var bottom = GetTerrain(x, y - 1);
            var left = GetTerrain(x - 1, y);

            var topRight = GetTerrain(x + 1, y + 1);
            var bottomRight = GetTerrain(x + 1, y - 1);
            var bottomLeft = GetTerrain(x - 1, y - 1);
            var topLeft = GetTerrain(x - 1, y + 1);


            // Terrains:

            var t = Terrain.Sand;

            var tileset = AutoTilesets[0];
            var tilepos = AutoTileset.TilePosition.Middle;

            if (GetTerrain(x, y) == t)
            {

                // Top Right tile

                tilepos =
                    (right == t && top == t)
                        ? (topRight == t)
                            ? AutoTileset.TilePosition.Middle
                            : AutoTileset.TilePosition.InsideTopRight
                        : (right == t)
                            ? AutoTileset.TilePosition.Top
                            : (top == t)
                                ? AutoTileset.TilePosition.Right
                                : AutoTileset.TilePosition.TopRight;

                Over.SetTile(new Vector3Int(tileX + 1, tileY + 1, 0), tileset.Tiles[tilepos][0]);

                // Bottom Right tile

                tilepos =
                    (right == t && bottom == t)
                        ? (bottomRight == t)
                            ? AutoTileset.TilePosition.Middle
                            : AutoTileset.TilePosition.InsideBottomRight
                        : (right == t)
                            ? AutoTileset.TilePosition.Bottom
                            : (bottom == t)
                                ? AutoTileset.TilePosition.Right
                                : AutoTileset.TilePosition.BottomRight;

                Over.SetTile(new Vector3Int(tileX + 1, tileY, 0), tileset.Tiles[tilepos][0]);

                // Bottom Left tile

                tilepos =
                    (left == t && bottom == t)
                        ? (bottomLeft == t)
                            ? AutoTileset.TilePosition.Middle
                            : AutoTileset.TilePosition.InsideBottomLeft
                        : (left == t)
                            ? AutoTileset.TilePosition.Bottom
                            : (bottom == t)
                                ? AutoTileset.TilePosition.Left
                                : AutoTileset.TilePosition.BottomLeft;

                Over.SetTile(new Vector3Int(tileX, tileY, 0), tileset.Tiles[tilepos][0]);

                // Top left tile

                tilepos =
                    (left == t && top == t)
                        ? (topLeft == t)
                            ? AutoTileset.TilePosition.Middle
                            : AutoTileset.TilePosition.InsideTopLeft
                        : (left == t)
                            ? AutoTileset.TilePosition.Top
                            : (top == t)
                                ? AutoTileset.TilePosition.Left
                                : AutoTileset.TilePosition.TopLeft;

                Over.SetTile(new Vector3Int(tileX, tileY + 1, 0), tileset.Tiles[tilepos][0]);

            }
        }



    }

    /// <summary>
    /// Sets a terrain.
    /// </summary>
    void SetTerrain(int x, int y, Terrain terrain)
    {

        if (TerrainMap.ContainsKey((x, y)))
        {
            Debug.LogWarning($"DataMap already has a key {(x, y)}");
            return;
        }
        else
        {
            TerrainMap.Add((x, y), terrain);
            UnSettedTiles.Add((x, y));
        }

    }

    Terrain GenerateTerrain(int x, int y)
    {
        var pnx = (float)(((double)x / MapMinWidth + 0.5) * PerlinNoiseZoom);
        var pny = (float)(((double)y / MapMinHeight + 0.5) * PerlinNoiseZoom);

        var noiseValueOfThePoint = Mathf.PerlinNoise(pnx, pny);

        if (noiseValueOfThePoint > 0.5)
        {
            return Terrain.Grass;
        }
        else
        {
            return Terrain.Sand;
        }

    }

    void SetUnSettedTiles()
    {
        for (int i = UnSettedTiles.Count - 1; i >= 0; i--)
        {
            var x = UnSettedTiles[i].Item1;
            var y = UnSettedTiles[i].Item2;
            if (
                      TerrainMap.ContainsKey((x, y + 1))
                && TerrainMap.ContainsKey((x + 1, y + 1))
                && TerrainMap.ContainsKey((x + 1, y))
                && TerrainMap.ContainsKey((x + 1, y - 1))
                && TerrainMap.ContainsKey((x, y - 1))
                && TerrainMap.ContainsKey((x - 1, y - 1))
                && TerrainMap.ContainsKey((x - 1, y))
                && TerrainMap.ContainsKey((x - 1, y + 1)))
            {
                SetTiles(x, y);
                UnSettedTiles.RemoveAt(i);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TerrainMap = new Dictionary<(int, int), Terrain>();
        UnSettedTiles = new List<(int, int)>();
        for (var i = 0; i < AutoTilesets.Length; i++)
        {
            AutoTilesets[i] = Instantiate(AutoTilesets[i]);
        }
        

        // Generate first map that just covers the camera
        CameraPositionInTerrainCellsWhenLastGenerate = Over.LocalToCell(Camera.transform.position);
        for (var x = -MapMinWidth / 2; x < MapMinWidth / 2; x++)
        {
            for (var y = -MapMinHeight / 2; y < MapMinHeight / 2; y++)
            {

                SetTerrain(x, y, GenerateTerrain(x, y));
            }
        }

        SetUnSettedTiles();

    }

    // Update is called once per frame
    void Update()
    {
        var w = MapMinWidth / 2;
        var h = MapMinHeight / 2;

        var cameraMoved = false;


        // here could be something to optimize :-)

        // these are measured in terrain cells that are twice bigger than cells/tiles
        var cameraPosition = Over.LocalToCell(Camera.transform.position) / 2;
        var cameraMove = CameraPositionInTerrainCellsWhenLastGenerate - cameraPosition;

        if (cameraMove.x >= 1)
        {
            var x = cameraPosition.x - MapMinWidth / 2;
            for (var i = -MapMinHeight / 2; i < MapMinHeight / 2; i++)
            {
                var y = cameraPosition.y + i;
                if (GetTerrain(x, y) == Terrain.Empty) SetTerrain(x, y, GenerateTerrain(x, y));
            }
            CameraPositionInTerrainCellsWhenLastGenerate = cameraPosition;
            cameraMoved = true;
        }

        if (cameraMove.x <= -1)
        {
            var x = cameraPosition.x + MapMinWidth / 2 - 1;
            for (var i = -MapMinHeight / 2; i < MapMinHeight / 2; i++)
            {
                var y = cameraPosition.y + i;
                if (GetTerrain(x, y) == Terrain.Empty) SetTerrain(x, y, GenerateTerrain(x, y));
            }
            CameraPositionInTerrainCellsWhenLastGenerate = cameraPosition;
            cameraMoved = true;
        }

        if (cameraMove.y >= 1)
        {
            var y = cameraPosition.y - MapMinHeight / 2;
            for (var i = -MapMinWidth / 2; i < MapMinWidth / 2; i++)
            {
                var x = cameraPosition.x + i;
                if (GetTerrain(x, y) == Terrain.Empty) SetTerrain(x, y, GenerateTerrain(x, y));
            }
            CameraPositionInTerrainCellsWhenLastGenerate = cameraPosition;
            cameraMoved = true;
        }

        if (cameraMove.y <= -1)
        {
            var y = cameraPosition.y + MapMinHeight / 2 - 1;
            for (var i = -MapMinWidth / 2; i < MapMinWidth / 2; i++)
            {
                var x = cameraPosition.x + i;
                if (GetTerrain(x, y) == Terrain.Empty) SetTerrain(x, y, GenerateTerrain(x, y));
            }
            CameraPositionInTerrainCellsWhenLastGenerate = cameraPosition;
            cameraMoved = true;
        }


        // good enough for now, can be optimized
        if (cameraMoved)
        {
            SetUnSettedTiles();
        }

        cameraMoved = false;
    }

}
