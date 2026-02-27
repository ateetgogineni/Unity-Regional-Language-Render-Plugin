// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Text.RegularExpressions;
// using System;

// public class RegionalTextHandler : MonoBehaviour
// {

//     public int textAdjustsize = 0;
//     public int text_Width = 200;
//     public int text_Height = 100;
//     public int text_Size = 60;
//     public int textQuality = 100;
//     public int extra_text_size = 0; // to match hindi text font size

//     TextClass tempTextRef;
//     //TextMeshClass tempTextMeshRef;
//     Text textRef;
//     TextMeshProUGUI textmeshRef;



//     public enum Allignment
//     {
//         Center,
//         Left,
//         Right,
//         Top,
//         Down
//     };

//     public enum FontStyle
//     {
//         Normal,
//         Bold,
//         Italic,
//         Bold_Italic
//     };


//     public Allignment text_Allignment = Allignment.Center;
//     public FontStyle font_Style = FontStyle.Bold;

//     [Header("TextMeshPro")]
//     public bool isTextMesh;
//     public Color txtColor = Color.white;
//     public Color txtOutlineColor = Color.black;
//     public TextAlignmentOptions textMesh_Alignment = TextAlignmentOptions.Midline;
//     public bool isWrappingEabled = false;
//     public bool useFontSize = false;

//     string langText;
//     float fontSize;
//     float textMinSize;
//     float textMaxSize;
//     bool isBestFit;
//     Color textColor;


//     static bool convert = true;
//     private static RegionalTextHandler instance;
//     private void Awake()
//     {
//         convert = true;
//         if (instance == null)
//         {
//             instance = this;
//         }
//     }


//     private void Start()
//     {
//         //if (isTextMesh)
//         //{
//         //    textmeshRef = this.GetComponent<TextMeshProUGUI>();
//         //}
//         //else
//         //{
//         //    textRef = this.GetComponent<Text>();            
//         //}
//         Convert();
//     }

//     public void Convert(object font = null, string text = null, string appendText = null)
//     {
//         try
//         {
// #if UNITY_ANDROID && !UNITY_EDITOR
//                     if (convert)
//                     {
//                         if (isTextMesh)
//                         {
//                             textmeshRef = this.GetComponent<TextMeshProUGUI>();
//                         }
//                         else
//                         {
//                             textRef = this.GetComponent<Text>();
//                         }

//                         // if (GPLocalizationController.Instance != null)
//                         {
//                             // if (GPLocalizationController.Instance.User_Language == Language.English)
//                             if(false)
//                             {
//                                 Image imageComponent = this.GetComponent<Image>();
//                                 if (imageComponent != null)
//                                 {
//                                     DestroyImmediate(imageComponent);
//                                 }

//                                 if (isTextMesh)
//                                 {
//                                     //if(textmeshRef.GetComponent<Outline>() != null)
//                                     for (int i = 0; i < 2; i++)
//                                     {
//                                         Outline outline = gameObject.GetComponent<Outline>();
//                                         if (outline != null)
//                                         {
//                                             DestroyImmediate(outline);
//                                         }
//                                     }

//                                     if (textmeshRef == null)
//                                     {
//                                         textmeshRef = gameObject.AddComponent<TextMeshProUGUI>();
//                                     }

//                                     if (isWrappingEabled)
//                                     {
//                                         textmeshRef.enableWordWrapping = isWrappingEabled;
//                                     }

//                                     textmeshRef.text = text;
//                                     textmeshRef.font = font as TMP_FontAsset;
//                                     //textmeshRef.fontSize = text_Size + extra_text_size;
//                                     textmeshRef.alignment = textMesh_Alignment;
//                                     //textmeshRef.faceColor = textFaceColor;
//                                     if(!useFontSize)
//                                     {
//                                         textmeshRef.fontSize = text_Size + extra_text_size;
//                                     }

//                                     //RectTransform rectTransform = textmeshRef.GetComponent<RectTransform>();
//                                     //rectTransform.sizeDelta = new Vector2(text_Width, text_Height);
//                                 }
//                                 else
//                                 {
//                                     if (textRef == null)
//                                     {
//                                         textRef = this.gameObject.AddComponent<Text>();
//                                     }
//                                     if (text != null)
//                                     {
//                                         textRef.text = text;
//                                         RetrieveTextRef(textRef, font as Font);
//                                     }
//                                 }
//                             }
//                             else
//                             {
//                                 if (isTextMesh)
//                                 {
//                                     if (text != null)
//                                     {
//                                         textmeshRef.text = RemoveTags(text);
//                                         //langText = RemoveTags(text);

