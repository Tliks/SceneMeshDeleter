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
        private SerializedProperty _triangleSelectionProperty;
        
        private void OnEnable()
        {
            _target = target as SceneMeshDeleter;
            var skinnedMeshRenderer = _target.GetComponent<SkinnedMeshRenderer>();
            _renderSelector = CreateInstance<RenderSelector>();
            _renderSelector.Initialize(skinnedMeshRenderer, _target.triangleSelection);
            _renderSelector.RegisterApplyCallback(OnTriangleSelectionChanged);
            _triangleSelectionProperty = serializedObject.FindProperty(nameof(SceneMeshDeleter.triangleSelection));
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
            serializedObject.Update();
            // SerializedPropertyの配列の書き換えが普通に遅い
            _triangleSelectionProperty.arraySize = newSelection.Count;
            for (int i = 0; i < newSelection.Count; i++)
            {
                _triangleSelectionProperty.GetArrayElementAtIndex(i).intValue = newSelection[i];
            }
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            _renderSelector.RenderGUI();
            NDMFToggleButton.RenderNDMFToggle(SceneMeshDeleterPreview.ToggleNode);
        }
    }
}