using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Finish_Clear_UI : PopupUI
{
    [SerializeField] Button endBtn;


    private void Start()
    {
        if (endBtn != null) endBtn.onClick.AddListener(OnClickEndBtn);
    }

    public void OnClickEndBtn()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
       Application.Quit();  
#endif
    }
}
