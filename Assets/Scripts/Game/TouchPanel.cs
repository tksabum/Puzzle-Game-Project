using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool Touch
    {
        get
        {
            if (touch)
            {
                touch = false;
                return true;
            }

            return touch; 
        }
        set { touch = value; }
    }

    private bool touch = false;

    void Awake()
    {
        touch = false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        touch = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        touch= false;
    }
}
