using System;
using System.Collections.Generic;
using Scripts.UI.Models;
using Scripts.UI.Presenters;
using Scripts.UI.Views;
using Zenject;
namespace Scripts.UI
{
    public class PresenterFactory
    {
        private readonly ViewFactory _viewFactory;
        private readonly Dictionary<EUIScreen, Func<IPresenter>> _presenterBuilders;
        private readonly Dictionary<EUIScreen, IPresenter> _cachedPresenters;


        [Inject]
        public PresenterFactory(ViewFactory viewFactory)
        {
            _viewFactory = viewFactory;

            _presenterBuilders = new Dictionary<EUIScreen, Func<IPresenter>>
            {
                { EUIScreen.MainMenu, () => CreatePresenter<MainMenuPresenter, MainMenuModel, MainMenuView>() },
                { EUIScreen.Score, () => CreatePresenter<ScorePresenter, ScoreModel, ScoreView>() }
            };

            _cachedPresenters = new Dictionary<EUIScreen, IPresenter>();
        }

        private IPresenter CreatePresenter<TPresenter, TModel, TView>(params object[] additionalArgs)
        where TPresenter : IPresenter
        where TView : IView
        where TModel : new()
        {
            var view = _viewFactory.GetView<TView>();
            var model = new TModel();

            var args = new List<object> { model, view };
            args.AddRange(additionalArgs);
            return (TPresenter)Activator.CreateInstance(typeof(TPresenter), args.ToArray());
        }

        public IPresenter GetPresenter(EUIScreen screen)
        {
            if (_cachedPresenters.TryGetValue(screen, out var cachedPresenter))
            {
                return cachedPresenter;
            }

            if (_presenterBuilders.TryGetValue(screen, out var builder))
            {
                var newPresenter = builder();
                _cachedPresenters[screen] = newPresenter;
                return newPresenter;
            }

            throw new Exception($"No presenter found for view type {screen}");
        }
    }
}
