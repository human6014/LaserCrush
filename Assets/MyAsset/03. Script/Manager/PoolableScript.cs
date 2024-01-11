using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolableScript : MonoBehaviour
{
    protected Manager.ObjectPoolManager.PoolingObject m_PoolingObject;
    public abstract void ReturnObject();
}
