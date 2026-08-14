using UnityEngine;

public static class SafeAreaProvider
{
    public static Rect? Override;
    public static Rect Current
    {
        get
        {
            if (Override.HasValue)
                return Override.Value;
            return Screen.safeArea;
        }
    }
}
