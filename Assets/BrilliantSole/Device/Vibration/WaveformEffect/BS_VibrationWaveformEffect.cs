using UnityEngine;

public enum BS_VibrationWaveformEffect : byte
{
    [InspectorName("None")]
    None = 0,

    [InspectorName("Strong Click - 100%")]
    StrongClick_100 = 1,
    [InspectorName("Strong Click - 60%")]
    StrongClick_60 = 2,
    [InspectorName("Strong Click - 30%")]
    StrongClick_30 = 3,

    [InspectorName("Sharp Click - 100%")]
    SharpClick_100 = 4,
    [InspectorName("Sharp Click - 60%")]
    SharpClick_60 = 5,
    [InspectorName("Sharp Click - 30%")]
    SharpClick_30 = 6,

    [InspectorName("Soft Bump - 100%")]
    SoftBump_100 = 7,
    [InspectorName("Soft Bump - 60%")]
    SoftBump_60 = 8,
    [InspectorName("Soft Bump - 30%")]
    SoftBump_30 = 9,

    [InspectorName("Double Click - 100%")]
    DoubleClick_100 = 10,
    [InspectorName("Double Click - 60%")]
    DoubleClick_60 = 11,

    [InspectorName("Triple Click - 100%")]
    TripleClick_100 = 12,

    [InspectorName("Soft Fuzz - 60%")]
    SoftFuzz_60 = 13,

    [InspectorName("Strong Buzz - 100%")]
    StrongBuzz_100 = 14,

    [InspectorName("750 ms Alert - 100%")]
    Alert_750ms = 15,
    [InspectorName("1000 ms Alert - 100%")]
    Alert_1000ms = 16,

    [InspectorName("Strong Click 1 100%")]
    StrongClick1_100 = 17,
    [InspectorName("Strong Click 2 - 80%")]
    StrongClick2_80 = 18,
    [InspectorName("Strong Click 3 - 60%")]
    StrongClick3_60 = 19,
    [InspectorName("Strong Click 4 - 30%")]
    StrongClick4_30 = 20,

    [InspectorName("Medium Click 1 - 100%")]
    MediumClick_100 = 21,
    [InspectorName("Medium Click 2 - 80%")]
    MediumClick_80 = 22,
    [InspectorName("Medium Click 3 - 60%")]
    MediumClick_60 = 23,

    [InspectorName("Sharp Tick 1 - 100%")]
    SharpTick_100 = 24,
    [InspectorName("Sharp Tick 2 - 80%")]
    SharpTick_80 = 25,
    [InspectorName("Sharp Tick 3 - 60%")]
    SharpTick_60 = 26,

    [InspectorName("Short Double Click Strong 1 - 100%")]
    ShortDoubleClickStrong_100 = 27,
    [InspectorName("Short Double Click Strong 2 - 80%")]
    ShortDoubleClickStrong_80 = 28,
    [InspectorName("Short Double Click Strong 3 - 60%")]
    ShortDoubleClickStrong_60 = 29,
    [InspectorName("Short Double Click Strong 4 - 30%")]
    ShortDoubleClickStrong_30 = 30,

    [InspectorName("Short Double Click Medium 1 - 100%")]
    ShortDoubleClickMedium_100 = 31,
    [InspectorName("Short Double Click Medium 2 - 80%")]
    ShortDoubleClickMedium_80 = 32,
    [InspectorName("Short Double Click Medium 3 - 60%")]
    ShortDoubleClickMedium_60 = 33,

    [InspectorName("Short Double Sharp Tick 1 - 100%")]
    ShortDoubleSharpTick_100 = 34,
    [InspectorName("Short Double Sharp Tick 2 - 80%")]
    ShortDoubleSharpTick_80 = 35,
    [InspectorName("Short Double Sharp Tick 3 - 60%")]
    ShortDoubleSharpTick_60 = 36,

