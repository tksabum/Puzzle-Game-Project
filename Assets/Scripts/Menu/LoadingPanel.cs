using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    public MainManager mainManager;

    public Image loadingBar;


    // Start is called before the first frame update
    void Start()
    {
        loadingBar.fillAmount = 0f;
        CheckMapData();
    }

    void CheckMapData()
    {
        string dataPath;
        if (Application.platform == RuntimePlatform.Android)
        {
            dataPath = Application.persistentDataPath;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            dataPath = Application.persistentDataPath;
        }
        else
        {
            dataPath = Application.persistentDataPath;
        }

        StartCoroutine(DownLoadMapData(dataPath));
    }

    IEnumerator DownLoadMapData(string dataPath)
    {
        if (!Directory.Exists(dataPath + "/Mapdata"))
        {
            Directory.CreateDirectory(dataPath + "/Mapdata");
        }

        List<KeyValuePair<string, string>> mapList = new()
        { 
            new KeyValuePair<string, string>("Story 1-1", "https://github.com/tksabum/Mapdata/raw/main/Story%201-1.dat"),
            new KeyValuePair<string, string>("Story 1-1", "https://github.com/tksabum/Mapdata/raw/main/Story%201-1.dat"),
            new KeyValuePair<string, string>("Story 1-2", "https://github.com/tksabum/Mapdata/raw/main/Story%201-2.dat"),
            new KeyValuePair<string, string>("Story 1-3", "https://github.com/tksabum/Mapdata/raw/main/Story%201-3.dat"),
            new KeyValuePair<string, string>("Story 1-4", "https://github.com/tksabum/Mapdata/raw/main/Story%201-4.dat"),
            new KeyValuePair<string, string>("Story 1-5", "https://github.com/tksabum/Mapdata/raw/main/Story%201-5.dat"),
            new KeyValuePair<string, string>("Story 1-6", "https://github.com/tksabum/Mapdata/raw/main/Story%201-6.dat"),
            new KeyValuePair<string, string>("Story 1-7", "https://github.com/tksabum/Mapdata/raw/main/Story%201-7.dat"),
            new KeyValuePair<string, string>("Story 1-8", "https://github.com/tksabum/Mapdata/raw/main/Story%201-8.dat"),
            new KeyValuePair<string, string>("Story 1-9", "https://github.com/tksabum/Mapdata/raw/main/Story%201-9.dat"),
            new KeyValuePair<string, string>("Story 1-10", "https://github.com/tksabum/Mapdata/raw/main/Story%201-10.dat"),
            new KeyValuePair<string, string>("Story 2-1", "https://github.com/tksabum/Mapdata/raw/main/Story%202-1.dat"),
            new KeyValuePair<string, string>("Story 2-2", "https://github.com/tksabum/Mapdata/raw/main/Story%202-2.dat"),
            new KeyValuePair<string, string>("Story 2-3", "https://github.com/tksabum/Mapdata/raw/main/Story%202-3.dat"),
            new KeyValuePair<string, string>("Story 2-4", "https://github.com/tksabum/Mapdata/raw/main/Story%202-4.dat"),
            new KeyValuePair<string, string>("Story 2-5", "https://github.com/tksabum/Mapdata/raw/main/Story%202-5.dat"),
            new KeyValuePair<string, string>("Story 2-6", "https://github.com/tksabum/Mapdata/raw/main/Story%202-6.dat"),
            new KeyValuePair<string, string>("Story 2-7", "https://github.com/tksabum/Mapdata/raw/main/Story%202-7.dat"),
            new KeyValuePair<string, string>("Story 2-8", "https://github.com/tksabum/Mapdata/raw/main/Story%202-8.dat"),
            new KeyValuePair<string, string>("Story 2-9", "https://github.com/tksabum/Mapdata/raw/main/Story%202-9.dat"),
            new KeyValuePair<string, string>("Story 2-10", "https://github.com/tksabum/Mapdata/raw/main/Story%202-10.dat"),
            new KeyValuePair<string, string>("Story 3-1", "https://github.com/tksabum/Mapdata/raw/main/Story%203-1.dat"),
            new KeyValuePair<string, string>("Story 3-2", "https://github.com/tksabum/Mapdata/raw/main/Story%203-2.dat"),
            new KeyValuePair<string, string>("Story 3-3", "https://github.com/tksabum/Mapdata/raw/main/Story%203-3.dat"),
            new KeyValuePair<string, string>("Story 3-4", "https://github.com/tksabum/Mapdata/raw/main/Story%203-4.dat"),
            new KeyValuePair<string, string>("Story 3-5", "https://github.com/tksabum/Mapdata/raw/main/Story%203-5.dat"),
            new KeyValuePair<string, string>("Story 3-6", "https://github.com/tksabum/Mapdata/raw/main/Story%203-6.dat"),
            new KeyValuePair<string, string>("Story 3-7", "https://github.com/tksabum/Mapdata/raw/main/Story%203-7.dat"),
            new KeyValuePair<string, string>("Story 3-8", "https://github.com/tksabum/Mapdata/raw/main/Story%203-8.dat"),
            new KeyValuePair<string, string>("Story 3-9", "https://github.com/tksabum/Mapdata/raw/main/Story%203-9.dat"),
            new KeyValuePair<string, string>("Story 3-10", "https://github.com/tksabum/Mapdata/raw/main/Story%203-10.dat")
    };
        

        for (int i = 0; i < mapList.Count; i++)
        {
            bool existData = false;
            bool downloadCheck = false;
            if (File.Exists(dataPath + "/Mapdata/" + mapList[i].Key + ".dat"))
            {
                existData = true;
            }
            if (File.Exists(dataPath + "/Mapdata/" + mapList[i].Key + ".dlc"))
            {
                downloadCheck = true;
            }

            if (!existData || downloadCheck)
            {
                File.WriteAllText(dataPath + "/Mapdata/" + mapList[i].Key + ".dlc", "");

                UnityWebRequest request = UnityWebRequest.Get(mapList[i].Value);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                    i--;
                    continue;
                }
                else
                {
                    File.WriteAllBytes(dataPath + "/Mapdata/" + mapList[i].Key + ".dat", request.downloadHandler.data);
                    File.Delete(dataPath + "/Mapdata/" + mapList[i].Key + ".dlc");
                }
            }

            loadingBar.fillAmount = (float)i / (float)mapList.Count;

            yield return null;
        }

        mainManager.EndState((int)State.LOADING);
    }
}
