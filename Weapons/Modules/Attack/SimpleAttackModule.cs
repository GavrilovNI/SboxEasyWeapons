using EasyWeapons.Bullets.Spawners;
using EasyWeapons.Weapons.Modules.Attack.ShootingModes;
using Sandbox;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyWeapons.Weapons.Modules.Attack;

public partial class SimpleAttackModule : AttackModule
{
    public virtual Ray AimRay => Weapon.AimRay;

    [Net, Local]
    public InputButton AttackButton { get; protected set; }

    [Net, Local]
    public BulletSpawner BulletSpawner { get; protected set; }

    [Net, Predicted, Local]
    public TimeSince TimeSinceFailedAttack { get; protected set; }

    [Net, Predicted, Local]
    public ShootingMode ShootingMode { get; protected set; }

    public SimpleAttackModule()
    {
        Game.AssertClient();
        BulletSpawner = null!;
        ShootingMode = null!;
    }

    public SimpleAttackModule(BulletSpawner bulletSpawner, ShootingMode shootingMode, InputButton attackButton = InputButton.PrimaryAttack)
    {
        BulletSpawner = bulletSpawner;
        AttackButton = attackButton;
        ShootingMode = shootingMode;
    }

    public override SimulationResult Simulate()
    {
        if(ShouldAttack())
        {
            if(CanStartAttack())
            {
                Attack();
                return SimulationResult.Continuing;
            }
            else
            {
                OnFailedAttack();
                TimeSinceFailedAttack = 0;
                return SimulationResult.Finished;
            }
        }

        return ShootingMode.IsShooting ? SimulationResult.Continuing : SimulationResult.Finished;
    }

    public sealed override void Attack()
    {
        base.Attack();
        ShootingMode.OnShot();
    }

    protected virtual void OnFailedAttack()
    {
        ShootingMode.OnFail();
        DoDryifireEffects();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        ShootingMode.OnFail();
    }

    protected virtual bool ShouldAttack()
    {
        bool timePassed = TimeSinceLastAttack >= MinTimeBetweenAttacks && TimeSinceFailedAttack >= MinTimeBetweenAttacks;
        if(timePassed == false)
            return false;

        bool hasOwner = Weapon.Owner.IsValid();
        return hasOwner ? ShootingMode.ShouldAttack(AttackButton) : ShootingMode.ShouldAttack();
    }

    public override bool CanStartAttack()
    {
        return TimeSinceFailedAttack >= MinTimeBetweenAttacks && base.CanStartAttack();
    }

    protected override void Shoot()
    {
        BulletSpawner.Spawn(AimRay, ModifyDamageInfo);
        DoShootEffects();
    }

    protected virtual DamageInfo ModifyDamageInfo(DamageInfo damageInfo)
    {
        return damageInfo.WithAttacker(Weapon.Owner, Weapon);
    }

    protected virtual void DoShootEffects()
    {
        if(Game.IsServer == false)
            return;

        /*_ = AttackSound?.PlayOnEntity(Weapon);
        Weapon.CreateParticle(AttackParticlePath, AttackParticleAttachment);
        Weapon.SetViewModelAnimParameter(AttackAnimation, true);*/
    }

    protected virtual void DoDryifireEffects()
    {
        if(Game.IsServer == false)
            return;

        //_ = DryfireSound?.PlayOnEntity(Weapon);
    }
}
