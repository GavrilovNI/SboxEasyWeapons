using Sandbox;

namespace EasyWeapons.Effects.Animations.Parameters;
public partial class FloatAnimationParameter : AnimationParameter
{
    [Net, Predicted, Local]
    public float Value { get; set; }

    public override void Apply(AnimatedEntity entity)
    {
        entity.SetAnimParameter(Name, Value);
    }
}
