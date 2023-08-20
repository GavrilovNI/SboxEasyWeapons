using EasyWeapons.Utils;
using Sandbox;
using System;
using System.Threading.Tasks;

namespace EasyWeapons.Bullets.Datas;

[GameResource("Entity Bullet Data", "bentity", "", Category = "bullets", Icon = "bullet")]
public class EntityBulletData : BulletData
{
    public string BulletEntityName { get; set; } = "TracingBulletEntity";
    public string ModelName { get; set; } = null!;
    public float StartVelocity { get; set; } = 5000;


    public Type? GetEntityType()
    {
        var entityType = TypeLibrary.GetType<Entity>(BulletEntityName)?.TargetType;
        if(entityType is null)
        {
            Log.Error($"Bullet entity '{BulletEntityName}' not found");
            return null;
        }

        return entityType;
    }

    public async Task Initialize(Entity entity, Ray ray)
    {
        if(entity is ModelEntity modelEntity)
            modelEntity.Model = await CloudUtils.LoadModel(ModelName);

        if(entity.IsValid() == false)
            return;

        entity.Position = ray.Position;
        var initialDirection = ray.Forward;
        entity.Rotation = initialDirection.EulerAngles.ToRotation();
        entity.Velocity = ray.Forward * StartVelocity;
        Log.Info("test3");
    }
}
