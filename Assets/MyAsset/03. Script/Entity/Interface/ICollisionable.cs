using System;

public interface ICollisionable
{

    enum EntityType
    {
        //Attable//
        NormalBlock,
        ReflectBlock,

        ///////
        Prisim,
        Floor,
        Wall,
        Launcher
    }

    public EntityType GetEntityType();

    public void GetDamage(int damage);

    bool IsAttackable();
}
