using EasyWeapons.Entities;
using EasyWeapons.Entities.Components;
using Sandbox;
using System;

namespace EasyWeapons.Recoiles;

public partial class DefaultPlayerRecoilApplier : EntityComponent, IRecoilApplier, ISimulatedComponent
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
        if(Entity is not IControllableEntity controllable)
            return;

        var recoilAngles = GetSmoothAngles(AnglesToApply, RecoilSpeed);
        AnglesToApply -= recoilAngles;
        controllable.ViewAngles += recoilAngles;
        AppliedAngles += recoilAngles;
    }

    protected virtual void CompensateRecoil()
    {
        if(Entity is not IControllableEntity controllable)
            return;

        var compensatingAngles = GetSmoothAngles(AppliedAngles * -1, CompensateSpeed);
        AppliedAngles += compensatingAngles;
        controllable.ViewAngles += compensatingAngles;
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

    public virtual void Simulate(IClient client)
    {
        DoRecoil();
        CompensateRecoil();
    }
}
