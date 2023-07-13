using Sandbox;

namespace EasyWeapons.Effects.Animations.Parameters;
public partial class TransformAnimationParameter : AnimationParameter
{
    [Net, Predicted, Local]
    public Transform Value { get; set; }

    public override void Apply(AnimatedEntity entity)
    {
        entity.SetAnimParameter(Name, Value);
    }
}
