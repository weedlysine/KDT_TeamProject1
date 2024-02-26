//#define LOCAL

using UnityEngine;
using Vizario;
using Vizario.H264DecoderLib;
using Vizario.H264DecoderLib.Utils;
using Vizario.AACDecoderLib;
using System;
using Unity.Collections;
using System.IO;
using System.Collections;
using System.Collections.Generic;

// using Microsoft.Extensions.Logging;

/// <summary>
/// Creates a class to automatically set up an decoding/streaming/file writing instance.
/// </summary>
public class H264RtspPlayerDriver : MonoBehaviour
{
    // THIS NEEDS TO BE SET TO THE IMAGE/RENDERER USED FOR DISPLAY
    public GameObject renderplane = null;
    public GameObject spriteplane = null;
    public GameObject transmit = null;
    public GameObject fpstext = null;
    public GameObject address = null;

    private UnityEngine.UI.InputField m_address;
    private UnityEngine.UI.Text m_fpstext;

    // THIS IS THE INTERNAL DATA
    private H264Decoder m_h264decoder = null;
    private Byte m_h264decoderId = 0;

    private AACDecoder m_aacdecoder = null;
    private Byte m_aacdecoderId = 0;

    private H264AACBridge m_bridge = null;
    private bool m_buffering = true; // only works if on RTSP (not LOCAL)!

    private static bool m_isH265 = false;

    private bool m_save = false; // should ALWAYS be off for local!
    private string m_h264OutFileName = null;
    private string m_aacOutFileName = null;

#if LOCAL
    private string m_testh264FileName = m_isH265 ? "test.h265" : "test.h264";
    private RawH264FileReader m_rawh264datareader = null;
    private string m_testaacFileName = "test.aac";
    private RawAACFileReader m_rawaacdatareader = null;

    private RTSPClient.MEDIA_REQUEST m_request = RTSPClient.MEDIA_REQUEST.VIDEO_AND_AUDIO;
#else
    //private RTSPClient.MEDIA_REQUEST m_request = RTSPClient.MEDIA_REQUEST.VIDEO_ONLY;
    private RTSPClient.MEDIA_REQUEST m_request = RTSPClient.MEDIA_REQUEST.VIDEO_AND_AUDIO;
    //private RTSPClient.MEDIA_REQUEST m_request = RTSPClient.MEDIA_REQUEST.AUDIO_ONLY;
#endif

#if !LOCAL
    RTSPClient m_rtspclient = null;
#endif

    private IEnumerator m_caller = null;
    /// <summary>
    /// Initialize certain variables at the start of the script.
    /// </summary>
    private void Awake()
    {
        //UnitySystemConsoleRedirector.Redirect();
#if LOCAL
#if UNITY_ANDROID || UNITY_IOS
        m_rawh264datareader = new RawH264FileReader(Path.Combine(Application.streamingAssetsPath, m_testh264FileName), m_isH265, true);
        m_rawaacdatareader = new RawAACFileReader(Path.Combine(Application.streamingAssetsPath, m_testaacFileName), true);
#else
#if UNITY_EDITOR
        m_rawh264datareader = new RawH264FileReader(Path.Combine(Application.streamingAssetsPath, m_testh264FileName), m_isH265, true);
        m_rawaacdatareader = new RawAACFileReader(Path.Combine(Application.streamingAssetsPath, m_testaacFileName), true);
#else
        m_rawh264datareader = new RawH264FileReader("Assets/StreamingAssets/" + m_testh264FileName, m_isH265, true);
        m_rawaacdatareader = new RawAACFileReader("Assets/StreamingAssets/", m_testaacFileName), true);
#endif
#endif
#endif
        if (address != null)
        {
            m_address = address.GetComponent<UnityEngine.UI.InputField>();
        }

