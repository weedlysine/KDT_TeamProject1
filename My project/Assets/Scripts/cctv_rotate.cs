using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cctv_rotate : MonoBehaviour
{
    public Canvas[] RI = new Canvas[4];
    public GameObject MC;
    // Start is called before the first frame update
    void Start()
    {
        fix_rotation();
    }

    public void fix_rotation()
    {
        for (int i = 0; i < RI.Length; i++)
        {
            RI[i].GetComponent<RectTransform>().rotation = MC.transform.rotation;
        }
    }

    private void Update()
    {
        fix_rotation() ;
    }

    public void btn_test()
    {
        Debug.Log("wpqkf");
    }
}