//                                     }
//                                     langText = RemoveTags(textmeshRef.text) + appendText;
//                                     textColor = textmeshRef.faceColor;
//                                     //textFaceColor = textmeshRef.faceColor;


//                                     if (textmeshRef.enableAutoSizing)
//                                     {
//                                         isBestFit = true;
//                                         textMinSize = textmeshRef.fontSizeMin;
//                                         textMaxSize = textmeshRef.fontSizeMax;
//                                     }
//                                     else
//                                     {
//                                         isBestFit = false;
//                                         fontSize = textmeshRef.fontSize;
//                                     }



//                                     //Color32 tempOutlineColor = textmeshRef.outlineColor;

//                                     Debug.Log(textmeshRef.fontMaterial.shader.name);

//                                     if (textmeshRef.fontMaterial.shader.name == "TextMeshPro/Distance Field")
//                                     {
//                                         Outline outlineColor1 = textmeshRef.gameObject.AddComponent<Outline>();
//                                         outlineColor1.effectColor = txtOutlineColor;
//                                         //Outline outlineColor2 = textmeshRef.gameObject.AddComponent<Outline>();
//                                         //outlineColor2.effectDistance = new Vector2(-1, 1);
//                                         //outlineColor2.effectColor = txtOutlineColor;
//                                     }

//                                     DestroyImmediate(textmeshRef);
//                                 }
//                                 else
//                                 {
//                                     if (textRef == null)
//                                     {
//                                         textRef = this.GetComponent<Text>();
//                                     }
//                                     if (text != null)
//                                     {
//                                         textRef.text = RemoveTags(text);
//                                         //langText = RemoveTags(text);
//                                     }
//                                     langText = RemoveTags(textRef.text) + appendText;
//                                     textColor = textRef.color;
//                                     StoreTextRef(textRef);
//                                     DestroyImmediate(textRef);
//                                 }

//                                 ConvertToNativeTextImage(gameObject, langText, textColor, text_Width);
//                                 //Debug.Log("<regional> text : " + langText + ", Game Object : " + gameObject.name + "color " + ColorUtility.ToHtmlStringRGBA(textColor));
//                             }
//                         }
//                     }
// #endif
//         }
//         catch (Exception ex)
//         {
//             Debug.Log("RegionalTextHandler => " + text);
//             Debug.Log("RegionalTextHandler " + ex.Message);
//         }
//     }

//     public void ConvertToNativeTextImage(GameObject textObject, string TextToAdd, Color color, float textWidthOffset = 1.0f, bool isLongString = false)
//     {
//         //Debug.Log("<regional> Start ConvertToNativeTextImage");

//         // Check if the user's language is not English and ensure GPLocalizationController is not null
//         //Debug.Log("<regional> ConvertToNativeTextImage not ENG");
//         textObject.SetActive(false);

//         Outline outlineComponent = textObject.GetComponent<Outline>();
//         Color outlineColor = Color.white;
//         int outlineWidth = 0;

//         if (outlineComponent != null)
//         {
//             outlineColor = outlineComponent.effectColor;
//             outlineWidth = (int)outlineComponent.effectDistance.x;
//         }

//         Shadow shadowComponent = textObject.GetComponent<Shadow>();
//         if (shadowComponent != null)
//         {
//             outlineColor = shadowComponent.effectColor;
//             outlineWidth = (int)shadowComponent.effectDistance.x;
//         }

//         //Color color;
//         //if (isTextMesh)
//         //{
//         //    color = textmeshRef.color;
//         //}
//         //else
//         //{
//         //    color = textRef.color;
//         //}

//         // MultiColor Support.
//         bool isColorSpan = false;
//         string colorValue = "#FFFFFF"; // default white
//         if (TextToAdd.Contains("<span")) // check for any html color span
//         {
//             isColorSpan = true;
//             colorValue = "";//"#"+ColorUtility.ToHtmlStringRGBA(color);
//         }


//         // Assuming text_Width, text_Height, text_Size, text_Quality, getAllignmentText(), getFontStyle() are defined and initialized elsewhere

//         //Debug.Log("<regional> size before : " + text_Size + "," + TextSizeSlider.textOffSet);
//         //text_Size += extra_text_size;
//         //text_Size += TextSizeSlider.textOffSet;
//         //Debug.Log("<regional> size after : " + text_Size);

//         Sprite sprite = null;

