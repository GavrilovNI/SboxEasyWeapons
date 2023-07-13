using Sandbox;

namespace EasyWeapons.Effects.Animations.Parameters;
public partial class Vector3AnimationParameter : AnimationParameter
{
    [Net, Predicted, Local]
    public Vector3 Value { get; set; }

    public override void Apply(AnimatedEntity entity)
    {
        entity.SetAnimParameter(Name, Value);
    }
}
