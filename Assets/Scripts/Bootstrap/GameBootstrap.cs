using Cysharp.Threading.Tasks;
using Scripts.States;
using UnityEngine;
using Zenject;

public class GameBootstrap : MonoBehaviour
{
    private StateMachine _stateMachine;

    [Inject]
    public void Initialize(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    void Start()
    {
        _stateMachine.ChangeState(EGameState.MainMenu).Forget();
    }
}
