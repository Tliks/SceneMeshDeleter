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
            return context.GetComponentsByType<SceneMeshDeleter>()
            .Select(component => (component, context.GetComponent<SkinnedMeshRenderer>(component.gameObject)))
            .Where(pair => pair.Item2 != null && pair.Item2.sharedMesh != null)
            .Select(pair => RenderGroup.For(pair.Item2).WithData(new[] { pair.Item1 }))
            .ToImmutableList();
        }

        public Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            var component = group.GetData<SceneMeshDeleter[]>().SingleOrDefault();
            if (component == default) return null;

            context.Observe(component, component => component.triangleSelection, (a, b) => a.SetEquals(b));

            var pair = proxyPairs.SingleOrDefault();
            if (pair == default) return null;

            if (!(pair.Item1 is SkinnedMeshRenderer original)) return null;
            if (!(pair.Item2 is SkinnedMeshRenderer proxy)) return null;

            Mesh mesh = proxy.sharedMesh;
            if (mesh == null) return null;

            var triangleSelection = component.triangleSelection;
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
