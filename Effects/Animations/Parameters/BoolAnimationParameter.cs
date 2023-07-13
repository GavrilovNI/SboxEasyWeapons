using Sandbox;

namespace EasyWeapons.Effects.Animations.Parameters;
public partial class BoolAnimationParameter : AnimationParameter
{
    [Net, Predicted, Local]
    public bool Value { get; set; }

    public override void Apply(AnimatedEntity entity)
    {
        entity.SetAnimParameter(Name, Value);
    }
}
