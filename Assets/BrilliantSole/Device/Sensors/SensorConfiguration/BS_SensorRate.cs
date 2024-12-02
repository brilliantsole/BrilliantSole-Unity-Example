using UnityEngine;

public enum BS_SensorRate : ushort
{
    [InspectorName("off")]
    _0ms = 0,
    [InspectorName("5ms")]
    _5ms = 5,
    [InspectorName("10ms")]
    _10ms = 10,
    [InspectorName("20ms")]
    _20ms = 20,
    [InspectorName("40ms")]
    _40ms = 40,
    [InspectorName("100ms")]
    _100ms = 100,
    [InspectorName("500ms")]
    _500ms = 500,
    [InspectorName("1000ms")]
    _1000ms = 1000,
}
