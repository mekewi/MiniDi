using UnityEngine;
namespace MiniDi.Samples
{
    public class GameManager : MonoBehaviour
    {
        [Inject] IServerManager serverManager;
        [Inject] IMainScene mainScene;
        private void Start()
        {
            serverManager.Connect();
            mainScene.OnLoaded();
        }
    }
}