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
        UpdatePoints();
    }
    public void UpdatePoints()
    {

        text = GetComponent<Text>();
        text.text = $"points : {PointsUtil.GetCurrentPoints()}";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
