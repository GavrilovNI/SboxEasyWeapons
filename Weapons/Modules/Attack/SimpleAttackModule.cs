﻿using EasyWeapons.Bullets.Spawners;
using EasyWeapons.Inventories;
using EasyWeapons.Recoiles;
using EasyWeapons.Recoiles.Modules;
using EasyWeapons.Sounds;
using EasyWeapons.Weapons.Modules.Attack.ShootingModes;
using Sandbox;

namespace EasyWeapons.Weapons.Modules.Attack;

public partial class SimpleAttackModule : AttackModule
{
    public virtual Ray AimRay => Weapon.AimRay;

    [Net, Local]
    public string AttackAction { get; protected set; } = "attack1";

    [Net, Local]
    public BulletSpawner BulletSpawner { get; protected set; }

    [Net]
    public PlayableDelayedSound? AttackSound { get; set; }

    [Net]
    public PlayableDelayedSound? DryfireSound { get; set; }

    [Net]
    public string AttackAnimation { get; set; } = "fire";

    [Net]
    public string AttackParticlePath { get; set; } = "particles/pistol_muzzleflash.vpcf";

    [Net]
    public string AttackParticleAttachment { get; set; } = "muzzle";

    [Net, Local]
    public Recoil? Recoil { get; set; }

    [Net, Predicted, Local]
    public ShootingMode ShootingMode { get; protected set; }


    public SimpleAttackModule()
    {
        Game.AssertClient();
        BulletSpawner = null!;
        ShootingMode = null!;
    }

    public SimpleAttackModule(AmmoInventory weaponClip, BulletSpawner bulletSpawner, ShootingMode shootingMode, string attackAction = "attack1") : base(weaponClip)
    {
        BulletSpawner = bulletSpawner;
        AttackAction = attackAction;
        ShootingMode = shootingMode;
    }

    public override SimulationResult Simulate()
    {
        if(ShouldAttack())
            base.Simulate();

        return ShootingMode.IsShooting ? SimulationResult.Continuing : SimulationResult.Finished;
    }

    public override void Attack()
    {
        base.Attack();
        ShootingMode.OnShot();
    }

    protected override void Fail()
    {
        base.Fail();
        ShootingMode.OnFail();
        DoFailEffects();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        ShootingMode.OnFail();
    }

    protected virtual bool ShouldAttack()
    {
        bool timePassed = TimeSinceLastAttackTry >= MinTimeBetweenAttacks;
        if(timePassed == false)
            return false;

        bool hasOwner = Weapon.Owner.IsValid();
        return hasOwner ? ShootingMode.ShouldAttack(AttackAction) : ShootingMode.ShouldAttack();
    }

    protected override void Shoot()
    {
        BulletSpawner.Spawn(AimRay, GetDamageInfo());
        DoShootEffects();
    }

    protected virtual DamageInfo GetDamageInfo()
    {
        return new DamageInfo().WithAttacker(Weapon.Owner, Weapon);
    }

    protected virtual void ApplyRecoil()
    {
        if(Recoil is null)
            return;

        var owner = Entity.Owner;
        if(owner is null)
            return;

        var recoilApplier = owner.Components.Get<IRecoilApplier>();
        if(recoilApplier is null)
            return;

        Recoil.ApplyRecoil(recoilApplier);
    }

    protected virtual void DoShootEffects()
    {
        ApplyRecoil();

        if(Game.IsServer == false)
            return;

        _ = AttackSound?.PlayOnEntity(Weapon);
        Weapon.CreateParticle(AttackParticlePath, AttackParticleAttachment);
        Weapon.SetViewModelAnimParameter(AttackAnimation, true);
    }

    protected virtual void DoFailEffects()
    {
        if(Game.IsServer == false)
            return;

        _ = DryfireSound?.PlayOnEntity(Weapon);
    }
}
