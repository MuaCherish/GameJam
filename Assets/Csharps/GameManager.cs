using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Transforms")]
    public GameObject PathObject;
    public GameObject cam;
    public GameObject player;
    public GameObject DebugCylinder;
    public GameObject garbage;
    public GameObject StartScreen;
    public GameObject RestartScreen;

    [Header("GameState")]
    public bool Gamestart = false;
    public bool Qte_press = false;
    public bool needToQte = false;

    [Header("GameValues")]
    public List<Vector3> Paths = new List<Vector3>();
    private Vector3 previous_PlayerLocation;
    public int Nextpathnumber = 1;
    public float moveDuration = 1f;
    public float qte_radious = 0.1f;

    //Save
    private Vector3 Save_CamPosition;
    private float radious_DebugCylinder;
    
    //Coroutine
    Coroutine PlayerMovingCoroutine;
    Coroutine CameraMovingCoroutine;
    Coroutine QTEMovingCoroutine;




    //------------------------------ 生命周期 -----------------------------------
    private void Start()
    {
        Game_Init();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Qte_press = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Qte_press = false;
        }

        if (Gamestart)
        {
            //初始化位置
            ExecOnce(0, () =>
            {
                Game_Start();
            });


            //向后平滑移动，直到终点
            ExecOnce(1, () =>
            {
                PlayerMovingCoroutine = StartCoroutine(PlayerMoving());
                CameraMovingCoroutine = StartCoroutine(CameraMoving());
                QTEMovingCoroutine = StartCoroutine(QTE());
            });

        }

        
    }


    //---------------------------------------------------------------------------






    //------------------------------- Game ------------------------------------
    //GameInit
    void Game_Init()
    {
        //Clear
        Paths.Clear();

        //Init
        foreach (Transform child in PathObject.transform)
        {
            Paths.Add(child.position);
        }
        previous_PlayerLocation = player.transform.position;

        //save
        Save_CamPosition = cam.transform.position;
        radious_DebugCylinder = DebugCylinder.transform.localScale.x;
    }

    //GameStart
    void Game_Start()
    {
        //Set
        player.transform.position = Paths[0];
        cam.transform.position = Save_CamPosition;
    }

    //GameEnd
    void Game_Stop()
    {
        RestartScreen.SetActive(true);

        Gamestart = false;
        needToQte = false;

        StopAllCoroutines();

        PlayerMovingCoroutine = null;
        CameraMovingCoroutine = null;
        Nextpathnumber = 1;
        executedNumbers.Clear();
    }

    //-------------------------------------------------------------------------





    //-------------------------------- Objects ---------------------------------

    //玩家移动
    IEnumerator PlayerMoving()
    {
        for (int i = 1; i < Paths.Count; i++)
        {
            yield return StartCoroutine(SmoothMoving(player,player.transform.position, Paths[i],moveDuration));
            Nextpathnumber = i + 1;
        }
        Game_Stop();
    }

    //摄像机移动
    IEnumerator CameraMoving()
    {
        for (int i = 0; i < Paths.Count - 1; i++)
        {
            float x = Paths[i + 1].x - Paths[i].x;
            float y = Paths[i + 1].y - Paths[i].y;
            Vector3 offset = new Vector3(x, y, 0);

            yield return StartCoroutine(SmoothMoving(cam, cam.transform.position, cam.transform.position + offset, moveDuration));
        }

    }

    //检测QTE
    IEnumerator QTE()
    {
        bool hasExec = true;
        while (Gamestart)
        {
            if ((player.transform.position - Paths[Nextpathnumber]).magnitude <= qte_radious)
            {
                if (hasExec)
                {
                    needToQte = true;
                    hasExec = false;
                }

                if (needToQte)
                {
                    PlaceDebugCylinder();
                }
                else
                {
                    ResetDebugCylinder();
                }

                //如果未打QTE
                if (Qte_press)
                {
                    needToQte = false;
                }

            }
            else
            {

                //如果提前打QTE || 上一次未打上QTE
                if (Qte_press || needToQte)
                {
                    Game_Stop();
                }
                else
                {
                    hasExec = true;
                }
            }
            

            yield return null;
        }
        
    }

    //--------------------------------------------------------------------------






    //------------------------------- Canvas -----------------------------------

    //点击开始
    public void ClickTo_Start()
    {
        Gamestart = true;
        StartScreen.SetActive(false);
    }

    //点击重新开始
    public void ClickTo_Restart()
    {
        Gamestart = true;
        RestartScreen.SetActive(false);
    }

    //---------------------------------------------------------------------------






    //------------------------------- 工具类 ------------------------------------

    //ExecOnce
    private static Dictionary<int, bool> executedNumbers = new Dictionary<int, bool>();
    public void ExecOnce(int number, System.Action action)
    {
        if (!executedNumbers.ContainsKey(number))
        {
            //Invoke
            action?.Invoke();

            //update
            executedNumbers.Add(number, true);
        }
    }

    //MovingFunction
    IEnumerator SmoothMoving(GameObject MovingObject,Vector3 startLocation, Vector3 TargetLocation, float duration)
    {

        Vector3 initialPosition = MovingObject.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 插值计算当前位置
            MovingObject.transform.position = Vector3.Lerp(initialPosition, TargetLocation, elapsedTime / duration);

            // 等待一帧
            yield return null;

            // 累加已经过去的时间
            elapsedTime += Time.deltaTime;
        }

        // 确保物体最终位置准确
        MovingObject.transform.position = TargetLocation;

    }

    //PlaceDebugCylinder
    void PlaceDebugCylinder()
    {
        //position
        DebugCylinder.transform.position = (player.transform.position + Paths[Nextpathnumber]) / 2;

        //direction
        Vector3 direct = player.transform.position - Paths[Nextpathnumber];
        DebugCylinder.transform.up = direct;

        //scaleY
        float Yaw = direct.magnitude / 2;
        DebugCylinder.transform.localScale = new Vector3(radious_DebugCylinder, Yaw, radious_DebugCylinder);
    }
    void ResetDebugCylinder()
    {
        //position
        DebugCylinder.transform.position = garbage.transform.position;
    }

    //----------------------------------------------------------------------------





}
