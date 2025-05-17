using UnityEngine;
namespace MiniDi.Samples
{
    public class GameObjectInstaller : InjectionInstallerBase
    {
        public PlayerManager playerManager;
        public override void SetContainer(BaseContainer baseContainer)
        {
            base.SetContainer(baseContainer);
            container.Bind<IPlayerManager>(playerManager, Lifetime.Singleton);
        }
    }
}