// #if UNITY_ANDROID && !UNITY_EDITOR
//             sprite = NativeTextToImageManager.Instance.getTextToImageFromNative(TextToAdd, text_Width, text_Height, color, outlineColor, text_Size, textQuality, getAllignmentText(), getFontStyle(), outlineWidth, textWidthOffset, isLongString , colorValue);
// #endif

//         //textRef.enabled = false;

//         if (sprite != null)
//         {
//             Image image = this.gameObject.AddComponent<Image>();
//             if (!isColorSpan) // if no multicolor then only we apply image color
//             {
//                 image.color = color;
//             }
//             if (isTextMesh)
//             {
//                 image.color = txtColor;
//             }
//             image.sprite = sprite;
//             //image.SetNativeSize();
//             image.preserveAspect = true;
//         }

//         textObject.SetActive(true);
//     }

//     void StoreTextRef(Text textRef)
//     {
//         tempTextRef = new TextClass();
//         tempTextRef.m_FontData.font = textRef.font;
//         tempTextRef.m_FontData.fontSize = textRef.fontSize;
//         tempTextRef.alignment = textRef.alignment;
//         tempTextRef.resizeTextForBestFit = textRef.resizeTextForBestFit;
//         tempTextRef.resizeTextMinSize = textRef.resizeTextMinSize;
//         tempTextRef.resizeTextMaxSize = textRef.resizeTextMaxSize;
//         tempTextRef.TextColor = textRef.color;

//     }

//     void RetrieveTextRef(Text textRef, Font font)
//     {
//         if (tempTextRef != null)
//         {
//             textRef.font = font;//tempRef.m_FontData.font;
//             textRef.fontSize = tempTextRef.m_FontData.fontSize + extra_text_size;
//             textRef.alignment = tempTextRef.alignment;
//             textRef.resizeTextForBestFit = tempTextRef.resizeTextForBestFit;
//             textRef.resizeTextMinSize = tempTextRef.resizeTextMinSize;
//             textRef.resizeTextMaxSize = tempTextRef.resizeTextMaxSize;
//             textRef.color = tempTextRef.TextColor;

//             RectTransform rectTransform = textRef.GetComponent<RectTransform>();
//             rectTransform.sizeDelta = new Vector2(text_Width, text_Height);
//         }
//     }

//     public string RemoveTags(string input)
//     {
//         // Define the regular expression pattern to match <color=yellow>, </color>, and <sprite> tags
//         string pattern = "<color=yellow>|</color>|<sprite>";
//         // Replace the matched patterns with an empty string
//         string result = Regex.Replace(input, pattern, string.Empty);
//         return result;
//     }

//     public string RemoveSpanTags(string text)
//     {
//         string[] openingTags = { "<span style=\"color: #FFA500\">", "<span style=\"color: #FFFF00\">" };
//         string closingTag = "</span>";

//         foreach (string openingTag in openingTags)
//         {
//             text = text.Replace(openingTag, "");
//         }

//         text = text.Replace(closingTag, "");

//         return text;
//     }



//     string getAllignmentText()
//     {
//         if (text_Allignment.Equals(Allignment.Left))
//             return "Left";
//         else if (text_Allignment.Equals(Allignment.Right))
//             return "Right";
//         else
//             return "Center";
//     }

//     string getFontStyle()
//     {
//         if (font_Style.Equals(FontStyle.Bold))
//             return "Bold";
//         else if (font_Style.Equals(FontStyle.Italic))
//             return "Italic";
//         else if (font_Style.Equals(FontStyle.Bold_Italic))
//             return "BI";
//         else
//             return "Normal";
//     }

//     public static void ConvertToImage(GameObject localizeObj, TextMeshProUGUI textObj, string message, string alignment = "Center")
//     {
//         // Debug.Log("<###ConverToImage>" + message);
// #if !STANDALONE
//         // if (GPLocalizationController.Instance.User_Language == Language.English)
//         if(false)
// #endif
//         {
//             if (localizeObj != null && textObj != null)
//             {
//                 textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 1);
//                 localizeObj.SetActive(false);
//             }
//         }
// #if UNITY_ANDROID && !UNITY_EDITOR
//         // if (GPLocalizationController.Instance.User_Language != Language.English && GPLocalizationController.Instance.User_Language != Language.None)
//         if(true)
//         {

//             RectTransform rect = textObj.GetComponent<RectTransform>();
//             bool isColorSpan = false;
//             string colorValue = "#FFFFFF"; // default white

