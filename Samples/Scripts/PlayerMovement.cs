using UnityEngine;

namespace MiniDi.Samples
{
    public class PlayerMovement : MonoBehaviour
    {
        [Inject] public IPlayerManager playerManager;
        [Inject] public IServerManager serverManager;
        [Inject] public IMainScene mainScene;
        private void Start()
        {
            playerManager.StartMove();
            serverManager.Connect();
            mainScene.OnLoaded();
        }
    }
}