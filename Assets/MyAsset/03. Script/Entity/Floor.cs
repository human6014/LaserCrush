using LaserCrush.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Floor : MonoBehaviour, ICollisionable
{
    public List<Vector2> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
    {
        Energy.UseEnergy(int.MaxValue);
        laser.ChangeLaserState(ELaserStateType.Hitting);
        return new List<Vector2>();
    }

    public bool IsGetDamageable()
    {
        return false;
    }

    public bool GetDamage(int damage)
    {
        return false;
    }

    public bool Waiting()
    {
        return true;
    }
}
