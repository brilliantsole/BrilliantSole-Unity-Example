using UnityEngine;

[CreateAssetMenu(fileName = "NewPianoTrackNoteColors", menuName = "Brilliant Sole/PianoTrackNoteColors")]
public class BS_PianoTrackNoteColors : ScriptableObject
{
    public Color UnhoveredOffWhiteColor = Color.white;
    public Color HoveredOffWhiteColor = Color.white;
    public Color UnhoveredOnWhiteColor = Color.white;
    public Color HoveredOnWhiteColor = Color.white;

    public Color UnhoveredOffBlackColor = Color.black;
    public Color HoveredOffBlackColor = Color.black;
    public Color UnhoveredOnBlackColor = Color.black;
    public Color HoveredOnBlackColor = Color.black;

    public Color GetColor(bool isWhite, bool isOn, bool isHovered)
    {
        if (isWhite)
        {
            if (isOn)
            {
                return isHovered ? HoveredOnWhiteColor : UnhoveredOnWhiteColor;
            }
            else
            {
                return isHovered ? HoveredOffWhiteColor : UnhoveredOffWhiteColor;
            }
        }
        else
        {
            if (isOn)
            {
                return isHovered ? HoveredOnBlackColor : UnhoveredOnBlackColor;
            }
            else
            {
                return isHovered ? HoveredOffBlackColor : UnhoveredOffBlackColor;
            }
        }
    }
}