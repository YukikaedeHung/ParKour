using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkourScript : MonoBehaviour
{
    #region 宣告
    private Animator ani;
    private AnimatorStateInfo aniStateInfo;
    public RaycastScript nearCubeScript;
    public RaycastScript farCubeScript;
    public RaycastScript nearStickScript;
    public RaycastScript farStickScript;

    [SerializeField]
    private UnityChanScript unityChan;
    [SerializeField]
    private BackGroundActionScript backGroundAction;

    public GameObject objPreCube;
    public GameObject objPreStick;   
    private GameObject objTarget;

    public Text txtScore;
    public Text txtGameState;
    public Text txtGameLevel;
    public Text txtMessage;
    public Image[] imgHP = new Image[5];
    public Image imgVictory;
    public Image imgLose;
    public Image imgKeyBoardTip;

    public Sprite sprDeadHeart;
    public Sprite sprHeart;

    private const string CubeMoving = "CubeMoving";

    private int iKey;//角色動作：0-Idle、1-Run、2-Slide、3-Jump
    private int iGameLevel;
    private int iObjType;//物件種類：0為沒有物件，2為Cube，3為Stick
    private int iHP;
    private int iScore;
    private int iCount;
    private int iCountCube;
    private int iCountStick;
    private int iActionRight;//判斷動作時機是否正確：0為IDLE或位置不對先不判斷，1為正確，2為錯誤

    private float fMoveSpd;
    private float fActionTime;

    private bool bObjCreat;
    private bool bAction;
    private bool bArea1;
    private bool bArea2;
    private bool bBackGroundAction;
    private bool bGameOver;

    public bool bCheatHP;//關閉血條判定避免遊戲結束
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Initial();
    }

    void Initial()
    {
        ani = gameObject.GetComponent<Animator>();
        DataInitial();
    }
    void DataInitial()
    {
        /*遊戲開始前或重新開始後的預設狀態*/
        imgVictory.gameObject.SetActive(false);
        imgLose.gameObject.SetActive(false);
        imgKeyBoardTip.gameObject.SetActive(true);//開啟鍵盤提示
        txtGameState.gameObject.SetActive(true);//開啟遊戲開始的提示

        iKey = 0;
        iGameLevel = 0;
        iObjType = 0;
        iHP = 5;
        iScore = 0;
        iCount = 0;
        iCountCube = 0;
        iCountStick = 0;
        fMoveSpd = 1000f;

        bObjCreat = false;
        bAction = false;
        bArea1 = false;
        bArea2 = false;
        bGameOver = false;

        for (int i = 0; i < 5; i++)
        {
            imgHP[i].GetComponent<Image>().sprite = sprHeart;
        }
    }

    void ParKour()
    {
        KeyboardSetting();
        CharaActionCheck();
        ObjPosCheck();
        TargetAction(iObjType);

        if (bGameOver)
        {
            GameOver();
        }
        if (iKey >= 0)
        {
            if (bObjCreat)
            {
                StartCoroutine(corObjCreat(2.0f - (iGameLevel * 0.5f)));
            }
        }
    }

    //鍵盤配置
    void KeyboardSetting()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) && iKey == 0)
        {
            if (bGameOver)
            {
                DataInitial();
            }
            else
            {
                Debug.Log("GameStart");
                iKey = 1;//角色動作開始跑
                unityChan.CharaAction(iKey);
                ParKourStart();
            }
        }
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0)) && iKey == 1 && !bGameOver)
        {
            Debug.Log("Action-Slide");
            iKey = 2;
            unityChan.CharaAction(iKey);
            bAction = true;
        }
        if ((Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(1)) && iKey == 1 && !bGameOver)
        {
            Debug.Log("Action-Jump");
            iKey = 3;
            unityChan.CharaAction(iKey);
            bAction = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && iKey == 1 && !bGameOver)
        {
            iHP = 0;
            bGameOver = true;
        }
        if (unityChan.bActionDone && iKey > 1)
        {
            iKey = 1;
        }
    }

    void ParKourStart()
    {
        bObjCreat = true;//開始產生物件
        backGroundAction.BackGroundAction(true);//背景開始移動
        txtGameState.gameObject.SetActive(false);//關閉開始前的提示
        imgKeyBoardTip.gameObject.SetActive(false);//關閉鍵盤提示
    }

    //隨機生成預置物：0為沒有物件，2為Cube，3為Stick
    IEnumerator corObjCreat(float f)
    {
        if (iKey >= 1 && iObjType == 0 && objTarget == null)
        {
            bObjCreat = false;
            yield return new WaitForSeconds(f);
            iObjType = Random.Range(2, 4);
            if (iObjType == 2 && iCountCube == 1)
            {
                iObjType = Random.Range(2, 4);
                iCountCube = 0;
            }
            if (iObjType == 3 && iCountStick == 1)
            {
                iObjType = Random.Range(2, 4);
                iCountStick = 0;
            }
            //iObjType = 3;
            switch (iObjType)
            {
                case 2:
                    Instantiate(objPreCube, new Vector3(1440, 250, -30), Quaternion.identity);
                    objTarget = GameObject.Find("Cube(Clone)");
                    objTarget.SetActive(true);
                    iCountCube++;
                    iCountStick = 0;
                    Debug.Log("Creat-Cube");
                    break;
                case 3:
                    Instantiate(objPreStick, new Vector3(1440, 90, -30), Quaternion.identity);
                    objTarget = GameObject.Find("Stick(Clone)");
                    objTarget.SetActive(true);
                    iCountStick++;
                    iCountCube = 0;
                    Debug.Log("Creat-Stick");
                    break;
            }
        }
    }

    //物件動作
    void TargetAction(int i)
    {
        if (i == 2 || i == 3)
        {
            objTarget.transform.position += new Vector3(-fMoveSpd * Time.deltaTime, 0, 0);
            //Debug.Log("objMoving-objPos" + objTarget.transform.position);
        }
    }
    #region 方法1.擷取物件當前位置，失敗：會有取值上時間延遲的問題
    //角色、物體位置判斷
    //void ObjPosCheck()
    //{
    //    if (iObjType != 0)
    //    {
    //        Vector3 objPos = objTarget.transform.position;
    //        Vector3 objPosUnityChan = unityChan.objUnityChan.transform.position;
    //        Debug.LogFormat("Action1 {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", bArea1, bArea2, bArea3, bAction, iKey, bSafe, bHit, objPos.x);
    //        //安全區域會依照iObjType的不同而不同：Cube需對應Slide的時間；Stick需對應Jump的時間
    //        if (iObjType == 2)
    //        {
    //            //Cube with Slide
    //            //Debug.Log(Time.deltaTime);
    //            //fNearX = objPosUnityChan.x + 90f + 25f + (fMoveSpd * (Time.deltaTime) * (2.05f));//UnityChan最晚的下落點，向下過程可閃避
    //            //fFarX = objPosUnityChan.x + 90f + 25f + (fMoveSpd * (Time.deltaTime) * (33.0f));//UnityChan最早的爬起點，爬起過程可閃避※※※待修改
    //            fNearX = 550f;
    //            fFarX = 850f;
    //        }
    //        if (iObjType == 3)
    //        {
    //            //Stick with Jump
    //            fNearX = objPosUnityChan.x + 90f + (fMoveSpd * (Time.deltaTime) * (1.12f));//UnityChan最遲的起跳點，起跳過程可閃避
    //            fFarX = objPosUnityChan.x + 90f + (fMoveSpd * (Time.deltaTime) * (2.00f));//UnityChan最早的起跳點，落下過程可閃避
    //        }
    //        //Debug.Log(objPos.x + " / " + fNearX + " / " + fFarX);
    //        //安全區
    //        if (objPos.x <= fFarX && objPos.x > fNearX)
    //        {                
    //            bArea1 = true;
    //        }
    //        //死亡區
    //        if (objPos.x <= fNearX && objPos.x > 390)
    //        {
    //            bArea2 = true;
    //        }
    //        //物體與角色碰撞的位置為390
    //        if (objPos.x <= 390 && objPos.x > 0)
    //        {
    //            bArea3 = true;
    //        }
    //        //物件銷毀區
    //        if (objPos.x <= -100)
    //        {
    //            bArea3 = false;
    //            ObjDestroy();
    //            bObjCreat = true;
    //        }
    //    }
    //}
    #endregion

    #region 方法2.採用射線偵測區間內是否有物體
    void ObjPosCheck()
    {
        if (iObjType != 0)
        {
            Vector3 objPos = objTarget.transform.position;
            Vector3 objPosUnityChan = unityChan.objUnityChan.transform.position;            
            if (iObjType == 2)
            {
                if (farCubeScript.bObjInArea)
                {
                    bArea1 = true;
                    //Debug.Log("CubeAreaF");
                    //bArea1.x = 物件通過角色後的位置 + (fMoveSpd * 動畫滑行後要爬起快碰到物體的最後時間(s)) - 物件寬度的一半
                    //bArea1.x = 250 + (fMoveSpd * 0.8) -25 
                }
                if (nearCubeScript.bObjInArea)
                {
                    bArea2 = true;
                    //Debug.Log("CubeAreaN");
                    //bArea1.x = 物件碰觸角色前的位置 + (fMoveSpd * 動畫滑行前落下過程快碰到物體的最後時間(s)) - 物件寬度的一半
                    //bArea1.x = 390 + (fMoveSpd * 0.23) - 25
                }
            }
            if (iObjType == 3)
            {
                if (farStickScript.bObjInArea)
                {
                    bArea1 = true;
                    //Debug.Log("StickAreaF");
                    //bArea1.x = 物件通過角色後的位置 + (fMoveSpd * 動畫跳起後落下過程快碰到物體的最後時間(s)) - 物件寬度的一半
                    //bArea1.x = 200 + (fMoveSpd * 1.03) -12.5 
                }
                if (nearStickScript.bObjInArea)
                {
                    bArea2 = true;
                    //Debug.Log("StickAreaN");
                    //bArea1.x = 物件碰觸角色前的位置 + (fMoveSpd * 動畫跳起的過程中快碰到物體的最後時間(s)) - 物件寬度的一半
                    //bArea1.x = 4000 + (fMoveSpd * 0.4) - 25
                }
            }
            //UnityChan位置/得分判斷區
            if (objPos.x <= 390 && objPos.x > 0)
            {
                bArea1 = false;
                bArea2 = false;
                ScoreCheck();
            }
            //物件銷毀區
            if (objPos.x <= -100)
            {
                iCount++;
                GameLevel();
                ObjDestroy();
            }
        }
    }

    #endregion

    //#region 方法3.將射線安裝於預置物上，失敗：UnityChan未套用Material導致射線無法偵測

    //#endregion

    void ObjDestroy()
    {
        Debug.Log("Destroy");
        Destroy(objTarget);
        iObjType = 0;
        bObjCreat = true;
    }

    void CharaActionCheck()
    {
        if (bAction && !bArea1)
        {
            Debug.Log("Too Early " + "\n" + txtMessage.text);
            bAction = false;
        }
        if (bArea1 && !bArea2 && iKey == iObjType && bAction)
        {
            Debug.Log("Action1 " + "\n" + txtMessage.text);
            iActionRight = 1;
            bAction = false;
        }
        if (bArea1 && !bArea2 && iKey == iObjType && bAction)
        {
            Debug.Log("Action2 " + "\n" + txtMessage.text);
            iActionRight = 1;
            bAction = false;
        }
        if (bArea1 && bArea2 && iActionRight != 1)
        {
            Debug.Log("Fail " + "\n" + txtMessage.text);
            iActionRight = 2;
            bAction = false;
        }
    }

    void ScoreCheck()
    {
        //Debug.LogFormat("Action1 {0}, {1}, {2}, {3}, {4}, {5}, {6}", bArea1, bArea2, bArea3, bAction, iKey, bSafe, bHit);
        if (iActionRight == 1)
        {
            iActionRight = 0;//重製動作是否成功的紀錄，避免一次動作但分數累加
            iScore++;
            Debug.Log("Got Score");
        }
        if (iActionRight == 2)
        {
            iActionRight = 0;//重製動作是否成功的紀錄，避免一次動作但分數累扣
            if (iScore > 0)
            {
                iScore--;
            }
            if (iHP > 0)
            {
                if (!bCheatHP)
                {
                    iHP--;
                    StartCoroutine(corImgHpEven(iHP));
                }
                if (iHP == 0)
                {
                    bGameOver = true;
                }
            }
            objTarget.SetActive(false);
            Debug.Log("Got Hit");
        }

    }

    void TxtInfoUpdate()
    {
        txtScore.text = "Score : " + iScore;
        txtGameLevel.text = "Level : " + iGameLevel;
        txtGameState.text = bGameOver ? "PRESS ENTER TO RESTART" : "PRESS ENTER TO START";
        txtMessage.text = "bArea1 = " + bArea1 + "    bArea2 = " + bArea2 + "    iActionRight = " + iActionRight + "    iKey = " + iKey + "\n" +
                          "iObjType = " + iObjType + "    bAction = " + bAction + "    bObjCreat = " + bObjCreat + "    iCountCube = " + iCountCube + "    iCountStick = " + iCountStick + "\n" +
                          "objTarget = " + objTarget + "\n" +
                          "bFarCubeArea = " + farCubeScript.bObjInArea + "    bNearCubeArea = " + nearCubeScript.bObjInArea +"\n" +
                          "bFarStickArea = " + farStickScript.bObjInArea + "     bNearStickArea = " + nearStickScript.bObjInArea;
    }

    IEnumerator corImgHpEven(int i)
    {        
        imgHP[i].GetComponent<Image>().sprite = sprDeadHeart;
        yield return new WaitForSeconds(0.2f);
        imgHP[i].GetComponent<Image>().sprite = sprHeart;
        yield return new WaitForSeconds(0.2f);
        imgHP[i].GetComponent<Image>().sprite = sprDeadHeart;
        yield return new WaitForSeconds(0.2f);
    }

    void GameOver()
    {
        backGroundAction.BackGroundAction(false);//停止背景
        iKey = 0;//停止角色動作
        unityChan.CharaAction(0);//停止角色動作
        ObjDestroy();//當前物件摧毀

        txtGameState.gameObject.SetActive(true);//開啟重玩的提示
        if (iHP != 0)
        {
            Debug.Log("Player Win");
            imgVictory.gameObject.SetActive(true);
        }
        if (iHP == 0)
        {
            Debug.Log("Player Dead");
            imgLose.gameObject.SetActive(true);
        }
        Debug.Log("GameOver");
    }

    void GameLevel()
    {
        if (iCount == 5)
        {
            iGameLevel++;
            iCount = 0;

            if(iGameLevel == 5 && !bCheatHP)
            {
                bGameOver = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ParKour();
        TxtInfoUpdate();
    }
}
