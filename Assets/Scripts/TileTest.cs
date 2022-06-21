using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTest : MonoBehaviour
{
    Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        BoundsInt size = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(size);

        Vector3Int v = new Vector3Int(-7, 3, 0);
        TileBase t = tilemap.GetTile(v);

        tilemap.SetTile(new Vector3Int(-8, 3, 0), t);
        tilemap.RefreshAllTiles();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
