using EasyWeapons.Enums;
using EasyWeapons.Mathematics;
using Sandbox;

namespace EasyWeapons.Weapons.Modules.Aiming.Effects;

public partial class FovEffect : AimingEffect
{
    [Net, Local]
    public float FieldOfView { get; set; } = 30;

    [Net, Local]
    public float FieldOfViewChangeSpeed { get; set; } = 200;

    [Net, Predicted, Local]
    public EnabledState State { get; set; } = EnabledState.Disabled;

    [Net, Predicted, Local]
    public float OldFieldOfView { get; protected set; }

    [Net, Predicted, Local]
    public float TargetFieldOfView { get; set; }

    [Net, Predicted, Local]
    public float CurrentFieldOfView { get; set; }

    protected float CurrentFieldOfViewOnClient { get; set; }


    public FovEffect() : base()
    {
        Game.AssertClient();
        Event.Register(this);
    }

    public FovEffect(Weapon weapon) : base(weapon)
    {
        Event.Register(this);
    }

    ~FovEffect()
    {
        Event.Unregister(this);
    }

    public override void Cancel()
    {
        CurrentFieldOfView = OldFieldOfView;
        CurrentFieldOfViewOnClient = OldFieldOfView;
        Camera.FieldOfView = OldFieldOfView;
        State = EnabledState.Disabled;
    }

    public override void EnableAiming()
    {
        OldFieldOfView = Camera.FieldOfView;
        State = EnabledState.Enabling;
        StartLerping(FieldOfView);
    }

    public override void DisableAiming()
    {
        State = EnabledState.Disabling;
        StartLerping(OldFieldOfView);
    }

    public override SimulationResult Simulate()
    {
        CurrentFieldOfView = TranslateFieldOfView(CurrentFieldOfView);
        bool rechedTarget = CurrentFieldOfView.AlmostEqual(TargetFieldOfView);

        if(rechedTarget)
            OnFinishedLerping();

        return State.IsChanging() ? SimulationResult.Continuing : SimulationResult.Finished;
    }

    protected virtual void OnFinishedLerping()
    {
        CurrentFieldOfView = TargetFieldOfView;
        CurrentFieldOfViewOnClient = TargetFieldOfView;
        Camera.FieldOfView = TargetFieldOfView;
        State = State.ToFinishedState();
    }

    protected virtual void StartLerping(float targetFieldOfView)
    {
        CurrentFieldOfView = Camera.FieldOfView;
        CurrentFieldOfViewOnClient = CurrentFieldOfView;
        TargetFieldOfView = targetFieldOfView;
    }

    protected virtual float TranslateFieldOfView(float currentFieldOfView)
    {
        return MovingMath.Translate(CurrentFieldOfView, TargetFieldOfView, Time.Delta * FieldOfViewChangeSpeed);
    }

    [GameEvent.Client.PostCamera]
    protected virtual void OnPreRender()
    {
        if(State == EnabledState.Disabled)
            return;

        CurrentFieldOfViewOnClient = TranslateFieldOfView(CurrentFieldOfViewOnClient);
        Camera.FieldOfView = CurrentFieldOfViewOnClient;
    }
}
