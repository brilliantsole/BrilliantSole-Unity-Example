using UnityEngine;

public class BS_CenterOfPressureRange
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_CenterOfPressureRange");

    private readonly BS_Range x = new();
    private readonly BS_Range y = new();

    public void Reset()
    {
        x.Reset();
        y.Reset();
    }

    public void Update(Vector2 value)
    {
        x.Update(value.x);
        y.Update(value.y);
        Logger.Log($"Updated CenterOfPressureRange to {ToString()}");
    }

    public Vector2 GetNormalization(Vector2 value)
    {
        return new Vector2(x.GetNormalization(value.x, false), y.GetNormalization(value.y, false));
    }

    public Vector2 UpdateAndGetNormalization(Vector2 value)
    {
        Update(value);
        return GetNormalization(value);
    }

    public override string ToString()
    {
        return $"x: {x}, y: {y}";
    }
}
