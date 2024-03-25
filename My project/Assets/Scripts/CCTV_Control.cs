using bosqmode.libvlc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV_Control : MonoBehaviour
{
    public GameObject[] cctv_list;
    bool cctv_changing;
    public string[] cctv_url;
    int cctv_num = 100000;
    VLCPlayerMono vlc;
    // Start is called before the first frame update
    void Start()
    {
        vlc = gameObject.GetComponent<VLCPlayerMono>();
        vlc.enabled = true;
        vlc.url = cctv_url[cctv_num % cctv_list.Length];
        cctv_list[cctv_num % cctv_list.Length].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Horizontal") == 1)//¿ÞÂÊ
        {
            if(!cctv_changing)
                StartCoroutine(cctv_change(Input.GetAxisRaw("Horizontal")));
        }
          
    }

    public IEnumerator cctv_change (float a)
    {
        cctv_changing = true;
        cctv_list[cctv_num % cctv_list.Length].SetActive(false);
        cctv_num += (int)a;
        vlc.url = cctv_url[cctv_num % cctv_list.Length];
        cctv_list[cctv_num % cctv_list.Length].SetActive(true);
        yield return new WaitForSeconds(1);
        vlc.playerUpdate();
        cctv_changing = false;
    }
}
