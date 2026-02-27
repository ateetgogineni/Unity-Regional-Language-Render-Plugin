using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#pragma warning disable IDE1006

namespace YG_Text
{
public class YGText : MonoBehaviour
{

	#region Properties

	[TextArea(3, 10)]
    [SerializeField]
    [FormerlySerializedAs("text")]
    private string m_Text;

	[Header("Character")]

	[Space(10)]
	[SerializeField]
	[FormerlySerializedAs("font")]
	private Font m_Font;

	[SerializeField]
	[FormerlySerializedAs("fontStyle")]
	private FontStyle m_FontStyle = (FontStyle)0;

	[SerializeField]
	[FormerlySerializedAs("fontSize")]
	private int m_FontSize = 14;

	[SerializeField]
	[FormerlySerializedAs("lineSpacing")]
	private float m_LineSpacing = 1f;

	[SerializeField]
	[FormerlySerializedAs("richText")]
	private bool m_RichText = true;


	[Space(10)]
	[Header("Paragraph")]

	[Space(10)]
	//[SerializeField]
	//[FormerlySerializedAs("alignment")]
	//private TextAlignmentOptions m_Alignment = TextAlignmentOptions.TopLeft;
	[SerializeField]
	[FormerlySerializedAs("alignment")]
	private TextAnchor m_Alignment = TextAnchor.UpperLeft;

	[SerializeField]
	[FormerlySerializedAs("alignByGeometry")]
	private bool m_AlignByGeometry = false;

	[SerializeField]
	[FormerlySerializedAs("horizontalOverflow")]
	private HorizontalWrapMode m_HorizontalOverflow = (HorizontalWrapMode)0; 

	[SerializeField]
	[FormerlySerializedAs("verticalOverflow")]
	private VerticalWrapMode m_VerticalOverflow = (VerticalWrapMode)0;

	[SerializeField]
	[FormerlySerializedAs("bestFit")]
	private bool m_BestFit = false;

	[SerializeField]
	[FormerlySerializedAs("minSize")]
	private int m_MinSize = 10;

	[SerializeField]
	[FormerlySerializedAs("maxSize")]
	private int m_MaxSize = 40;

	[Space(10)]
	[Header("Color")]

	[Space(10)]
	[SerializeField]
	[FormerlySerializedAs("color")]
	private Color m_Color = new Color(0.196f, 0.196f, 0.196f, 1.0f);

	[SerializeField]
	[FormerlySerializedAs("material")]
	protected Material m_Material;

	[SerializeField]
	[FormerlySerializedAs("raycastTarget")]
	private bool m_raycastTarget = true;

	[SerializeField]
	[FormerlySerializedAs("raycastPadding")]
	private Vector4 m_raycastPadding;

	[SerializeField]
	[FormerlySerializedAs("maskable")]
	private bool m_Maskable = true;


	float textWidthOffset = 1.0f;
	bool isLongString = false;


	public string text
	{
		get
		{
			return m_Text;
		}
		set
		{
			string oldText = m_Text;
			m_Text = value;
		
			if (oldText != m_Text)
			{
				if(gameObject.activeInHierarchy)
					OnTextChanged();
			}
		}
	}

	public Font font
	{
		get
		{
			return m_Font;
		}
		set
		{
			m_Font = value;
		}
	}

	public FontStyle fontStyle
	{
		get
		{
			return m_FontStyle;
		}
		set
		{
			m_FontStyle = value;
		}
	}

	public int fontSize
	{
		get
		{
			return m_FontSize;
		}
		set
		{
			m_FontSize = value;
		}
	}

	public float lineSpacing
	{
		get
		{
			return m_LineSpacing;
		}
		set
		{
			m_LineSpacing = value;
		}
	}

	public bool richText
	{
		get
		{
			return m_RichText;
		}
		set
		{
			m_RichText = value;
		}
	}

	public TextAnchor alignment
	{
		get
		{
			return m_Alignment;
		}
		set
		{
			m_Alignment = value;
		}
	}

	public bool alignByGeometry
	{
		get
		{
			return m_AlignByGeometry;
		}
		set
		{
			m_AlignByGeometry = value;
		}
	}



	public HorizontalWrapMode horizontalOverflow
	{
		get
		{
			return m_HorizontalOverflow;
		}
		set
		{
			m_HorizontalOverflow = value;
		}
	}

	public VerticalWrapMode verticalOverflow
	{
		get
		{
			return m_VerticalOverflow;
		}
		set
		{
			m_VerticalOverflow = value;
		}
	}

	public bool bestFit
	{
		get
		{
			return m_BestFit;
		}
		set
		{
			m_BestFit = value;
		}
	}

	public int minSize
	{
		get
		{
			return m_MinSize;
		}
		set
		{
			m_MinSize = value;
		}
	}

	public int maxSize
	{
		get
		{
			return m_MaxSize;
		}
		set
		{
			m_MaxSize = value;
		}
	}

	public Color color
	{
		get
		{
			return m_Color;
		}
		set
		{
			m_Color = value;
		}
	}

	protected Material material
	{
		get
		{
			return m_Material;
		}
		set
		{
			m_Material = value;
		}
	}

	public bool raycastTarget
	{
		get
		{
			return m_raycastTarget;
		}
		set
		{
			m_raycastTarget = value;
		}
	}

	protected Vector4 raycastPadding
	{
		get
		{
			return m_raycastPadding;
		}
		set
		{
			m_raycastPadding = value;
		}
	}

	public bool maskable
	{
		get
		{
			return m_Maskable;
		}
		set
		{
			m_Maskable = value;
		}
	}


    #endregion

