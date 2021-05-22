//
// Mini Editor Iteration Profiler for Unity. Copyright (c) 2020 Peter Schraut (www.console-dev.de). See LICENSE.md
// https://github.com/pschraut/UnityMiniEditorIterationProfiler
//
#pragma warning disable IDE1006 // Naming Styles
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

namespace Oddworm.EditorFramework
{
    public static class MiniEditorIterationProfiler
    {
        /// <summary>
        /// Show the compilation and assembly reload timings.
        /// </summary>
        public static bool showCompilationAndAssemblyReload
        {
            get { return EditorPrefs.GetBool(k_ShowCompilationAndAssemblyReload, true); }
            set { EditorPrefs.SetBool(k_ShowCompilationAndAssemblyReload, value); }
        }

        /// <summary>
        /// Show the enter play mode timing.
        /// </summary>
        public static bool showEnterPlayMode
        {
            get { return EditorPrefs.GetBool(k_ShowEnterPlayMode, true); }
            set { EditorPrefs.SetBool(k_ShowEnterPlayMode, value); }
        }

        /// <summary>
        /// Show the exit play mode timing.
        /// </summary>
        public static bool showExitPlayMode
        {
            get { return EditorPrefs.GetBool(k_ShowEnterEditMode, true); }
            set { EditorPrefs.SetBool(k_ShowEnterEditMode, value); }
        }

        /// <summary>
        /// Amount of time to display the message on screen for.
        /// </summary>
        public static float displayTime
        {
            get { return EditorPrefs.GetFloat(k_DisplayTime, 1); }
            set { EditorPrefs.SetFloat(k_DisplayTime, value); }
        }

        /// <summary>
        /// Gets the duration of the last assembly reload.
        /// </summary>
        static TimeSpan lastAssemblyReloadDuration
        {
            get
            {
                if (!long.TryParse(EditorPrefs.GetString(k_AssemblyReloadStarted, ""), out long startedTicks))
                    return new TimeSpan(0);

                if (!long.TryParse(EditorPrefs.GetString(k_AssemblyReloadFinished, ""), out long finishedTicks))
                    return new TimeSpan(0);

                return TimeSpan.FromTicks(finishedTicks - startedTicks);
            }
        }

        /// <summary>
        /// Gets the duration of the last compilation.
        /// </summary>
        static TimeSpan lastCompilationDuration
        {
            get
            {
                if (!long.TryParse(EditorPrefs.GetString(k_CompilationStarted, ""), out long startedTicks))
                    return new TimeSpan(0);

                if (!long.TryParse(EditorPrefs.GetString(k_CompilationFinished, ""), out long finishedTicks))
                    return new TimeSpan(0);

                return TimeSpan.FromTicks(finishedTicks - startedTicks);
            }
        }

        /// <summary>
        /// Gets the duration of the last enter play mode.
        /// That is pressing play to switch from edit mode to play mode.
        /// </summary>
        static TimeSpan lastEnterPlayModeDuration
        {
            get
            {
                if (!long.TryParse(EditorPrefs.GetString(k_EnterPlayModeStarted, ""), out long startedTicks))
                    return new TimeSpan(0);

                if (!long.TryParse(EditorPrefs.GetString(k_EnterPlayModeFinished, ""), out long finishedTicks))
                    return new TimeSpan(0);

                return TimeSpan.FromTicks(finishedTicks - startedTicks);
            }
        }

        /// <summary>
        /// Gets the duration of the last enter edit mode.
        /// That is pressing stop to switch from play mode to edit mode.
        /// </summary>
        static TimeSpan lastEnterEditModeDuration
        {
            get
            {
                if (!long.TryParse(EditorPrefs.GetString(k_EnterEditModeStarted, ""), out long startedTicks))
                    return new TimeSpan(0);

                if (!long.TryParse(EditorPrefs.GetString(k_EnterEditModeFinished, ""), out long finishedTicks))
                    return new TimeSpan(0);

                return TimeSpan.FromTicks(finishedTicks - startedTicks);
            }
        }

        // This field is used to match compilationStarted and compilationFinished events
        static object s_CompileObj = null;

        // We store the started/finished time to EditorPrefs, so they survive domain reloads
        const string k_Prefix = "MiniEditorIterationProfiler_";
        const string k_CompilationStarted = k_Prefix + "CompilationStarted";
        const string k_CompilationFinished = k_Prefix + "CompilationFinished";
        const string k_AssemblyReloadStarted = k_Prefix + "AssemblyReloadStarted";
        const string k_AssemblyReloadFinished = k_Prefix + "AssemblyReloadFinished";
        const string k_ShowCompilationAndAssemblyReload = k_Prefix + "ShowCompilationAndAssemblyReload";
        const string k_EnterPlayModeStarted = k_Prefix + "EnterPlayModeStarted";
        const string k_EnterPlayModeFinished = k_Prefix + "EnterPlayModeFinished";
        const string k_ShowEnterPlayMode = k_Prefix + "ShowEnterPlayMode";
        const string k_EnterEditModeStarted = k_Prefix + "EnterEditModeStarted";
        const string k_EnterEditModeFinished = k_Prefix + "EnterEditModeFinished";
        const string k_ShowEnterEditMode = k_Prefix + "ShowEnterEditMode";
        const string k_DisplayTime = k_Prefix + "DisplayTime";