        if (fpstext != null)
        {
            m_fpstext = fpstext.GetComponent<UnityEngine.UI.Text>();
        }
        return;
    }

    public void updateFPS(string value)
    {
        if (m_fpstext != null)
        {
            m_fpstext.text = value;
        }

    }

    // note that this is safe because it is internally invoked to be run on the
    // main thread of unity!
    public void SafeFormatChangeNotification(H264Decoder.TextureInfo textureInfo)
    {
        if (renderplane.activeSelf)
        {
            Debug.Log("Format change message received!");
            // this is to inform the main script 
            // you can resize the texture aspect ratio here if you want
            // keep x fixed, change z only; note that if you put something upside down it needs to respect that in the formula
            float oldX = renderplane.gameObject.transform.localScale.x;
            float oldY = renderplane.gameObject.transform.localScale.y;
            float oldZ = renderplane.gameObject.transform.localScale.z;
            float newZ = (float)textureInfo.height / (float)textureInfo.width * oldX;
            renderplane.gameObject.transform.localScale = new Vector3(oldX, oldY, ((oldX * oldZ) < 0) ? -1.0f * newZ : newZ);
        }
        else if(spriteplane.activeSelf)
        {
            spriteplane.transform.localScale = new Vector3(0.2f, -0.2f, 1.0f);
        }
    }

    public async void StartPlay()
    {
        m_bridge = new H264AACBridge(m_buffering);

        if (renderplane == null && spriteplane == null)
        {
            Debug.LogError("Need to specify renderplane or spriteplane first!");
            return;
        }

#if !LOCAL
        // only create h264decoder if requested!
        if (m_request != RTSPClient.MEDIA_REQUEST.AUDIO_ONLY)
        {
#endif // !LOCAL
            if (m_h264decoder == null)
            {
                // CONFIGURE THE DECODER TO YOUR LIKES
                H264Decoder.DecoderConfiguration config = new H264Decoder.DecoderConfiguration
                {
                    isH265 = m_isH265,
#if UNITY_ANDROID
                    enforcedColorFormat = DECODERCOLORFORMAT.YUV420P,
#endif
                };
                // SWITCH BETWEEN SPRITE AND MESH RENDERER BASED ON GAMEOBJECT BEING ACTIVE OR NOT
                if (renderplane.activeSelf)
                {
                    Renderer renderer = renderplane.GetComponent<Renderer>();
                    m_h264decoder = new H264Decoder(m_h264decoderId, renderer, out bool success, config);
                }
                else if (spriteplane.activeSelf)
                {
                    m_h264decoder = new H264Decoder(m_h264decoderId, spriteplane, out bool success, config);
                }
                m_h264decoder.m_streamFormatChangedNotifyCallback += SafeFormatChangeNotification;
            }
            // note this is how fast the loop in Unity runs, not the frame
            // rate of the decoder!
            m_h264decoder.m_updateFpsText += updateFPS;
#if !LOCAL
        }
#endif // RTSP

        // only create aacdecoder if requested!
        if (m_request != RTSPClient.MEDIA_REQUEST.VIDEO_ONLY)
            if (m_aacdecoder == null)
            {
                m_aacdecoder = new AACDecoder(m_aacdecoderId, out bool success);
            }

#if !LOCAL
        string url = "rtsp://127.0.0.1:8554/live";
        if (m_address != null)
        {
            Debug.Log("Url: taking url from text field...");
            url = m_address.text;
        }

        string username = "";
        string password = "";

        int transporttype = 0;
        if(transmit != null)
        {
            transporttype = transmit.GetComponent<UnityEngine.UI.Dropdown>().value;
        }

        m_rtspclient = new RTSPClient(false);

        // or it is the first SPS/PPS from the H264 video stream
        if (m_request != RTSPClient.MEDIA_REQUEST.AUDIO_ONLY)
        {
            if (m_isH265)
            {
                // Video NALs. May also include the SPS and PPS in-band for H264
                m_rtspclient.Received_VPS_SPS_PPS += (byte[] vps, byte[] sps, byte[] pps, uint rtp_timestamp) =>
                {
                    m_bridge.Received_VPS_SPS_PPS(vps, sps, pps, rtp_timestamp);
                };
            }
            else
            {
                m_rtspclient.Received_SPS_PPS += (byte[] sps, byte[] pps, uint rtp_timestamp) =>
                {
                    m_bridge.Received_SPS_PPS(sps, pps, rtp_timestamp);
                };
            }

            // Video NALs. May also include the SPS and PPS in-band for H264
            m_rtspclient.Received_NALs += (List<byte[]> nal_units, uint rtp_timestamp) =>
            {
                m_bridge.Received_NALs(nal_units, rtp_timestamp);
            };
        }

        if (m_request != RTSPClient.MEDIA_REQUEST.VIDEO_ONLY)
        {

            m_rtspclient.Received_AAC += (String format, List<byte[]> aac_units, uint ObjectType, uint FrequencyIndex, uint ChannelConfiguration, uint rtp_timestamp) =>
            {
                m_bridge.Received_AAC(format, aac_units, ObjectType, FrequencyIndex,
                    ChannelConfiguration, rtp_timestamp);
            };
        }

        string errormsg;
        bool ok = false;
        switch (transporttype)
        {
            case 0:
                Debug.Log("Using UDP...");
                ok = m_rtspclient.Connect(url, RTSPClient.RTP_TRANSPORT.UDP, out errormsg, m_request);
                break;
            case 1:
                Debug.Log("Using TCP...");
                ok = m_rtspclient.Connect(url, RTSPClient.RTP_TRANSPORT.TCP, out errormsg, m_request);
                break;
            case 2:
                Debug.Log("Using Multicast...");
                ok = m_rtspclient.Connect(url, RTSPClient.RTP_TRANSPORT.MULTICAST, out errormsg, m_request);
                break;
            default:
                Debug.Log("Using UDP...");
                ok = m_rtspclient.Connect(url, RTSPClient.RTP_TRANSPORT.UDP, out errormsg, m_request);
                break;
        }
        if(!ok)
        {
            Debug.LogError(errormsg);
            return;
        }
        Debug.Log(errormsg);

        // saving only goes for RTSP code!
        if (m_save)
        {
            m_h264OutFileName = m_isH265 ? "out.h265" : "out.h264";
            m_aacOutFileName = "out.aac";

            m_h264OutFileName = Path.Combine(Application.streamingAssetsPath, m_h264OutFileName);
            m_aacOutFileName = Path.Combine(Application.streamingAssetsPath, m_aacOutFileName);
        }
#endif // !LOCAL

        m_bridge.StartBridge(m_h264decoder, m_aacdecoder, m_h264OutFileName, m_aacOutFileName);

        m_caller = CallPluginAtEndOfFrames();
        StartCoroutine(m_caller);
    }

    private void FixedUpdate()
    {

#if UNITY_2020_2_OR_NEWER
        double now = Time.fixedTimeAsDouble;
#else
        double now = (double)Time.fixedTime;
#endif

#if LOCAL
        m_bridge?.FixedTimeUpdateLocal(now, m_rawh264datareader, m_rawaacdatareader);
#else
        m_bridge?.FixedTimeUpdate(now);
#endif // RTSP
    }

    private IEnumerator CallPluginAtEndOfFrames()
    {
        while (true)
        {
            // Wait until all frame rendering is done
            yield return new WaitForEndOfFrame();

#if !LOCAL
            if (m_request != RTSPClient.MEDIA_REQUEST.AUDIO_ONLY)
            {
#endif // RTSP
                // handle rendering
                m_h264decoder?.HandleUpdate();
#if !LOCAL
            }
#endif // RTSP
            if (m_request != RTSPClient.MEDIA_REQUEST.VIDEO_ONLY)
            {
                m_aacdecoder?.HandleUpdate();
            }
        }
    }

    public void StopPlay()
    {
        if(m_caller != null)
            StopCoroutine(m_caller);
        m_caller = null;

        m_bridge?.StopBridge();
        m_bridge?.Dispose();
        m_bridge = null;

        m_h264OutFileName = null;
        m_aacOutFileName = null;

    // needs to go first
#if !LOCAL
        if (m_rtspclient != null)
        {
            m_rtspclient.Stop();
            // check client.StreamingFinished();
        }
        m_rtspclient = null;
#endif // RTSP

#if !LOCAL
        if (m_request != RTSPClient.MEDIA_REQUEST.AUDIO_ONLY)
        {
#endif // RTSP
            m_h264decoder?.Dispose();
            m_h264decoder = null;
#if !LOCAL
        }
#endif // RTSP

        if (m_request != RTSPClient.MEDIA_REQUEST.VIDEO_ONLY)
        {
            m_aacdecoder?.Dispose();
            m_aacdecoder = null;
        }
    }

    private void OnDestroy()
    {
        StopPlay();
#if LOCAL
        m_rawh264datareader?.Dispose();
        m_rawh264datareader = null;
        m_rawaacdatareader?.Dispose();
        m_rawaacdatareader = null;
#endif // LOCAL
    }
}
