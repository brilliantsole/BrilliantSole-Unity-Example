public enum BS_BleConnectionStage
{
    None,
    Connecting,
    WaitForUuids,
    RequestingMtu,
    ReadingCharacteristics,
    WritingTxCharacteristic,
    SubscribingToCharacteristics,
    UnsubscribingFromCharacteristics,
    Disconnecting,
}