using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    public abstract class PoolableMonoBehaviour : MonoBehaviour
    {
        public abstract void ReturnObject();
    }
}
