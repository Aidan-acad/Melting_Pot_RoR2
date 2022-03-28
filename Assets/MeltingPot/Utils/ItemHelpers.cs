using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace MeltingPot.Utils
{
    public static class ItemHelpers
    {
        
        public static void DroneHunt(this CharacterMaster owner, Action<CharacterMaster> logic) {
            MinionOwnership thisMinionOwnership = owner.minionOwnership;
            if (!thisMinionOwnership) return;
            MinionOwnership[] minionOwnerships = UnityEngine.Object.FindObjectsOfType<MinionOwnership>();
            foreach (MinionOwnership minionOwnership in minionOwnerships) {
                if (minionOwnership && minionOwnership.ownerMaster) {
                    bool ownerCondition = !thisMinionOwnership.ownerMaster && minionOwnership.ownerMaster == owner;
                    bool minionCondition = thisMinionOwnership.ownerMaster && minionOwnership.ownerMaster == thisMinionOwnership.ownerMaster;
                    if (ownerCondition || minionCondition) {
                        CharacterMaster minion = minionOwnership.GetComponent<CharacterMaster>();
                        logic(minion);
                    }
                }
            }
        }

        public static CharacterModel.RendererInfo[] ItemDisplaySetup(GameObject obj, bool debugmode = false)
        {

            List<Renderer> AllRenderers = new List<Renderer>();

            var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers.Length > 0) { AllRenderers.AddRange(meshRenderers); }

            var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderers.Length > 0) { AllRenderers.AddRange(skinnedMeshRenderers); }

            CharacterModel.RendererInfo[] renderInfos = new CharacterModel.RendererInfo[AllRenderers.Count];

            for (int i = 0; i < AllRenderers.Count; i++)
            {
                renderInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = AllRenderers[i] is SkinnedMeshRenderer ? AllRenderers[i].sharedMaterial : AllRenderers[i].material,
                    renderer = AllRenderers[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false //We allow the mesh to be affected by overlays like OnFire or PredatoryInstinctsCritOverlay.
                };
            }

            return renderInfos;
        }
    }
}
