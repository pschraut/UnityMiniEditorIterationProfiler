//
// Mini Editor Iteration Profiler for Unity. Copyright (c) 2020 Peter Schraut (www.console-dev.de). See LICENSE.md
// https://github.com/pschraut/UnityMiniEditorIterationProfiler
//
using UnityEngine;
using UnityEditor;

namespace Oddworm.EditorFramework
{
    static class MiniEditorIterationProfilerSettings
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/MiniEditorIterationProfilerSettings", SettingsScope.User)
            {
                label = "Mini Editor Iteration Profiler",

                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.Space();

                    var labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * 0.25f;

                    EditorGUI.indentLevel++;
                    MiniEditorIterationProfiler.showCompilationAndAssemblyReload = EditorGUILayout.Toggle("Show Compilation and Assembly Reload", MiniEditorIterationProfiler.showCompilationAndAssemblyReload);
                    MiniEditorIterationProfiler.showEnterPlayMode = EditorGUILayout.Toggle("Show Enter Play Mode", MiniEditorIterationProfiler.showEnterPlayMode);
                    MiniEditorIterationProfiler.showExitPlayMode = EditorGUILayout.Toggle("Show Exit Play Mode", MiniEditorIterationProfiler.showExitPlayMode);
                    MiniEditorIterationProfiler.displayTime = Mathf.Clamp(EditorGUILayout.FloatField("Message Display Time (seconds)", MiniEditorIterationProfiler.displayTime), 0, 60);
                    EditorGUI.indentLevel--;

                    EditorGUIUtility.labelWidth = labelWidth;
                },
            };

            return provider;
        }
    }
}
