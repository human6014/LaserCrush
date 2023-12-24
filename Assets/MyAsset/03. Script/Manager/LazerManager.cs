using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;


public class LazerManager : MonoBehaviour
{
    #region Property
    //lazer저장하는 자료구조
    private Lazer m_InitLazer;
    private List<Lazer> m_Lazers = new List<Lazer> ();
    //지우기 시작할 레이저보관 자료구조
    private List<Lazer> m_RootLazer = new List<Lazer> ();
    private bool m_Initialized = false;
    #endregion
    /// <summary>
    /// 레이저가 파괴당하는 이벤트 발생 시 호출
    /// 파괴당할 lazer를 매개변수로 받아 해당 개체의 자식lazer를 
    /// 새로운 rootlazer에 추가
    /// </summary>
    /// <param name="lazer"></param>
    void DestroyLazer(Lazer lazer)
    {
        m_RootLazer.Remove(lazer);

        if(lazer.HasChild())
        {
            List<Lazer> child = lazer.GetChildLazer();
            foreach(var now in child)
            {
                m_RootLazer.Add(now);
            }
        }
    }

    public void EraseLazer()
    {
        foreach(var lazer in m_RootLazer)
        {
            lazer.Erase();
        }
    }

    public void Activate()
    {
        /*
         * 턴 시작
         * 1. 루트 배열 비우기
         * 2. 시작 레이저 셋팅 후 루트배열과 레이저 배열에 추가
         * 3. 초기화변수 셋팅
         */
        if(!m_Initialized)//턴 시작
        {
            m_RootLazer.Clear();
            m_InitLazer.Init(Energy.GetPosion(), Launcher.GetPosion(), Launcher.GetPosion() - Energy.GetPosion());
            m_RootLazer.Add(m_InitLazer);
            m_Lazers.Add(m_InitLazer);
            m_Initialized = true;
        }
        /*
         * 1. 레이저 순회하며 모든 레이저 진행 
         */
        else
        {
            for(int i = 0; i < m_Lazers.Count; i++) 
            {
                if (!m_Lazers[i].IsComplite())//진행 불가능 체크
                {
                    m_Lazers[i].Shoot();
                }
            }
        }

    }
}
