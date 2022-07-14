using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainMap : MonoBehaviour
{
    public enum Terrain
    {
        Empty,
        Blue,
        Red
    }

    [Header("Cell size has to be tilemap cell size * 2")]
    public Vector3 CellSize;

    private Dictionary<(int, int), Terrain> Data;
    private List<(int, int)> ReadyKeys;

    public Terrain? GetTerrain(int x, int y)
    {

        if (Data.ContainsKey((x, y)))
        {
            return Data[(x, y)];
        }

        return null;
    }

    public (int, int)? TakeReadyKey()
    {
        if (ReadyKeys.Count == 0) return null;

        var key = ReadyKeys.Last();
        ReadyKeys.Remove(key);
        return key;
    }

    public void AddReadyKey((int, int) key)
    {
        ReadyKeys.Add(key);
    }


    /// <summary>
    /// Sets a terrain.
    /// </summary>
    public void SetTerrain(int x, int y, Terrain terrain)
    {

        if (Data.ContainsKey((x, y)))
        {
            Debug.LogWarning($"DataMap already has a key {(x, y)}");
            return;
        }
        else
        {
            Data.Add((x, y), terrain);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        Data = new Dictionary<(int, int), Terrain>();
        ReadyKeys = new List<(int, int)>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
