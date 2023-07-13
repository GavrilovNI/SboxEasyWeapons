using Sandbox;

namespace EasyWeapons.Effects.Animations.Parameters;
public partial class RotationAnimationParameter : AnimationParameter
{
    [Net, Predicted, Local]
    public Rotation Value { get; set; }

    public override void Apply(AnimatedEntity entity)
    {
        entity.SetAnimParameter(Name, Value);
    }
}
