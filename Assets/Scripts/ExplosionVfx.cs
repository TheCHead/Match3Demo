using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class ExplosionVfx : MonoBehaviour, IPoolObject<ExplosionVfx>
{
    [SerializeField] private ParticleSystem _particles;
    private IObjectPool<ExplosionVfx> _pool;

    public void InitializePoolObject(IObjectPool<ExplosionVfx> pool)
    {
        _pool = pool;
    }
    
    public async void Fire(Vector2 position)
    {
        _particles.transform.position = position;
        _particles.Play();
        await UniTask.Delay(1000);
        _pool.Release(this);
    }
}
