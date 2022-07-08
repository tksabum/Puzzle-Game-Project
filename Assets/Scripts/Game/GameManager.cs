using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("- UI -")]
    public UIManager uiManager;
    public Button pauseButton;

    [Header("- Block -")]
    public BlockManager blockManager;

    [Header("- Player - ")]
    public GameObject player;
    public Animator playerAnimator;
    public float playerSpeed;

    bool playerWalk;
    BlockManager.Direction playerDirection;
    Vector2Int walkStartPos;
    Vector2Int walkEndPos;
    
    Vector2Int startidx;
    int startlife;
    int maxlife;

    Vector2Int playeridx;
    int life;

    BlockManager.Obj preparedItem;

    [Header(" - Camera - ")]
    public Camera mainCamera;
    public float zoomSpeed;
    public float minCameraSize;
    public float maxCameraSize;
    [Tooltip("Zoom In 상태에서 플레이어가 구석에 있을 때 보이는 맵 밖의 여백")]
    public float padding;

    [Header(" - File - ")]
    public string testMapName;

    string mapName;
    string dataPath;

    MapData mapData;

    // Start is called before the first frame update
    void Start()
    {
        mapName = DataBus.Instance.ReadMapName();

        if (mapName == null)
        {
            mapName = testMapName;
        }

        dataPath = Application.dataPath + "/MapData/" + mapName + ".dat";

        mapData = Load();

        startidx = new Vector2Int(mapData.startIdx.x, mapData.startIdx.y);
        startlife = mapData.startLife;
        maxlife = mapData.maxLife;

        Init();

        blockManager.SetBlock(mapData);

        StartCoroutine(CoroutineMove());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        // 아이템 사용
        if (!playerWalk && preparedItem != BlockManager.Obj.EMPTY && Input.GetKey(InputManager.Instance.keyDic[CustomKeyCode.USE_ITEM]))
        {
            if (preparedItem == BlockManager.Obj.HAMMER)
            {
                if (blockManager.ItemUsable(preparedItem, playeridx, playerDirection))
                {
                    blockManager.ItemUseEvent(preparedItem, playeridx, playerDirection);
                    preparedItem = BlockManager.Obj.EMPTY;
                    uiManager.SetItem(preparedItem);
                }
            }
        }

        // 카메라 줌
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = mainCamera.aspect * mainCamera.orthographicSize;

        float horizMin = -0.5f + cameraWidth - padding;
        float horizMax = (float)mapData.mapWidth - 0.5f - cameraWidth + padding;
        float verMin = -0.5f + cameraHeight - padding;
        float verMax = (float)mapData.mapHeight - 0.5f - cameraHeight + padding;

        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        float maxCameraSizeHeight = ((float)mapData.mapHeight + 2 * padding) / 2f;
        float maxCameraSizeWidth = ((float)mapData.mapWidth + 2 * padding) / (2f * mainCamera.aspect);
        float maxCameraSizeTotal = Mathf.Clamp(Mathf.Max(maxCameraSizeHeight, maxCameraSizeWidth), minCameraSize, maxCameraSize);

        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - scroll, minCameraSize, maxCameraSizeTotal);
        
        /* 카메라 크기 조정에 필요
        if(scroll != 0f)
        {
            Debug.Log(mainCamera.orthographicSize);
        }
        /* */

        // 카메라 이동
        Vector3 targetPos = player.transform.position;
        Vector3 centerPos = new Vector3(((float)mapData.mapWidth - 1f) / 2f, ((float)mapData.mapHeight - 1f) / 2f, 0);
        if (horizMin < horizMax)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, horizMin, horizMax);
        }
        else
        {
            targetPos.x = centerPos.x;
        }
        if (verMin < verMax)
        {
            targetPos.y = Mathf.Clamp(targetPos.y, verMin, verMax);
        }
        else
        {
            targetPos.y = centerPos.y;
        }
        targetPos.z = mainCamera.transform.position.z;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, 0.1f);
    }

    private void Init()
    {
        // 변수 초기화
        playeridx = startidx;
        life = startlife;
        preparedItem = BlockManager.Obj.EMPTY;

        // 플레이어 초기화
        player.transform.position = (Vector2)playeridx;
        playerDirection = BlockManager.Direction.DOWN;
        playerWalk = false;

        //애니메이터 초기화
        playerAnimator.SetBool("walking", playerWalk);
        playerAnimator.SetFloat("DirX", 0f);
        playerAnimator.SetFloat("DirY", -1f);

        // UI 초기화
        pauseButton.interactable = true;
        uiManager.Reset(life, mapName, preparedItem);
    }

    IEnumerator CoroutineMove()
    {
        playerWalk = false;

        while (true)
        {
            
            if (Time.timeScale == 0f)
            {
                yield return null;
            }

            if (!playerWalk)
            {
                Vector2Int moveDir = Vector2Int.zero;

                if (Input.GetKey(InputManager.Instance.keyDic[CustomKeyCode.RIGHT]))
                {
                    moveDir = Vector2Int.right;
                    playerDirection = BlockManager.Direction.RIGHT;
                }
                else if (Input.GetKey(InputManager.Instance.keyDic[CustomKeyCode.LEFT]))
                {
                    moveDir = Vector2Int.left;
                    playerDirection = BlockManager.Direction.LEFT;
                }
                else if (Input.GetKey(InputManager.Instance.keyDic[CustomKeyCode.UP]))
                {
                    moveDir = Vector2Int.up;
                    playerDirection = BlockManager.Direction.UP;
                }
                else if (Input.GetKey(InputManager.Instance.keyDic[CustomKeyCode.DOWN]))
                {
                    moveDir = Vector2Int.down;
                    playerDirection = BlockManager.Direction.DOWN;
                }
                else
                {
                    playerAnimator.SetBool("walking", playerWalk);
                    yield return null;
                    continue;
                }

                playerAnimator.SetFloat("DirX", (float)moveDir.x);
                playerAnimator.SetFloat("DirY", (float)moveDir.y);


                Vector2Int nowidx = playeridx;
                Vector2Int nextidx = playeridx + moveDir;

                // 갈 수 있는 위치인지 체크
                if (blockManager.Movable(BlockManager.Obj.PLAYER, nowidx, nextidx))
                {
                    // 갈 수 있다면 이동
                    playeridx = nextidx;
                    walkStartPos = nowidx;
                    walkEndPos = nextidx;

                    playerWalk = true;
                    playerAnimator.SetBool("walking", playerWalk);

                    // 이동 전 처리
                    blockManager.PreMoveEvent(BlockManager.Obj.PLAYER, nowidx, nextidx);
                }
            }
            else
            {
                Vector3 nextPos = player.transform.position + (playerSpeed * (Vector3)(Vector2)(walkEndPos - walkStartPos) * Time.deltaTime);
                
                float nextPosXY = nextPos.y;
                float playPosXY = player.transform.position.y;
                float endPosXY = walkEndPos.y;
                if (playerDirection == BlockManager.Direction.LEFT || playerDirection == BlockManager.Direction.RIGHT)
                {
                    nextPosXY = nextPos.x;
                    playPosXY = player.transform.position.x;
                    endPosXY = walkEndPos.x;
                }

                if (((endPosXY - playPosXY) > 0 && nextPosXY < endPosXY) || ((endPosXY - playPosXY) < 0 && nextPosXY > endPosXY))
                {
                    player.transform.position = nextPos;
                }
                else
                {
                    player.transform.position = (Vector2)walkEndPos;
                    playerWalk = false;
                    blockManager.PostMoveEvent(BlockManager.Obj.PLAYER, walkStartPos, walkEndPos);
                }
            }

            yield return null;
        }
    }

    public void GetItem(BlockManager.Obj obj)
    {
        if (obj == BlockManager.Obj.HAMMER)
        {
            preparedItem = BlockManager.Obj.HAMMER;
        }
        else if (obj == BlockManager.Obj.LIFE)
        {
            addLife(1);
        }
        else
        {
            throw new System.Exception("Error:get wrong item");
        }

        uiManager.SetItem(preparedItem);
    }

    public void GetGoal()
    {
        GameClear();
    }

    public void AttackedPlayer()
    {
        addLife(-1);
    }

    public void MovePortal(Vector2Int portalidx)
    {
        Vector2Int nextPos = blockManager.GetPortalExit(playeridx);
        playeridx = nextPos;
        player.transform.position = (Vector2)nextPos;
    }

    void addLife(int value)
    {
        life = Mathf.Clamp(life + value, 0, maxlife);
        uiManager.SetLife(life);
        if (life == 0)
        {
            GameOver();
        }
    }

    public MapData Load()
    {
        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            MapData mapData = (MapData)binaryFormatter.Deserialize(file);
            file.Close();

            return mapData;
        }
        else
        {
            throw new System.Exception("데이터 불러오기 실패 File Name: " + mapName);
        }
    }

    public void ButtonPause()
    {
        if (Time.timeScale == 0f)
        {
            uiManager.SetPause(false);
            Time.timeScale = 1f;
        }
        else
        {
            uiManager.SetPause(true);
            Time.timeScale = 0f;
        }
    }

    public void ButtonBack()
    {
        DataBus.Instance.WriteStartState(State.MENU);
        SceneManager.LoadScene("MainScene");
    }

    void GameOver()
    {
        Time.timeScale = 0f;
        pauseButton.interactable = false;
        uiManager.SetGameOver(true);
    }

    void GameClear()
    {
        Time.timeScale = 0f;
        pauseButton.interactable = false;
        uiManager.SetGameClear(true);

        // 클리어 기록 갱신
        if (mapName.Length > 6 && mapName.Substring(0, 6) == "Story ")
        {
            string str1 = "";
            string str2 = "";
            bool flag = true;
            foreach(char c in mapName.Substring(6, mapName.Length - 6))
            {
                if (flag)
                {
                    if (c != '-')
                    {
                        str1 += c;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    str2 += c;
                }
            }

            int nowStoryNum = int.Parse(str1);
            int nowMapNum = int.Parse(str2);
            int saveStoryNum = PlayerPrefs.GetInt("clearInfo_storyNum", 0);
            int saveMapNum = PlayerPrefs.GetInt("clearInfo_mapNum", 10);

            //Debug.Log("now: " + nowStoryNum + ", " + nowMapNum);
            //Debug.Log("save: " + saveStoryNum + ", " + saveMapNum);

            if ((saveStoryNum < nowStoryNum) || (saveStoryNum == nowStoryNum && saveMapNum < nowMapNum))
            {
                PlayerPrefs.SetInt("clearInfo_storyNum", nowStoryNum);
                PlayerPrefs.SetInt("clearInfo_mapNum", nowMapNum);
            }
        }
    }

    public void ButtonResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    public void ResetGame()
    {
        Init();

        blockManager.ResetBlock();
    }

    public void NextGame()
    {
        string str1 = "";
        string str2 = "";
        bool flag = true;
        foreach (char c in mapName.Substring(6, mapName.Length - 6))
        {
            if (flag)
            {
                if (c != '-')
                {
                    str1 += c;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                str2 += c;
            }
        }

        int nowStoryNum = int.Parse(str1);
        int nowMapNum = int.Parse(str2);
        int nextStoryNum = nowStoryNum + (nowMapNum / 10);
        int nextMapNum = (nowMapNum % 10) + 1;

        string nextMapName = "Story " + nextStoryNum + "-" + nextMapNum;

        mapName = nextMapName;


        dataPath = Application.dataPath + "/MapData/" + mapName + ".dat";

        mapData = Load();

        startidx = new Vector2Int(mapData.startIdx.x, mapData.startIdx.y);
        maxlife = mapData.maxLife;

        Init();

        blockManager.RemoveBlock();
        blockManager.SetBlock(mapData);
    }

    public Vector2Int GetPlayerIdx()
    {
        return playeridx;
    }
}
