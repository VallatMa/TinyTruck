#if UNITY_EDITOR

using UnityEngine;

public class FileUtils
{
    public static void SaveInput(string input)
    {
        Debug.Log($"Save the input: {input}");
    }
}

#elif UNITY_WEBGL

using System.Runtime.InteropServices;

public class FileUtils
{
    [DllImport("__Internal")]
    public static extern void SaveInput(string input);
}

#else

public class FileUtils
{
    public static void SaveInput(string uinputrl)
    {
    }
}

#endif