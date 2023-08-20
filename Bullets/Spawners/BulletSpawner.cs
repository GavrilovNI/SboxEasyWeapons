using EasyWeapons.Bullets.Datas;
using Sandbox;

namespace EasyWeapons.Bullets.Spawners;

public abstract class BulletSpawner : BaseNetworkable
{
    public abstract void Spawn(Ray ray, string ammoId);
}

public abstract partial class BulletSpawner<T> : BulletSpawner where T : BulletData
{
    public abstract void Spawn(Ray ray, T data);

    public override void Spawn(Ray ray, string ammoId)
    {
        var extension = TypeLibrary.GetType<T>().GetAttribute<GameResourceAttribute>().Extension;
        var dataPath = $"bullets/{ammoId}.{extension}";
        var data = ResourceLibrary.Get<T>(dataPath);
        if(data == null)
        {
            Log.Error($"Bullet data of type '{typeof(T).FullName}' not found in '{dataPath}'");
            return;
        }

        Spawn(ray, data);
    }
}
