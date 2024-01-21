using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMessage : MonoBehaviour
{
    Text textArea;
    public string text;

    void Start()
    {
        textArea = GameObject.FindWithTag("InGameMessage").GetComponent<Text>();
        textArea.text = "";
    }
    public void showText()
    {
        textArea.text = text.Replace("\\n", "\n");
    }

    public void cleanText()
    {
        textArea.text = "";
    }
    void OnTriggerEnter() { 
        showText();
    }

    void OnTriggerExit()
    {
        cleanText();
    }
}
