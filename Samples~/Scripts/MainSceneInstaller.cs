using UnityEngine;
namespace MiniDi.Samples
{
    public class MainSceneInstaller : InjectionInstallerBase
    {
        public override void SetContainer(BaseContainer baseContainer)
        {
            base.SetContainer(baseContainer);
            container.Bind<IMainScene>(new MainScene(), Lifetime.Singleton);
        }
    }
    public class MainScene : IMainScene
    {
        public void OnLoaded()
        {
            Debug.Log("OnLoaded > ");
        }
    }
}