using Sandbox;

namespace EasyWeapons.Effects.Animations.Parameters;

public abstract partial class AnimationParameter : BaseNetworkable
{
    [Net, Predicted, Local]
    public string Name { get; set; } = null!;

    public abstract void Apply(AnimatedEntity entity);

    public static BoolAnimationParameter Of(string name, bool value) => new() { Name = name, Value = value };
    public static FloatAnimationParameter Of(string name, float value) => new() { Name = name, Value = value };
    public static IntAnimationParameter Of(string name, int value) => new() { Name = name, Value = value };
    public static RotationAnimationParameter Of(string name, Rotation value) => new() { Name = name, Value = value };
    public static TransformAnimationParameter Of(string name, Transform value) => new() { Name = name, Value = value };
    public static Vector3AnimationParameter Of(string name, Vector3 value) => new() { Name = name, Value = value };
}
