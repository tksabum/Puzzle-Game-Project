using System.Collections;
using System.Collections.Generic;
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

    [Header("- Loading UI -")]
    
    
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

    public void EndState(int nowstate)
    {
        if (nowstate == (int)State.LOADING)
        {
            Refresh((int)State.TITLE);
        }
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
}
