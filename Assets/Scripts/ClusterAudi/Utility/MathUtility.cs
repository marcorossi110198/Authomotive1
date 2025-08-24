using UnityEngine;

public static class MathUtility
{
    public static float Remap(this float In, Vector2 InMinMax, Vector2 OutMinMax)
    {
        return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
    }
}
