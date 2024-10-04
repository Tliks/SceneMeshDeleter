using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using nadena.dev.ndmf.preview;
using com.aoyon.triangleselector;
using com.aoyon.triangleselector.utils;

namespace com.aoyon.scenemeshdeleter
{
    [CustomEditor(typeof(SceneMeshDeleter))]
    public class SceneMeshDeleterEditor: Editor
    {
        private SceneMeshDeleter _target;
        private RenderSelector _renderSelector;
        private void OnEnable()
        {
            _target = target as SceneMeshDeleter;
            var skinnedMeshRenderer = _target.GetComponent<SkinnedMeshRenderer>();
            _renderSelector = CreateInstance<RenderSelector>();
            _renderSelector.Initialize(skinnedMeshRenderer, _target.triangleSelection);
            _renderSelector.RegisterApplyCallback(OnTriangleSelectionChanged);
        }

        private void OnDisable()
        {
            // Editorスクリプトの破棄ではなく、コンポーネントの無効化時に実行
            if (_target == null)
            {
                _renderSelector.Dispose();
            }
        }

        private void OnTriangleSelectionChanged(List<int> newSelection)
        {
            // SerializedPropertyの書き換えが遅いので直接変更
            _target.triangleSelection = new HashSet<Vector3>(newSelection);
            // NDMFに明示的に通知
            ChangeNotifier.NotifyObjectUpdate(_target);
        }

        public override void OnInspectorGUI()
        {
            _renderSelector.RenderGUI();
            NDMFToggleButton.RenderNDMFToggle(SceneMeshDeleterPreview.ToggleNode);
        }
    }
}