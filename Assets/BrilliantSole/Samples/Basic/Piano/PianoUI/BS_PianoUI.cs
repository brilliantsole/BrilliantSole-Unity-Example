using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BS_PianoUI : MonoBehaviour
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_PianoUI");

    public GameObject WhiteKeyPrefab;
    public GameObject BlackKeyPrefab;

    public void SetVisibility(bool isVisible)
    {
        foreach (var image in GetComponentsInChildren<Image>())
        {
            image.enabled = isVisible;
        }
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.enabled = isVisible;
        }
    }

    private int truncateMidiNote(int note)
    {
        int noteInOctave = note % 12;
        return noteInOctave + note / 12 % (Octaves + 1) * 12;
    }
    public void OnMidiNote(int note, bool isOn)
    {
        Logger.Log($"OnMidiNote {note} isOn? {isOn}");
        note = truncateMidiNote(note);
        if (truncatedMidiMapping.TryGetValue(note, out var key))
        {
            var pianoKeyData = key.GetComponent<BS_PianoKeyData>();
            pianoKeyData.IsDown = isOn;
            SimulateKeyEvent(key, isOn);
        }
        else
        {
            Logger.LogError($"couldn't find key for midi note {note}");
        }
    }

    public int Octaves = 2;
    public int StartOctave = 4;
    private readonly string[] whiteKeyNames = { "C", "D", "E", "F", "G", "A", "B" };
    private readonly string[] blackKeyNames = { "C#", "D#", "", "F#", "G#", "A#" };
    private readonly int[] whiteKeyMidiOffsets = { 0, 2, 4, 5, 7, 9, 11 };
    private readonly int[] blackKeyMidiOffsets = { 1, 3, -1, 6, 8, 10 };

    private readonly string[] blackKeyBindings = { "2", "3", "5", "6", "7", "9", "0", "=" };
    private readonly string[] whiteKeyBindings = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]", "\\" };

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.TryGetComponent<EventTrigger>(out var trigger))
            {
                trigger = child.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry pointerDownEntry = new() { eventID = EventTriggerType.PointerDown };
            pointerDownEntry.callback.AddListener((data) => { onKeyDown(child.gameObject); });
            trigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new() { eventID = EventTriggerType.PointerUp };
            pointerUpEntry.callback.AddListener((data) => { onKeyUp(child.gameObject); });
            trigger.triggers.Add(pointerUpEntry);

            var textComponent = child.GetComponentInChildren<TextMeshProUGUI>();
            var keyText = textComponent.text.ToLower();
            if (keyText.Length > 0)
            {
                keyMapping[keyText] = child.gameObject;
            }

            var keyData = child.GetComponentInChildren<BS_PianoKeyData>();
            midiMapping[keyData.MidiNote] = child.gameObject;
            truncatedMidiMapping[keyData.TruncatedMidiNote] = child.gameObject;

            if (child.gameObject.TryGetComponent<BS_EyeInteractable>(out var eyeInteractable))
            {
                eyeInteractable.OnIsHovered += OnIsHovered;
            }
        }
    }

    private BS_PianoKeyData hoveredKeyData = null;
    public BS_PianoKeyData HoveredKeyData => hoveredKeyData;
    public Action<BS_PianoKeyData, bool> OnHoveredKeyData;
    private void OnIsHovered(BS_EyeInteractable eyeInteractable, bool isHovered)
    {
        var keyData = eyeInteractable.transform.GetComponentInChildren<BS_PianoKeyData>();
        if (isHovered)
        {
            hoveredKeyData = keyData;
            OnHoveredKeyData?.Invoke(keyData, isHovered);
        }
        else if (hoveredKeyData == keyData)
        {
            hoveredKeyData = null;
            OnHoveredKeyData?.Invoke(keyData, isHovered);
        }
    }

    public Action<BS_PianoKeyData> OnKeyDown;
    public Action<BS_PianoKeyData> OnKeyUp;

    private readonly Dictionary<string, GameObject> keyMapping = new();
    private readonly Dictionary<int, GameObject> midiMapping = new();
    private readonly Dictionary<int, GameObject> truncatedMidiMapping = new();


    private void onKeyDown(GameObject key)
    {
        var keyData = key.GetComponent<BS_PianoKeyData>();
        if (keyData.IsDown) { return; }
        keyData.IsDown = true;
        Logger.Log($"onKeyDown {key.name}");
        OnKeyDown?.Invoke(keyData);
    }
    private void onKeyUp(GameObject key)
    {
        var keyData = key.GetComponent<BS_PianoKeyData>();
        if (!keyData.IsDown) { return; }
        keyData.IsDown = false;
        Logger.Log($"onKeyUp {key.name}");
        OnKeyUp?.Invoke(keyData);
    }

    public void GenerateKeys()
    {
        while (transform.childCount > 0)
        {
            var child = transform.GetChild(0);
            if (Application.isEditor)
            {
                DestroyImmediate(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }

        var whiteKeyRectTramsform = WhiteKeyPrefab.GetComponent<RectTransform>();
        var whiteKeyHeight = whiteKeyRectTramsform.rect.height;
        var whiteKeyWidth = whiteKeyRectTramsform.rect.width;

        var blackKeyRectTramsform = BlackKeyPrefab.GetComponent<RectTransform>();
        var blackKeyHeight = blackKeyRectTramsform.rect.height;
        var blackKeyWidth = blackKeyRectTramsform.rect.width;
        var blackKeyY = -blackKeyRectTramsform.rect.y;

        var fullWidth = (Octaves * whiteKeyWidth * 7) + 1;
        var xOffset = -fullWidth / 2;

        Debug.Log($"whiteKeyHeight: {whiteKeyHeight}, whiteKeyWidth: {whiteKeyWidth}");
        Debug.Log($"blackKeyHeight: {blackKeyHeight}, blackKeyWidth: {blackKeyWidth}, blackKeyY: {blackKeyY}");

        int whiteKeyIndex = 0;
        int blackKeyIndex = 0;
        List<GameObject> blackKeys = new();
        bool didAddLastWhiteKey = false;
        for (int octave = StartOctave; (octave < StartOctave + Octaves) || !didAddLastWhiteKey; octave++)
        {
            bool isAddingLastWhiteKey = octave >= StartOctave + Octaves;

            for (int i = 0; (i < whiteKeyNames.Length) && !didAddLastWhiteKey; i++)
            {
                int midiNote = whiteKeyMidiOffsets[i] + octave * 12;

                var whiteKey = Instantiate(WhiteKeyPrefab, transform);
                var whiteKeyName = whiteKeyNames[i];
                whiteKey.name = $"{whiteKeyName}{octave}";
                whiteKey.GetComponentInChildren<TextMeshProUGUI>().text = whiteKeyIndex < whiteKeyBindings.Length ? whiteKeyBindings[whiteKeyIndex] : "";
                whiteKey.transform.localPosition = new Vector3(whiteKeyIndex * whiteKeyWidth + xOffset, 0, 0);

                var whiteKeyData = whiteKey.AddComponent<BS_PianoKeyData>();
                whiteKeyData.MidiNote = midiNote;
                whiteKeyData.TruncatedMidiNote = truncateMidiNote(midiNote);
                whiteKeyData.Color = BS_PianoKeyData.BS_PianoKeyColor.White;

                whiteKeyIndex++;

                if (isAddingLastWhiteKey)
                {
                    didAddLastWhiteKey = true;
                    break;
                }

                if (i < blackKeyNames.Length && !string.IsNullOrEmpty(blackKeyNames[i]))
                {
                    midiNote = blackKeyMidiOffsets[i] + octave * 12;

                    var blackKey = Instantiate(BlackKeyPrefab, transform);
                    var blackKeyName = blackKeyNames[i];
                    blackKey.name = $"{blackKeyName}{octave}";
                    blackKey.GetComponentInChildren<TextMeshProUGUI>().text = blackKeyIndex < blackKeyBindings.Length ? blackKeyBindings[blackKeyIndex] : "";
                    blackKey.transform.localPosition = new Vector3((whiteKeyIndex * whiteKeyWidth) - (whiteKeyWidth / 2) + xOffset, blackKeyY, 0);
                    blackKeys.Add(blackKey);

                    var blackKeyData = blackKey.AddComponent<BS_PianoKeyData>();
                    blackKeyData.MidiNote = midiNote;
                    blackKeyData.TruncatedMidiNote = truncateMidiNote(midiNote);

                    blackKeyData.Color = BS_PianoKeyData.BS_PianoKeyColor.Black;

                    blackKeyIndex++;
                }
            }
        }

        foreach (var blackKey in blackKeys)
        {
            blackKey.transform.SetAsLastSibling();
        }
    }

    private void Update()
    {
        foreach (var pair in keyMapping)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                SimulateKeyEvent(pair.Value, true);
            }
            if (Input.GetKeyUp(pair.Key))
            {
                SimulateKeyEvent(pair.Value, false);
            }
        }
    }

    public void SimulateKeyEvent(BS_PianoKeyData keyData, bool isHovered)
    {
        SimulateKeyEvent(keyData.gameObject, isHovered);
    }

    public void SimulateKeyEvent(GameObject key, bool isDown)
    {
        var pointer = new PointerEventData(EventSystem.current);
        if (isDown)
        {
            ExecuteEvents.Execute(key, pointer, ExecuteEvents.pointerDownHandler);
        }
        else
        {
            ExecuteEvents.Execute(key, pointer, ExecuteEvents.pointerUpHandler);
        }
    }
}
