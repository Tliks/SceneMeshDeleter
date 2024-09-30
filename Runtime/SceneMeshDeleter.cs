using UnityEngine;
using VRC.SDKBase;
using System.Collections.Generic;

namespace com.aoyon.scenemeshdeleter
{

    [AddComponentMenu("SceneMeshDeleter/SMD SceneMeshDeleter")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class SceneMeshDeleter: MonoBehaviour, IEditorOnly
    {
        [SerializeField]
        public List<int> triangleSelection = new();
    }
}