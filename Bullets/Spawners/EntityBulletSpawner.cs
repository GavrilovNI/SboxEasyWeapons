using EasyWeapons.Bullets.Datas;
using Sandbox;

namespace EasyWeapons.Bullets.Spawners;

public class EntityBulletSpawner : BulletSpawner
{
    public override void Spawn(Ray ray, IBulletDataSet bulletDataSet, string ammoId)
    {
        if(Game.IsServer == false)
            return;

        EntityBulletData? bulletData = bulletDataSet.Get<EntityBulletData>(ammoId);
        if(bulletData == null)
        {
            Log.Error($"{nameof(EntityBulletData)} wasn't found for {nameof(ammoId)} {ammoId}");
            return;
        }

        var entityType = bulletData.GetEntityType();
        if(entityType is null)
        {
            Log.Error($"Bullet entity not found");
            return;
        }

        if(!TypeLibrary.HasAttribute<SpawnableAttribute>(entityType))
        {
            Log.Error($"Bullet entity is not spawnable");
            return;
        }

        var entityBullet = TypeLibrary.Create<Entity>(entityType);

        if(entityBullet is null)
        {
            Log.Error($"Couldn't create bullet entity");
            return;
        }

        bulletData.InitializeEntity(entityBullet, ray);
    }
}
