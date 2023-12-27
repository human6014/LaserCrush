using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public void OnMouseDown()
    {
        Debug.Log(transform.name + "MouseDown");
    }
}
