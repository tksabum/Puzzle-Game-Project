using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryPanel : MonoBehaviour
{
    public List<Button> buttons;

    public void SetButton(int chapter, int lockNumber)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < lockNumber)
            {
                buttons[i].transform.Find("Text").GetComponent<Text>().text = chapter + "-" + (i + 1);
                buttons[i].interactable = true;
            }
            else
            {
                buttons[i].transform.Find("Text").GetComponent<Text>().text = "LOCK";
                buttons[i].interactable = false;
            }
        }
    }
}
