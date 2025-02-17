using System.Collections.Generic;
using Scripts.UI.Presenters;
using UnityEngine;

namespace Scripts.UI
{
    public class UIService : MonoBehaviour
    {
        [SerializeField] private Transform uiContainer;

        private ViewFactory _viewFactory;
        private PresenterFactory _presenterFactory;
        private readonly Dictionary<EUIScreen, IPresenter> _activePresenters = new();

        private void Awake()
        {
            _viewFactory = new ViewFactory(uiContainer);
            _presenterFactory = new PresenterFactory(_viewFactory);
        }

        void Start()
        {
            ShowScreen(EUIScreen.Score);
        }

        public void ShowScreen(EUIScreen screen)
        {
            if (_activePresenters.ContainsKey(screen))
                return; // Screen is already active

            IPresenter presenter = _presenterFactory.GetPresenter(screen);

            _activePresenters[screen] = presenter;
        }

        public void HideScreen(EUIScreen screen)
        {
            if (_activePresenters.TryGetValue(screen, out var presenter))
            {
                _activePresenters.Remove(screen);
                //presenter.Dispose();
                //_presenterFactory.DisposePresenter(screen); // Remove from cache
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


