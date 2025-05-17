using System.Collections.Generic;
using UnityEngine.SceneManagement;
namespace MiniDi
{
    public static class SceneContainerRegistry
    {
        private static readonly Dictionary<Scene, SceneContainer> _map = new();

        public static void Register(Scene scene, SceneContainer container)
        {
            _map[scene] = container;
        }

        public static SceneContainer GetForScene(Scene scene)
        {
            _map.TryGetValue(scene, out var container);
            return container;
        }
    }
}