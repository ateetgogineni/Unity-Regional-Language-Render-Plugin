using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

/// <summary>
/// Manager for generating Sprite from text using Native (iOS/Android) code.
/// </summary>
public class NativeTextToImageManager
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern string _getImageBytesFromString(
        string key, float textWidth, float textHeight, int fontSize,
        string alignment, string fontStyle, string color,
        int outlineWidth, string outLineColor, float textWidthOffset, bool isLongText, string virticalalignment);

    /// <summary>
    /// Calls native iOS function to get base64 image string from text.
    /// </summary>
    public static string GetByteArrayFromString(
        string key, float textWidth, float textHeight, int fontSize,
        string alignment, string fontStyle, string color,
        int outlineWidth, string outLineColor, float textWidthOffset, bool isLongText, string virticalalignment)
    {
       //Debug.Log("[NativeTextToImageManager][iOS] Calling native method with text and size info.");
        return _getImageBytesFromString(key, textWidth, textHeight, fontSize, alignment, fontStyle, color, outlineWidth, outLineColor, textWidthOffset, isLongText, virticalalignment);
    }
#endif

    private static AndroidJavaClass nativeSdk = null;

    private static AndroidJavaClass NativeSDK
    {
        get
        {
            if (nativeSdk == null)
            {
                AndroidJNI.AttachCurrentThread();
                nativeSdk = new AndroidJavaClass("com.yesgnome.anylanguagerenderer.MainActivity");
                //Debug.Log("[NativeTextToImageManager][Android] Native SDK initialized.");
            }
            return nativeSdk;
        }
        set => nativeSdk = value;
    }

    private static NativeTextToImageManager instance = null;

    public static NativeTextToImageManager Instance
    {
        get
        {
            if (instance == null)
                instance = new NativeTextToImageManager();
            return instance;
        }
    }


    public static string DumpFontToDisk()
    {
        string fileName = "Poppins-Medium.ttf"; 
        
        
        string srcPath  = Path.Combine(Application.streamingAssetsPath, "CustomFonts", fileName);
        string dstPath  = Path.Combine(Application.persistentDataPath, fileName);

        // Already copied?
        if (File.Exists(dstPath))
        {
            //Debug.Log($"[DumpFontToDisk] Font already exists at: {dstPath}");
            return dstPath;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        // Android: read from inside the APK
        //Debug.Log($"[DumpFontToDisk] Android source URI: {srcPath}");
        using (UnityWebRequest uwr = UnityWebRequest.Get(srcPath))
        {
            var op = uwr.SendWebRequest();
            while (!op.isDone) { /* blocking, but OK here */ }

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(dstPath, uwr.downloadHandler.data);
                //Debug.Log($"[DumpFontToDisk] Copied font to: {dstPath}");
                return dstPath;
            }
            else
            {
                //Debug.Log($"[DumpFontToDisk] Android copy failed: {uwr.error}");
                return string.Empty;
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
        // iOS: streamingAssetsPath is a real folder
        if (File.Exists(srcPath))
        {
            try
            {
                File.Copy(srcPath, dstPath, true);
                //Debug.Log($"[DumpFontToDisk] Copied font to: {dstPath}");
                return dstPath;
            }
            catch (Exception ex)
            {
                //Debug.Log($"[DumpFontToDisk] iOS copy failed: {ex.Message}");
                return string.Empty;
            }
        }
        else
        {
            //Debug.Log($"[DumpFontToDisk] iOS font file not found at: {srcPath}");
            return string.Empty;
        }
#else
        // Editor or other platforms
        if (File.Exists(srcPath))
        {
            try
            {
                File.Copy(srcPath, dstPath, true);
                //Debug.Log($"[DumpFontToDisk] Copied font to: {dstPath}");
                return dstPath;
            }
            catch (Exception ex)
            {
                //Debug.Log($"[DumpFontToDisk] Copy failed: {ex.Message}");
                return string.Empty;
            }
        }
        else
        {
            //Debug.Log($"[DumpFontToDisk] Font file not found at: {srcPath}");
            return string.Empty;
        }
#endif
    }
    /// <summary>
    /// Generates a Sprite from provided text using native platform rendering.
    /// </summary>
    public Sprite getTextToImageFromNative(string localized_Text, int text_Width, int text_height, Color textColor, Color outLineColor, 
		int text_Size = 50, int text_Quality = 100, string text_Allignment = "Left", string fontStyle = "Normal", int outlineWidth = 0, 
		float textWidthOffset = 1.0f, bool isLongString = false, string color = "#FFFFFF", string virticalalignment = "Top")
    {
        //Debug.Log("[NativeTextToImageManager] --- getTextToImageFromNative Start ---");

        Texture2D texture2D = null;

        if (text_Width <= 0)
        {
            Debug.LogWarning("[NativeTextToImageManager] text_Width is 0. Resetting to 50.");
            text_Width = 50;
        }

        // get a real .ttf on-disk before calling into native
        string fontPath = string.Empty; //DumpFontToDisk(); // call this to pass custom font to native code

        // Debug.Log($"[NativeTextToImageManager] Using font path: {fontPath}");

#if UNITY_ANDROID && !UNITY_EDITOR
        //Debug.Log($"[NativeTextToImageManager][Android] Requesting image. Width: {text_Width}, image Height {text_height}, FontSize: {text_Size}");

        string texPtr = NativeSDK.CallStatic<string>("getImageFromText", localized_Text, text_Width, text_Size, text_Quality, text_Allignment, fontStyle, color, virticalalignment, text_height, fontPath);
        byte[] data = Convert.FromBase64String(texPtr);

        texture2D = new Texture2D(2, 2);
        texture2D.LoadImage(data);

        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        texture2D = null; 
        texPtr = null;
        //Debug.Log($"[NativeTextToImageManager][Android] Sprite created. Size: {sprite.texture.width}x{sprite.texture.height}");

        return sprite;

#elif UNITY_IOS && !UNITY_EDITOR
        //Debug.Log($"[NativeTextToImageManager][iOS] Requesting image. Width: {text_Width}, Height: {text_height}, FontSize: {text_Size}");

        string colorStr = HexConverter(textColor);
        string outLineColorStr = HexConverter(outLineColor);

        // Ensure the native function is being called with correct parameters.
        string texPtr = GetByteArrayFromString(localized_Text, text_Width, text_height, text_Size, text_Allignment, fontStyle, colorStr, outlineWidth, outLineColorStr, textWidthOffset, isLongString, virticalalignment);
        byte[] data = Convert.FromBase64String(texPtr);

        texture2D = new Texture2D(1, 1);
        texture2D.LoadImage(data);

        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        texture2D = null; 
        texPtr = null;
        //Debug.Log($"[NativeTextToImageManager][iOS] Sprite created. Size: {sprite.texture.width}x{sprite.texture.height}");

        return sprite;
#endif

        // Fallback for Editor mode
        //Debug.Log("[NativeTextToImageManager][Editor] Creating placeholder sprite.");

        texture2D = new Texture2D(text_Width, text_height);
        Sprite fallbackSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        texture2D = null; 
        //Debug.Log($"[NativeTextToImageManager][Editor] Placeholder sprite created. Size: {fallbackSprite.texture.width}x{fallbackSprite.texture.height}");

        //Debug.Log("[NativeTextToImageManager] --- getTextToImageFromNative End ---");
        return fallbackSprite;
    }

    public static String HexConverter(Color c)
    {
        Color32 color32 = c;
        return "#" + color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2");
    }
}
