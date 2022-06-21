using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFloor : Floorbase
{
    [Header("- Sprite -")]
    public Sprite spritePressed;
    public Sprite spriteReleased;

    SpriteRenderer spriteRenderer;

    bool isPressed = false;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        if (obj == BlockManager.Obj.PLAYER || obj == BlockManager.Obj.WOODENBOX)
        {
            blockManager.PowerToggle(idx);
            isPressed = true;
            RefreshSprite();
        }
    }

    public override void OnObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        if (obj == BlockManager.Obj.PLAYER || obj == BlockManager.Obj.WOODENBOX)
        {
            blockManager.PowerToggle(idx);
            isPressed = false;
            RefreshSprite();
        }
    }

    public override void PowerToggle()
    {
        
    }

    void RefreshSprite()
    {
        if (isPressed)
        {
            spriteRenderer.sprite = spritePressed;
        }
        else
        {
            spriteRenderer.sprite = spriteReleased;
        }
    }

    private void OnDisable()
    {
        isPressed = false;
        RefreshSprite();
    }
}
