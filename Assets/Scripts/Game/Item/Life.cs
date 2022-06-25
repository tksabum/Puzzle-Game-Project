using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : Itembase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPlayerEnter(GameManager gameManager)
    {
        gameManager.GetItem(BlockManager.Obj.LIFE);
    }

    public override void OnPlayerExit()
    {

    }
}
