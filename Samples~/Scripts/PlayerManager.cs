using UnityEngine;
namespace MiniDi.Samples
{
    public class PlayerManager : MonoBehaviour, IPlayerManager
    {
        public void StartMove()
        {
            Debug.Log("PlayerManager StartMove");
        }
    }
}