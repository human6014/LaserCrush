using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    public abstract class PoolableScript : MonoBehaviour
    {
        public abstract void ReturnObject();
    }
}
