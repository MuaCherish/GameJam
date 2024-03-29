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

    //һ����
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
            //��ʼ��λ��
            if (hasExec_Init)
            {

                InitPlayer();

                hasExec_Init = false;
            }

            //���ƽ���ƶ���ֱ���յ�
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
                // ��ֵ���㵱ǰλ��
                transform.position = Vector3.Lerp(initialPosition, pathmanager.Paths[targetIndex].transform.position, elapsedTime / moveDuration);
                cam.transform.LookAt(pathmanager.Paths[targetIndex].transform);
                transform.LookAt(pathmanager.Paths[targetIndex].transform); 

                // �ȴ�һ֡
                yield return null;

                // �ۼ��Ѿ���ȥ��ʱ��
                elapsedTime += Time.deltaTime;
            }

            // ȷ����������λ��׼ȷ
            transform.position = pathmanager.Paths[targetIndex].transform.position;

        }


        //End
        Gamestart = false;
        hasExec_Init = true;
        PlayerMovingCoroutine = null;

    }


}
