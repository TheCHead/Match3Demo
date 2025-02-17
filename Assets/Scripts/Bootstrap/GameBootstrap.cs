using Cysharp.Threading.Tasks;
using Scripts.StateMachine;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    void Start()
    {
        StateMachine stateMachine = new StateMachine(new StateFactory());

        stateMachine.ChangeState(EGameState.Match3).Forget();
    }
}
