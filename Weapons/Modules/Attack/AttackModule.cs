using Sandbox;
using System;

namespace EasyWeapons.Weapons.Modules.Attack;

public abstract partial class AttackModule : WeaponModule
{
    [Net, Predicted, Local]
    public TimeSince TimeSinceLastAttack { get; protected set; }

    [Net, Local]
    public float FireRate { get; set; } = 10f;

    public float MinTimeBetweenAttacks => FireRate < 0.0001f ? float.MaxValue : 1f / FireRate;


    public override SimulationResult Simulate()
    {
        if(CanStartAttack())
        {
            Attack();
            return SimulationResult.Continuing;
        }

        return SimulationResult.Finished;
    }

    public virtual bool CanStartAttack()
    {
        return TimeSinceLastAttack >= MinTimeBetweenAttacks;
    }


    public virtual void Attack()
    {
        if(CanStartAttack() == false)
            throw new InvalidOperationException($"Can't attack. Test {CanStartAttack} first.");

        Enabled = true;

        using(Sandbox.Entity.LagCompensation())
        {
            Shoot();
        }
        TimeSinceLastAttack = 0;
    }

    protected abstract void Shoot();
}
