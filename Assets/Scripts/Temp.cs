using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Temp : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonClick(Button button)
    {
        var state = button.spriteState;
        Debug.LogError("Clicked");
    }
    public void foo()
    {
        Debug.LogError("Clicked");

    }

}
