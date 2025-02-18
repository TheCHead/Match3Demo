using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Views
{
    public class MainMenuView : MonoBehaviour, IView
    {
        public event Action OnStartGame;
        [SerializeField] private Button startGameButton;

        void Awake()
        {
            startGameButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void OnStartButtonClicked()
        {
            OnStartGame.Invoke();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}

