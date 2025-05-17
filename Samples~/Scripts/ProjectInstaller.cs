using UnityEngine;
namespace MiniDi.Samples
{
    public class ProjectInstaller : InjectionInstallerBase
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            container.Bind<IServerManager>(new ServerManager(), Lifetime.Singleton);
        }
    }
}