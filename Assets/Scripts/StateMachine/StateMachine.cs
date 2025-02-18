using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Scripts.States
{
    public class StateMachine
    {
        private IState _currentState;
        private readonly Dictionary<EGameState, IState> _states = new();

        [Inject]
        public StateMachine(StateFactory factory)
        {
            foreach (EGameState stateType in Enum.GetValues(typeof(EGameState)))
            {
                _states[stateType] = factory.GetState(stateType);
            }

            EventBus.OnStateChangeRequested += ChangeState;
        }

        public async UniTask ChangeState(EGameState nextStateName)
        {
            if (!_states.TryGetValue(nextStateName, out var nextState))
            {
                Debug.LogError($"State {nextStateName} not found");
                return;
            }

            if (_currentState != null)
            {
                await _currentState.Exit(nextState);
            }
        
            await nextState.Enter(_currentState);
        
            _currentState = nextState;
        }
    }
}

