using EasyWeapons.Bullets.Datas;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace EasyWeapons.Bullets.Spawners;

public partial class InstantTraceBulletSpawner : BulletSpawner
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

    public override void Spawn(Ray ray, IBulletDataSet bulletDataSet, string ammoId)
    {
        InstantTraceBulletData? bulletData = bulletDataSet.Get<InstantTraceBulletData>(ammoId);
        if(bulletData == null)
        {
            Log.Error($"{nameof(InstantTraceBulletData)} wasn't found for {nameof(ammoId)} {ammoId}");
            return;
        }

        ray = ApplySpread(ray);

        IEnumerable<TraceResult> traceResults;
        using(Prediction.Off())
            traceResults = Trace(ray);

        foreach(var traceResult in traceResults)
            bulletData.OnHit(traceResult);

    }


    protected virtual Ray ApplySpread(Ray ray)
    {
        Game.SetRandomSeed(Time.Tick);

        var forward = ray.Forward;
        forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) / 4f * Spread;
        forward = forward.Normal;

        return new Ray(ray.Position, forward);
    }

    protected virtual IEnumerable<TraceResult> Trace(Ray ray)
    {
        var trace = Sandbox.Trace.Ray(ray, Distance)
            .Size(Vector3.One * TraceRadius)
            .UseHitboxes()
            .WithAnyTags(TraceTags.ToArray());

        if(IgnoreEntity != null && IgnoreEntity.IsValid())
            trace = trace.Ignore(IgnoreEntity);

        bool isStartingUnderWater = Sandbox.Trace.TestPoint(ray.Position, WaterTag);
        if(!isStartingUnderWater)
            trace = trace.WithAnyTags(WaterTag);

        var traceResult = trace.Run();

        if(traceResult.Hit)
            yield return traceResult;
    }
}
