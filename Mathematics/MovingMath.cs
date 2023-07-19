using Sandbox;
using System;

namespace EasyWeapons.Mathematics;

public static class MovingMath
{
    public static float Translate(float from, float to, float distance, bool clamp = true)
    {
        var fromToDelta = to - from;
        float normal = MathF.Sign(fromToDelta);

        if(clamp)
            distance = MathF.Min(Math.Abs(fromToDelta), distance);

        var translateDelta = normal * distance;
        return from + translateDelta;
    }

    public static Vector3 Translate(Vector3 from, Vector3 to, float distance, bool clamp = true)
    {
        var fromToDelta = to - from;
        var normal = fromToDelta.Normal;

        if(clamp)
            distance = MathF.Min(fromToDelta.Length, distance);

        var translateDelta = normal * distance;
        return from + translateDelta;
    }

    public static Rotation Translate(Rotation from, Rotation to, float angle, bool clamp = true)
    {
        var fromToDelta = to * from.Inverse;
        var fromToDeltaAngle = fromToDelta.Angle();

        var normal = fromToDeltaAngle.AlmostEqual(0f) ? Rotation.Identity : fromToDelta / fromToDeltaAngle;

        if(clamp)
            angle = MathF.Min(fromToDeltaAngle, angle);

        var translateDelta = normal * angle;
        return from * translateDelta;
    }
}
