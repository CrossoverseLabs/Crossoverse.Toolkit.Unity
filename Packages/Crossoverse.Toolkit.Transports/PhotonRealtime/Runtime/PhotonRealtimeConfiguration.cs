using System;
using UnityEngine;

namespace Crossoverse.Toolkit.Transports.PhotonRealtime
{
    [Serializable]
    [CreateAssetMenu(menuName = "Crossoverse/Toolkit/Transports/Create PhotonRealtimeConfiguration", fileName = "PhotonRealtimeConfiguration")]
    public sealed class PhotonRealtimeConfiguration : ScriptableObject
    {
        public string PunVersion = "2.41";
        public string AppVersion = "";
        public string Region = "jp";
        public string AppId = "";
    }
}