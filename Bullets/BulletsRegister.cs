using EasyWeapons.Bullets.Datas;
using Sandbox;
using System.Collections.Generic;

namespace EasyWeapons.Bullets;

public partial class BulletsRegister : EntityComponent, ISingletonComponent, IBulletDataSet
{
    public static BulletsRegister Instanse => GameManager.Current.Components.GetOrCreate<BulletsRegister>();

    [Net]
    protected IDictionary<string, RegisteredBulletDatas> BulletDatas { get; set; } = new Dictionary<string, RegisteredBulletDatas>(); // TODO : use GUID instead of string ammoId


    public void Add<T>(string ammoId, T data) where T : BulletData
    {
        Game.AssertServer();

        var datas = GetOrCreateBulletDatasByBulletId(ammoId);
        datas.Add(data);
    }

    public bool Contains<T>(string ammoId) where T : BulletData
    {
        if(BulletDatas.TryGetValue(ammoId, out var datas))
            return datas.Contains<T>();

        return false;
    }

    public T? Get<T>(string ammoId) where T : BulletData
    {
        if(BulletDatas.TryGetValue(ammoId, out var datas))
            return datas.Get<T>();

        return null;
    }


    protected RegisteredBulletDatas GetOrCreateBulletDatasByBulletId(string ammoId)
    {
        if(BulletDatas.TryGetValue(ammoId, out var result))
            return result;

        result = new RegisteredBulletDatas();
        BulletDatas.Add(ammoId, result);

        return result;
    }


    protected partial class RegisteredBulletDatas : BaseNetworkable
    {
        [Net]
        protected IDictionary<string, BulletData> BulletDataByClass { get; set; } = new Dictionary<string, BulletData>();

        public void Add<T>(T data) where T : BulletData
        {
            BulletDataByClass[GetDataId<T>()] = data;
        }

        public bool Contains<T>() where T : BulletData
        {
            return BulletDataByClass.ContainsKey(GetDataId<T>());
        }

        public T? Get<T>() where T : BulletData
        {
            if(BulletDataByClass.TryGetValue(GetDataId<T>(), out var bulletData))
                return bulletData as T;

            return null;
        }

        protected string GetDataId<T>() where T : BulletData
        {
            return typeof(T).FullName!; // TODO : use GUID instead of class FullName
        }
    }
}