//             if (message.Contains("<span")) // check for any html color span
//             {
//                 isColorSpan = true;
//                 colorValue = "";//"#"+ColorUtility.ToHtmlStringRGBA(color);
//             }
//             // Debug.Log("<###rect.sizeDelta>" + rect.rect.width + "<> " + (int)rect.rect.height);
//             // Debug.Log("<###rect.fontsize>" + textObj.fontSize);
//             Sprite sprite = NativeTextToImageManager.Instance.getTextToImageFromNative(message, (int)rect.rect.width, 170, textObj.color, textObj.outlineColor, (int)textObj.fontSize, 75, alignment , "Bold", 1, 1, false, colorValue);
//             localizeObj.GetComponent<Image>().sprite = sprite;
//             localizeObj.GetComponent<Image>().SetNativeSize();
//             //localizationObj.GetComponent<Image>().preserveAspect = true;
//             localizeObj.SetActive(true);

//             if (!isColorSpan)
//             {
//                 Color tempColor = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 1);
//                 localizeObj.GetComponent<Image>().color = tempColor; // textObj.color;
//             }
//             textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 0);
//         }
// #endif
//     }

//     public static void ConvertTextToImage(GameObject localizeObj, Text textObj, string message, string alignment = "Center")
//     {
//         //Debug.Log("<###ConverToImage>" + message);
// #if !STANDALONE
//         // if (GPLocalizationController.Instance.User_Language == Language.English)
//         if(false)
// #endif
//         {
//             textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 1);
//             localizeObj.SetActive(false);
//         }
// #if UNITY_ANDROID && !UNITY_EDITOR
//         // if (GPLocalizationController.Instance.User_Language != Language.English && GPLocalizationController.Instance.User_Language != Language.None)
//         if(true)
//         {

//             RectTransform rect = textObj.GetComponent<RectTransform>();
//             bool isColorSpan = false;
//             string colorValue = "#FFFFFF"; // default white

//             if (message.Contains("<span")) // check for any html color span
//             {
//                 isColorSpan = true;
//                 colorValue = "";//"#"+ColorUtility.ToHtmlStringRGBA(color);
//             }
//             // Debug.Log("<###rect.sizeDelta>" + rect.rect.width + "<> " + (int)rect.rect.height);
//             // Debug.Log("<###rect.fontsize>" + textObj.fontSize);
//             Sprite sprite = NativeTextToImageManager.Instance.getTextToImageFromNative(message, (int)rect.rect.width, 170, textObj.color, textObj.color, (int)textObj.fontSize, 75, alignment, "Bold", 1, 1, false, colorValue);
//             localizeObj.GetComponent<Image>().sprite = sprite;
//             localizeObj.GetComponent<Image>().SetNativeSize();
//             //localizationObj.GetComponent<Image>().preserveAspect = true;
//             localizeObj.SetActive(true);

//             if (!isColorSpan)
//             {
//                 Color tempColor = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 1);
//                 localizeObj.GetComponent<Image>().color = tempColor; // textObj.color;
//             }
//             textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 0);
//         }
// #endif
//     }

//     public static string HtmlToUnity(string input)
//     {
//         try
//         {
//             int startIndex = 0;

//             while ((startIndex = input.IndexOf("<span style=\"color:", startIndex)) != -1)
//             {
//                 int colorStartIndex = startIndex + 19; // length of '<span style="color:'
//                 int colorEndIndex = input.IndexOf("\">", colorStartIndex);
//                 if (colorEndIndex == -1) break;

//                 string colorCode = input.Substring(colorStartIndex, colorEndIndex - colorStartIndex).TrimStart();

//                 int textStartIndex = colorEndIndex + 2; // length of '">'
//                 int textEndIndex = input.IndexOf("</span>", textStartIndex);
//                 if (textEndIndex == -1) break;

//                 string innerText = input.Substring(textStartIndex, textEndIndex - textStartIndex);

//                 string unityColor = $"<color={colorCode.ToLower()}ff>{innerText}</color>";
//                 string htmlSpan = input.Substring(startIndex, textEndIndex + 7 - startIndex); // length of '</span>'

//                 input = input.Replace(htmlSpan, unityColor);
//                 startIndex += unityColor.Length;
//             }

//             return input;
//         }
//         catch (Exception e)
//         {
//             Debug.Log($"HtmlToUnity Exception: {e.Message}");
//         }
//         return input;
//     }

//     public static string ReplaceBrWithNewline(string input)
//     {
//         if (string.IsNullOrEmpty(input))
//         {
//             return input;
//         }

//         // Replace <br /> or <br> with \n
//         string pattern = @"<br\s*/?>";
//         string replacement = "\n";
//         string result = Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);

//         return result;
//     }
// }
