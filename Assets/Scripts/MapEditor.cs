using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PairInt
{
    public int x;
    public int y;

    public PairInt(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}

/*

[System.Serializable]
public class MapData
{
    public string appVersion;
    public string mapDesigner;
    public int mapWidth;
    public int mapHeight;
    public List<List<string>> floorData;
    public List<List<string>> itemData;
    public PairInt startidx;
    public int maxlife;
    public Dictionary<PairInt, List<PairInt>> powerData;
    public Dictionary<PairInt, PairInt> portalData;
}

 */

[System.Serializable]
public class MapData
{
    public string appVersion;
    public string mapDesigner;
    public PairInt startIdx;
    public int startLife;
    public int maxLife;
    public int mapWidth;
    public int mapHeight;
    public List<List<string>> floorData;
    public List<List<string>> itemData;
    public Dictionary<PairInt, List<PairInt>> powerData;
    public Dictionary<PairInt, PairInt> portalData;
}

public class MapEditor : MonoBehaviour
{
    [Header("- TileMap -")]
    public Tilemap floorTilemap;
    public Tilemap itemTilemap;

    [Header("- Player -")]
    public Vector2Int startidx;
    [Range(1, 5)]
    public int startlife;
    [Range(1, 5)]
    public int maxlife;

    [Header("- Power -")]
    public PowerDic powerDic;

    [Header("- Portal -")]
    public PortalDic portalDic;

    [Header("- File -")]
    public string fileName;

    string dataPath;

    // Start is called before the first frame update
    void Start()
    {
        dataPath = "Assets/MapData/" + fileName + ".dat";
        Save();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save()
    {
        if (fileName != "Test" && File.Exists(dataPath))
        {
            throw new System.Exception("이미 존재함");
        }

        // MapData 객체생성
        int mapwidth = 0;
        int mapheight = 0;

        while (floorTilemap.GetTile(new Vector3Int(mapwidth, 0, 0)) != null)
        {
            mapwidth += 1;
        }

        while (floorTilemap.GetTile(new Vector3Int(0, mapheight, 0)) != null)
        {
            mapheight += 1;
        }

        MapData mapData = new MapData();
        mapData.appVersion = Application.version;
        mapData.mapDesigner = "BeomJoon";
        mapData.startIdx = new PairInt(startidx.x, startidx.y);
        mapData.startLife = startlife;
        mapData.maxLife = maxlife;
        mapData.mapWidth = mapwidth;
        mapData.mapHeight = mapheight;

        mapData.floorData = new List<List<string>>();
        for (int i = 0; i < mapwidth; i++)
        {
            mapData.floorData.Add(new List<string>());
            for (int j = 0; j < mapheight; j++)
            {
                mapData.floorData[i].Add(floorTilemap.GetTile(new Vector3Int(i, j, 0)).name);
            }
        }

        mapData.itemData = new List<List<string>>();
        for (int i = 0; i < mapwidth; i++)
        {
            mapData.itemData.Add(new List<string>());
            for (int j = 0; j < mapheight; j++)
            {
                TileBase tile = itemTilemap.GetTile(new Vector3Int(i, j, 0));
                string tileName = "";
                if (tile != null) tileName = tile.name;
                mapData.itemData[i].Add(tileName);
            }
        }

        mapData.powerData = new Dictionary<PairInt, List<PairInt>>();
        foreach(KeyValuePair<Vector2Int, PowerList> pair in powerDic)
        {
            PairInt key = new PairInt(pair.Key.x, pair.Key.y);
            List<PairInt> value = new List<PairInt>();
            foreach(Vector2Int v in pair.Value.GetList())
            {
                value.Add(new PairInt(v.x, v.y));
            }
            mapData.powerData.Add(key, value);
        }

        mapData.portalData = new Dictionary<PairInt, PairInt>();
        foreach(KeyValuePair<Vector2Int, Vector2Int> pair in portalDic)
        {
            PairInt key = new PairInt(pair.Key.x, pair.Key.y);
            PairInt value = new PairInt(pair.Value.x, pair.Value.y);
            mapData.portalData.Add(key, value);
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(dataPath);

        binaryFormatter.Serialize(file, mapData);
        file.Close();
        
        Debug.Log("저장완료");
    }

    public MapData Load()
    {
        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            MapData mapData = (MapData)binaryFormatter.Deserialize(file);
            file.Close();

            return mapData;
        }
        else
        {
            throw new System.Exception("데이터 불러오기 실패");
        }
    }
}
