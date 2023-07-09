using Sandbox;

namespace EasyWeapons.Bullets;

public interface IBullet
{
    void Initialize(Ray ray, DamageInfo? damageInfo);
}