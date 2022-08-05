using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sturdybox : Itembase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPrePlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx)
    {
        Vector2Int move = itemidx - enteridx;

        blockManager.MoveItem(this, itemidx, itemidx + move);
    }

    public override void OnPlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx)
    {

    }

    public override void OnPostPlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx)
    {

    }

    public override void OnPlayerExit()
    {

    }
}
