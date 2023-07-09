using EasyWeapons.Extensions;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyWeapons.Bullets.Spawners;

public partial class TraceBulletSpawner : BulletSpawner
{
    [Net, Local]
    public float Force { get; set; }

    [Net, Local]
    public float Damage { get; set; }

    [Net, Local]
    public float Distance { get; set; }

    [Net, Local]
    public float Radius { get; set; }

    [Net, Local]
    public float Spread { get; set; }

    [Net, Local]
    public IEntity? IgnoreEntity { get; set; }

    [Net, Local]
    public IList<string> TraceTags { get; set; }

    [Net, Local]
    public string WaterTag { get; set; } = "water";



    public TraceBulletSpawner()
    {
        Game.AssertClient();
        Damage = 0;
        Distance = 0;
        Radius = 0;
        TraceTags = null!;
    }

    public TraceBulletSpawner(float spread, float force, float damage, float distance, float radius, IEntity? ignoreEntity = null)
    {
        Spread = spread;
        Force = force;
        Damage = damage;
        Distance = distance;
        Radius = radius;
        IgnoreEntity = ignoreEntity;
        TraceTags = new List<string>() { "solid", "player", "npc", "glass" };
    }

    public override void Spawn(Ray ray, DamageInfo? damageInfo)
    {
        Game.SetRandomSeed(Time.Tick);

        var forward = ray.Forward;
        forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) / 4f * Spread;
        forward = forward.Normal;

        ray.Forward = forward;

        IEnumerable<TraceResult> traceResults;
        using(Prediction.Off())
            traceResults = DoTrace(ray);

        foreach(var traceResult in traceResults)
        {
            traceResult.Surface.DoBulletImpact(traceResult);

            if(!Game.IsServer)
                continue;
            if(!traceResult.Entity.IsValid())
                continue;

            var totalDamageInfo = damageInfo.GetValueOrDefault(new DamageInfo());
            totalDamageInfo = totalDamageInfo.AsBullet(traceResult.EndPosition, traceResult.Direction * Force, Damage).UsingTraceResult(traceResult);
            traceResult.Entity.TakeDamage(totalDamageInfo);
        }
    }

    protected virtual IEnumerable<TraceResult> DoTrace(Ray ray)
    {
        var trace = Trace.Ray(ray, Distance)
            .Size(Vector3.One * Radius)
            .UseHitboxes()
            .WithAnyTags(TraceTags.ToArray());

        if(IgnoreEntity != null && IgnoreEntity.IsValid())
            trace = trace.Ignore(IgnoreEntity);

        bool isStartingUnderWater = Trace.TestPoint(ray.Position, WaterTag);
        if(!isStartingUnderWater)
            trace = trace.WithAnyTags(WaterTag);

        var traceResult = trace.Run();

        if(traceResult.Hit)
            yield return traceResult;
    }
}
