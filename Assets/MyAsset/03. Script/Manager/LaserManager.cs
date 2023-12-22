using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laser.Manager
{
    public class LaserManager : MonoBehaviour
    {
        [SerializeField] private LineRenderer m_SubLine;

        public void SetFirstPos(Vector3 pos)
        {
            m_SubLine.SetPosition(0, pos);
        }

        public void SetSecondPos(Vector3 pos)
        {
            m_SubLine.SetPosition(1, pos);
        }

        public void ResetPos()
        {
            m_SubLine.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
        }
    }
}
