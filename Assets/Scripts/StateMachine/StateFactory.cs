using System;
using System.Collections.Generic;


namespace Scripts.StateMachine
{
    public class StateFactory
    {
        private readonly Dictionary<EGameState, IState> _states = new();

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
                EGameState.MainMenu => new MainMenuState(),
                EGameState.Match3 => new Match3State(),
                _ => throw new Exception($"{gameState} state not found.")
            };
        }
    }
}
