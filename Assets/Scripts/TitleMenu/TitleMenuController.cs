using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TitleMenu
{
    public class TitleMenuController : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button offlineStartButton;

        private void Awake()
        {
            startButton
                .OnClickAsObservable()
                .Subscribe(static _ =>
                {
                    ClientStarter.IsOfflineMode = false;
                    LoadGameScene();
                })
                .AddTo(this);

            offlineStartButton
                .OnClickAsObservable()
                .Subscribe(static _ =>
                {
                    ClientStarter.IsOfflineMode = true;
                    LoadGameScene();
                })
                .AddTo(this);
        }

        private static void LoadGameScene()
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}