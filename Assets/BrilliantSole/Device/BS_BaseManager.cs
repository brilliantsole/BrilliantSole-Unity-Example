using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public abstract class BS_BaseManager
{
    public BS_BaseManager() { }

    public virtual void Reset() { }
    public virtual void OnSendTxData() { }
    public abstract bool OnRxMessage(byte messageTypeEnum, in byte[] data);

    public Action<BS_TxMessage[], bool> SendTxMessages;
}

public abstract class BS_BaseManager<TEnum> : BS_BaseManager where TEnum : Enum
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseManager", BS_Logger.LogLevel.Log);

    public static readonly Type EnumType = typeof(TEnum);

    public BS_BaseManager()
    {
        if (Enum.GetUnderlyingType(typeof(TEnum)) != typeof(byte))
        {
            throw new InvalidOperationException($"Enum {typeof(TEnum).Name} must be backed by byte");
        }
    }

    public override bool OnRxMessage(byte messageTypeEnum, in byte[] data)
    {
        if (!TxRxToEnum.ContainsKey(messageTypeEnum))
        {
            return false;
        }
        var messageType = (TEnum)Enum.ToObject(typeof(TEnum), messageTypeEnum);
        OnRxMessage(messageType, data);
        return true;
    }

    public virtual void OnRxMessage(TEnum messageType, in byte[] data)
    {
        Logger.Log($"OnRxMessage {messageType} ({data.Length} bytes)");
    }

    protected static readonly Dictionary<TEnum, byte> EnumToTxRx = new();
    protected static readonly Dictionary<byte, TEnum> TxRxToEnum = new();

    public static void InitTxRxEnum(ref byte offset, List<string> enumStrings)
    {
        foreach (TEnum value in EnumType.GetEnumValues())
        {
            //Logger.Log($"enum {offset}: {value}");
            EnumToTxRx.Add(value, offset);
            TxRxToEnum.Add(offset, value);
            enumStrings.Add(value.ToString());
            offset++;
        }
    }

    protected static byte[] EnumArrayToTxRxArray(TEnum[] enumArray)
    {
        byte[] byteArray = new byte[enumArray.Length];

        for (int i = 0; i < enumArray.Length; i++)
        {
            byteArray[i] = EnumToTxRx[enumArray[i]];
        }

        return byteArray;
    }

    protected BS_TxMessage CreateTxMessage(TEnum enumValue, in List<byte> data) { return new(EnumToTxRx[enumValue], data); }
    protected BS_TxMessage CreateTxMessage(TEnum enumValue) { return new(EnumToTxRx[enumValue]); }
}