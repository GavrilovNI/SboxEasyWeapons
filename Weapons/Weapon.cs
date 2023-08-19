using EasyWeapons.Effects;
using EasyWeapons.ViewModels;
using EasyWeapons.Weapons.Modules;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace EasyWeapons.Weapons;

public partial class Weapon : BaseCarriable
{
    [Net, Local]
    public IList<WeaponEffect> DeployEffects { get; set; } = null!;

    [Net, Local]
    public CitizenAnimationHelper.HoldTypes HoldType { get; set; } = CitizenAnimationHelper.HoldTypes.Pistol;

    [Net, Local]
    public CitizenAnimationHelper.Hand Handedness { get; set; } = CitizenAnimationHelper.Hand.Both;

    [Net, Local]
    public Model? ViewModel { get; set; }

    [Net, Local]
    public float DeployTime { get; set; } = 0.5f;

    [Net, Local]
    public Ray DefaultLocalAimRay { get; set; }

    [Net, Predicted]
    public bool UseOwnerAimRay { get; set; } = false;

    [Net, Predicted]
    public TimeSince TimeSinceDeploy { get; protected set; }


    protected IEnumerable<WeaponModule> Modules => Components.GetAll<WeaponModule>(true);

    public override Ray AimRay
    {
        get
        {
            if(UseOwnerAimRay && Owner != null)
                return Owner.AimRay;

            return new Ray(Transform.PointToWorld(DefaultLocalAimRay.Position * Scale), Transform.NormalToWorld(DefaultLocalAimRay.Forward).Normal);
        }
    }


    public override void CreateViewModel()
    {
        Game.AssertClient();

        ViewModelEntity = new WeaponViewModel()
        {
            Owner = Owner,
            EnableViewmodelRendering = true
        };

        if(ViewModel is not null)
            ViewModelEntity.Model = ViewModel;
        else
            ViewModelEntity.SetModel(ViewModelPath);
    }

    public override void Spawn()
    {
        base.Spawn();
        DefaultLocalAimRay = new Ray(Vector3.Zero, Transform.RotationToLocal(Rotation).Forward);

        Event.Register(this);
    }

    protected override void OnDestroy()
    {
        Event.Unregister(this);
    }

    [GameEvent.Tick.Server]
    protected virtual void ServerTick()
    {
        if(Owner.IsValid() == false)
            SimulateEnabledModule(SimulationType.Ticking);
    }

    public override void Simulate(IClient client)
    {
        base.Simulate(client);

        if(TimeSinceDeploy <= DeployTime)
        {
            SimulateEnabledModule(SimulationType.Simulating);
            return;
        }

        SimulateModules(SimulationType.Simulating);
    }

    protected override void OnComponentAdded(EntityComponent component)
    {
        if(component is WeaponModule)
            component.Enabled = false;
    }

    protected virtual SimulationResult HandleModuleSimulation(SimulationType simulationType, WeaponModule module)
    {
        var simulationResult = module.Simulate(simulationType);
        module.Enabled = simulationResult == SimulationResult.Continuing;
        return simulationResult;
    }

    protected virtual SimulationResult SimulateEnabledModule(SimulationType simulationType)
    {
        var enabledModules = Modules.Where(m => m.Enabled);
        bool hasEnabledModule = enabledModules.Any();

        if(hasEnabledModule)
        {
            WeaponModule module = enabledModules.First();
            return HandleModuleSimulation(simulationType, module);
        }
        return SimulationResult.Finished;
    }

    protected virtual SimulationResult SimulateModules(SimulationType simulationType)
    {
        var enabledModuleSimulatingResult = SimulateEnabledModule(simulationType);
        if(enabledModuleSimulatingResult == SimulationResult.Finished)
        {
            foreach(var module in Modules)
            {
                var simulationResult = HandleModuleSimulation(simulationType, module);
                if(simulationResult == SimulationResult.Continuing)
                    return simulationResult;
            }
        }

        return enabledModuleSimulatingResult;
    }

    protected virtual void DisableModules()
    {
        foreach(var module in Modules)
            module.Enabled = false;
    }

    public override void ActiveStart(Entity owner)
    {
        base.ActiveStart(owner);
        TimeSinceDeploy = 0;
        DeployEffects.Play(this);
    }

    public override void ActiveEnd(Entity ent, bool dropped)
    {
        base.ActiveEnd(ent, dropped);
        if(dropped == false)
            DisableModules();
    }

    public override void SimulateAnimator(CitizenAnimationHelper anim)
    {
        anim.HoldType = HoldType;
        anim.Handedness = Handedness;
        anim.AimBodyWeight = 1.0f;
    }
}
