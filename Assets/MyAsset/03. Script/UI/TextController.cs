using TMPro;
using UnityEngine;

namespace LaserCrush.UI.Controller
{
    public class TextController : MonoBehaviour
    {
        private TextMeshProUGUI m_Text;

        public void Init()
            => m_Text = GetComponent<TextMeshProUGUI>();
        
        public void SetText(string text)
            => m_Text.text = text;
        
        public void SetTextWithThousandsSeparate(string text)
            => m_Text.text = string.Format("{0:N0}", text);
    }
}
