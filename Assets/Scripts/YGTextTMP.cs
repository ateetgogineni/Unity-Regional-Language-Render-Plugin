using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Net;

namespace YG_Text
{
    /// <summary>
    /// YGTextTMP manages text display using either TextMeshProUGUI (Editor) or Native text-to-image sprite (Device).
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class YGTextTMP : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Main Text Content")]
        [TextArea(3, 10)]
        [SerializeField] private string m_Text = "";

        [Header("Character Settings")]
        [SerializeField] private TMP_FontAsset m_Font;
        [SerializeField] private FontStyles m_FontStyle = FontStyles.Normal;
        [SerializeField] private int m_FontSize = 14;

        [Header("Spacing Settings")]
        [SerializeField] private float m_CharacterSpacing = 0f;
        [SerializeField] private float m_WordSpacing = 0f;
        [SerializeField] private float m_LineSpacing = 1f;
        [SerializeField] private bool m_RichText = true;

        [Header("Paragraph Settings")]
        [SerializeField] private TextAlignmentOptions m_Alignment = TextAlignmentOptions.TopLeft;
        [SerializeField] private bool m_BestFit = false;
        [SerializeField] private int m_MinSize = 10;
        [SerializeField] private int m_MaxSize = 40;

        [Header("Color Settings")]
        [SerializeField] private Color m_Color = Color.black;

        [Header("Gradient Settings")]
        [SerializeField] private bool m_UseGradient = false;
        [SerializeField] private Color m_TopLeftColor = Color.white;
        [SerializeField] private Color m_TopRightColor = Color.white;
        [SerializeField] private Color m_BottomLeftColor = Color.white;
        [SerializeField] private Color m_BottomRightColor = Color.white;

        [Header("Wrapping and Overflow")]
        [SerializeField] private bool m_EnableWordWrapping = true;
        [SerializeField] private TextOverflowModes m_TextOverflow = TextOverflowModes.Overflow;

        [Header("Parent RectTransform")]
        [SerializeField] public RectTransform parentRectTransform;

        [Header("Native Upscaling Settings")]
        private static int upscaleFactor = 1; // Upscaling for sharper native text

        [Header("Font Size Offset")]
        [SerializeField] private int fontSizeOffset = 0;  // Extra font size added after dynamic calculation
        [SerializeField] private bool useGradientColor = false; // Use gradient color for native text


        #endregion

        #region Private Fields

        private TMP_Text textComponentRef;
        private Image imageComponentRef;
        private RectTransform rectTransformRef;
        private Coroutine updateCoroutine;

        private Vector2 originalAnchorMin = new Vector2(0.5f, 0.5f);
        private Vector2 originalAnchorMax = new Vector2(0.5f, 0.5f);
        private Vector2 originalSizeDelta;

        private int rectHeight = 0;
        private int rectWidth = 0;
        public int offSetRectHeight = 0;
        public int offSetRectWidth = 0;
        private int tmpFontSize = 0;
        public bool multiPropChanging = false;

        #endregion

        #region Properties

        public string text
        {
            get => m_Text;
            set
            {
                if (m_Text != value)
                {
                    m_Text = value;
                    OnTextChanged();
                }
            }
        }

        public Color color
        {
            get => m_Color;
            set
            {
                if (m_Color != value)
                {
                    m_Color = value;
                    OnTextChanged();
                }
            }
        }

        public TextAlignmentOptions alignment
        {
            get => m_Alignment;
            set
            {
                if (m_Alignment != value)
                {
                    m_Alignment = value;
                    OnTextChanged();
                }
            }
        }

        public FontStyles fontStyles
        {
            get => m_FontStyle;
            set
            {
                if (m_FontStyle != value)
                {
                    m_FontStyle = value;
                    OnTextChanged();
                }
            }
        }

        public int fontSize
        {
            get => m_FontSize;
            set
            {
                if (m_FontSize != value)
                {
                    m_FontSize = value;
                    OnTextChanged();
                }
            }
        }
        
        public int FontSizeOffset
        {
            get => fontSizeOffset;
            set
            {
                if (fontSizeOffset != value)
                {
                    fontSizeOffset = value;
                    OnTextChanged();
                }
            }
        }

