using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum State {
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

    public List<AudioSource> audioSources;
    public List<AudioClip> audioClips;
    
    const int EMPTY = -1;

    int state = EMPTY;

    // 저장 필요
    Vector2Int clearInfo;

    // Start is called before the first frame update
    void Start()
    {
        clearInfo = new Vector2Int(PlayerPrefs.GetInt("clearInfo_storyNum", 0), PlayerPrefs.GetInt("clearInfo_mapNum", 10));

        Refresh((int)DataBus.Instance.ReadStartState());
    }

    // Update is called once per frame
    void Update()
    {
        if (state == (int)State.TITLE)
        {
            UpdateTitle();
        }
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
            bgmSource.Play();
        }
        else if (bgmClips[state] != bgmClips[nextstate])
        {
            bgmSource.Stop();
            bgmSource.clip = bgmClips[nextstate];
            bgmSource.Play();
        }
        
        state = nextstate;
    }
}
