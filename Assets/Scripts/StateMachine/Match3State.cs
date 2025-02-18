using Cysharp.Threading.Tasks;
using Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Scripts.States
{
    public class Match3State : IState
    {
        private UIService _uiService;
        public Match3State(UIService uiService)
        {
            _uiService = uiService;
        }
        public async UniTask Enter(IState prevState)
        {
            await SceneManager.LoadSceneAsync("Match3");
            _uiService.ShowScreen(EUIScreen.Score);
            await UniTask.NextFrame();
        }

        public async UniTask Exit(IState nextState)
        {
            await UniTask.NextFrame();
        }
    }
}
