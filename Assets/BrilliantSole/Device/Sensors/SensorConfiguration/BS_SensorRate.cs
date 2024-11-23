using UnityEngine;

public enum BS_SensorRate : ushort
{
    [InspectorName("off")]
    _0 = 0,
    [InspectorName("5ms")]
    _5 = 5,
    [InspectorName("10ms")]
    _10 = 10,
    [InspectorName("20ms")]
    _20 = 20,
    [InspectorName("40ms")]
    _40 = 40,
    [InspectorName("100ms")]
    _100 = 100,
    [InspectorName("500ms")]
    _500 = 500,
    [InspectorName("1000ms")]
    _1000 = 1000,
}
