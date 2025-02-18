using Scripts.UI.Models;
using Scripts.UI.Views;

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

        public void Dispose()
        {
            _view.Disable();
        }

        public void Initialize()
        {
            _view.Enable();
        }

        public void OnScore(float score, float mult)
        {
            _model.AddScore(score, mult);
            _view.UpdateScore(_model.GetScore(), _model.GetMultiplier());
        }

        public void UpdateTotal()
        {
            float fromTotal = _model.GetTotal();
            _model.UpdateTotal();

            _view.ResetScore(_model.GetScoreXMult());
            _view.UpdateTotal(fromTotal, _model.GetTotal());
            _model.SoftReset();
        }
    }

    public interface IScorePresenter : IPresenter
    {
        public void OnScore(float score, float mult);
        public void UpdateTotal();
    }
}
