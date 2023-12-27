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

    EntityType GetEntityType();

    void GetDamage(int damage);

    bool IsAttackable();
}
