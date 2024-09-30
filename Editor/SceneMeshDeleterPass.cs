using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using nadena.dev.ndmf;
using com.aoyon.triangleselector.utils;

namespace com.aoyon.scenemeshdeleter
{
    public class SceneMeshDeleterPass : Pass<SceneMeshDeleterPass>
    {
        protected override void Execute(BuildContext context)
        {
            var comonents = context.AvatarRootObject.GetComponentsInChildren<SceneMeshDeleter>(true);

            foreach (var comonent in comonents)
            {
                List<int> triangleSelection = comonent.triangleSelection;

                if (triangleSelection == null || triangleSelection.Count == 0)
                {
                    Object.DestroyImmediate(comonent);
                    return;
                }

                SkinnedMeshRenderer skinnedMeshRenderer = comonent.GetComponent<SkinnedMeshRenderer>();

                Mesh newMesh = MeshHelper.DeleteMesh(skinnedMeshRenderer.sharedMesh, triangleSelection.ToHashSet());

                skinnedMeshRenderer.sharedMesh = newMesh;

                Object.DestroyImmediate(comonent);
            }
        }
    }
}
            