        [InitializeOnLoadMethod]
        static void RegisterCallbacks()
        {
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.compilationFinished += OnCompilationFinished;

            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange value)
        {
            if (value == PlayModeStateChange.ExitingEditMode)
            {
                EditorPrefs.SetString(k_EnterPlayModeStarted, DateTime.Now.Ticks.ToString());
                EditorPrefs.DeleteKey(k_EnterPlayModeFinished);
            }

            if (value == PlayModeStateChange.EnteredPlayMode)
            {
                EditorPrefs.SetString(k_EnterPlayModeFinished, DateTime.Now.Ticks.ToString());

                if (showEnterPlayMode)
                {
                    EditorApplication.delayCall += delegate ()
                    {
                        ShowNotification(string.Format("Enter Play Mode {0:F2}s", lastEnterPlayModeDuration.TotalSeconds));

                        EditorPrefs.DeleteKey(k_EnterPlayModeStarted);
                        EditorPrefs.DeleteKey(k_EnterPlayModeFinished);
                    };
                }
            }


            if (value == PlayModeStateChange.ExitingPlayMode)
            {
                EditorPrefs.SetString(k_EnterEditModeStarted, DateTime.Now.Ticks.ToString());
                EditorPrefs.DeleteKey(k_EnterEditModeFinished);
            }

            if (value == PlayModeStateChange.EnteredEditMode)
            {
                EditorPrefs.SetString(k_EnterEditModeFinished, DateTime.Now.Ticks.ToString());

                if (showExitPlayMode)
                {
                    EditorApplication.delayCall += delegate ()
                    {
                        ShowNotification(string.Format("Exit Play Mode {0:F2}s", lastEnterEditModeDuration.TotalSeconds));

                        EditorPrefs.DeleteKey(k_EnterEditModeStarted);
                        EditorPrefs.DeleteKey(k_EnterEditModeFinished);
                    };
                }
            }
        }

        static void OnCompilationStarted(object obj)
        {
            if (s_CompileObj != null)
                return;

            s_CompileObj = obj;
            EditorPrefs.SetString(k_CompilationStarted, DateTime.Now.Ticks.ToString());
            EditorPrefs.DeleteKey(k_CompilationFinished);
        }

        static void OnCompilationFinished(object obj)
        {
            if (s_CompileObj != obj)
                return;

            s_CompileObj = null;
            EditorPrefs.SetString(k_CompilationFinished, DateTime.Now.Ticks.ToString());
        }

        static void OnBeforeAssemblyReload()
        {
            EditorPrefs.SetString(k_AssemblyReloadStarted, DateTime.Now.Ticks.ToString());
            EditorPrefs.DeleteKey(k_AssemblyReloadFinished);
        }

        static void OnAfterAssemblyReload()
        {
            // Enter Play Mode can also cause an assembly reload. If we don't clear the timings,
            // it would displays an incorrect value when we restart Unity.
            // Enter Playmode > Exit Playmode > Close Unity > Open Unity = Wrong reload duration
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorPrefs.DeleteKey(k_AssemblyReloadStarted);
                EditorPrefs.DeleteKey(k_AssemblyReloadFinished);
                return;
            }

            if (!showCompilationAndAssemblyReload)
                return;

            EditorPrefs.SetString(k_AssemblyReloadFinished, DateTime.Now.Ticks.ToString());

            EditorApplication.delayCall += delegate ()
            {
                var total = lastCompilationDuration.TotalSeconds + lastAssemblyReloadDuration.TotalSeconds;
                if (total > 0)
                {
                    var text = string.Format("Compile {0:F2}s + Reload {1:F2}s = {2:F2}s",
                        lastCompilationDuration.TotalSeconds,
                        lastAssemblyReloadDuration.TotalSeconds,
                        total);

                    ShowNotification(text);
                }

                EditorPrefs.DeleteKey(k_CompilationStarted);
                EditorPrefs.DeleteKey(k_CompilationFinished);
                EditorPrefs.DeleteKey(k_AssemblyReloadStarted);
                EditorPrefs.DeleteKey(k_AssemblyReloadFinished);
            };
        }

        static void ShowNotification(string text)
        {
            var message = new GUIContent(text);
            var showSecs = MiniEditorIterationProfiler.displayTime; // how many seconds to show the notification

            UnityEngine.Profiling.Profiler.BeginSample("MiniEditorIterationProfiler.ShowSceneViewNotification");
            foreach (var sceneView in SceneView.sceneViews)
            {
                var window = sceneView as EditorWindow;
                if (window != null)
                {
                    window.RemoveNotification();
                    window.ShowNotification(message, showSecs);
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("MiniEditorIterationProfiler.ShowGameViewNotification");
            foreach (var gameView in Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView")))
            {
                var window = gameView as EditorWindow;
                if (window != null)
                {
                    window.RemoveNotification();
                    window.ShowNotification(message, showSecs);
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }
}
