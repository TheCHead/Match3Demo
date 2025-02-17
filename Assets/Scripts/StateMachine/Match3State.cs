using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Scripts.StateMachine
{
    public class Match3State : IState
    {
        public async UniTask Enter(IState prevState)
        {
            Debug.Log("Enter Match3Game");
            // TODO extract loading to SceneLoading service
            SceneManager.LoadScene("Match3");
            await UniTask.NextFrame();
        }

        public async UniTask Exit(IState nextState)
        {
            Debug.Log("Exit Match3Game");
            await UniTask.NextFrame();
        }
    }
}
