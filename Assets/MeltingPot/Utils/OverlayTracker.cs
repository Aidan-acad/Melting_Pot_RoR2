using UnityEngine;

namespace MeltingPot.Utils
{
    public class OverlayTracker : MonoBehaviour
    {
        public RoR2.TemporaryOverlay Overlay;
        public RoR2.CharacterBody Body;
        public RoR2.BuffIndex Buff;

        public void FixedUpdate()
        {
            if (!Body.HasBuff(Buff))
            {
                UnityEngine.Object.Destroy(Overlay);
                UnityEngine.Object.Destroy(this);
            }
        }
    }
}
