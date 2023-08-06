using Sandbox;

namespace EasyWeapons.Bullets.Spawners;

public abstract class BulletSpawner : BaseNetworkable
{
    public abstract void Spawn(Ray ray, IBulletDataSet bulletDataSet, string ammoId);
}
