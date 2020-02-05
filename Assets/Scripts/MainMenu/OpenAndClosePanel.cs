using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenAndClosePanel : MonoBehaviour
{
    public GameObject ToOpen;
    public GameObject ToClose;

    void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(()=>
        {
            ToClose?.SetActive(false);
            ToOpen?.SetActive(true);
        });
    }
}
