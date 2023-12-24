using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    #region Property
    private LazerManager lazerManager;
    private List<ICollisionable> m_CollisionableDbjects = new List<ICollisionable>();
    private bool m_OnDeploying;
    #endregion

    public void Update()
    {
        //시뮬레이션
        if(m_OnDeploying)
        {
            lazerManager.Activate();
        }
        //배치턴
        else
        {
            //어떤식으로 해야할지 감이 안온다
            //완료버튼(에너지 충전소 누르기) 누르면 배치완료변수 true로 바꾸기

        }
    }
}
