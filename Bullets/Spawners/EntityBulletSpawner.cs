using EasyWeapons.Bullets.Datas;
using Sandbox;

namespace EasyWeapons.Bullets.Spawners;

public class EntityBulletSpawner : BulletSpawner<EntityBulletData>
{
    public override void Spawn(Ray ray, EntityBulletData data)
    {
        if(Game.IsServer == false)
            return;

        var entityType = data.GetEntityType();
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
        Log.Info("test1");
        data.Initialize(entityBullet, ray);
        Log.Info("test2");
    }
}
