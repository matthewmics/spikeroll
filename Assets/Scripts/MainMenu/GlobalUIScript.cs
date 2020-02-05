using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalUIScript : MonoBehaviour
{

    public string SceneToOpen;

    public enum ButtonFunctions
    {
        RETURN_TO_MENU, OPEN_SCENE
    }

    public ButtonFunctions ButtonFunction;

    // Start is called before the first frame update
    void Start()
    {
        var btn = GetComponent<Button>();

        switch (ButtonFunction)
        {
            case ButtonFunctions.RETURN_TO_MENU:
                btn.onClick.AddListener(()=>
                {
                    SceneManager.LoadScene("MainMenu");
                });
                break;

            case ButtonFunctions.OPEN_SCENE:
                btn.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene(SceneToOpen);
                });
                break;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
