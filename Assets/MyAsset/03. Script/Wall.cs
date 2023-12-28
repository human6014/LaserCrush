using Laser.Entity;
using Laser.Manager;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;
using Debug = UnityEngine.Debug;

public class Wall : ICollisionable
{
    public void Awake()
    {
        m_Type = EntityType.Wall;
        Debug.Log("벽 초기화");
    }
    public override EntityType GetEntityType()
    {
        return EntityType.Wall;
    }


    public override void GetDamage(int damage)
    {
        return;
    }

    public override bool IsAttackable()
    {
        return false;
    }

    public override void Hitted(RaycastHit2D hit, Vector2 parentDirVector)
    {
        Debug.Log("벽과 충돌 후 자식생성");
        Vector2 dir = Vector2.Reflect(parentDirVector, hit.normal);
        Vector2 tem = new Vector2(-1, 0);
        Debug.Log("반사벡터 : " + dir + "부모 벡터 : " + parentDirVector);
        /*
        Laser.Entity.Laser laser = Instantiate(gameObject).GetComponent<Laser.Entity.Laser>();
        laser.Init(hit.transform.position, tem);
        LaserManager.AddLaser(laser);
        */
    }
}
