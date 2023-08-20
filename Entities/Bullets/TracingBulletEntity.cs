using EasyWeapons.Entities.Components;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyWeapons.Entities.Bullets;

[Spawnable]
public partial class TracingBulletEntity : ModelEntity
{
    [Net, Local]
    public Vector3 ExternalForce { get; set; } = Vector3.Down * 386f;

    [Net, Local]
    public float HitForce { get; set; } = 10f;

    [Net, Local]
    public float Damage { get; set; } = 10f;


    [Net, Local]
    public float TraceRadius { get; set; } = 3f;

    [Net, Local]
    public IList<string> TraceTags { get; set; }

    [Net, Local]
    public string WaterTag { get; set; } = "water";


    public DamageInfo DamageInfo { get; set; }

    public TracingBulletEntity()
    {
        EnableTouch = true;

        if(Game.IsServer)
            Components.Add(new TimedDestroyer() { TimeToDestroy = TimeSpan.FromMinutes(1) });

        TraceTags = new List<string>() { "solid", "player", "npc", "glass" };
    }

    public override void Spawn()
    {
        base.Spawn();

        PhysicsEnabled = false;
        UsePhysicsCollision = false;
        Tags.Add("bullet");
    }

    public void Initialize(Ray ray, DamageInfo? damageInfo)
    {
        Position = ray.Position;
        var initialDirection = ray.Forward;
        Rotation = initialDirection.EulerAngles.ToRotation();
        DamageInfo = damageInfo.GetValueOrDefault(new DamageInfo());
    }


    [GameEvent.Tick.Server]
    protected void Tick()
    {
        UpdateVelocity();
        Rotation = Rotation.LookAt(Velocity.Normal);

        Move();
    }

    protected virtual void Move()
    {
        var ray = new Ray(Position, Velocity.Normal);
        var distance = Velocity.Length * Time.Delta;

        IEnumerable<TraceResult> traceResults;
        using(Prediction.Off())
            traceResults = DoTrace(ray, distance);

        foreach(var traceResult in traceResults)
        {
            Position = traceResult.EndPosition;

            if(traceResult.Hit)
            {
                OnHit(traceResult);
                return;
            }
        }
    }

    protected virtual IEnumerable<TraceResult> DoTrace(Ray ray, float distance)
    {
        var trace = Trace.Ray(ray, distance)
            .Size(Vector3.One * TraceRadius)
            .UseHitboxes()
            .WithAnyTags(TraceTags.ToArray());

        if(DamageInfo.Weapon != null)
            trace = trace.Ignore(DamageInfo.Weapon);

        bool isStartingUnderWater = Trace.TestPoint(ray.Position, WaterTag);
        if(!isStartingUnderWater)
            trace = trace.WithAnyTags(WaterTag);

        var traceResult = trace.Run();

        yield return traceResult;
    }

    protected virtual void UpdateVelocity()
    {
        Velocity += ExternalForce * Time.Delta;
    }

    protected virtual void OnHit(TraceResult traceResult)
    {
        traceResult.Surface.DoBulletImpact(traceResult);

        if(!Game.IsServer)
            return;
        if(!traceResult.Entity.IsValid())
            return;

        var totalDamageInfo = DamageInfo.FromBullet(traceResult.EndPosition, traceResult.Direction * HitForce, Damage).UsingTraceResult(traceResult);
        traceResult.Entity.TakeDamage(totalDamageInfo);

        Delete();
    }
}