        public bool UseGradientColor
        {
            get => useGradientColor;
            set
            {
                if (useGradientColor != value)
                {
                    useGradientColor = value;
                    OnTextChanged();
                }
            }
        }

        #endregion

        static int conversionCounter = 0;
        static int maxConversionCounter = 50;
        static bool maxCounterFetched = false;
        public static bool localizationImageLoading = false;
        public static int languageChangeSequenceCounter = 0;
        public static int languageChangeAttemptCounter = 0;

        int instanceLocalCounter = 0;

        #region Unity Methods

        private void OnEnable()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                 upscaleFactor = 2;
#endif
            if (m_Text != null && m_Text.Length > 0)
            {
                StartTextUpdate();
            }
        }

        #endregion

        #region Initialization and Setup

        private void StartTextUpdate()
        {
            if (updateCoroutine != null)
                StopCoroutine(updateCoroutine);

            if (gameObject.activeInHierarchy)
            {
                updateCoroutine = StartCoroutine(DelayTextUpdating());
            }
            else
            {
                Debug.LogWarning($"{name}: GameObject is not active in hierarchy. Text update skipped.");
            }
        }

        private IEnumerator DelayTextUpdating()
        {
            yield return new WaitForEndOfFrame();

            SetupRectTransform();
            CacheFontSizeFromParent();

#if !UNITY_EDITOR
            if(LanguageHandler.GetLanguageEnum() != LanguageHandler.InGameLanguage.en)
                StartCoroutine(AddImageComponentAndUpdateContent());
#else
            AddTextComponentAndUpdateContent();
#endif
        }

        private void SetupRectTransform()
        {
            if (rectTransformRef == null)
                rectTransformRef = GetComponent<RectTransform>();

            if (parentRectTransform != null)
            {
                rectHeight = (int)parentRectTransform.rect.height + offSetRectHeight;
                rectWidth = (int)parentRectTransform.rect.width + offSetRectWidth;
                rectTransformRef.sizeDelta = new Vector2(rectWidth, rectHeight);
            }
            else
            {
                Debug.LogWarning($"{name}: Parent RectTransform not assigned!");
            }
        }

        private void CacheFontSizeFromParent()
        {
            if (parentRectTransform != null)
            {
                var tmp = parentRectTransform.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmpFontSize = (int)tmp.fontSize;
                }
            }
        }

        #endregion

        #region TextMeshPro Component Update (Editor)

        private void AddTextComponentAndUpdateContent()
        {
            textComponentRef = GetComponent<TMP_Text>() ?? gameObject.AddComponent<TextMeshProUGUI>();

            textComponentRef.text = m_Text;
            textComponentRef.font = m_Font;
            textComponentRef.fontStyle = m_FontStyle;
            textComponentRef.fontSize = m_FontSize;
            textComponentRef.characterSpacing = m_CharacterSpacing;
            textComponentRef.wordSpacing = m_WordSpacing;
            textComponentRef.lineSpacing = m_LineSpacing;
            textComponentRef.richText = m_RichText;
            textComponentRef.alignment = m_Alignment;
            textComponentRef.enableWordWrapping = m_EnableWordWrapping;
            textComponentRef.overflowMode = m_TextOverflow;
            textComponentRef.enableAutoSizing = m_BestFit;
            textComponentRef.fontSizeMin = m_MinSize;
            textComponentRef.fontSizeMax = m_MaxSize;

            if (m_UseGradient)
            {
                textComponentRef.colorGradient = new VertexGradient(m_TopLeftColor, m_TopRightColor, m_BottomLeftColor, m_BottomRightColor);
                textComponentRef.enableVertexGradient = true;
            }
            else
            {
                textComponentRef.color = m_Color;
                textComponentRef.enableVertexGradient = false;
            }
        }

        #endregion

        #region Native Image Component Update (Device)

        IEnumerator AddImageComponentAndUpdateContent()
        {
            try
            {
                if (localizationImageLoading)
                {
                    languageChangeSequenceCounter++;
                    languageChangeAttemptCounter++;
                    instanceLocalCounter++;
                    int localCounter = languageChangeSequenceCounter;
                    for (int i = 0; i < localCounter; i++)
                    {
                        yield return null;
                    }
                }

                imageComponentRef = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
                if (rectTransformRef == null)
                    rectTransformRef = GetComponent<RectTransform>();

                Color outlineColor = Color.white;
                int outlineWidth = 0;

                if (TryGetComponent(out Outline outline))
                {
                    outlineColor = outline.effectColor;
                    outlineWidth = (int)outline.effectDistance.x;
                }
                else if (TryGetComponent(out Shadow shadow))
                {
                    outlineColor = shadow.effectColor;
                    outlineWidth = (int)shadow.effectDistance.x;
                }

                //bool hasColorSpan = false;// m_Text.Contains("<span") || m_Text.Contains("U000");
                string defaultColorHex = ColorUtility.ToHtmlStringRGB(m_Color); // hasColorSpan ? "" : "#FFFFFF";
                defaultColorHex = "#" + defaultColorHex;
                //Debug.LogError(defaultColorHex);
                int nativeWidth = rectWidth * upscaleFactor;
                int nativeHeight = rectHeight * upscaleFactor;

                // Calculate font size dynamically based on text length
                int nativeFontSize = CalculateDynamicFontSize(nativeWidth, nativeHeight, tmpFontSize * upscaleFactor);

                //Debug.Log($"[YGTextTMP] Native Request: Width={nativeWidth}, Height={nativeHeight}, FontSize={nativeFontSize}");

                Sprite generatedSprite = null;
                m_Text = ReplaceAllHexColorTags(m_Text);
#if !UNITY_EDITOR
                generatedSprite = NativeTextToImageManager.Instance.getTextToImageFromNative(
                    m_Text, nativeWidth, nativeHeight, m_Color, outlineColor, nativeFontSize, 100,
                    GetAlignmentText(m_Alignment.ToString()), m_FontStyle.ToString(), 0, 0, false, defaultColorHex, GetVirticalAlignmentText(m_Alignment.ToString())
                );
#endif

                if (generatedSprite != null)
                {
                    imageComponentRef.overrideSprite = generatedSprite;
                    //Debug.Log($"[YGTextTMP] Sprite Size: {generatedSprite.texture.width} x {generatedSprite.texture.height}");

                    if (upscaleFactor > 1)
                        rectTransformRef.sizeDelta = new Vector2(rectWidth, generatedSprite.texture.height / upscaleFactor);
                    else
                        rectTransformRef.sizeDelta = new Vector2(rectWidth, generatedSprite.texture.height);

                    //if (!hasColorSpan)
                    //    imageComponentRef.color = m_Color;

                    ApplyImageAspectSettings();
                    imageComponentRef.raycastTarget = false; // Disable raycast to avoid blocking UI interactions
                }
                else
                {
                    Debug.LogError("[YGTextTMP] Failed to create sprite from native text!");
                }
                generatedSprite = null;
                if(maxCounterFetched == false)
                {
                    maxCounterFetched = true;
                    maxConversionCounter = Config.GetCleanResOnCount();
                }
                conversionCounter++;
                if (conversionCounter >= maxConversionCounter)
                {
                    conversionCounter = 0;
                    Debug.Log("CleanResOnCount called");
                    Resources.UnloadUnusedAssets();
                }
                
                if(useGradientColor)
                {
                    AddGradientColor();
                }
                
            }
            finally
            {
                handleCounter();
            }

            
        }

        //private static HashSet<GameObject> gradientAddedObjects = new HashSet<GameObject>();

        private void AddGradientColor()
        {
            if (this.gameObject == null)
                return;
            // if (gradientAddedObjects.Contains(this.gameObject))
            //     return;
            
            var gradient = this.gameObject.GetComponent<UIGradientHMB>();
            if (gradient == null)
            {
                gradient = this.gameObject.AddComponent<UIGradientHMB>();
                if (gradient == null)
                    return;
            }
            gradient.color1 = new Color(1f, 0.549f, 0.0667f, 1f); // #FF8C11FF
            gradient.color2 = new Color(1f, 0.8235f, 0f, 1f); // #FFD200FF
            gradient.direction = UIGradientHMB.Direction.Vertical;
            //gradientAddedObjects.Add(this.gameObject);
        }

        private void ApplyImageAspectSettings()
        {
            if (originalAnchorMin == new Vector2(0.5f, 0.5f) && originalAnchorMax == new Vector2(0.5f, 0.5f))
            {
#if UNITY_IOS
                imageComponentRef.preserveAspect = true;
#endif
            }
            else
            {
                rectTransformRef.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransformRef.anchorMax = new Vector2(0.5f, 0.5f);

                imageComponentRef.SetNativeSize();

                rectTransformRef.anchorMin = originalAnchorMin;
                rectTransformRef.anchorMax = originalAnchorMax;
                rectTransformRef.sizeDelta = originalSizeDelta;

                imageComponentRef.preserveAspect = true;
            }
        }

        void OnDestroy()
        {
            handleCounter();
        }

        void handleCounter()
        {
            if (instanceLocalCounter > 0)
            {
                instanceLocalCounter = 0;
                languageChangeAttemptCounter--;
                if (languageChangeAttemptCounter < 0)
                {
                    languageChangeAttemptCounter = 0;
                }
            }
        }

        #endregion

        #region Helper Methods

        private void OnTextChanged()
        {   if(multiPropChanging)
                return;
            if (m_Text != null && m_Text.Length > 0)
            {
                StartTextUpdate();
            }
        }

        private string GetAlignmentText(string alignmentString)
        {
            if (alignmentString.Contains("Left"))
                return "Left";
            else if (alignmentString.Contains("Right"))
                return "Right";
            else
                return "Center";
        }

        private string GetVirticalAlignmentText(string alignmentString)
        {
            //Debug.Log("GetVirticalAlignmentText " + alignmentString + "," + this.name);
            if (alignmentString.Contains("Top"))
                return "Top";
            else if (alignmentString.Contains("Bottom"))
                return "Bottom";
            else if (alignmentString.Contains("TopLeft"))
                return "Top";
            else if (alignmentString.Contains("TopRight"))
                return "Top";
            else if (alignmentString.Contains("BottomLeft"))
                return "Bottom";
            else if (alignmentString.Contains("BottomRight"))
                return "Bottom";
            else if (alignmentString.Contains("Center"))
                return "Center";
            else
                return "Center";
        }


        /// <summary>
        /// Dynamically adjust font size based on text length and fit inside target area,
        /// then apply extra fontSizeOffset after fitting.
        /// </summary>
        // private int CalculateDynamicFontSize(int targetWidth, int targetHeight, int baseFontSize)
        // {
        //     int adjustedFontSize = baseFontSize;
        //     // m_Text = RemoveColorSpan(m_Text);
        //     // Step 1: Approximate initial shrink based on text length (every 25 characters = 5% reduction)
        //     string newString = new string(m_Text.ToCharArray());
        //     newString = RemoveColorSpan(newString);
        //     int approxCharCount = newString.Length;
        //     int reduceSteps = approxCharCount / 25;
        //     float reductionFactor = 1f - (reduceSteps * 0.05f);

        //     adjustedFontSize = Mathf.RoundToInt(baseFontSize * reductionFactor);

        //     // Step 2: Estimate total text height
        //     int estimatedLineHeight = Mathf.RoundToInt(adjustedFontSize * 1.2f); // Rough multiplier (1.2) for line height
        //     int estimatedLineCount = Mathf.Max(1, newString.Split('\n').Length + approxCharCount / 30); // Rough estimate: 30 characters per line
        //     int totalEstimatedHeight = estimatedLineHeight * estimatedLineCount;

        //     //($"[YGTextTMP] AutoFit Estimation -> FontSize={adjustedFontSize}, TotalHeight={totalEstimatedHeight}, TargetHeight={targetHeight}");

        //     // Step 3: Shrink font further if total height exceeds available height
        //     while (totalEstimatedHeight > targetHeight && adjustedFontSize > m_MinSize * upscaleFactor)
        //     {
        //         adjustedFontSize--;
        //         estimatedLineHeight = Mathf.RoundToInt(adjustedFontSize * 1.2f);
        //         totalEstimatedHeight = estimatedLineHeight * estimatedLineCount;
        //     }

        //     // Step 4: After fitting, apply the manual offset
        //     adjustedFontSize += fontSizeOffset * upscaleFactor;

        //     //Debug.Log($"[YGTextTMP] After Fit + Offset -> FinalFontSize={adjustedFontSize} (Offset={fontSizeOffset})");

        //     // Step 5: Clamp to a minimum safe size
        //     int minimumAllowedSize = Mathf.Max(m_MinSize * upscaleFactor, 1);
        //     adjustedFontSize = Mathf.Max(adjustedFontSize, minimumAllowedSize);

        //     return adjustedFontSize;
        // }

        private int CalculateDynamicFontSize(int targetWidth, int targetHeight, int baseFontSize)
        {
            // strip out any tags
            string cleanText = RemoveColorSpan(m_Text);
            // int totalChars = cleanText.Length;

            // start at your base size (clamped to your min/max)
            // int minSize = m_MinSize * upscaleFactor;
            // int maxSize = m_MaxSize * upscaleFactor;
            int fontSize = Mathf.Clamp(baseFontSize, m_MinSize * upscaleFactor, m_MaxSize * upscaleFactor);

            // average glyph width ≈ half the font size, line height ≈ 1.2×
            float avgCharWidth = fontSize * 0.5f;
            float lineHeight   = fontSize * 1.2f;

            // how many chars can we fit per line at this size?
            int approxCharsPerLine = Mathf.Max(1, Mathf.FloorToInt(targetWidth / avgCharWidth));
            // how many lines will we need?
            int lineCount = Mathf.CeilToInt((float)cleanText.Length / approxCharsPerLine);

            // estimated total height
            float estHeight = lineCount * lineHeight;

            // now shrink until both fit
            while ((estHeight > targetHeight || (approxCharsPerLine * avgCharWidth) > targetWidth) && fontSize > m_MinSize * upscaleFactor)
            {
                fontSize--;

                // recalc with the new size
                avgCharWidth = fontSize * 0.5f;
                lineHeight   = fontSize * 1.2f;
                approxCharsPerLine = Mathf.Max(1,
                    Mathf.FloorToInt(targetWidth / avgCharWidth));
                lineCount = Mathf.CeilToInt((float)cleanText.Length / approxCharsPerLine);
                estHeight = lineCount * lineHeight;
            }

            // apply your offset and final clamp
            fontSize = fontSize + fontSizeOffset * upscaleFactor;
            return Mathf.Clamp(fontSize, m_MinSize * upscaleFactor, m_MaxSize * upscaleFactor);
        }


        public static string ReplaceAllHexColorTags(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            //if(LanguageHandler.IsLowEndDevice)
            //    return RemoveAllHtmlTags(input);

            // Handle nested spans by processing from innermost to outermost
            while (input.Contains("style=color;HEX") || input.Contains("style=color;"))
            {
                // First replace HEX format to # format
#if UNITY_ANDROID
                string hexPattern = @"style=color;HEX([0-9a-fA-F]{6})>";
                string hexReplacement = "style=color:#$1>";
#endif
#if UNITY_IOS
                // First convert HEXXXXXXX to proper style="color:#XXXXXX"
                string hexPattern = @"style=color;HEX([0-9a-fA-F]{6})";
                string hexReplacement = "style=\"color:#$1\"";
#endif
                string newInput = Regex.Replace(input, hexPattern, hexReplacement, RegexOptions.IgnoreCase);

                // If no changes were made, try the # format
                if (newInput == input)
                {
#if UNITY_ANDROID
                    string hashPattern = @"style=color;([0-9a-fA-F]{6})>";
                    string hashReplacement = "style=color:#$1>";
#endif
#if UNITY_IOS
                    string hashPattern = @"style=color;([0-9a-fA-F]{6})";
                    string hashReplacement = "style=\"color:#$1\"";
#endif
                    newInput = Regex.Replace(input, hashPattern, hashReplacement, RegexOptions.IgnoreCase);
                }

                // If no changes were made in either case, break the loop
                if (newInput == input)
                    break;

                input = newInput;
            }
            //Debug.Log($"[YGTextTMP] Replaced HEX color tags: {input}");

            return input;
        }

        public string RemoveColorSpan(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string newString = Regex.Replace(input, "<.*?>", "", RegexOptions.IgnoreCase);
            // Remove emoji unicode sequences like \U0001F496
            // newString = Regex.Replace(newString, @"\\U[0-9A-Fa-f]{8}", "");

            return newString;
        }



        public static Image CreateTextToImage(string text, int width, int height, Color color, Color outlineColor, int fontSize, int outlineWidth, string defaultColorHex = "#FFFFFF",
                                            string alignment = "Center", string fontStyle = "Bold", int offSetHeight = 0, int offSetWidth = 0, bool isBold = false,
                                            string verticalAlignment = "Center")
        {
            Image imageComponent;
            GameObject imageGO = new GameObject("RegionalTextToImage");
            imageComponent = imageGO.AddComponent<Image>();

            text = ReplaceAllHexColorTags(text);
            width = width * upscaleFactor;
            height = height * upscaleFactor;
            fontSize = fontSize * upscaleFactor;

            imageComponent.sprite = NativeTextToImageManager.Instance.getTextToImageFromNative(
                text, width, height, color, outlineColor, fontSize, outlineWidth,
                alignment, fontStyle, offSetHeight, offSetWidth, isBold, defaultColorHex, verticalAlignment
            );

            return imageComponent;
        }


        public static void RegionalTextConvertor(GameObject regional, string text, int fontsize = 50, bool isOutline = false, int outlineWidth = 0)
        {
            if (LanguageHandler.GetLanguageEnum() != LanguageHandler.InGameLanguage.en && !LanguageHandler.IsAppLangSetInEnlish())
            {
                GameObject child = new GameObject("Regional_" + regional.gameObject.name);
                child.transform.SetParent(regional.transform, false);
                RectTransform rectTransform = child.AddComponent<RectTransform>();
                RectTransform logoRectTransform = regional.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(logoRectTransform.sizeDelta.x, logoRectTransform.sizeDelta.y);
                Image image = child.AddComponent<Image>();
                string color = ColorUtility.ToHtmlStringRGB(Color.black);
                image.sprite = CreateTextToImage(text, (int)logoRectTransform.sizeDelta.x, (int)logoRectTransform.sizeDelta.y,
                                                            Color.black, Color.white, fontsize, outlineWidth, color).sprite;
                image.preserveAspect = true;

                var regionalImage = regional.GetComponent<Image>();
                if (regionalImage != null)
                {
                    regionalImage.enabled = false;
                }

                if (isOutline)
                {
                    // var outline = child.AddComponent<Outline>();
                    // outline.effectColor = Color.black;
                    // outline.effectDistance = new Vector2(2f, -2.5f);

                    // var shadow = child.AddComponent<Shadow>();
                    // shadow.effectColor = Color.black;
                    // shadow.effectDistance = new Vector2(2f, -1.5f);
                    
                    var mat = new Material(RunnerLibrary.GetInstance().textOutlineMaterial);
                    if (mat != null)
                        image.material = mat;
                }
            }
            else
            {
                if (regional.TryGetComponent(out Image image))
                {
                    image.enabled = true;
                    Transform child = regional.transform.Find("Regional_" + regional.gameObject.name);
                    if (child != null)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }

        public static string StripTags(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            // 1) Normalise <br> / <br/> to newline so you don’t lose manual breaks.
            html = Regex.Replace(html, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);

            // 2) Remove everything else between '<' and '>'.
            string withoutTags = Regex.Replace(html, @"<[^>]+>", string.Empty, RegexOptions.Singleline);

            // 3) Decode common HTML entities (&nbsp;, &amp;, etc.).
            return WebUtility.HtmlDecode(withoutTags);
        }

        public static string RemoveNewLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Handles Windows (\r\n), Unix/Linux (\n) and old-Mac (\r) endings.
            return Regex.Replace(text, @"\r?\n|\r", string.Empty);
        }
        
        public static string RemoveAllHtmlTags(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            string result = input;

            // Convert <br> or <br/> to actual newlines
            result = Regex.Replace(result, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);

            // Remove all other HTML-like tags including <span>, <b>, <size>, etc.
            result = Regex.Replace(result, "<.*?>", string.Empty);

            // Replace escaped newlines (\n) with real newlines
            result = result.Replace("\\n", "\n");

            // Clean up multiple spaces and newlines
            result = Regex.Replace(result, @"[ \t]+", " ");
            result = Regex.Replace(result, @"\n{2,}", "\n");

            return result.Trim();
        }

 

        #endregion
    }
}
