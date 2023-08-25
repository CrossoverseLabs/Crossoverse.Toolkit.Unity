namespace Crossoverse.Toolkit.Transports
{
    public struct SendOptions
    {
        public BroadcastingType BroadcastingType;
        public BufferingType BufferingType;
        public BufferingKey BufferingKey;
        public bool Reliability;
        public bool Encrypt;
    }
}
