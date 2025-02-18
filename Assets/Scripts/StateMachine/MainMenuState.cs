using Cysharp.Threading.Tasks;
using Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Scripts.States
{
    public class MainMenuState : IState
    {
        private UIService _uiService;
        public MainMenuState(UIService uiService)
        {
            _uiService = uiService;
        }

        public async UniTask Enter(IState prevState)
        {
            await SceneManager.LoadSceneAsync("MainMenu");
            _uiService.ShowScreen(EUIScreen.MainMenu);  
        }

        public async UniTask Exit(IState nextState)
        {
            await UniTask.NextFrame();
        }
    }
}
