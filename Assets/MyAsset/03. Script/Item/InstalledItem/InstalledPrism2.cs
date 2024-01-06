using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class InstalledPrism2 : InstalledItem
{
    public override void Init()
    {
        m_EjectionPorts.Add(new Vector2(1, 0).normalized);
        m_EjectionPorts.Add(new Vector2(-1, 0).normalized);
        m_IsActivate = false;
        Debug.Log("√ ±‚»≠µ ");
    }

}
