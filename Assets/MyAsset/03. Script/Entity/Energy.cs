using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Energy : MonoBehaviour
{
    #region Variable
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private int INITENERGY = 10000;
    private static event UnityAction m_TextUpdate;

    private static int m_MaxEnergy;
    private static int m_CurrentEnergy;

    private static int m_HittingFloorLaserNum;
    #endregion

    private static int CurrentEnergy 
    { 
        get => m_CurrentEnergy;
        set
        {
            m_CurrentEnergy = value;
            m_TextUpdate?.Invoke();
        }
    }

    private void Awake()
    {
        m_TextUpdate = null;
        m_TextUpdate += () => m_Text.text = (m_CurrentEnergy / 100).ToString();

        m_MaxEnergy = INITENERGY;
        CurrentEnergy = INITENERGY;
    }
    /// 반환형은 총 사용한 에너지의 양이다.
    /// 적게남
    /// </summary>
    /// <param name="energy">
    /// 사용할 에너지
    /// </param>
    /// <returns></returns>
    public static int UseEnergy(int energy)
    {
        if (energy <= CurrentEnergy) 
        {
            CurrentEnergy -= energy;
        }
        else
        {
            energy = CurrentEnergy;
            CurrentEnergy = 0;
        }
        return energy;
    }
    
    public static bool CheckEnergy()
    {
        return CurrentEnergy > 0;
    }

    /// <summary>
    /// 일단 부딪힐 마다 10퍼 삭제
    /// </summary>
    public static void CollideWithWall()
    {
        UseEnergy(m_MaxEnergy / 10);
    }

    public static void CollideWithFloor()
    {
        UseEnergy(m_MaxEnergy / 5);
        //만약 바닥에 닿으면 꾸준히 대미지 주고 싶으면 윗 코드 주석하면 됨
    }

    public static int ChargeEnergy()
    {
        CurrentEnergy = m_MaxEnergy;
        m_HittingFloorLaserNum = 0;
        return CurrentEnergy;
    }

    public static void EnergyUpgrade(int additionalEnergy)
    {
        m_MaxEnergy += additionalEnergy;
    }

    public static int GetEnergy()
    {
        return m_CurrentEnergy;
    }
    
    //-95 ~ 95
    //게이지 표시 경우 -95 + GetGaugeNum으로 해줘야되더라
    //Fill Green
    private int GetGaugeNum()
    {
        return (190 * m_CurrentEnergy) / m_MaxEnergy;
    }

    public void Reset()
    {
        m_MaxEnergy = INITENERGY;
        m_CurrentEnergy = INITENERGY;
    }

}
