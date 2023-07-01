using EasyWeapons.Weapons.Modules;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace EasyWeapons.Weapons;

public partial class Weapon : BaseCarriable
{
    [Net, Local]
    public string? ViewMoidelArmsPath { get; set; } = "models/first_person/first_person_arms.vmdl";

    [Net, Local]
    public Model? ViewModel { get; set; }

    [Net, Local]
    public float DeployTime { get; set; } = 0.5f;

    [Net, Local]
    public Ray DefaultLocalAimRay { get; set; }


    [Net, Predicted]
    public TimeSince TimeSinceDeploy { get; protected set; }


    public AnimatedEntity? ViewModelArms { get; set; }

    protected IEnumerable<WeaponModule> Modules => Components.GetAll<WeaponModule>(true);

    public override Ray AimRay => Owner == null
        ? new Ray(Transform.PointToWorld(DefaultLocalAimRay.Position * Scale), Transform.NormalToWorld(DefaultLocalAimRay.Forward).Normal)
        : Owner.AimRay;



    public override void CreateViewModel()
    {
        Game.AssertClient();

        ViewModelEntity = new BaseViewModel();
        ViewModelEntity.Position = Position;
        ViewModelEntity.Owner = Owner;
        ViewModelEntity.EnableViewmodelRendering = true;

        if(ViewModel is not null)
            ViewModelEntity.Model = ViewModel;
        else
            ViewModelEntity.SetModel(ViewModelPath);

        if(ViewMoidelArmsPath is not null)
        {
            ViewModelArms = new AnimatedEntity(ViewMoidelArmsPath);
            ViewModelArms.SetParent(ViewModelEntity, true);
            ViewModelArms.EnableViewmodelRendering = true;
        }
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


    [Event.Tick.Server]
    private void ServerTick()
    {
        if(Owner.IsValid() == false)
            SimulateEnabledModule();
    }

    public override void Simulate(IClient client)
    {
        base.Simulate(client);

        if(TimeSinceDeploy <= DeployTime)
        {
            SimulateEnabledModule();
            return;
        }

        SimulateModules();
    }

    protected override void OnComponentAdded(EntityComponent component)
    {
        if(component is WeaponModule)
            component.Enabled = false;
    }

    protected virtual SimulationResult HandleModuleSimulation(WeaponModule module)
    {
        var simulationResult = module.Simulate();
        module.Enabled = simulationResult == SimulationResult.Continuing;
        return simulationResult;
    }

    protected virtual SimulationResult SimulateEnabledModule()
    {
        var enabledModules = Modules.Where(m => m.Enabled);
        bool hasEnabledModule = enabledModules.Any();

        if(hasEnabledModule)
        {
            WeaponModule module = enabledModules.First();
            return HandleModuleSimulation(module);
        }
        return SimulationResult.Finished;
    }

    protected virtual SimulationResult SimulateModules()
    {
        var enabledModuleSimulatingResult = SimulateEnabledModule();
        if(enabledModuleSimulatingResult == SimulationResult.Finished)
        {
            foreach(var module in Modules)
            {
                var simulationResult = HandleModuleSimulation(module);
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
        if(Game.IsServer)
        {
            using(Prediction.Off())
                TimeSinceDeploy = 0;
        }
        DoDeployEffects();
    }

    public override void ActiveEnd(Entity ent, bool dropped)
    {
        base.ActiveEnd(ent, dropped);
        if(dropped == false)
            DisableModules();
    }

    [ClientRpc]
    public void CreateParticle(string name, string attachment, bool follow = true)
    {
        Game.AssertClient();
        Particles.Create(name, EffectEntity, attachment, follow);
    }

    [ClientRpc]
    public void SetViewModelAnimParameter(string name, bool value)
    {
        Game.AssertClient();
        ViewModelEntity?.SetAnimParameter(name, value);
    }

    public virtual void DoDeployEffects()
    {
        if(Game.IsServer == false)
            return;

        //_ = DeploySound?.PlayOnEntity(this);
    }
}
