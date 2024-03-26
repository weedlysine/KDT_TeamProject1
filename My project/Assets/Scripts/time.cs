using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class time : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<TextMeshProUGUI>().text = DateTime.Now.ToString("HH:mm");
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<TextMeshProUGUI>().text = DateTime.Now.ToString("HH:mm");
    }
}
