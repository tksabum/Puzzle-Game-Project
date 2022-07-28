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

    [Header("- Item Generator -")]
    public bool powerDefault;
    public int generationCycle;

    BlockManager.Obj product;
    GeneratorType generatorType;

    bool isObjectEntered;

    private new void Awake()
    {
        base.Awake();

        isObjectEntered = false;
        power = false;
    }

    // ItemGenerator���� �����Ǵ� ������ ����
    public void SetProduct(BlockManager.Obj obj)
    {
        product = obj;
    }

    // ������ ������� ����
    public void SetGeneratorType(GeneratorType type)
    {
        generatorType = type;
    }

    // ���ӽ��۽� �÷��̾�, �������� ���� �ִٸ� ����Ǿ� ��
    // �÷��̾� �̵��� PreMoveEvent���� ����Ǿ� ��
    public override void OnObjectEnter(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        isObjectEntered = true;
    }

    public override void OnObjectExit(GameManager gameManager, BlockManager blockManager, BlockManager.Obj obj)
    {
        isObjectEntered = false;
    }

    public override void PowerToggle(GameManager gameManager)
    {
        power = !power;
        
        // ������ ���� �� ���� ������ �������� ���� ���
        if (power && !isObjectEntered)
        {
            GenerateItem(gameManager.blockManager);
        }
    }

    public void GenerateItem(BlockManager blockManager)
    {

    }

    public void Tick(BlockManager blockManager, int tickCount)
    {
        if (tickCount % generationCycle == 0 && !isObjectEntered)
        {
            GenerateItem(blockManager);
        }
    }
}
