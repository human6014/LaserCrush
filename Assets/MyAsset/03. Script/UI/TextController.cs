using TMPro;
using UnityEngine;

namespace LaserCrush.UI.Controller
{
    public class TextController : MonoBehaviour
    {
        #region Variable
        private TextMeshProUGUI m_Text;
        #endregion

        public void Init()
        {
            m_Text = GetComponent<TextMeshProUGUI>();
        }

        public void SetText(string text)
        {
            m_Text.text = text;
        }
    }
}
