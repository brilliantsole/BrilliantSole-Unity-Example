# BrilliantSole-Unity-Plugin

## Modifying the Bluetooth LE plugin

_This plugin uses the [Bluetooth LE for iOS, tvOS and Android](https://assetstore.unity.com/packages/tools/network/bluetooth-le-for-ios-tvos-and-android-26661) Plugin. You will need to modify a few files in this Plugin:_

In `BluetoothHardwareInterface.cs`, modify `public static void RequestMtu(string name, int mtu, Action<string, int> action)` by commenting out the lines that reduce the mtu:

```
#if EXPERIMENTAL_MACOS_EDITOR && (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
    // if (mtu > 184)
    // 	mtu = 184;
    OSXBluetoothLERequestMtu(name, mtu);
#elif UNITY_IOS || UNITY_TVOS
    // if (mtu > 180)
    //     mtu = 180;
    _iOSBluetoothLERequestMtu (name, mtu);
```

in `BluetoothDeviceScript.cs`, add the lines at the end of `void Start()`:

```
#if UNITY_IOS
    BLEStandardUUIDs["HEART RATE MEASUREMENT"] = "2A37";

    // MODIFY START
    BLEStandardUUIDs["BATTERY"] = "180F";
    BLEStandardUUIDs["BATTERY LEVEL"] = "2A19";

    BLEStandardUUIDs["DEVICE INFORMATION"] = "180A";
    BLEStandardUUIDs["MODEL NUMBER STRING"] = "2A24";
    BLEStandardUUIDs["SERIAL NUMBER STRING"] = "2A25";
    BLEStandardUUIDs["FIRMWARE REVISION STRING"] = "2A26";
    BLEStandardUUIDs["HARDWARE REVISION STRING"] = "2A27";
    BLEStandardUUIDs["SOFTWARE REVISION STRING"] = "2A28";
    BLEStandardUUIDs["MANUFACTURER NAME STRING"] = "2A29";
    // MODIFY END
#endif
```

and modify the lines in `public void OnBluetoothMessage(string message)`:

```
else if (message.Length >= deviceDidUpdateNotificationStateForCharacteristic.Length && message.Substring(0, deviceDidUpdateNotificationStateForCharacteristic.Length) == deviceDidUpdateNotificationStateForCharacteristic)
{
    if (parts.Length >= 3)
    {
        // MODIFY START
#if UNITY_IOS
        if (BLEStandardUUIDs.ContainsKey(parts[1].ToUpper()))
            parts[1] = BLEStandardUUIDs[parts[1].ToUpper()];
        if (BLEStandardUUIDs.ContainsKey(parts[2].ToUpper()))
            parts[2] = BLEStandardUUIDs[parts[2].ToUpper()];
#endif
        // MODIFY END
```

and modify the lines in `public void OnBluetoothData(string deviceAddress, string characteristic, string base64Data)`:

```
if (bytes.Length > 0)
{
    deviceAddress = deviceAddress.ToUpper();
    characteristic = characteristic.ToUpper();

    // MODIFY START
#if UNITY_IOS
    if (BLEStandardUUIDs.ContainsKey(characteristic.ToUpper()))
        characteristic = BLEStandardUUIDs[characteristic.ToUpper()];
#endif
    // MODIFY END
```

## Dealing with optional Plugins for Piano demos

When opening the project you may get some compiler issues - that's fine, you can select "Ignore" when opening.

The Piano demos require the Maestro - [Midi Player Tool Kit - Free](https://assetstore.unity.com/packages/tools/audio/maestro-midi-player-tool-kit-free-107994) plugin for playing sounds and the [MIDI Plugin for Mobile and Desktop](https://assetstore.unity.com/packages/tools/audio/midi-plugin-for-mobile-and-desktop-198917) plugin for connecting to a MIDI instrument via usb.

If you don't want to use those plugins, you can delete the `/Assets/BrilliantSole/Editor/Piano` folder and `/Assets/BrilliantSole/Samples/Basic/Piano` folder.
