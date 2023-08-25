#if ENABLE_MONO || ENABLE_IL2CPP
#define UNITY_ENGINE
#endif

using System;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Realtime.Extension;

namespace Crossoverse.Toolkit.Transports.PhotonRealtime
{
    public sealed class PhotonRealtimeConnectParameters
    {
        public AppSettings AppSettings;
    }
    
    public sealed class PhotonRealtimeJoinParameters
    {
        public string RoomName;
        public RoomOptions RoomOptions;
        public TypedLobby TypedLobby;
        public string[] ExpectedUsers;
    }

    public partial class PhotonRealtimeTransport : ITransport, IOnEventCallback
    {
        public static readonly byte CrossoverseEventCode = 128;
        
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<byte[]> OnReceiveMessage;
        
        public event Action OnConnectedToMaster;
        public event Action<DisconnectCause> OnDisconnectedWithCause;
        
        public event Action OnJoinedRoom;
        public event Action<(short ReturnCode, string message)> OnJoinRoomFailed;
        public event Action OnLeftRoom;
        
        public bool IsConnected => _connected && _joined;
        public int ClientId => _photonRealtimeClient.NetworkingClient.LocalPlayer.ActorNumber;
        
        public string PunVersion => _photonRealtimeClient.PunVersion;
        public AppSettings AppSettings => _connectParameters.AppSettings;

        private static readonly object keyByteZero = (byte)0;
        private static readonly object keyByteOne = (byte)1;
        private static readonly object keyByteTwo = (byte)2;
        private static readonly object keyByteThree = (byte)3;

        private readonly Hashtable _eventContent = new();
        private readonly RaiseEventOptions _raiseEventOptions = new();

        private readonly PhotonRealtimeClient _photonRealtimeClient;
        private readonly PhotonRealtimeConnectParameters _connectParameters;
        private readonly PhotonRealtimeJoinParameters _joinParameters;

        private bool _connected;
        private bool _joined;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="punVersion"></param>
        /// <param name="connectParameters"></param>
        /// <param name="joinParameters"></param>
        /// <param name="targetFrameRate"></param>
        /// <param name="isBackgroundThread"></param>
        /// <param name="protocol"></param>
        /// <param name="receiverGroup"></param>
        public PhotonRealtimeTransport
        (
            string punVersion,
            PhotonRealtimeConnectParameters connectParameters,
            PhotonRealtimeJoinParameters joinParameters,
            int targetFrameRate = 30,
            bool isBackgroundThread = false,
            ConnectionProtocol protocol = ConnectionProtocol.Udp
        )
        {
            _connectParameters = connectParameters;
            _joinParameters = joinParameters;

            _photonRealtimeClient = new PhotonRealtimeClient(punVersion, targetFrameRate, isBackgroundThread, protocol);
            _photonRealtimeClient.NetworkingClient.AddCallbackTarget(this);
        }
        
        /// <summary>
        /// Dispose
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            DisconnectAsyncCore().GetAwaiter().GetResult();
            Log("[PhotonRealtimeTransport] End of DisconnectAsyncCore");
            _photonRealtimeClient?.Dispose();
            _photonRealtimeClient?.NetworkingClient.RemoveCallbackTarget(this);
            Log("[PhotonRealtimeTransport] Disposed");
        }
        
        /// <summary>
        /// ConnectAsync
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(string roomId = "")
        {
            if (!string.IsNullOrEmpty(roomId))
            {
                _joinParameters.RoomName = roomId;
            }
            await ConnectAsyncCore(_connectParameters);
            await JoinAsync(_joinParameters);
            return IsConnected;
        }
        
        /// <summary>
        /// DisconnectAsync
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            await DisconnectAsyncCore();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializedMessage"></param>
        /// <param name="sendOptions"></param>
        /// <param name="destClientIds"></param>
        public void Send(ArraySegment<byte> serializedMessage, SendOptions sendOptions = default, int[] destClientIds = null)
        {
            var reliability = sendOptions.Reliability;
            var broadcastingType = sendOptions.BroadcastingType;
            var bufferingType = sendOptions.BufferingType;
            var bufferingKey = sendOptions.BufferingKey;

            var photonSendOptions = reliability
                                    ? ExitGames.Client.Photon.SendOptions.SendReliable
                                    : ExitGames.Client.Photon.SendOptions.SendUnreliable;

            _raiseEventOptions.Receivers = broadcastingType switch
            {
                BroadcastingType.All => ReceiverGroup.All,
                BroadcastingType.ExceptSelf => ReceiverGroup.Others,
                _ => throw new NotImplementedException(),
            };

            _raiseEventOptions.CachingOption = bufferingType switch
            {
                BufferingType.DoNotBuffering => EventCaching.DoNotCache,
                BufferingType.AddToBuffer => EventCaching.AddToRoomCache,
                BufferingType.RemoveFromBuffer => EventCaching.RemoveFromRoomCache,
                _ => EventCaching.DoNotCache,
            };

            _eventContent.Clear();
            if (bufferingType is BufferingType.AddToBuffer || bufferingType is BufferingType.RemoveFromBuffer)
            {
                _eventContent[keyByteZero] = bufferingKey.FirstKey;
                _eventContent[keyByteOne] = bufferingKey.SecondKey;
                _eventContent[keyByteTwo] = bufferingKey.ThirdKey;
            }
            if (bufferingType is not BufferingType.RemoveFromBuffer)
            {
                _eventContent[keyByteThree] = serializedMessage.Array;
            }

            _photonRealtimeClient.RaiseEvent(CrossoverseEventCode, _eventContent, _raiseEventOptions, photonSendOptions);
        }

        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="eventData"></param>
        void IOnEventCallback.OnEvent(EventData eventData)
        {           
            if (eventData.Code == CrossoverseEventCode)
            {
                // Log($"[PhotonRealtimeTransport] OnEvent - Sender:{eventData.Sender}, EventCode:{eventData.Code}");
                var contentData = (Hashtable)eventData.CustomData;
                var serializedMessage = (byte[])contentData[keyByteThree];
                OnReceiveMessage?.Invoke(serializedMessage);
            }
        }
        
        /// <summary>
        /// Logs a message to the console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), 
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        private void Log(object message)
#if UNITY_ENGINE
            => UnityEngine.Debug.Log(message);
#else
            => System.Console.WriteLine(message);
#endif
        
        /// <summary>
        /// Logs a warning message to the console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), 
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        private void LogWarning(object message)
#if UNITY_ENGINE
            => UnityEngine.Debug.LogWarning(message);
#else
            => System.Console.WriteLine($"[WARNING] {message}");
#endif
        
        /// <summary>
        /// Logs an error message to the console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), 
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        private void LogError(object message)
#if UNITY_ENGINE
            => UnityEngine.Debug.LogError(message);
#else
            => System.Console.WriteLine($"[ERROR] {message}");
#endif
    }
}