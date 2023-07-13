using Sandbox;

namespace EasyWeapons.Effects.Animations.Parameters;
public partial class IntAnimationParameter : AnimationParameter
{
    [Net, Predicted, Local]
    public int Value { get; set; }

    public override void Apply(AnimatedEntity entity)
    {
        entity.SetAnimParameter(Name, Value);
    }
}
