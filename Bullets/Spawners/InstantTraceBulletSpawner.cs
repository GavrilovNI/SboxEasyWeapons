using EasyWeapons.Bullets.Datas;
using Sandbox;
using System.Collections.Generic;

namespace EasyWeapons.Bullets.Spawners;

public partial class InstantTraceBulletSpawner : BulletSpawner<InstantTraceBulletData>
{
    [Net, Predicted, Local]
    public float Spread { get; set; }

    [Net, Predicted, Local]
    public float Distance { get; set; }

    [Net, Predicted, Local]
    public float TraceRadius { get; set; }

    [Net, Predicted, Local]
    public IEntity? IgnoreEntity { get; set; }

    [Net, Predicted, Local]
    public IList<string> TraceTags { get; set; } = null!;

    [Net, Predicted, Local]
    public string WaterTag { get; set; } = "water";

    public InstantTraceBulletSpawner()
    {
        Game.AssertClient();
        TraceTags = null!;
    }

    public InstantTraceBulletSpawner(float spread, float distance, float traceRadius, IEntity? ignoreEntity = null)
    {
        Spread = spread;
        Distance = distance;
        TraceRadius = traceRadius;
        IgnoreEntity = ignoreEntity;
        TraceTags = new List<string>() { "solid", "player", "npc", "glass" };
    }

    public override void Spawn(Ray ray, InstantTraceBulletData data)
    {
        ray = ApplySpread(ray);

        var bullet = new InstantTraceBullet()
        {
            Ray = ray,
            Distance = Distance,
            TraceRadius = TraceRadius,
            IgnoreEntity = IgnoreEntity,
            TraceTags = TraceTags,
            WaterTag = WaterTag,
            Data = data,
        };

        bullet.Run();
    }

    protected virtual Ray ApplySpread(Ray ray)
    {
        Game.SetRandomSeed(Time.Tick);

        var forward = ray.Forward;
        forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) / 4f * Spread;
        forward = forward.Normal;

        return new Ray(ray.Position, forward);
    }
}
