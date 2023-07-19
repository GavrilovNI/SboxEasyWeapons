using EasyWeapons.Entities.Components;
using EasyWeapons.Enums;
using EasyWeapons.Weapons.Modules.Aiming.Effects;
using Sandbox;
using System.Collections.Generic;

namespace EasyWeapons.Weapons.Modules.Aiming;

public partial class AimingModule : WeaponModule, ICancelableComponent
{
    [Net, Local]
    public string AimingAction { get; set; } = "attack2";

    [Net, Local]
    public IList<AimingEffect> Effects { get; set; } = null!;

    [Net, Predicted, Local]
    public EnabledState State { get; protected set; }


    public override SimulationResult Simulate(SimulationType simulationType)
    {
        if (State == EnabledState.Enabling || State == EnabledState.Enabled)
        {
            if(Input.Down(AimingAction) == false)
            {
                State = EnabledState.Disabling;
                foreach(var effect in Effects)
                    effect.DisableAiming();
            }
        }
        else
        {
            if(Input.Down(AimingAction))
            {
                State = EnabledState.Enabling;
                foreach(var effect in Effects)
                    effect.EnableAiming();
            }
        }

        if(State == EnabledState.Enabling || State == EnabledState.Disabling)
        {
            bool finished = true;
            foreach(var effect in Effects)
            {
                var result = effect.Simulate();
                if(result == SimulationResult.Continuing)
                    finished = false;
            }

            if(finished)
                State = State == EnabledState.Enabling ? EnabledState.Enabled : EnabledState.Disabled;

            return finished ? SimulationResult.Finished : SimulationResult.Continuing;
        }

        return SimulationResult.Finished;
    }

    public void Cancel()
    {
        foreach(var effect in Effects)
            effect.Cancel();
        State = EnabledState.Disabled;
    }
}
