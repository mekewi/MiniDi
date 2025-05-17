using System.ComponentModel;
using UnityEngine;

namespace MiniDi
{
    public class GameObjectContainer : BaseContainer
    {
        protected override void Awake()
        {
            var sceneContainer = SceneContainerRegistry.GetForScene(gameObject.scene);
            if (sceneContainer != null)
                SetParent(sceneContainer);
            base.Awake();
            InstallBindings();
        }
        private void Start()
        {
            InjectSelfAndChildren();
        }

        private void InjectSelfAndChildren()
        {
            var monoBehaviours = GetComponentsInChildren<MonoBehaviour>(true); // include inactive
            foreach (var mb in monoBehaviours)
            {
                InjectDependencies(mb); // ✅ Inject here
            }
        }

    }
}