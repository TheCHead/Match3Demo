using System.Collections.Generic;
using Scripts.UI.Presenters;
using Zenject;

namespace Scripts.UI
{
    public class UIService
    {
        private PresenterFactory _presenterFactory;
        private readonly Dictionary<EUIScreen, IPresenter> _activePresenters = new();

        [Inject]
        public UIService(PresenterFactory presenterFactory, ViewFactory viewFactory)
        {
            _presenterFactory = presenterFactory;
        }

        public void ShowScreen(EUIScreen screen)
        {
            if (_activePresenters.ContainsKey(screen))
                return; 

            IPresenter presenter = _presenterFactory.GetPresenter(screen);
            _activePresenters[screen] = presenter;
            presenter.Initialize();
        }

        public void HideScreen(EUIScreen screen)
        {
            if (_activePresenters.TryGetValue(screen, out var presenter))
            {
                _activePresenters.Remove(screen);
                presenter.Dispose();
            }
        }

        public IPresenter GetPresenter(EUIScreen screen)
        {
            if (_activePresenters.TryGetValue(screen, out var presenter))
            {
                return presenter;
            }
            return null;
        }
    }
}


