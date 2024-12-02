public enum BS_BleConnectionStage
{
    None,
    Connecting,
    WaitForUuids,
    RequestingMtu,
    ReadingCharacteristics,
    SubscribingToCharacteristics,
    UnsubscribingFromCharacteristics,
    Disconnecting,
}