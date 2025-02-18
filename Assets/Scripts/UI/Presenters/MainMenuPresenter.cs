using Cysharp.Threading.Tasks;
using Scripts.UI.Models;
using Scripts.UI.Views;

namespace Scripts.UI.Presenters
{
    public class MainMenuPresenter : IPresenter
    {
        private MainMenuModel _model;
        private MainMenuView _view;

        public MainMenuPresenter(MainMenuModel model, MainMenuView view)
        {
            _model = model;
            _view = view;
            _view.OnStartGame += OnStartGame;
        }

        public void Dispose()
        {
            _view.Disable();
        }

        public void Initialize()
        {
            _view.Enable();
        }

        private void OnStartGame()
        {
            EventBus.RequestStateChange(States.EGameState.Match3).Forget();
        }
    }
}

