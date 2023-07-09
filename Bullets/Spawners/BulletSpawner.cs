using Sandbox;
using System;

namespace EasyWeapons.Bullets.Spawners;

public abstract class BulletSpawner : BaseNetworkable
{
    public abstract void Spawn(Ray ray, DamageInfo? damageInfo);
}