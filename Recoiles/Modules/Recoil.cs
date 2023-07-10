using Sandbox;
using System;

namespace EasyWeapons.Recoiles.Modules;

public abstract class Recoil : BaseNetworkable
{
    public abstract void ApplyRecoil(IRecoilApplier recoilApplier);
}
