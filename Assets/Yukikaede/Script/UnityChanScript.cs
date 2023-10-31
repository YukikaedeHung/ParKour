using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanScript : MonoBehaviour
{
    private Animator ani;
    public GameObject objUnityChan;
    private AnimatorStateInfo aniStateInfo;
    public int iCharaAction;
    public bool bActionDone;

    // Start is called before the first frame update
    void Start()
    {
        ani = this.gameObject.GetComponent<Animator>();//將UnityChan置入
    }

    public void CharaAction(int i)
    {
        iCharaAction = i;
        StartCoroutine(corCharaAction(iCharaAction));//啟用Coroutine
    }

    //使用Coroutine，透過修改iCharaAction的值設定UnityChan的動作模式：0-Idle、1-Run、2-Slide、3-Jump、4-Win、5-Lose
    IEnumerator corCharaAction(int i)
    {
        if (iCharaAction == 0)
        {
            ani.SetInteger("ActionID", 0);
        }
        if (iCharaAction == 1)
        {
            bActionDone = true;
            ani.SetInteger("ActionID", 1);            
            //Debug.Log("Start-Idle to Run");
        }
        if (iCharaAction == 2)
        {
            bActionDone = false;
            ani.SetInteger("ActionID", 2);
            //Debug.Log("Run to Slide");
            yield return new WaitForSeconds(0.54f);//角色觸發滑行到角色爬起閃過Cube的時間
            ActionDone(iCharaAction);
            //Debug.Log("Slide to Run");
        }
        if (iCharaAction == 3)
        {
            bActionDone = false;
            ani.SetInteger("ActionID", 3);
            //Debug.Log("Run to Jump");
            yield return new WaitForSeconds(0.69f);//角色觸發跳躍到角色落下閃過Stick的時間
            ActionDone(iCharaAction);
            //Debug.Log("Jump to Run");
        }
    }

    //動作結束復原-因為為跑酷遊戲，故設定回到1-Run的模式而非0-Idle
    public void ActionDone(int iCharaAction)
    {
        iCharaAction = 1;
        ani.SetInteger("ActionID", 1);
        bActionDone = true;
    }

    // Update is called once per frame
    void Update()
    {
        aniStateInfo = ani.GetCurrentAnimatorStateInfo(0);//將UnityChan的動畫置入
    }
}
