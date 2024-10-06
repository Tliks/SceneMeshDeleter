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
            var renderers = context.AvatarRootObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            foreach (var renderer in renderers)
            {
                var components = renderer.GetComponents<SceneMeshDeleter>();
                if (components.Count() == 0) continue;

                var triangleSelection = new HashSet<Vector3>();
                foreach(var component in components)
                {
                    triangleSelection.UnionWith(component.triangleSelection);
                    Object.DestroyImmediate(component);
                }

                Mesh newMesh = MeshHelper.DeleteMesh(renderer.sharedMesh, triangleSelection);

                renderer.sharedMesh = newMesh;
            }
        }
    }
}
            
