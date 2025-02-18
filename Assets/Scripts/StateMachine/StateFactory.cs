using System;
using System.Collections.Generic;
using Scripts.UI;
using Zenject;


namespace Scripts.States
{
    public class StateFactory
    {
        private readonly Dictionary<EGameState, IState> _states = new();
        private UIService _uiService;

        [Inject]
        public StateFactory(UIService uIService)
        {
            _uiService = uIService;
        }

        public IState GetState(EGameState gameState)
        {
            if (!_states.ContainsKey(gameState))
            {
                _states[gameState] = CreateState(gameState);
            }

            return _states[gameState];
        }

        private IState CreateState(EGameState gameState)
        {

            return gameState switch
            {
                EGameState.MainMenu => new MainMenuState(_uiService),
                EGameState.Match3 => new Match3State(_uiService),
                _ => throw new Exception($"{gameState} state not found.")
            };
        }
    }
}
