using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 無線巡迴背景製作 */

public class BackGroundActionScript : MonoBehaviour
{
    private Renderer renderer;//宣告渲染器
    private Material material;//宣告材質
    private Vector3 offset;

    public float fBackGroundSpd;
    private float fOffsetX;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();//取得當前物件渲染器-即為用來做背景的Plane
        material = renderer.material;//取得當前物件渲染器中的材質-即為用來做背景的Plane的材質
        offset = material.GetTextureOffset("_MainTex");//取得當前物件渲染器中的材質-即為用來做背景的Plane的主要材質(名稱必需為_MainTex而非使用中的材質名稱)
        fBackGroundSpd = 0f;//初始狀態下不移動
    }

    public void BackGroundAction(bool b)
    {
        if (b)
        {
            Debug.Log("BackGroundAction");
            fBackGroundSpd = 0.25f;
        }
        else
        {
            fBackGroundSpd = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        offset.x += fBackGroundSpd * Time.deltaTime;//設定Plane向x方向移動
        material.SetTextureOffset("_MainTex", offset);//將上述值套如Plane-Material中的offset正式使他開始動作
    }
}
