using EasyWeapons.Mathematics;
using EasyWeapons.ViewModels;
using Sandbox;
using System;

namespace EasyWeapons.Weapons.Modules.Aiming.Effects;

public partial class ViewModelPositioningEffect : AimingEffect
{
    public struct ViewModelPositioning
    {
        public Vector3 PositionOffset { get; set; } = Vector3.Zero;

        public Rotation RotationOffset { get; set; } = Rotation.Identity;

        public float FieldOfView { get; set; } = 60;


        public ViewModelPositioning()
        {
        }

        public bool AlmostEqual(ViewModelPositioning other)
        {
            return PositionOffset.AlmostEqual(other.PositionOffset) &&
                    (RotationOffset * other.RotationOffset.Inverse).Angle().AlmostEqual(0f) &&
                    FieldOfView.AlmostEqual(other.FieldOfView);
        }
    }

    [Net, Local]
    public float MoveSpeed { get; set; } = 100f;

    [Net, Local]
    public float RotationSpeed { get; set; } = 200f;

    [Net, Local]
    public float FieldOfViewChangeSpeed { get; set; } = 200f;


    [Net, Local]
    public ViewModelPositioning DefaultPositioning { get; set; }

    [Net, Local]
    public ViewModelPositioning AimedPositioning { get; set; }


    [Net, Predicted, Local]
    public ViewModelPositioning CurrentPositioning { get; protected set; } = new();

    [Net, Predicted, Local]
    public ViewModelPositioning TargetPositioning { get; protected set; } = new();


    protected ViewModelPositioning CurrentPositioningOnClient { get; set; } = new();


    public ViewModelPositioningEffect() : base()
    {
        Game.AssertClient();
        Event.Register(this);
    }

    public ViewModelPositioningEffect(Weapon weapon) : base(weapon)
    {
        Event.Register(this);
    }

    ~ViewModelPositioningEffect()
    {
        Event.Unregister(this);
    }


    public override void EnableAiming()
    {
        TargetPositioning = AimedPositioning;
    }

    public override void DisableAiming()
    {
        TargetPositioning = DefaultPositioning;
    }

    public override void Cancel()
    {
        TargetPositioning = DefaultPositioning;
        CurrentPositioning = DefaultPositioning;
        CurrentPositioningOnClient = DefaultPositioning;
        ApplyPositioning(CurrentPositioning);
    }

    public override SimulationResult Simulate()
    {
        bool finished = CurrentPositioning.AlmostEqual(TargetPositioning);
        if(finished)
            ApplyPositioning(CurrentPositioning);

        CurrentPositioning = TranslatePositioning(CurrentPositioning);
        return finished ? SimulationResult.Finished : SimulationResult.Continuing;
    }

    protected virtual ViewModelPositioning TranslatePositioning(ViewModelPositioning currentPositioning)
    {
        return new ViewModelPositioning()
        {
            PositionOffset = MovingMath.Translate(currentPositioning.PositionOffset, TargetPositioning.PositionOffset, Time.Delta * MoveSpeed),
            RotationOffset = MovingMath.Translate(currentPositioning.RotationOffset, TargetPositioning.RotationOffset, Time.Delta * RotationSpeed),
            FieldOfView = MovingMath.Translate(currentPositioning.FieldOfView, TargetPositioning.FieldOfView, Time.Delta * FieldOfViewChangeSpeed),
        };
    }

    public virtual void ApplyPositioning(ViewModelPositioning currentPositioning)
    {
        if(Weapon.ViewModelEntity is not WeaponViewModel viewModel)
            return;

        viewModel.PositionOffset = currentPositioning.PositionOffset;
        viewModel.RotationOffset = currentPositioning.RotationOffset;
        viewModel.FieldOfView = currentPositioning.FieldOfView;
    }

    [GameEvent.Client.Frame]
    protected virtual void OnFrame()
    {
        bool finished = CurrentPositioningOnClient.AlmostEqual(TargetPositioning);
        if(finished)
            return;

        CurrentPositioningOnClient = TranslatePositioning(CurrentPositioningOnClient);
        ApplyPositioning(CurrentPositioningOnClient);
    }
}
