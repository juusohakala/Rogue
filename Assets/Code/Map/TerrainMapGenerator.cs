using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainMapGenerator : MonoBehaviour
{
    //[Header("AutoTileMapper:")]
    //public AutoTileMapper AutoTileMapper;

    [Header("TerrainMap:")]
    public TerrainMap TerrainMap;

    [Header("Generator settings:")]
    public double PerlinNoiseZoom;

    private Camera Camera;

    private Dictionary<(int, int), bool> Chunks;

    /// <summary>
    /// These values should be big enough to cover visible play area when camera is zoomed out to max.
    /// These could be calculated from camera dimensions somehow but I don't care for now.
    /// Even numbers only!
    /// </summary>
    private int MapMinWidth = 100;
    private int MapMinHeight = 100;

    private int ChunkWidth = 20;
    private int ChunkHeight = 20;

    private int ChunkCounter;

    private Vector3 CameraPositionInTerrainCellsWhenLastGenerate;


    private double CameraPositionWhenLastGenerateX;
    private double CameraPositionWhenLastGenerateY;


    TerrainMap.Terrain GenerateTerrain(int x, int y)
    {
        var pnx = (float)(((double)x / MapMinWidth + 0.5) * PerlinNoiseZoom);
        var pny = (float)(((double)y / MapMinHeight + 0.5) * PerlinNoiseZoom);

        var noiseValueOfThePoint = Mathf.PerlinNoise(pnx, pny);

        if (noiseValueOfThePoint < 0.3)
        {
            return TerrainMap.Terrain.Empty;
        }
        else if(noiseValueOfThePoint < 0.8)
        {
            return TerrainMap.Terrain.Blue;
        }
        else
        {
            return TerrainMap.Terrain.Red;
        }

    }

    void GenerateTerrainChunk(int chunkX, int chunkY)
    {
        ChunkCounter++;
        Chunks.Add((chunkX, chunkY), true);

        var firstX = -ChunkWidth / 2 - 1 + chunkX * ChunkWidth;
        var lastX = ChunkWidth / 2 + chunkX * ChunkWidth;

        var firstY = -ChunkHeight / 2 - 1 + chunkY * ChunkHeight;
        var lastY = ChunkHeight / 2 + chunkY * ChunkHeight;

        for (var x = firstX; x <= lastX; x++)
        {
            for (var y = firstY; y <= lastY; y++)
            {
                if (TerrainMap.GetTerrain(x, y) == null) TerrainMap.SetTerrain(x, y, GenerateTerrain(x, y));

                if (!(x == firstX || y == firstY || x == lastX || y == lastY))
                {
                    // not border
                    TerrainMap.AddReadyKey((x, y));
                }
            }
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        Camera = Camera.main;

        Chunks = new Dictionary<(int, int), bool>();
    }

    // Update is called once per frame
    void Update()
    {

        // these are measured in terrain cells that are twice bigger than cells/tiles

        var cameraPositionX = Camera.transform.position.x / TerrainMap.CellSize.x;
        var cameraPositionY = Camera.transform.position.y / TerrainMap.CellSize.y;

        var cameraPositionInChunksX = (int)Math.Round(cameraPositionX / ChunkWidth);
        var cameraPositionInChunksY = (int)Math.Round(cameraPositionY / ChunkHeight);

        // this isnt necessary to check in every update, but only when moved across chunk. But its fast enough for now :-) 

        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                if (!Chunks.ContainsKey((cameraPositionInChunksX + x, cameraPositionInChunksY + y)))
                {
                    GenerateTerrainChunk(cameraPositionInChunksX + x, cameraPositionInChunksY + y);
                }
            }
        }
    }
}