    [InspectorName("Long Double Sharp Click Strong 1 - 100%")]
    LongDoubleSharpClickStrong_100 = 37,
    [InspectorName("Long Double Sharp Click Strong 2 - 80%")]
    LongDoubleSharpClickStrong_80 = 38,
    [InspectorName("Long Double Sharp Click Strong 3 - 60%")]
    LongDoubleSharpClickStrong_60 = 39,
    [InspectorName("Long Double Sharp Click Strong 4 - 30%")]
    LongDoubleSharpClickStrong_30 = 40,

    [InspectorName("Long Double Sharp Click Medium 1 - 100%")]
    LongDoubleSharpClickMedium_100 = 41,
    [InspectorName("Long Double Sharp Click Medium 2 - 80%")]
    LongDoubleSharpClickMedium_80 = 42,
    [InspectorName("Long Double Sharp Click Medium 3 - 60%")]
    LongDoubleSharpClickMedium_60 = 43,

    [InspectorName("Long Double Sharp Tick 1 - 100%")]
    LongDoubleSharpTick_100 = 44,
    [InspectorName("Long Double Sharp Tick 2 - 80%")]
    LongDoubleSharpTick_80 = 45,
    [InspectorName("Long Double Sharp Tick 3 - 60%")]
    LongDoubleSharpTick_60 = 46,

    [InspectorName("Buzz 1 - 100%")]
    Buzz_100 = 47,
    [InspectorName("Buzz 2 - 80%")]
    Buzz_80 = 48,
    [InspectorName("Buzz 3 - 60%")]
    Buzz_60 = 49,
    [InspectorName("Buzz 4 - 40%")]
    Buzz_40 = 50,
    [InspectorName("Buzz 5 - 20%")]
    Buzz_20 = 51,

    [InspectorName("Pulsing Strong 1 - 100%")]
    PulsingStrong_100 = 52,
    [InspectorName("Pulsing Strong 2 - 60%")]
    PulsingStrong_60 = 53,

    [InspectorName("Pulsing Medium 1 - 100%")]
    PulsingMedium_100 = 54,
    [InspectorName("Pulsing Medium 2 - 60%")]
    PulsingMedium_60 = 55,

    [InspectorName("Pulsing Sharp 1 - 100%")]
    PulsingSharp_100 = 56,
    [InspectorName("Pulsing Sharp 2 - 60%")]
    PulsingSharp_60 = 57,

    [InspectorName("Transition Click 1 - 100%")]
    TransitionClick_100 = 58,
    [InspectorName("Transition Click 2 - 80%")]
    TransitionClick_80 = 59,
    [InspectorName("Transition Click 3 - 60%")]
    TransitionClick_60 = 60,
    [InspectorName("Transition Click 4 - 40%")]
    TransitionClick_40 = 61,
    [InspectorName("Transition Click 5 - 20%")]
    TransitionClick_20 = 62,
    [InspectorName("Transition Click 6 - 10%")]
    TransitionClick_10 = 63,

    [InspectorName("Transition Hum 1 - 100%")]
    TransitionHum_100 = 64,
    [InspectorName("Transition Hum 2 - 80%")]
    TransitionHum_80 = 65,
    [InspectorName("Transition Hum 3 - 60%")]
    TransitionHum_60 = 66,
    [InspectorName("Transition Hum 4 - 40%")]
    TransitionHum_40 = 67,
    [InspectorName("Transition Hum 5 - 20%")]
    TransitionHum_20 = 68,
    [InspectorName("Transition Hum 6 - 10%")]
    TransitionHum_10 = 69,

    [InspectorName("Transition Ramp Down Long Smooth 1 - 100 to 0%")]
    TransitionRampDownLongSmooth1_100 = 70,
    [InspectorName("Transition Ramp Down Long Smooth 2 - 100 to 0%")]
    TransitionRampDownLongSmooth2_100 = 71,

