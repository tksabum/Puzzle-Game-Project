using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Itembase : MonoBehaviour
{
    public BlockManager.Obj obj;
    public bool getable;
    public bool breakable;
    public bool pushable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �÷��̾ �ö���� ����
    public abstract void OnPrePlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx);
    public abstract void OnPlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx);
    public abstract void OnPostPlayerEnter(GameManager gameManager, BlockManager blockManager, Vector2Int enteridx, Vector2Int itemidx);

    // �÷��̾ ������ ����
    public abstract void OnPlayerExit();
}
