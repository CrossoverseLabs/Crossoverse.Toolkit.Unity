namespace Crossoverse.Toolkit.Transports
{
    public enum BroadcastingType : byte
    {
        All = 0,
        ExceptSelf = 1,
        ToOne = 2,
        ToMany = 3,
        ExceptOne = 4,
        ExceptMany = 5,
    }
}
