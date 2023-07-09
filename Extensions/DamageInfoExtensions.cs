using Sandbox;

namespace EasyWeapons.Extensions;

public static class DamageInfoExtensions
{
    public static DamageInfo AsBullet(this DamageInfo damageInfo, Vector3 hitPosition, Vector3 hitForce, float damage)
    {
        return damageInfo.WithPosition(hitPosition).WithForce(hitForce).WithDamage(damage).WithTag("bullet");
    }

    public static DamageInfo WithCollision(this DamageInfo damageInfo, CollisionEntityData collisionData)
    {
        return damageInfo.WithHitBody(collisionData.PhysicsShape.Body);
    }
}
