using Sandbox;

namespace EasyWeapons.Bullets.Datas;

public partial class InstantTraceBulletData : BulletData
{
    [Net, Predicted, Local]
    public float HitForce { get; set; }

    [Net, Predicted, Local]
    public float Damage { get; set; }


    public virtual void OnHit(TraceResult traceResult)
    {
        traceResult.Surface.DoBulletImpact(traceResult);

        if(!Game.IsServer || !traceResult.Entity.IsValid())
            return;

        var damageInfo = DamageInfo.FromBullet(traceResult.EndPosition, HitForce, Damage).UsingTraceResult(traceResult);
        traceResult.Entity.TakeDamage(damageInfo);
    }
}
