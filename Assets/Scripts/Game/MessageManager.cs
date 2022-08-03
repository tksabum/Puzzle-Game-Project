using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class MessageManager : MonoBehaviour
{
    [Header("- UI -")]
    public Transform content;

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
        manualList = new List<List<GameObject>>();
        manualList.Add(firstManual);
        manualList.Add(secondManual);
        manualList.Add(thirdManual);

        storyList = new List<List<GameObject>>();
        storyList.Add(firstStory);
        storyList.Add(secondStory);
        storyList.Add(thirdStory);
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

    }

    public void ButtonPrev()
    {

    }

    public void ButtonNext()
    {

    }

    public void ButtonClose()
    {
        CloseMessage();
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

        if (story != null)
        {
            SetContent(story);
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

        if (manual != null)
        {
            SetContent(manual);
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

        gameObject.SetActive(true);
    }
}
