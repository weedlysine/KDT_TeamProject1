using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace bosqmode.libvlc
{
    public class VLCPlayerMono : MonoBehaviour
    {
        public string url;
        public class ButtonUrlPair
        {
            public Button button;
            public string url;
        }

        [SerializeField]
        private RawImage m_rawImage;

        [SerializeField]
        private List<ButtonUrlPair> buttonUrlPairs;

        [SerializeField]
        [Min(0)]
        [Tooltip("Output resolution width, can be left 0 for automatic scaling")]
        private int width = 480;

        [SerializeField]
        [Min(0)]
        [Tooltip("Output resolution height, can be left 0 for automatic scaling")]
        private int height = 256;

        [Tooltip("Whether to automatically adjust the rawImage's scale to fit the aspect ratio")]
        [SerializeField]
        private bool autoscaleRawImage = true;

        [SerializeField]
        [Tooltip("Mute")]
        private bool mute = true;

        private Texture2D tex;
        private VLCPlayer player;

        private void Start()
        {
            foreach (var pair in buttonUrlPairs)
            {
                pair.button.onClick.AddListener(() => PlayVideo(pair.url));
            }
        }

        public void PlayVideo(string url)
        {
            if (player != null) // 추가된 부분
            {
                player?.Dispose(); // 추가된 부분
                tex = null; // 추가된 부분
                m_rawImage.texture = null; // 추가된 부분
            }
            player = new VLCPlayer(width, height, url, !mute);
        }

        private void Update()
        {
            byte[] img;
            if (player != null && player.CheckForImageUpdate(out img))
            {
                if (tex == null)
                {
                    if ((width <= 0 || height <= 0) && player.VideoTrack != null)
                    {
                        width = (int)player.VideoTrack.Value.i_width;
                        height = (int)player.VideoTrack.Value.i_height;
                    }

                    if (width > 0 && height > 0)
                    {
                        tex = new Texture2D(width, height, TextureFormat.RGB24, false, false);
                        m_rawImage.texture = tex;

                        if (autoscaleRawImage)
                        {
                            RectTransform rect = m_rawImage.rectTransform;
                            float ratio = height / (float)width;
                            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.rect.width * ratio);
                        }
                    }
                }
                else
                {
                    tex.LoadRawTextureData(img);
                    tex.Apply(false);
                }
            }
        }

        private void OnDestroy()
        {
            player?.Dispose();
        }

        public void playerUpdate()
        {
            player = new VLCPlayer(width, height, url, !mute);
        }
    }
}
