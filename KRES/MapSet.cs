using System;
using System.IO;
using UnityEngine;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class MapSet : MonoBehaviour
    {
        private bool showing = false, initiated = false;
        private Texture2D texture = new Texture2D(360, 180, TextureFormat.ARGB32, false);
        ResourceMap map = null;
        private void Update()
        {
            if (!initiated)
            {
                print("[KRES]: Initiating overlay");
                texture.LoadImage(File.ReadAllBytes(Path.Combine(KRESUtils.GetSavePath(), "KRESTextures/Kerbin/Water.png")));
                initiated = true;
                print("[KRES]: Overlay initiated");
            }

            if (FlightUIModeController.Instance.Mode == FlightUIMode.ORBITAL)
            {
                if (!showing)
                {
                    print("[KRES]: Map mode engaged, showing texture");
                    map = new ResourceMap("Water", texture);
                    map.ShowTexture("Kerbin");
                    DebugWindow.Instance.SetTexture(texture, "Water");
                    showing = true;
                }
            }

            else
            {
                if (map != null && showing)
                {
                    print("[KRES]: Map mode left, removing texture");
                    map.HideTexture("Kerbin");
                    DebugWindow.Instance.ClearTexture();
                    showing = false;
                }
            }
        }
    }
}
