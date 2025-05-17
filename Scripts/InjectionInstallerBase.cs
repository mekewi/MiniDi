using UnityEngine;

namespace MiniDi
{
    public abstract class InjectionInstallerBase : MonoBehaviour
    {
        internal BaseContainer container;
        public virtual void SetContainer(BaseContainer baseContainer)
        {
            container = baseContainer;
        }
        public virtual void InstallBindings()
        {
        }
    }
}