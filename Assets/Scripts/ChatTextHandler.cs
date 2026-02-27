// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class ChatTextHandler : MonoBehaviour
// {
// 	[Header("TextMeshPro")]
// 	public TextMeshProUGUI textRef;
// 	public TextMeshProUGUI placeHolderTextRef;

// 	[Header("Text")]
// 	public Text txtRef;
// 	public Text placeHolderTxtRef;


// 	public Image imageRef;
// 	public int text_Width = 200;
// 	public int text_Size = 60;
// 	public int textQuality = 100;
// 	int text_Height;
// 	Color txtColor;
// 	Color txtOutlineColor;


// 	public enum Allignment
// 	{
// 		Center,
// 		Left,
// 		Right,
// 		Top,
// 		Down
// 	};

// 	public enum FontStyle
// 	{
// 		Normal,
// 		Bold,
// 		Italic,
// 		Bold_Italic
// 	};



// 	public Allignment text_Allignment = Allignment.Center;
// 	public FontStyle font_Style = FontStyle.Bold;

// 	void Start() {
// 		//Debug.Log(" start in regional ");
// 		//if (placeHolderTextRef != null)
// 		//	ConvertTextToImage(placeHolderTextRef);

// 		//if(placeHolderTxtRef != null)
// 		//	ConvertTxtToImage(placeHolderTxtRef);


// 	}


// 	public void ConvertTextToImage(TextMeshProUGUI txtRef)	
// 	{
// 		Debug.Log("<Start>" + txtRef.text);
// #if !UNITY_EDITOR && UNITY_ANDROID
// 		Debug.Log(txtRef.color.GetHashCode ());

		
// 		Sprite sprite = NativeTextToImageManager.Instance.getTextToImageFromNative(txtRef.text, text_Width, text_Height, txtColor, txtOutlineColor, text_Size, textQuality, getAllignmentText(), getFontStyle());

// 		Color imageColor = new Color(textRef.color.r, textRef.color.g, textRef.color.b, 1);
// 		imageRef.color = imageColor;
// 		if (txtRef.color.a > 0)
// 		{
// 			imageColor.a = 0;
// 			txtRef.color = imageColor;
// 		}

// 		imageRef.sprite = sprite;
// 		imageRef.SetNativeSize();
// 		if (!imageRef.gameObject.activeInHierarchy)
// 			imageRef.gameObject.SetActive(true);

// #endif
// 	}

// 	public void ConvertTxtToImage(Text txtRef)
// 	{
// 		Debug.Log("<Start> ConvertTxtToImage -- " + txtRef.text);
// #if !UNITY_EDITOR && UNITY_ANDROID
// 		Debug.Log(txtRef.color.GetHashCode());
// 		if (txtRef != null)
// 		{

// 			Sprite sprite = NativeTextToImageManager.Instance.getTextToImageFromNative(txtRef.text, text_Width, text_Height, txtColor, txtOutlineColor, text_Size, textQuality, getAllignmentText(), getFontStyle());

// 			Color imageColor = new Color(txtRef.color.r, txtRef.color.g, txtRef.color.b, 1);
// 			imageRef.color = imageColor;
// 			if (txtRef.color.a > 0)
// 			{
// 				imageColor.a = 0;
// 				txtRef.color = imageColor;
// 			}

// 			imageRef.sprite = sprite;
// 			imageRef.SetNativeSize();
// 			if (!imageRef.gameObject.activeInHierarchy)
// 				imageRef.gameObject.SetActive(true);

// 		}
// #endif
// 	}

// 	string getAllignmentText() {
// 		if(text_Allignment.Equals(Allignment.Left))
// 			return "Left";
// 		else if(text_Allignment.Equals(Allignment.Right))
// 			return "Right";
// 		else
// 			return "Center";
// 	}

// 	string getFontStyle() {
// 		if(font_Style.Equals(FontStyle.Bold))
// 			return "Bold";
// 		else if(font_Style.Equals(FontStyle.Italic))
// 			return "Italic";
// 		else if(font_Style.Equals(FontStyle.Bold_Italic))
// 			return "BI";
// 		else
// 			return "Normal";
// 	}



// }
