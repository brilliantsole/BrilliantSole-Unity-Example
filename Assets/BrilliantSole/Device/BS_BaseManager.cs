using System;
using System.Collections.Generic;

public abstract class BS_BaseManager
{
    public BS_BaseManager() { }

    public virtual void Reset() { }
    public virtual void OnSendTxData() { }
    public abstract bool OnRxMessage(byte messageTypeEnum, byte[] data);

    public Action<BS_TxMessage[], bool> SendTxMessages;
}

public abstract class BS_BaseManager<TEnum> : BS_BaseManager where TEnum : Enum
{
    public static readonly Type EnumType = typeof(TEnum);

    public BS_BaseManager()
    {
        if (Enum.GetUnderlyingType(typeof(TEnum)) != typeof(byte))
        {
            throw new InvalidOperationException($"Enum {typeof(TEnum).Name} must be backed by byte");
        }
    }

    public override bool OnRxMessage(byte messageTypeEnum, byte[] data)
    {
        if (!TxRxToEnum.ContainsKey(messageTypeEnum))
        {
            return false;
        }
        var messageType = (TEnum)Enum.ToObject(typeof(TEnum), messageTypeEnum);
        OnRxMessage(messageType, data);
        return true;
    }

    public virtual void OnRxMessage(TEnum messageType, byte[] data) { }

    private static readonly Dictionary<TEnum, byte> EnumToTxRx = new();
    private static readonly Dictionary<byte, TEnum> TxRxToEnum = new();

    public static void InitTxRxEnum(ref byte offset, List<string> enumStrings)
    {
        foreach (TEnum value in EnumType.GetEnumValues())
        {
            EnumToTxRx.Add(value, offset);
            TxRxToEnum.Add(offset, value);
            enumStrings[offset] = value.ToString();
            offset++;
        }
    }
}