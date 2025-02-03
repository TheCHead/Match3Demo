using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(SpriteRenderer))]
public class Gem : MonoBehaviour, IPoolObject<Gem>
{
    [SerializeField] private ExplosionVfx explodeVfx;
    private GemTypeSO _type;
    private IObjectPool<Gem> _pool;
    private static IObjectPool<ExplosionVfx> _vfxPool;

    public void SetType(GemTypeSO type)
    {
        _type = type;
        GetComponent<SpriteRenderer>().sprite = type.sprite;
    }

    public GemTypeSO GetGemType() => _type;

    public void DestroyGem()
    {
        ExplodeVFX(transform.position);
        _pool.Release(this);
    }

    private void ExplodeVFX(Vector3 position)
    {
        var fx = _vfxPool.Get();
        fx.Fire(position);
    }

    public void InitializePoolObject(IObjectPool<Gem> pool)
    {
        _pool = pool;

        if (_vfxPool == null)
        {
            GameObject vfxs = new GameObject("Vfx");
            _vfxPool = new MonoPool<ExplosionVfx>(explodeVfx, vfxs.transform, 100).Pool;
        }
    }
}
