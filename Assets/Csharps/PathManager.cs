using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public GameObject PathObject;
    public GameObject LineObject;

    //路径数组
    public List<GameObject> Paths = new List<GameObject>();

    //Line
    public List<GameObject> Lines = new List<GameObject>();
    public int previous_Linecount;

    //stopUpdateLine
    bool stopupdateLine = false;

    private void Start()
    {
        previous_Linecount = 0;
    }

    void Update()
    {


        //创建路径
        if(Input.GetKeyDown(KeyCode.Space)){

            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.name = $"{Paths.Count}";
            point.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            point.transform.SetParent(PathObject.transform);
            Paths.Add(point);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            stopupdateLine = true;
        }


        if (stopupdateLine == false)
        {
            UpdatePathLine();
        }
     
        

    }



    //绘制
    void UpdatePathLine()
    {
        //2个再开始画
        if (Paths.Count >= 2)
        {
            //是否创建新椭圆
            if (previous_Linecount != Paths.Count - 1)
            {
                int steps = Paths.Count - 1 - previous_Linecount;

                //创建新椭圆
                while (steps-- > 0)
                {
                    GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    line.name = $"{Lines.Count}";
                    line.transform.SetParent(LineObject.transform);
                    Lines.Add(line);
                    previous_Linecount++;
                } 
            }

            //更新椭圆位置
            for (int i = 0;i < Lines.Count;i ++)
            {
                //position
                Lines[i].transform.position = (Paths[i].transform.position + Paths[i + 1].transform.position) / 2;

                //direction
                Vector3 direct = Paths[i].transform.position - Paths[i + 1].transform.position;
                Lines[i].transform.up = direct;

                //scaleY
                float Yaw = direct.magnitude / 2;
                Lines[i].transform.localScale = new Vector3(1, Yaw, 1);
            }


        }

    }


}
