// using System;
// using System.Collections.Generic;
// using UnityEngine.UI;
// using UnityEngine;
// using TMPro;

// public class TextClass
// {
//     public FontData m_FontData = FontData.defaultFontData;  
    

//     [TextArea(3, 10)] [SerializeField] protected string m_Text = String.Empty;

//     private TextGenerator m_TextCache;
//     private TextGenerator m_TextCacheForLayout;
//     private Color m_ColorData;

//     static protected Material s_DefaultText = null;

//     // We use this flag instead of Unregistering/Registering the callback to avoid allocation.
//     [NonSerialized] protected bool m_DisableFontTextureRebuiltCallback = false;


//     /// <summary>
//     /// Get or set the material used by this Text.
//     /// </summary>

//     public TextGenerator cachedTextGenerator
//     {
//         get { return m_TextCache ?? (m_TextCache = (m_Text.Length != 0 ? new TextGenerator(m_Text.Length) : new TextGenerator())); }
//     }

//     public TextGenerator cachedTextGeneratorForLayout
//     {
//         get { return m_TextCacheForLayout ?? (m_TextCacheForLayout = new TextGenerator()); }
//     }

//     /// <summary>
//     /// Text's texture comes from the font.
//     /// </summary>
//     //        public Texture mainTexture
//     //        {
//     //            get
//     //            {
//     //                if (font != null && font.material != null && font.material.mainTexture != null)
//     //                    return font.material.mainTexture;
//     //
//     ////                if (m_Material != null)
//     ////                    return m_Material.mainTexture;
//     ////
//     ////                return base.mainTexture;
//     //            }
//     //        }





//     /// <summary>
//     /// Text that's being displayed by the Text.
//     /// </summary>

//     public virtual string text
//     {
//         get
//         {
//             return m_Text;
//         }
//         set
//         {
//             if (String.IsNullOrEmpty(value))
//             {
//                 if (String.IsNullOrEmpty(m_Text))
//                     return;
//                 m_Text = "";
//             }
//             else if (m_Text != value)
//             {
//                 m_Text = value;
//             }
//         }
//     }

//     /// <summary>
//     /// Whether this Text will support rich text.
//     /// </summary>

//     public bool supportRichText
//     {
//         get
//         {
//             return m_FontData.richText;
//         }
//         set
//         {
//             if (m_FontData.richText == value)
//                 return;
//             m_FontData.richText = value;
//         }
//     }

//     /// <summary>
//     /// Wrap mode used by the text.
//     /// </summary>

//     public bool resizeTextForBestFit
//     {
//         get
//         {
//             return m_FontData.bestFit;
//         }
//         set
//         {
//             if (m_FontData.bestFit == value)
//                 return;
//             m_FontData.bestFit = value;
//         }
//     }

//     public int resizeTextMinSize
//     {
//         get
//         {
//             return m_FontData.minSize;
//         }
//         set
//         {
//             if (m_FontData.minSize == value)
//                 return;
//             m_FontData.minSize = value;
//         }
//     }

//     public int resizeTextMaxSize
//     {
//         get
//         {
//             return m_FontData.maxSize;
//         }
//         set
//         {
//             if (m_FontData.maxSize == value)
//                 return;
//             m_FontData.maxSize = value;
//         }
//     }

//     /// <summary>
//     /// Alignment anchor used by the text.
//     /// </summary>

//     public TextAnchor alignment
//     {
//         get
//         {
//             return m_FontData.alignment;
//         }
//         set
//         {
//             if (m_FontData.alignment == value)
//                 return;
//             m_FontData.alignment = value;
//         }
//     }

//     public int fontSize
//     {
//         get
//         {
//             return m_FontData.fontSize;
//         }
//         set
//         {
//             if (m_FontData.fontSize == value)
//                 return;
//             m_FontData.fontSize = value;
//         }
//     }

//     public HorizontalWrapMode horizontalOverflow
//     {
//         get
//         {
//             return m_FontData.horizontalOverflow;
//         }
//         set
//         {
//             if (m_FontData.horizontalOverflow == value)
//                 return;
//             m_FontData.horizontalOverflow = value;
//         }
//     }

//     public VerticalWrapMode verticalOverflow
//     {
//         get
//         {
//             return m_FontData.verticalOverflow;
//         }
//         set
//         {
//             if (m_FontData.verticalOverflow == value)
//                 return;
//             m_FontData.verticalOverflow = value;
//         }
//     }

//     public float lineSpacing
//     {
//         get
//         {
//             return m_FontData.lineSpacing;
//         }
//         set
//         {
//             if (m_FontData.lineSpacing == value)
//                 return;
//             m_FontData.lineSpacing = value;
//         }
//     }

//     /// <summary>
//     /// Font style used by the Text's text.
//     /// </summary>

//     public FontStyle fontStyle
//     {
//         get
//         {
//             return m_FontData.fontStyle;
//         }
//         set
//         {
//             if (m_FontData.fontStyle == value)
//                 return;
//             m_FontData.fontStyle = value;
//         }
//     }

//     public Color TextColor
//     {
//         get
//         {
//             return m_ColorData;
//         }
//         set
//         {
//             if (m_ColorData == value)
//                 return;
//             m_ColorData = value;
//         }
//     }



// }