using System;
using System.Linq;
using UnityEngine.SceneManagement;
namespace MiniDi
{
    public class ProjectContainer : BaseContainer
    {
        public static ProjectContainer Instance;
        protected override void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            base.Awake();
            InstallBindings();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}