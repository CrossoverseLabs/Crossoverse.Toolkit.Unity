using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Realtime;

namespace Crossoverse.Toolkit.Transports.PhotonRealtime
{
    public partial class PhotonRealtimeTransport : IConnectionCallbacks
    {
        private TaskCompletionSource<bool> _onConnected = new TaskCompletionSource<bool>();
        private TaskCompletionSource<bool> _onConnectedToMaster = new TaskCompletionSource<bool>();
        private TaskCompletionSource<DisconnectCause> _onDisconnected = new TaskCompletionSource<DisconnectCause>(DisconnectCause.None);
        
        public async Task<bool> ConnectAsyncCore(PhotonRealtimeConnectParameters connectParameters)
        {
            _onConnectedToMaster = new TaskCompletionSource<bool>();
            _onDisconnected = new TaskCompletionSource<DisconnectCause>(DisconnectCause.None);

            var task = Task.WhenAny(_onConnectedToMaster.Task, _onDisconnected.Task); // .NET 6
            _photonRealtimeClient?.ConnectUsingSettings(connectParameters.AppSettings);
            await task;

            return _connected;
        }
        
        public async Task DisconnectAsyncCore()
        {
            Log($"[PhotonRealtimeTransport] DisconnectAsyncCore");
            Log($"[PhotonRealtimeTransport] NetworkingClient.State: {_photonRealtimeClient?.NetworkingClient.State}");
            if (_photonRealtimeClient?.NetworkingClient.State is ClientState.Disconnected
            ||  _photonRealtimeClient?.NetworkingClient.State is ClientState.Disconnecting
            ||  _photonRealtimeClient?.NetworkingClient.State is ClientState.PeerCreated)
            {
                return;
            }
            
            _onDisconnected = new TaskCompletionSource<DisconnectCause>(DisconnectCause.None);
            _photonRealtimeClient?.Disconnect();
            await _onDisconnected.Task;
        }
        
#region Photon Realtime Callbacks
        
        void IConnectionCallbacks.OnConnected()
        {
            Log($"[PhotonRealtimeTransport] OnConnected");

            OnConnected?.Invoke();

            _connected = true;
            _onConnected?.TrySetResult(true);
        }
        
        void IConnectionCallbacks.OnConnectedToMaster()
        {
            Log($"[PhotonRealtimeTransport] OnConnectedToMaster");

            OnConnectedToMaster?.Invoke();

            _connected = true;
            _onConnectedToMaster?.TrySetResult(true);
        }
        
        void IConnectionCallbacks.OnDisconnected(DisconnectCause disconnectCause)
        {
            Log($"[PhotonRealtimeTransport] OnDisconnected - DisconnectedCause: {disconnectCause}");

            OnDisconnected?.Invoke();
            OnDisconnectedWithCause?.Invoke(disconnectCause);

            _connected = false;
            _onDisconnected?.TrySetResult(disconnectCause);
        }
        
        void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler)
        {
        }
        
        void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
        }
        
        void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage)
        {
        }
        
#endregion
    }
}