    [InspectorName("Transition Ramp Down Medium Smooth 1 - 100 to 0%")]
    TransitionRampDownMediumSmooth1_100 = 72,
    [InspectorName("Transition Ramp Down Medium Smooth 2 - 100 to 0%")]
    TransitionRampDownMediumSmooth2_100 = 73,

    [InspectorName("Transition Ramp Down Short Smooth 1 - 100 to 0%")]
    TransitionRampDownShortSmooth1_100 = 74,
    [InspectorName("Transition Ramp Down Short Smooth 2 - 100 to 0%")]
    TransitionRampDownShortSmooth2_100 = 75,

    [InspectorName("Transition Ramp Down Long Sharp 1 - 100 to 0%")]
    TransitionRampDownLongSharp1_100 = 76,
    [InspectorName("Transition Ramp Down Long Sharp 2 - 100 to 0%")]
    TransitionRampDownLongSharp2_100 = 77,

    [InspectorName("Transition Ramp Down Medium Sharp 1 - 100 to 0%")]
    TransitionRampDownMediumSharp1_100 = 78,
    [InspectorName("Transition Ramp Down Medium Sharp 2 - 100 to 0%")]
    TransitionRampDownMediumSharp2_100 = 79,

    [InspectorName("Transition Ramp Down Short Sharp 1 - 100 to 0%")]
    TransitionRampDownShortSharp1_100 = 80,
    [InspectorName("Transition Ramp Down Short Sharp 2 - 100 to 0%")]
    TransitionRampDownShortSharp2_100 = 81,

    [InspectorName("Transition Ramp Up Long Smooth 1 - 100 to 0%")]
    TransitionRampUpLongSmooth1_100 = 82,
    [InspectorName("Transition Ramp Up Long Smooth 2 - 100 to 0%")]
    TransitionRampUpLongSmooth2_100 = 83,

    [InspectorName("Transition Ramp Up Medium Smooth 1 - 100 to 0%")]
    TransitionRampUpMediumSmooth1_100 = 84,
    [InspectorName("Transition Ramp Up Medium Smooth 2 - 100 to 0%")]
    TransitionRampUpMediumSmooth2_100 = 85,

    [InspectorName("Transition Ramp Up Short Smooth 1 - 100 to 0%")]
    TransitionRampUpShortSmooth1_100 = 86,
    [InspectorName("Transition Ramp Up Short Smooth 2 - 100 to 0%")]
    TransitionRampUpShortSmooth2_100 = 87,

    [InspectorName("Transition Ramp Up Long Sharp 1 - 100 to 0%")]
    TransitionRampUpLongSharp1_100 = 88,
    [InspectorName("Transition Ramp Up Long Sharp 2 - 100 to 0%")]
    TransitionRampUpLongSharp2_100 = 89,

    [InspectorName("Transition Ramp Up Medium Sharp 1 - 100 to 0%")]
    TransitionRampUpMediumSharp1_100 = 90,
    [InspectorName("Transition Ramp Up Medium Sharp 2 - 100 to 0%")]
    TransitionRampUpMediumSharp2_100 = 91,

    [InspectorName("Transition Ramp Up Short Sharp 1 - 100 to 0%")]
    TransitionRampUpShortSharp1_100 = 92,
    [InspectorName("Transition Ramp Up Short Sharp 2 - 100 to 0%")]
    TransitionRampUpShortSharp2_100 = 93,

    [InspectorName("Transition Ramp Down Long Smooth 1 - 50 to 0%")]
    TransitionRampDownLongSmooth1_50 = 94,
    [InspectorName("Transition Ramp Down Long Smooth 2 - 50 to 0%")]
    TransitionRampDownLongSmooth2_50 = 95,

    [InspectorName("Transition Ramp Down Medium Smooth 1 - 50 to 0%")]
    TransitionRampDownMediumSmooth1_50 = 96,
    [InspectorName("Transition Ramp Down Medium Smooth 2 - 50 to 0%")]
    TransitionRampDownMediumSmooth2_50 = 97,

