using UnityEngine;
namespace MiniDi.Samples
{
    public interface IServerManager
    {
        public void Connect();
    }
    public class ServerManager : IServerManager
    {
        public void Connect()
        {
            Debug.Log("Connect > ");
        }
    }
}