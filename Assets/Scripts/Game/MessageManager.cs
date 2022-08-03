using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class MessageManager : MonoBehaviour
{
    [Header("- Core -")]
    public GameManager gameManager;

    [Header("- UI -")]
    public Transform content;
    public Text textPage;
    public Toggle toggleIgnore;

    [Header("- Manual -")]
    public List<GameObject> firstManual;
    public List<GameObject> secondManual;
    public List<GameObject> thirdManual;

    [Header("- Story -")]
    public List<GameObject> firstStory;
    public List<GameObject> secondStory;
    public List<GameObject> thirdStory;


    List<List<GameObject>> manualList;
    List<List<GameObject>> storyList;

    Message lastMessage;
    Vector2Int lastMapNum;
    GameObject currentMessage;
    int currentPage;
    int pageCount;

    enum Message
    {
        MANUAL,
        STORY
    }

    public void Init()
    {
        if (manualList == null)
        {
            manualList = new List<List<GameObject>>();
            manualList.Add(firstManual);
            manualList.Add(secondManual);
            manualList.Add(thirdManual);
        }
        
        if (storyList == null)
        {
            storyList = new List<List<GameObject>>();
            storyList.Add(firstStory);
            storyList.Add(secondStory);
            storyList.Add(thirdStory);
        }

        int intIgnore = PlayerPrefs.GetInt("DontShowMessage", 0);
        if (intIgnore != 0)
        {
            toggleIgnore.isOn = true;
        }
        else
        {
            toggleIgnore.isOn = false;
        }
    }

    public void ShowMessage(string mapName)
    {
        Vector2Int mapNum = GetMapNum(mapName);

        if (mapNum.x == -1 || mapNum.y == -1)
        {
            return;
        }

        bool existStory = ShowStory(mapNum);
        if (!existStory)
        {
            ShowManual(mapNum);
        }
    }

    public void CloseMessage()
    {
        Destroy(currentMessage);
        currentMessage = null;

        string key = "Ignore_";
        if (lastMessage == Message.STORY)
        {
            key += "Story_";
        }
        else
        {
            key += "Manual_";
        }

        key += lastMapNum.x + "-" + lastMapNum.y;

        int value;
        if (toggleIgnore.isOn) value = 1;
        else value = 0;

        PlayerPrefs.SetInt(key, value);

        if (lastMessage == Message.STORY)
        {
            ShowManual(lastMapNum);
        }
        else
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }

    public void ButtonPrev()
    {
        int nextPage = Mathf.Clamp(currentPage - 1, 0, pageCount - 1);
        if (currentPage != nextPage)
        {
            currentMessage.transform.GetChild(currentPage).gameObject.SetActive(false);
            currentMessage.transform.GetChild(nextPage).gameObject.SetActive(true);
            currentPage = nextPage;
            textPage.text = (currentPage + 1) + " / " + pageCount;
        }
    }

    public void ButtonNext()
    {
        int nextPage = Mathf.Clamp(currentPage + 1, 0, pageCount - 1);
        if (currentPage != nextPage)
        {
            currentMessage.transform.GetChild(currentPage).gameObject.SetActive(false);
            currentMessage.transform.GetChild(nextPage).gameObject.SetActive(true);
            currentPage = nextPage;
            textPage.text = (currentPage + 1) + " / " + pageCount;
        }
    }

    public void ButtonClose()
    {
        CloseMessage();
    }

    public void ChangeToggleIgnore()
    {
        if (toggleIgnore.isOn)
        {
            PlayerPrefs.SetInt("DontShowMessage", 1);
        }
        else
        {
            PlayerPrefs.SetInt("DontShowMessage", 0);
        }
    }

    Vector2Int GetMapNum(string mapName)
    {
        if (mapName.Contains("Story "))
        {
            string str = mapName.Substring(mapName.IndexOf("Story ") + 6, mapName.Length - 6 - mapName.IndexOf("Story "));

            string strStoryNum = str.Substring(0, str.IndexOf('-'));
            string strMapNum = str.Substring(str.IndexOf('-') + 1);

            bool checkStoryNum = strStoryNum != "" && strStoryNum.All(char.IsDigit);
            bool checkMapNum = strMapNum != "" && strMapNum.All(char.IsDigit);

            if (checkStoryNum && checkMapNum)
            {
                int storyNum = int.Parse(strStoryNum);
                int mapNum = int.Parse(strMapNum);

                if (1 <= storyNum && 1 <= mapNum && mapNum <= 10)
                {
                    return new Vector2Int(storyNum - 1, mapNum - 1);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    bool ShowStory(Vector2Int mapNum)
    {
        if (mapNum.x >= storyList.Count)
        {
            throw new System.Exception("Error:선택된 스토리가 StoryList에 포함되지 않음 Story " + (mapNum.x + 1) + "-" + (mapNum.y + 1));
        }
        GameObject story = storyList[mapNum.x][mapNum.y];

        string key = "Ignore_Story_" + mapNum.x + "-" + mapNum.y;

        if (story != null && PlayerPrefs.GetInt(key, 0) != 1)
        {
            SetContent(story);
            lastMapNum = mapNum;
            lastMessage = Message.STORY;
            return true;
        }
        
        return false;
    }

    bool ShowManual(Vector2Int mapNum)
    {
        if (mapNum.x >= manualList.Count)
        {
            throw new System.Exception("Error:선택된 스토리가 ManualList에 포함되지 않음 Story " + (mapNum.x + 1) + "-" + (mapNum.y + 1));
        }
        GameObject manual = manualList[mapNum.x][mapNum.y];

        string key = "Ignore_Manual_" + mapNum.x + "-" + mapNum.y;

        if (manual != null && PlayerPrefs.GetInt(key, 0) != 1)
        {
            SetContent(manual);
            lastMapNum = mapNum;
            lastMessage = Message.MANUAL;
            return true;
        }

        return false;
    }

    void SetContent(GameObject message)
    {
        currentMessage = Instantiate(message, content.transform.position, Quaternion.identity, content);

        currentPage = 0;
        pageCount = currentMessage.transform.childCount;

        currentMessage.transform.GetChild(currentPage).gameObject.SetActive(true);

        textPage.text = (currentPage + 1) + " / " + pageCount;

        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }
}
