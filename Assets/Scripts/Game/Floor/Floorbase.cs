using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloorType
{
    PLANE,
    WATER,
    BUTTON,
    TRAP,
    PORTAL
}

public abstract class Floorbase : MonoBehaviour
{
    [Header("- EnterObject -")]
    public bool playerEnterable;
    public bool woodenboxEnterable;

    [Header("- ExitObject -")]
    public bool playerExitable;
    public bool woodenboxExitable;

    [Header("- EnterDirection -")]
    public bool leftEnterable;
    public bool rightEnterable;
    public bool upEnterable;
    public bool downEnterable;

    [Header("- ExitDirection -")]
    public bool leftExitable;
    public bool rightExitable;
    public bool upExitable;
    public bool downExitable;

    [Header(" - ETC -")]
    public FloorType floorType;

    protected Vector2Int idx;
    protected bool power;

    private void Awake()
    {
        power = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIdx(Vector2Int _idx)
    {
        idx = _idx;
    }

    // 들어갈 수 있는지 판단
    public bool Enterable(BlockManager.Obj obj, BlockManager.Direction dir)
    {

        if (obj == BlockManager.Obj.PLAYER)
        {
            if (!playerEnterable) return false;
        }
        else if (obj == BlockManager.Obj.WOODENBOX)
        {
            if (!woodenboxEnterable) return false;
        }
        else
        {
            throw new System.Exception("적절한 오브젝트 찾지 못함");
        }

        if (dir == BlockManager.Direction.LEFT) return leftEnterable;
        else if (dir == BlockManager.Direction.RIGHT) return rightEnterable;
        else if (dir == BlockManager.Direction.UP) return upEnterable;
        else if (dir == BlockManager.Direction.DOWN) return downEnterable;
        else
        {
            throw new System.Exception("적절한 방향 찾지 못함");
        }
    }

    // 나갈 수 있는지 판단
    public bool Exitable(BlockManager.Obj obj, BlockManager.Direction dir)
    {
        if (obj == BlockManager.Obj.PLAYER)
        {
            if (!playerExitable) return false;
        }
        else if (obj == BlockManager.Obj.WOODENBOX)
        {
            if (!woodenboxExitable) return false;
        }
        else
        {
            throw new System.Exception("적절한 오브젝트 찾지 못함");
        }

        if (dir == BlockManager.Direction.LEFT) return leftExitable;
        else if (dir == BlockManager.Direction.RIGHT) return rightExitable;
        else if (dir == BlockManager.Direction.UP) return upExitable;
        else if (dir == BlockManager.Direction.DOWN) return downExitable;
        else
        {
            throw new System.Exception("적절한 방향 찾지 못함");
        }
    }

    // 오브젝트가 올라오면 실행
    public abstract void OnObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj);

    // 오브젝트가 나가면 실행
    public abstract void OnObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj);

    public abstract void PowerToggle();
}
