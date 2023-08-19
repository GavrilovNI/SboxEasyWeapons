using Sandbox;

namespace EasyWeapons.Entities;

public interface IControllableEntity : IEntity
{
    Vector3 InputDirection { get; }
    Angles ViewAngles { get; set; }

    Trace CreateTrace(float liftFeet = 0f);
}
