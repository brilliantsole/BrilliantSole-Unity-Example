using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BS_VibrationWaveformEffectSegment))]
public class BS_VibrationWaveformEffectSegmentPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var indent = EditorGUI.indentLevel;

        // Calculate field heights
        var typeProperty = property.FindPropertyRelative("Type");
        var effectProperty = property.FindPropertyRelative("Effect");
        var delayProperty = property.FindPropertyRelative("Delay");
        var loopCountProperty = property.FindPropertyRelative("LoopCount");

        // Draw the Type Enum
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, typeProperty);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw the corresponding list
        if ((BS_VibrationWaveformEffectSegmentType)typeProperty.enumValueIndex == BS_VibrationWaveformEffectSegmentType.Effect)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, effectProperty);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        else if ((BS_VibrationWaveformEffectSegmentType)typeProperty.enumValueIndex == BS_VibrationWaveformEffectSegmentType.Delay)
        {
            var delayLabel = new GUIContent($"Delay (ms)");
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, delayProperty, delayLabel);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, loopCountProperty);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var typeProperty = property.FindPropertyRelative("Type");
        var effectProperty = property.FindPropertyRelative("Effect");
        var delayProperty = property.FindPropertyRelative("Delay");
        var loopCountProperty = property.FindPropertyRelative("LoopCount");

        // Default height for the type/(effect|delay)/loopCount
        float height = 3 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        return height;
    }
}