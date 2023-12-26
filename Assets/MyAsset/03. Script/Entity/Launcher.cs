using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laser.Entity
{
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

        public static void SetDirectionVector(Vector2 vec)
        {
            m_DirectionVector = vec;
        }

        public Vector2 GetDirectionVector()
        {
            return m_DirectionVector;
        }

    }
}
