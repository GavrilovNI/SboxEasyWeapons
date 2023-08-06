using Sandbox;
using System;

namespace EasyWeapons.Bullets.Datas;

public partial class EntityBulletData : BulletData
{
    [Net, Predicted, Local]
    public string ModelCloudIdentity { get; set; } = null!;

    [Net, Predicted, Local]
    public string BulletEntityName { get; set; } = null!;

    [Net, Predicted, Local]
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

    public void InitializeEntity(Entity entity, Ray ray)
    {
        if(entity is ModelEntity modelEntity)
        {
            Log.Info(ModelCloudIdentity);
            modelEntity.Model = Cloud.Model(ModelCloudIdentity);
        }

        using(Prediction.Off())
        {
            entity.Position = ray.Position;
            var initialDirection = ray.Forward;
            entity.Rotation = initialDirection.EulerAngles.ToRotation();
            entity.Velocity = ray.Forward * StartVelocity;
        }
    }
}
