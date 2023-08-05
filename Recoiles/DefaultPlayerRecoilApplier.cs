using EasyWeapons.Events;
using Sandbox;
using System;

namespace EasyWeapons.Recoiles;

public partial class DefaultPlayerRecoilApplier : EntityComponent<Sandbox.Player>, IRecoilApplier
{
    [Net, Predicted, Local]
    public Angles AnglesToApply { get; set; } = Angles.Zero;

    [Net, Predicted, Local]
    public Angles AppliedAngles { get; set; } = Angles.Zero;

    [Net, Local]
    public float RecoilSpeed { get; set; } = 100f;

    [Net, Local]
    public float CompensateSpeed { get; set; } = 35f;


    public void ApplyRecoil(Angles angles)
    {
        AnglesToApply += angles.WithRoll(0);
    }

    protected virtual Angles GetSmoothAngles(Angles angles, float speed)
    {
        var angle = MathF.Sqrt(angles.pitch * angles.pitch + angles.yaw * angles.yaw);
        if(angle.AlmostEqual(0f))
            return Angles.Zero;

        var angleToApply = speed * Time.Delta;
        angleToApply = Math.Min(angleToApply, angle);

        var anglesNormilized = angles / angle;
        return anglesNormilized * angleToApply;
    }

    protected virtual void DoRecoil()
    {
        var recoilAngles = GetSmoothAngles(AnglesToApply, RecoilSpeed);
        AnglesToApply -= recoilAngles;
        Entity.ViewAngles += recoilAngles;
        AppliedAngles += recoilAngles;
    }

    protected virtual void CompensateRecoil()
    {
        var compensatingAngles = GetSmoothAngles(AppliedAngles * -1, CompensateSpeed);
        AppliedAngles += compensatingAngles;
        Entity.ViewAngles += compensatingAngles;
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        Event.Register(this);
    }

    protected override void OnDeactivate()
    {
        Event.Unregister(this);
        base.OnDeactivate();
    }


    [CustomGameEvent.PreSimulate]

    protected virtual void OnPreSimulate(IClient client)
    {
        if(object.ReferenceEquals(Entity.Client, client) == false)
            return;

        DoRecoil();
        CompensateRecoil();
    }
}
