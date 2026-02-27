using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace YG_Text
{
    public class RegionalConverter : MonoBehaviour
    {
        public string textKey;
        public bool updateText;
        public bool AddGradientColor;
        TextMeshProUGUI tmPro;
        public static List<RegionalConverter> Instances { get; } = new List<RegionalConverter>();

        [Header("RegionalAlignemnet")]
        public int offSetWidth = 0;
        public int offSetHeight = 0;
        public int textOffSetSize = 0;


        private void Awake()
        {
            Instances.Add(this);
            if (updateText)
            {
                tmPro = GetComponent<TextMeshProUGUI>();
                TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
            }
        }

        private void OnDestroy()
        {
            Instances.Remove(this);
            if (updateText)
            {
                TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
            }
        }

        private string _lastText = null;

        public void OnTextChanged(Object obj)
        {
            try
            {
                if (tmPro != null && obj == tmPro)
                {
                    //Debug.Log("regionalconverter: OnTextChanged 1");

                    if (_lastText != tmPro.text && tmPro.text.Length > 0)
                    {
                        _lastText = tmPro.text;
                        RegionalRefresh(_lastText);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"OnTextChanged Exception: {ex}");
            }
        }

        // Call this method when the tmPro text is updated
        public void OnTextChangedFromLOC(string updatedText, TextMeshProUGUI tmproRef)
        {
            try
            {
                //Debug.Log("regionalconverter: OnTextChangedFromLOC");

                tmPro = tmproRef;
                if (_lastText != updatedText && updatedText.Length > 0)
                {
                    _lastText = updatedText;
                    RegionalRefresh(updatedText);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"OnTextChanged Exception: {ex}");
            }
        }

        public void SetTextKey()
        {
            if (updateText)
            {
                if (string.IsNullOrEmpty(textKey))
                {
                    Debug.LogWarning("Text key is not set for RegionalConverter on " + gameObject.name);
                    return;
                }
                tmPro.text = string.Format(App.T(textKey));

            }
        }

        public void RegionalRefresh(string updatedText)
        {
            //Debug.Log("regionalconverter: RegionalRefresh");

            if (LanguageHandler.GetLanguageEnum() != LanguageHandler.InGameLanguage.en && !LanguageHandler.IsAppLangSetInEnlish())
            {
                CreateChildWithYGTextTMP(updatedText);
            }
            else if(regionalObject != null)
            {
                if (this.isActiveAndEnabled)
                {  
                    StartCoroutine(DestroyChildWithYGTextTMP());
                }
                else
                {
                    // Call as a regular method if inactive
                    DestroyChildWithYGTextTMPImmediate();
                }
            }
            

        }

        GameObject regionalObject;

        // Immediate destroy version for inactive objects
        private void DestroyChildWithYGTextTMPImmediate()
        {
            if (regionalObject != null)
            {
                Destroy(regionalObject);
                regionalObject = null;
            }

            // Reset alpha to 1 for English
            var color = tmPro.color;
            color.a = 1f;
            tmPro.color = color;
           // tmPro.ForceMeshUpdate();
        }

        public GameObject CreateChildWithYGTextTMP(string updatedText)
        {
            //Debug.Log("regionalconverter: CreateChildWithYGTextTMP");
            YGTextTMP ygTextTMP;
            // Check if a child with the same name exists and destroy it
            // Transform existingChild = this.transform.Find("Regional_" + this.gameObject.name);
            if (regionalObject != null)
            {
                ygTextTMP = regionalObject.GetComponent<YGTextTMP>();
                if (ygTextTMP != null)
                {
                    ygTextTMP.multiPropChanging = true;
                    ygTextTMP.color = tmPro.color; // Copy color from original TextMeshPro
                    ygTextTMP.parentRectTransform = this.GetComponent<RectTransform>();
                    ygTextTMP.alignment = tmPro.alignment; // Copy alignment from original TextMeshPro
                    ygTextTMP.fontStyles = FontStyles.Bold;
                    ygTextTMP.fontSize = (int)tmPro.fontSize;
                    ygTextTMP.offSetRectHeight = offSetHeight;
                    ygTextTMP.offSetRectWidth = offSetWidth;
                    ygTextTMP.FontSizeOffset = textOffSetSize;
                    if (AddGradientColor)
                        ygTextTMP.UseGradientColor = true;
                    ygTextTMP.multiPropChanging = false;
                    ygTextTMP.text = updatedText;
                }
                return null;
            }

            regionalObject = new GameObject("Regional_" + this.gameObject.name);
            regionalObject.transform.SetParent(this.transform, false);
            ygTextTMP = regionalObject.AddComponent<YGTextTMP>();

            ygTextTMP.multiPropChanging = true;
            ygTextTMP.color = tmPro.color; // Copy color from original TextMeshPro
            ygTextTMP.parentRectTransform = this.GetComponent<RectTransform>();
            ygTextTMP.alignment = tmPro.alignment; // Copy alignment from original TextMeshPro
            ygTextTMP.fontStyles = FontStyles.Bold;
            ygTextTMP.fontSize = (int)tmPro.fontSize;
            ygTextTMP.offSetRectHeight = offSetHeight;
            ygTextTMP.offSetRectWidth = offSetWidth;
            ygTextTMP.FontSizeOffset = textOffSetSize;
            ygTextTMP.multiPropChanging = false;
            ygTextTMP.text = updatedText;

            // Set alpha to 0 for non-English languages
            var color = tmPro.color;
            color.a = 0f;
            tmPro.color = color;
            tmPro.ForceMeshUpdate();

            if (AddGradientColor)
                ygTextTMP.UseGradientColor = true;

            return null;
        }

        public IEnumerator DestroyChildWithYGTextTMP()
        {
            //Debug.Log("DestroyChildWithYGTextTMP");
            // Wait for end of frame to ensure UI updates are processed
            yield return new WaitForEndOfFrame();

            Destroy(regionalObject);
            regionalObject = null;

            // Reset alpha to 1 for English
            var color = tmPro.color;
            color.a = 1f;
            tmPro.color = color;
            tmPro.ForceMeshUpdate();
            yield return new WaitForEndOfFrame();

        }
    }
}