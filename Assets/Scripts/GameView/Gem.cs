using DG.Tweening;
using Scripts.DataModels;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.Pool;

namespace Scripts.GameView
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Gem : MonoBehaviour, IPoolObject<Gem>
    {
        [SerializeField] private ExplosionVfx explodeVfx;
        private GemTypeSO _type;
        private IObjectPool<Gem> _pool;
        private static IObjectPool<ExplosionVfx> _vfxPool;

        private bool _selected;
        private Tween _selectTween;
        private Tween _highlightTween;

        public void SetType(GemTypeSO type)
        {
            _type = type;
            GetComponent<SpriteRenderer>().sprite = type.sprite;
        }

        public GemTypeSO GetGemType() => _type;

        public void Select()
        {
            if (_selected)
                return;

            _selected = true;
            _selectTween = transform.DOScale(Vector3.one * 1.1f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        }

        public void Deselect()
        {
            if (!_selected)
                return;
            _selected = false;
            _selectTween.Kill();
            transform.localScale = Vector3.one;
        }

        public void Highlight(float time)
        {
            _highlightTween.Kill();
            transform.localScale = Vector3.one;
            _highlightTween = transform.DOScale(Vector3.one * 1.1f, time * 0.5f).SetLoops(2, LoopType.Yoyo);
        }

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

}

