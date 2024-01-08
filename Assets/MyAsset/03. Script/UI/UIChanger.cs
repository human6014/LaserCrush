using UnityEngine;
using System;

[Serializable]
public struct CanvasGroup
{
    public string m_CanvasName;
    public GameObject m_Canvas;
}

public class UIChanger : MonoBehaviour
{
    [SerializeField] private bool m_OnlyOneCanvas = false;
    [SerializeField] private CanvasGroup[] m_CanvasGroups;


}
