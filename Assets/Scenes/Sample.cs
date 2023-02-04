using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    public GameObject build_text;

    // Start is called before the first frame update
    void Start()
    {
        if (Debug.isDebugBuild)
        {
            build_text.GetComponent<Text>().text = "Debub Build.";
        }
        else
        {
            build_text.GetComponent<Text>().text = "Release Build.";
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
