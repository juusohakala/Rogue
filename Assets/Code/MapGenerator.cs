using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    enum Terrain
    {
        Grass,
        Sand
    }



    void GetTile(double x, double y)
    {

    }

    [Header("Camera:")]
    public Camera Camera;

    [Header("Tilesets:")]
    public AutoTileset AutoTileset;

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
    private int MapMinWidth = 6;
    private int MapMinHeight = 6;





    private Vector3 CameraPositionInCellsWhenLastGenerate;


    private Dictionary<(int, int), Terrain> TerrainMap;


    /// <summary>
    /// Sets tile by it's terrain and surrounding terrains.
    /// </summary>
    void SetTile(int x, int y)
    {
        if (!TerrainMap.ContainsKey((x, y)))
        {
            Debug.LogWarning($"Tried to set tile but terrain not found {(x, y)}");
            return;
        }
        else
        {
            if (TerrainMap[(x, y)] == Terrain.Sand)
            {
                var tiles = AutoTileset.Tiles[AutoTileset.BitMask.Middle];
                var randomTile = tiles[Random.Range(0, tiles.Length)];
                Over.SetTile(new Vector3Int(x, y, 0), randomTile);
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

    // Start is called before the first frame update
    void Start()
    {
        TerrainMap = new Dictionary<(int, int), Terrain>();
        AutoTileset = Instantiate(AutoTileset);

        // Generate first map that just covers the camera
        CameraPositionInCellsWhenLastGenerate = Over.LocalToCell(Camera.transform.position);
        for (var x = -MapMinWidth / 2; x < MapMinWidth / 2; x++)
        {
            for (var y = -MapMinHeight / 2; y < MapMinHeight / 2; y++)
            {

                SetTerrain(x, y, GenerateTerrain(x, y));
                SetTile(x, y);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        var w = MapMinWidth / 2;
        var h = MapMinHeight / 2;


        // here could be something to optimize :-)

        var cameraPositionInCells = Over.LocalToCell(Camera.transform.position);
        var cameraMove = CameraPositionInCellsWhenLastGenerate - cameraPositionInCells;

        if (cameraMove.x >= 1)
        {
            var x = cameraPositionInCells.x - MapMinWidth / 2;
            for (var i = -MapMinHeight / 2; i < MapMinHeight / 2; i++)
            {
                var y = cameraPositionInCells.y + i;
                SetTerrain(x, y, GenerateTerrain(x, y));
                SetTile(x, y);
            }
            CameraPositionInCellsWhenLastGenerate = cameraPositionInCells;
        }

        if (cameraMove.x <= -1)
        {
            var x = cameraPositionInCells.x + MapMinWidth / 2 - 1;
            for (var i = -MapMinHeight / 2; i < MapMinHeight / 2; i++)
            {
                var y = cameraPositionInCells.y + i;
                SetTerrain(x, y, GenerateTerrain(x, y));
                SetTile(x, y);
            }
            CameraPositionInCellsWhenLastGenerate = cameraPositionInCells;
        }

        if (cameraMove.y >= 1)
        {
            var y = cameraPositionInCells.y - MapMinHeight / 2;
            for (var i = -MapMinWidth / 2; i < MapMinWidth / 2; i++)
            {
                var x = cameraPositionInCells.x + i;
                SetTerrain(x, y, GenerateTerrain(x, y));
                SetTile(x, y);
            }
            CameraPositionInCellsWhenLastGenerate = cameraPositionInCells;
        }

        if (cameraMove.y <= -1)
        {
            var y = cameraPositionInCells.y + MapMinHeight / 2 - 1;
            for (var i = -MapMinWidth / 2; i < MapMinWidth / 2; i++)
            {
                var x = cameraPositionInCells.x + i;
                SetTerrain(x, y, GenerateTerrain(x, y));
                SetTile(x, y);
            }
            CameraPositionInCellsWhenLastGenerate = cameraPositionInCells;
        }

    }
}
