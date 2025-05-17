using UnityEngine;
using UnityEngine.SceneManagement;
namespace MiniDi
{
    public class SceneContainer : BaseContainer
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (scene != gameObject.scene) return;

            InstallBindings();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        protected override void Awake()
        {
            if (ProjectContainer.Instance != null)
                SetParent(ProjectContainer.Instance);

            SceneContainerRegistry.Register(gameObject.scene, this);
            base.Awake();
        }
        private void Start()
        {
            InjectSelfAndChildren();
        }
        protected void InjectSelfAndChildren()
        {
            var scene = gameObject.scene;
            if (!scene.IsValid() || !scene.isLoaded) return;

            var rootObjects = scene.GetRootGameObjects();
            foreach (var root in rootObjects)
            {
                if (root.GetComponent<GameObjectContainer>() != null)
                    continue;

                var monoBehaviours = root.GetComponentsInChildren<MonoBehaviour>(true);
                foreach (var mb in monoBehaviours)
                {
                    InjectDependencies(mb);
                }
            }
        }

        private void OnDestroy()
        {
            SceneContainerRegistry.Register(gameObject.scene, null);
        }
    }
}