    private void OnEnable()
    {
		
		rectTransformRef = gameObject.GetComponent<RectTransform>();
		
			originalAnchormax=rectTransformRef.anchorMax;
			originalAnchormin=rectTransformRef.anchorMin;
			sizeDelta=rectTransformRef.sizeDelta;
		
		
		if(font == null)
        {
			font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		}
		StartCoroutine(DelayTextUpdating());
	}

	IEnumerator DelayTextUpdating()
	{
		yield return null;
		#if UNITY_ANDROID && !UNITY_EDITOR
				AddImageComponentUpdateContent();
		#else
				AddTextComponentUpdateContent();
		#endif
	}	


	Text textComponentRef;
	Image imageComponentRef;
	RectTransform rectTransformRef;

	Vector2 originalAnchormin;
	Vector2 originalAnchormax;
	Vector2 sizeDelta;

	private void AddTextComponentUpdateContent()
  {
		textComponentRef = gameObject.GetComponent<Text>();
		if (textComponentRef == null)
		{
			textComponentRef = gameObject.AddComponent<Text>();
		}



		int width = (int)(rectTransformRef.rect.width);
		int height = (int)(rectTransformRef.rect.height);
		Debug.Log("width " + width + ", height " + height);
		textComponentRef.text = text;

		Debug.Log(" font " + fontSize);
		if (font != null)
		{
			textComponentRef.font = font;
		}

		textComponentRef.fontStyle = fontStyle;
		textComponentRef.fontSize = fontSize;
		textComponentRef.lineSpacing = lineSpacing;
		textComponentRef.supportRichText = richText;

		textComponentRef.alignment = alignment;
		textComponentRef.alignByGeometry = alignByGeometry;
		textComponentRef.horizontalOverflow = horizontalOverflow;
		textComponentRef.verticalOverflow = verticalOverflow;
		textComponentRef.resizeTextForBestFit = bestFit;
		textComponentRef.resizeTextMinSize = minSize;
		textComponentRef.resizeTextMaxSize = maxSize;

		textComponentRef.color = color;
		textComponentRef.material = material;
		textComponentRef.raycastTarget = raycastTarget;
		textComponentRef.raycastPadding = raycastPadding;
		textComponentRef.maskable = maskable;
    }

	private void AddImageComponentUpdateContent()
	{
		imageComponentRef = gameObject.GetComponent<Image>();
		if (imageComponentRef == null)
		{
			imageComponentRef = gameObject.AddComponent<Image>();
		}
		Outline outlineComponent = gameObject.GetComponent<Outline>();
		Color outlineColor = Color.white;
		int outlineWidth = 0;

		if (outlineComponent != null)
		{
			outlineColor = outlineComponent.effectColor;
			outlineWidth = (int)outlineComponent.effectDistance.x;
		}

		Shadow shadowComponent = gameObject.GetComponent<Shadow>();
		if (shadowComponent != null)
		{
			outlineColor = shadowComponent.effectColor;
			outlineWidth = (int)shadowComponent.effectDistance.x;
		}

		bool isColorSpan = false;
		string colorValue = "#FFFFFF"; // default white
		if (text.Contains("<span")) // check for any html color span
		{
			isColorSpan = true;
			colorValue = "";
		}

		if(rectTransformRef == null)
		{
			rectTransformRef = gameObject.GetComponent<RectTransform>();
		}

		int width = (int)(rectTransformRef.rect.width);
		int height = (int)(rectTransformRef.rect.height);
		width = width <= 0 ? 50 : width;
        
		Sprite sprite = null;
#if UNITY_ANDROID && !UNITY_EDITOR
            sprite = NativeTextToImageManager.Instance.getTextToImageFromNative(
				text, width, height,color, outlineColor, fontSize, 100,
				getAllignmentText(alignment.ToString()), getFontStyle(), outlineWidth, textWidthOffset, isLongString , colorValue);
#endif

		if (sprite != null)
		{
			imageComponentRef.sprite = sprite;
			if (!isColorSpan) // if no multicolor then only we apply image color
			{
				imageComponentRef.color = color;
			}
			
			if(originalAnchormin==new Vector2(0.5f, 0.5f) && originalAnchormax == new Vector2(0.5f, 0.5f))
			{
				imageComponentRef.SetNativeSize();
			}
			else
			{
				rectTransformRef.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransformRef.anchorMax = new Vector2(0.5f, 0.5f);

				imageComponentRef.SetNativeSize();
				rectTransformRef.anchorMin = originalAnchormin;
				rectTransformRef.anchorMax = originalAnchormax;

				rectTransformRef.sizeDelta=sizeDelta;
				imageComponentRef.preserveAspect=true;
			}

			
		}
		
	}


	void OnTextChanged()
    {
		StartCoroutine(DelayTextUpdating());
	}

	string getAllignmentText(string allignment)
	{
		if (allignment.Contains("Left"))
			return "Left";
		else if (allignment.Contains("Right"))
			return "Right";
		else
			return "Center";
	}

	string getFontStyle()
	{
		if (fontStyle.Equals(FontStyle.Bold))
			return "Bold";
		else if (fontStyle.Equals(FontStyle.Italic))
			return "Italic";
		else if (fontStyle.Equals(FontStyle.BoldAndItalic))
			return "BI";
		else
			return "Normal";
	}

}
}


// [CustomEditor(typeof(YGText))]
// public class YGTextEditor : Editor
// {
//    public override void OnInspectorGUI()
//    {
//        YGText myTarget = (YGText)target;

//        myTarget.Text = EditorGUILayout.TextField("TextEditor", myTarget.Text);
//        EditorGUILayout.LabelField("Text", myTarget.Text.ToString());
//    }
// }

