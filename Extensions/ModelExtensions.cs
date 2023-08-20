using Sandbox;

namespace EasyWeapons.Extensions;

public static class ModelExtensions
{
    public static bool IsError(this Model? model)
    {
        return model is null || model.IsError || model.ResourcePath == "models/dev/error.vmdl";
    }

    public static bool IsCitizen(this Model? model)
    {
        return model is not null && model.IsError == false && model.ResourcePath == "models/citizen/citizen.vmdl";
    }
}
