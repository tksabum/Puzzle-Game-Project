using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum State {
    LOADING,
    TITLE,
    MENU,
    STORYMODE,
    CUSTOMGAME,
    OPTION,
    FIRSTSTORY,
    SECONDSTORY = FIRSTSTORY + 1,
    THIRDSTORY = FIRSTSTORY + 2,
    OPTION_KEY,
    OPTION_SOUND
};

public class MainManager : MonoBehaviour
{
    [Header("- Core -")]
    public List<GameObject> canvas;

    [Header("- Audio -")]
    public AudioSource bgmSource;
    public List<AudioClip> bgmClips;

    public GameObject audioSource;
    public AudioClip audioClipMenuButton;
    
    const int EMPTY = -1;

    int state = EMPTY;

    Queue<AudioSource> waitingAudioSource;

    // 저장 필요
    Vector2Int clearInfo;

    private void Awake()
    {
        waitingAudioSource = new Queue<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 클리어정보 가져오기
        clearInfo = new Vector2Int(PlayerPrefs.GetInt("clearInfo_storyNum", 0), PlayerPrefs.GetInt("clearInfo_mapNum", 10));

        Refresh((int)DataBus.Instance.ReadStartState());
    }

    void StartLoading()
    {
        CheckMapData();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == (int)State.LOADING)
        {
            UpdateLoading();
        }
        else if (state == (int)State.TITLE)
        {
            UpdateTitle();
        }
    }

    void UpdateLoading()
    {

    }

    void UpdateTitle()
    {
        if (Input.anyKeyDown)
        {
            Refresh((int)State.MENU);
        }
    }

    public void ButtonEvent(StateSelectComponent nextstate)
    {
        Refresh((int)nextstate.state);
    }

    public void ButtonStartGame(string mapName)
    {
        DataBus.Instance.WriteMapName(mapName);
        SceneManager.LoadScene("GameScene");
    }

    public void ButtonQuitGame()
    {
        // 에디터에서는 플레이 종료
        // 앱에서는 앱 종료
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        #endif
        Application.Quit();
    }

    void Refresh(int nextstate)
    {
        // canvas
        if (state != EMPTY)
        {
            canvas[state].SetActive(false);
        }

        canvas[nextstate].SetActive(true);

        if (nextstate == (int)State.FIRSTSTORY || nextstate == (int)State.SECONDSTORY || nextstate == (int)State.THIRDSTORY)
        {
            int chapter = nextstate - (int)State.FIRSTSTORY + 1;

            int locknum;
            if (chapter < clearInfo.x)
            {
                locknum = 10;
            }
            else if (chapter == clearInfo.x)
            {
                locknum = Mathf.Min(10, clearInfo.y + 1);
            }
            else
            {
                if (chapter - 1 == clearInfo.x && clearInfo.y == 10)
                {
                    locknum = 1;
                }
                else
                {
                    locknum = 0;
                }
            }

            canvas[nextstate].GetComponent<StoryPanel>().SetButton(chapter, locknum);
        }

        // audio
        if (state == EMPTY)
        {
            bgmSource.clip = bgmClips[nextstate];
            SetBGMVolume(bgmSource);
            bgmSource.Play();
        }
        else if (bgmClips[state] != bgmClips[nextstate])
        {
            bgmSource.Stop();
            bgmSource.clip = bgmClips[nextstate];
            SetBGMVolume(bgmSource);
            bgmSource.Play();
        }
        
        state = nextstate;

        if (nextstate == (int)State.LOADING)
        {
            StartLoading();
        }
    }

    public void PlayMenuButtonAudio()
    {
        PlayAudio(audioClipMenuButton);
    }

    void PlayAudio(AudioClip audio)
    {
        if (waitingAudioSource.Count == 0)
        {
            AudioSource source = Instantiate(audioSource, Vector3.zero, Quaternion.identity, transform).GetComponent<AudioSource>();
            waitingAudioSource.Enqueue(source);
        }

        AudioSource audiosource = waitingAudioSource.Dequeue();
        audiosource.clip = audio;
        SetUIVolume(audiosource);
        audiosource.Play();

        StartCoroutine(WaitForSound(audiosource));
    }

    IEnumerator WaitForSound(AudioSource audiosource)
    {
        yield return new WaitUntil(() => audiosource.isPlaying == false);

        waitingAudioSource.Enqueue(audiosource);
    }

    void SetBGMVolume(AudioSource audiosource)
    {
        audiosource.volume = 0.1f * PlayerPrefs.GetInt("SoundSettingBGM", 5);
    }

    void SetUIVolume(AudioSource audiosource)
    {
        audiosource.volume = 0.1f * PlayerPrefs.GetInt("SoundSettingUI", 5);
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

        if (Directory.Exists(dataPath + "/Mapdata"))
        {
            Refresh((int)State.TITLE);
        }
        else
        {
            StartCoroutine(DownLoadMapData(dataPath));
        }
    }

    IEnumerator DownLoadMapData(string dataPath)
    {
        Directory.CreateDirectory(dataPath + "/Mapdata");

        List<string> URLs = new List<string>();
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%201-1.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%201-2.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%201-3.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%201-4.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%201-5.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%201-6.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%201-7.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%201-8.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%201-9.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%201-10.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%202-1.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%202-2.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%202-3.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%202-4.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%202-5.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%202-6.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%202-7.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%202-8.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%202-9.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%202-10.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%203-1.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%203-2.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%203-3.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%203-4.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%203-5.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%203-6.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%203-7.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%203-8.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%203-9.dat");
        URLs.Add("https://github.com/tksabum/Mapdata/raw/main/Story%203-10.dat");

        for (int i = 0; i < URLs.Count; i++)
        {
            UnityWebRequest request = UnityWebRequest.Get(URLs[i]);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Directory.Delete(dataPath + "/Mapdata");
                Debug.Log(request.error);
            }
            else
            {
                int storyNum = (i / 10) + 1;
                int mapNum = (i % 10) + 1;
                File.WriteAllBytes(dataPath + "/Mapdata/Story " + storyNum + "-" + mapNum + ".dat", request.downloadHandler.data);
            }
        }
        
        Refresh((int)State.TITLE);
    }
}
