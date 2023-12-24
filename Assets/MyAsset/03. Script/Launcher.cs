using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    #region Property
    private static Vector2 m_Posion;
    private static Vector2 m_DirectionVector;
    #endregion

    public static Vector2 GetPosion()
    {
        return m_Posion;
    }

}
