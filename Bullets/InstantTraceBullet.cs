using EasyWeapons.Bullets.Datas;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace EasyWeapons.Bullets;

public partial class InstantTraceBullet : BaseNetworkable
{
    [Net, Local]
    public Ray Ray { get; set; }

    [Net, Local]
    public float Distance { get; set; }

    [Net, Local]
    public float TraceRadius { get; set; }

    [Net, Local]
    public IEntity? IgnoreEntity { get; set; }

    [Net, Local]
    public IList<string> TraceTags { get; set; } = null!;

    [Net, Local]
    public string WaterTag { get; set; } = "water";

    [Net, Local]
    public InstantTraceBulletData Data { get; set; } = null!;


    public InstantTraceBullet()
    {
        TraceTags = new List<string>() { "solid", "player", "npc", "glass" };
    }

    public void Run()
    {
        IEnumerable<TraceResult> traceResults;
        using(Prediction.Off())
            traceResults = Trace(Ray);

        foreach(var traceResult in traceResults)
        {
            traceResult.Surface.DoBulletImpact(traceResult);

            if(Game.IsServer == false || traceResult.Entity.IsValid() == false)
                return;

            var damageInfo = DamageInfo.FromBullet(traceResult.EndPosition, Data.HitForce, Data.Damage).UsingTraceResult(traceResult);
            traceResult.Entity.TakeDamage(damageInfo);
        }
    }

    protected virtual IEnumerable<TraceResult> Trace(Ray ray)
    {
        var trace = Sandbox.Trace.Ray(ray, Distance)
            .Size(Vector3.One * TraceRadius)
            .UseHitboxes()
            .WithAnyTags(TraceTags.ToArray());

        if(IgnoreEntity.IsValid())
            trace = trace.Ignore(IgnoreEntity);

        bool isStartingUnderWater = Sandbox.Trace.TestPoint(ray.Position, WaterTag);
        if(!isStartingUnderWater)
            trace = trace.WithAnyTags(WaterTag);

        var traceResult = trace.Run();

        if(traceResult.Hit)
            yield return traceResult;
    }
}
