
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    #region Variable
    public static uint m_Score = 0;
    [SerializeField] private static TextMeshProUGUI m_Text;

    #endregion

    private void Awake()
    {
        m_Text = GetComponentInChildren<TextMeshProUGUI>();

    }

    public static void GetScore(uint score)
    {
        m_Score += score;
        m_Text.text = score.ToString();
    }
}
