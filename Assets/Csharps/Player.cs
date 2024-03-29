using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool Gamestart = false;
    public PathManager pathmanager;
    public Transform cam;

    public float moveDuration = 1f;

    //一次性
    Coroutine PlayerMovingCoroutine;
    bool hasExec_Init = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Gamestart = true;
        }

        if (Gamestart)
        {
            //初始化位置
            if (hasExec_Init)
            {

                InitPlayer();

                hasExec_Init = false;
            }

            //向后平滑移动，直到终点
            if (PlayerMovingCoroutine == null)
            {
                PlayerMovingCoroutine = StartCoroutine(PlayerMoving());
            }

        }
    }

    //Init
    void InitPlayer()
    {
        transform.position = pathmanager.Paths[0].transform.position;
    }

    //Moving
    IEnumerator PlayerMoving()
    {

        for (int targetIndex = 1; targetIndex < pathmanager.Paths.Count; targetIndex++)
        {
            Vector3 initialPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                // 插值计算当前位置
                transform.position = Vector3.Lerp(initialPosition, pathmanager.Paths[targetIndex].transform.position, elapsedTime / moveDuration);
                cam.transform.LookAt(pathmanager.Paths[targetIndex].transform);
                transform.LookAt(pathmanager.Paths[targetIndex].transform); 

                // 等待一帧
                yield return null;

                // 累加已经过去的时间
                elapsedTime += Time.deltaTime;
            }

            // 确保物体最终位置准确
            transform.position = pathmanager.Paths[targetIndex].transform.position;

        }


        //End
        Gamestart = false;
        hasExec_Init = true;
        PlayerMovingCoroutine = null;

    }


}
