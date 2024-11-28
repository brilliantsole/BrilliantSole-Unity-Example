using System;

public abstract class BS_BaseManager
{
    public BS_BaseManager() { }

    public virtual void Reset() { }
    public virtual void OnSendTxData() { }
    public abstract void OnRxMessage(byte messageTypeEnum, byte[] data);

    public Action<byte, byte[]> SendTxMessage;

    // FILL - add enum range
}

public abstract class BS_BaseManager<TEnum> : BS_BaseManager where TEnum : Enum
{
    public BS_BaseManager()
    {
        if (Enum.GetUnderlyingType(typeof(TEnum)) != typeof(byte))
        {
            throw new InvalidOperationException($"Enum {typeof(TEnum).Name} must be backed by byte");
        }
    }

    public override void OnRxMessage(byte messageTypeEnum, byte[] data)
    {
        TEnum messageType = (TEnum)Enum.ToObject(typeof(TEnum), messageTypeEnum);
        OnRxMessage(messageType, data);
    }

    public virtual void OnRxMessage(TEnum messageType, byte[] data) { }
}