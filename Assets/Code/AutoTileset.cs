using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AutoTileset : MonoBehaviour
{

    [field: SerializeField] public Sprite[] Middle { get; set; }

    [field: Space(10)]
    [field: SerializeField] public Sprite[] Top { get; set; }
    [field: SerializeField] public Sprite[] Right { get; set; }
    [field: SerializeField] public Sprite[] Bottom { get; set; }
    [field: SerializeField] public Sprite[] Left { get; set; }

    [field: Space(10)]
    [field: SerializeField] public Sprite[] TopRight { get; set; }
    [field: SerializeField] public Sprite[] BottomRight { get; set; }
    [field: SerializeField] public Sprite[] BottomLeft { get; set; }
    [field: SerializeField] public Sprite[] TopLeft { get; set; }

    [field: Space(10)]
    [field: SerializeField] public Sprite[] InsideTopRight { get; set; }
    [field: SerializeField] public Sprite[] InsideBottomRight { get; set; }
    [field: SerializeField] public Sprite[] InsideBottomLeft { get; set; }
    [field: SerializeField] public Sprite[] InsideTopLeft { get; set; }


    public enum BitMask
    {
        Middle,

        Top,
        Right,
        Bottom,
        Left,

        TopRight,
        BottomRight,
        BottomLeft,
        TopLeft,

        InsideTopRight,
        InsideBottomRight,
        InsideBottomLeft,
        InsideTopLeft
    }

    public Dictionary<BitMask, Tile[]> Tiles { get; set; } 

    void Awake()
    {
        Tiles = new Dictionary<BitMask, Tile[]>();

        foreach (int e in Enum.GetValues(typeof(BitMask)))
        {

            //var propName = ((BitMask)e).ToString();

            //var type = this.GetType();
            //var prop = this.GetType().GetProperty(((BitMask)e).ToString());
            //var propValue = this.GetType().GetProperty(((BitMask)e).ToString()).GetValue(this);


            var sprites = (Array)this.GetType().GetProperty(((BitMask)e).ToString()).GetValue(this);

            Tiles.Add((BitMask)e, new Tile[sprites.Length]);
            for (var i = 0; i < sprites.Length; i++)
            {
                Tiles[(BitMask)e][i] = ScriptableObject.CreateInstance<Tile>();
                Tiles[(BitMask)e][i].sprite = (Sprite)sprites.GetValue(i);
            }
        }
    }

    void Update()
    {

    }
}