    [InspectorName("Transition Ramp Down Short Smooth 1 - 50 to 0%")]
    TransitionRampDownShortSmooth1_50 = 98,
    [InspectorName("Transition Ramp Down Short Smooth 2 - 50 to 0%")]
    TransitionRampDownShortSmooth2_50 = 99,

    [InspectorName("Transition Ramp Down Long Sharp 1 - 50 to 0%")]
    TransitionRampDownLongSharp1_50 = 100,
    [InspectorName("Transition Ramp Down Long Sharp 2 - 50 to 0%")]
    TransitionRampDownLongSharp2_50 = 101,

    [InspectorName("Transition Ramp Down Medium Sharp 1 - 50 to 0%")]
    TransitionRampDownMediumSharp1_50 = 102,
    [InspectorName("Transition Ramp Down Medium Sharp 2 - 50 to 0%")]
    TransitionRampDownMediumSharp2_50 = 103,

    [InspectorName("Transition Ramp Down Short Sharp 1 - 50 to 0%")]
    TransitionRampDownShortSharp1_5 = 104,
    [InspectorName("Transition Ramp Down Short Sharp 2 - 50 to 0%")]
    TransitionRampDownShortSharp2_50 = 105,

    [InspectorName("Transition Ramp Up Long Smooth 1 - 0 to 50%")]
    TransitionRampUpLongSmooth1_50 = 106,
    [InspectorName("Transition Ramp Up Long Smooth 2 - 0 to 50%")]
    TransitionRampUpLongSmooth2_50 = 107,

    [InspectorName("Transition Ramp Up Medium Smooth 1 - 0 to 50%")]
    TransitionRampUpMediumSmooth1_50 = 108,
    [InspectorName("Transition Ramp Up Medium Smooth 2 - 0 to 50%")]
    TransitionRampUpMediumSmooth2_50 = 109,

    [InspectorName("Transition Ramp Up Short Smooth 1 - 0 to 50%")]
    TransitionRampUpShortSmooth1_50 = 110,
    [InspectorName("Transition Ramp Up Short Smooth 2 - 0 to 50%")]
    TransitionRampUpShortSmooth2_50 = 111,

    [InspectorName("Transition Ramp Up Long Sharp 1 - 0 to 50%")]
    TransitionRampUpLongSharp1_50 = 112,
    [InspectorName("Transition Ramp Up Long Sharp 2 - 0 to 50%")]
    TransitionRampUpLongSharp2_50 = 113,

    [InspectorName("Transition Ramp Up Medium Sharp 1 - 0 to 50%")]
    TransitionRampUpMediumSharp1_50 = 114,
    [InspectorName("Transition Ramp Up Medium Sharp 2 - 0 to 50%")]
    TransitionRampUpMediumSharp2_50 = 115,

    [InspectorName("Transition Ramp Up Short Sharp 1 - 0 to 50%")]
    TransitionRampUpShortSharp1_50 = 116,
    [InspectorName("Transition Ramp Up Short Sharp 2 - 0 to 50%")]
    TransitionRampUpShortSharp2_50 = 117,

    [InspectorName("Long buzz for programmatic stopping - 100%")]
    LongBuzz_100 = 118,
    [InspectorName("Smooth Hum 1 (No kick or brake pulse) - 50%")]
    SmoothHum_50 = 119,
    [InspectorName("Smooth Hum 2 (No kick or brake pulse) - 40%")]
    SmoothHum_40 = 120,
    [InspectorName("Smooth Hum 3 (No kick or brake pulse) - 30%")]
    SmoothHum_30 = 121,
    [InspectorName("Smooth Hum 4 (No kick or brake pulse) - 20%")]
    SmoothHum_20 = 122,
    [InspectorName("Smooth Hum 5 (No kick or brake pulse) - 10%")]
    SmoothHum_10 = 123,
}