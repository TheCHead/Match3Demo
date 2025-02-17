using Cysharp.Threading.Tasks;
using UnityEngine;


namespace Scripts.StateMachine
{
    public class MainMenuState : IState
    {
        public async UniTask Enter(IState prevState)
        {
            Debug.Log("Enter Main Menu");
            await UniTask.NextFrame();

        }

        public async UniTask Exit(IState nextState)
        {
            Debug.Log("Exit Main Menu");
            await UniTask.NextFrame();
        }
    }
}
