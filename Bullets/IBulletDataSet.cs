using EasyWeapons.Bullets.Datas;

namespace EasyWeapons.Bullets;

public interface IBulletDataSet
{
    void Add<T>(string ammoId, T data) where T : BulletData;
    bool Contains<T>(string ammoId) where T : BulletData;
    T? Get<T>(string ammoId) where T : BulletData;
}
