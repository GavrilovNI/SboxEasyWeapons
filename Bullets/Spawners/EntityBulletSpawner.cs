using Sandbox;

namespace EasyWeapons.Bullets.Spawners;

public partial class EntityBulletSpawner : BulletSpawner
{
    [Net, Local]
    public string BulletEntityName { get; set; } = null!;

    public override void Spawn(Ray ray, DamageInfo? damageInfo)
    {
        if(Game.IsServer == false)
            return;

        var entityType = TypeLibrary.GetType<Entity>(BulletEntityName)?.TargetType;

        if(entityType == null)
        {
            Log.Error($"Bullet entity '{BulletEntityName}' not found");
            return;
        }

        if(!TypeLibrary.HasAttribute<SpawnableAttribute>(entityType))
        {
            Log.Error($"Bullet entity '{BulletEntityName}' is not spawnable");
            return;
        }

        var entityBullet = TypeLibrary.Create<Entity>(entityType);

        if(entityBullet is not IBullet bullet)
        {
            Log.Error($"Bullet entity '{BulletEntityName}' doesn't implement {nameof(IBullet)}");
            entityBullet.Delete();
            return;
        }

        using(Prediction.Off())
            bullet.Initialize(ray, damageInfo);
    }
}