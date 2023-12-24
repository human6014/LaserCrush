using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Energy,
    Prism_1,
    Prism_2,
    Prism_3,
    Prism_4,
    Prism_5,
}
public class Item : MonoBehaviour
{
    #region Property
    private Vector2 m_Posion { get; }
    private ItemType m_Type;
    #endregion
}
