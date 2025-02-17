using DG.Tweening;
using TMPro;
using UnityEngine;


namespace Scripts.UI.Views
{
    public class ScoreView : MonoBehaviour, IScoreView
    {
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text totalLabel;

        private float _score;
        private float _mult;
        private float _total;
        private Tween _scoreTween;

        public void AddScore(float score, float mult)
        {
            _score += score;
            _mult += mult;

            scoreLabel.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
            scoreLabel.text = $"{_score} x {_mult} = {_score * _mult}";
        }

        public void UpdateTotal()
        {
            float finalScore = _score * _mult;

            float result = finalScore;
            scoreLabel.transform.localScale = Vector3.one;

            DOTween.To(() => result, x => result = x, 0, 1).OnUpdate(() =>
            {
                scoreLabel.text = Mathf.RoundToInt(result).ToString();
            });
            DOTween.To(() => _total, x => _total = x, _total + finalScore, 1).OnUpdate(() =>
            {
                totalLabel.text = Mathf.RoundToInt(_total).ToString();
            });
        }

        public void Reset()
        {
            _score = 0;
            _mult = 0;
        }
    }

    public interface IScoreView : IView
    {
        public void AddScore(float score, float mult);
        public void UpdateTotal();
        public void Reset();
    }
}
