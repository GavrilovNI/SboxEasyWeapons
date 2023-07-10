using Sandbox;
using System;

namespace EasyWeapons.Recoiles;

public interface IRecoilApplier : IComponent
{
    void ApplyRecoil(Angles angles);
}
