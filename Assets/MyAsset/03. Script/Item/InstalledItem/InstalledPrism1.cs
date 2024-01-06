using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class InstalledPrism1 : InstalledItem
{
    public override void Init()
    {
        m_EjectionPorts.Add(new Vector2(1, 0));
        m_IsActivate = false;
        Debug.Log("ÃÊ±âÈ­µÊ");
    }

}
