using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayReducer : MonoBehaviour
{
    public static float DELAY_VALUE = 0.5f;
    public static float REQUESTED_DELAY = 0.5f;
    [SerializeField]
    private Text delayReducertext;
    private Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        DELAY_VALUE = slider.value / 1000;
        Debug.Log(DELAY_VALUE);
        slider.onValueChanged.AddListener((a) =>
        {
            DELAY_VALUE = a / 1000;
            delayReducertext.text = a + "ms";
            Debug.Log(DELAY_VALUE);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
