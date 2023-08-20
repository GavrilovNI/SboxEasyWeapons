using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyWeapons.Utils;

public static class CloudUtils
{
    public static bool IsPackageIdentifier(string identifier)
    {
        if(identifier.Count(x => x == '.') != 1)
            return false;

        if(identifier.EndsWith(".vmdl", System.StringComparison.OrdinalIgnoreCase))
            return false;

        if(identifier.EndsWith(".vmdl_c", System.StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    public static async Task<Package?> TryFetchPackage(string identity)
    {
        var package = await Package.FetchAsync(identity, false);

        if(package == null || package.Revision == null)
            return null;

        return package;
    }

    public static async Task<Package?> TryFetchPackageOfType(string identity, Package.Type type)
    {
        var package = await TryFetchPackage(identity);
        if(package is null)
            return null;

        if(package.PackageType != type)
            return null;

        return package;
    }

    public static async Task<string> MountAsModelName(Package package)
    {
        var model = package.GetMeta("PrimaryAsset", "models/dev/error.vmdl");
        await package.MountAsync();
        return model;
    }

    public static async Task<string> MountAsEntityName(Package package)
    {
        var entityName = package.GetMeta("PrimaryAsset", "");
        await package.MountAsync();
        return entityName;
    }

    public static async Task<Entity?> CreateEntity(string entityName)
    {
        if(IsPackageIdentifier(entityName))
        {
            var package = await TryFetchPackageOfType(entityName, Package.Type.Addon);
            if(package is null)
                return null;

            if(package.Tags.Contains("runtime") == false)
                return null;
            entityName = await MountAsEntityName(package);
        }

        var entityType = TypeLibrary.GetType<Entity>(entityName)?.TargetType;
        if(entityType is null)
            return null;

        if(!TypeLibrary.HasAttribute<SpawnableAttribute>(entityType))
            return null;

        return TypeLibrary.Create<Entity>(entityType);
    }

    public static async Task<Model?> LoadModel(string modelName)
    {
        if(IsPackageIdentifier(modelName))
        {
            var package = await TryFetchPackageOfType(modelName, Package.Type.Model);
            if(package is null)
                return null;

            if(package.Tags.Contains("runtime") == false)
                return null;
            modelName = await MountAsModelName(package);
        }

        return Model.Load(modelName);
    }
}
