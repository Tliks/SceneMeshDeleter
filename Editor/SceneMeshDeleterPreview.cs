using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using nadena.dev.ndmf.preview;
using com.aoyon.triangleselector.utils;

namespace com.aoyon.scenemeshdeleter
{
    internal class SceneMeshDeleterPreview : IRenderFilter
    {
        public static TogglablePreviewNode ToggleNode = TogglablePreviewNode.Create(
            () => "SceneMeshDeleter",
            qualifiedName: "com.aoyon.scenemeshdeleter/SceneMeshDeleterPreview",
            true
        );
        
        public IEnumerable<TogglablePreviewNode> GetPreviewControlNodes()
        {
            yield return ToggleNode;
        }

        public bool IsEnabled(ComputeContext context)
        {
            return context.Observe(ToggleNode.IsEnabled);
        }

        public ImmutableList<RenderGroup> GetTargetGroups(ComputeContext context)
        {
            return context.GetComponentsByType<SkinnedMeshRenderer>()
                .Select(renderer => (renderer, context.GetComponents<SceneMeshDeleter>(renderer.gameObject)))
                .Where(pair => pair.Item2.Count() != 0)
                .Select(pair => RenderGroup.For(pair.Item1).WithData(pair.Item2))
                .ToImmutableList();
        }

        public Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            var components = group.GetData<SceneMeshDeleter[]>();

            var triangleSelection = new HashSet<Vector3>();
            foreach(var component in components){
                context.Observe(component, component => component.triangleSelection, (a, b) => a.SetEquals(b));
                triangleSelection.UnionWith(component.triangleSelection);
            }

            var pair = proxyPairs.First();

            if (!(pair.Item1 is SkinnedMeshRenderer original)) return null;
            if (!(pair.Item2 is SkinnedMeshRenderer proxy)) return null;

            Mesh mesh = proxy.sharedMesh;
            if (mesh == null) return null;

            Mesh modifiedMesh;
            if (triangleSelection.Count == 0) 
            {
                modifiedMesh = mesh;
            }
            else
            {
                modifiedMesh = MeshHelper.DeleteMesh(mesh, triangleSelection);
            }

            return Task.FromResult<IRenderFilterNode>(new SceneMeshDeleterPreviewNode(modifiedMesh));
        }
    }

    internal class SceneMeshDeleterPreviewNode : IRenderFilterNode
    {
        public RenderAspects WhatChanged => RenderAspects.Mesh;
        private Mesh _modifiedMesh; 

        public SceneMeshDeleterPreviewNode(Mesh modifiedMesh)
        {
            _modifiedMesh = modifiedMesh;
        }
        
        public void OnFrame(Renderer original, Renderer proxy)
        {
            if (original is SkinnedMeshRenderer o_smr && proxy is SkinnedMeshRenderer p_smr)
            {
                p_smr.sharedMesh = _modifiedMesh;
                return;
            }
        }

    }
}
