using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsDisplay : MonoBehaviour
{
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.text = $"Points : {PointsUtil.GetCurrentPoints()}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
