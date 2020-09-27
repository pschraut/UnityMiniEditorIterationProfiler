//
// Mini Editor Iteration Profiler for Unity. Copyright (c) 2020 Peter Schraut (www.console-dev.de). See LICENSE.md
// https://github.com/pschraut/UnityMiniEditorIterationProfiler
//
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

                    EditorGUI.indentLevel++;
                    MiniEditorIterationProfiler.showCompilationAndAssemblyReload = EditorGUILayout.ToggleLeft("Show Compilation and Assembly Reload", MiniEditorIterationProfiler.showCompilationAndAssemblyReload);
                    MiniEditorIterationProfiler.showEnterPlayMode = EditorGUILayout.ToggleLeft("Show Enter Play Mode", MiniEditorIterationProfiler.showEnterPlayMode);
                    MiniEditorIterationProfiler.showExitPlayMode = EditorGUILayout.ToggleLeft("Show Exit Play Mode", MiniEditorIterationProfiler.showExitPlayMode);
                    EditorGUI.indentLevel--;
                },
            };

            return provider;
        }
    }
}