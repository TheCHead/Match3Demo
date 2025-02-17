using Scripts.UI.Models;
using Scripts.UI.Views;
using UnityEngine;

namespace Scripts.UI.Presenters
{
    public class ScorePresenter : IScorePresenter
    {
        private ScoreModel _model;
        private IScoreView _view;

        public ScorePresenter(ScoreModel model, IScoreView view)
        {
            _model = model;
            _view = view;
        }

        public void Initialize()
        {
            Debug.Log("Score Initialized");
        }

        public void AddScore(float score, float mult)
        {
            _view.AddScore(score, mult);
        }

        public void UpdateTotal()
        {
            _view.UpdateTotal();
        }

        public void Reset()
        {
            _view.Reset();
        }
    }

    public interface IScorePresenter : IPresenter
    {
        public void AddScore(float score, float mult);
        public void UpdateTotal();
        public void Reset();
    }
}
