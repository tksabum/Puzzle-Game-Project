using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : Floorbase
{
    public enum GeneratorType
    {
        POWER,
        TIME
    }


    public BlockManager.Obj product;
    public GeneratorType generatorType;
    public int cycle;

    public bool powerDefault;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        throw new System.NotImplementedException();
    }

    public override void OnObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        throw new System.NotImplementedException();
    }

    public override void PowerToggle(GameManager gameManager)
    {
        throw new System.NotImplementedException();
    }
}
