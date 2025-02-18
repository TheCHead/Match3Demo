using DG.Tweening;
using TMPro;
using UnityEngine;


namespace Scripts.UI.Views
{
    public class ScoreView : MonoBehaviour, IScoreView
    {
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text totalLabel;

        public void UpdateScore(float score, float mult)
        {
            scoreLabel.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
            scoreLabel.text = $"{score} x {mult} = {score * mult}";
        }

        public void ResetScore(float from)
        {
            scoreLabel.transform.localScale = Vector3.one;
            DOTween.To(() => from, x => from = x, 0, 1).OnUpdate(() =>
            {
                scoreLabel.text = Mathf.RoundToInt(from).ToString();
            });
        }

        public void UpdateTotal(float from, float to)
        {            
            DOTween.To(() => from, x => from = x, to, 1).OnUpdate(() =>
            {
                totalLabel.text = Mathf.RoundToInt(from).ToString();
            });
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }

    public interface IScoreView : IView
    {
        public void UpdateScore(float score, float mult);
        public void UpdateTotal(float from, float to);
        public void ResetScore(float from);
    }
}
