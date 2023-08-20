using Sandbox;

namespace EasyWeapons.Bullets.Datas;

[GameResource("Instant Trace Bullet Data", "btrace", "", Category = "bullets", Icon = "bullet")]
public class InstantTraceBulletData : BulletData
{
    public float HitForce { get; set; } = 5f;
    public float Damage { get; set; } = 10f;
}
