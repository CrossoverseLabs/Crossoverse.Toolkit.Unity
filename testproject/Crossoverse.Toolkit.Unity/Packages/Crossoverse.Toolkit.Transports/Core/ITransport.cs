using System;
using System.Threading.Tasks;

namespace Crossoverse.Toolkit.Transports
{
    public interface ITransport : IDisposable
    {
        event Action OnConnected;
        event Action OnDisconnected;
        event Action<byte[]> OnReceiveMessage;
        
        bool IsConnected { get; }
        int ClientId { get; }
        
        Task<bool> ConnectAsync(string roomId = "");
        Task DisconnectAsync();
        
        void Send(ArraySegment<byte> data, SendOptions sendOptions = default, int[] destClientIds = null);
    }
}