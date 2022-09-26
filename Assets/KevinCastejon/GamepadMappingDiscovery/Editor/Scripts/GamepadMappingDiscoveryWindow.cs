using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GamepadMappingDiscovery
{
    internal class GamepadMappingDiscoveryWindow : EditorWindow
    {
        private static SceneAsset _inputScene;
        private static Preset _inputPreset;
        private static Preset _backupPreset;
        private static SceneAsset InputScene { get { if (_inputScene == null) { _inputScene = Resources.Load<SceneAsset>("GamepadMappingDiscoveryScene"); } return _inputScene; } }
        private static Preset InputPreset { get { if (_inputPreset == null) { _inputPreset = Resources.Load<Preset>("GamepadMappingDiscoveryPreset"); } return _inputPreset; } }
        private static Preset BackupPreset { get { if (_backupPreset == null) { _backupPreset = Resources.Load<Preset>("GamepadMappingDiscoveryBackupPreset"); } return _backupPreset; } }

        [MenuItem("Window/Gamepad Mapping Discovery", true)]
        internal static bool OpenWindowValidation()
        {
            return !Application.isPlaying;
        }

        [MenuItem("Window/Gamepad Mapping Discovery")]
        internal static void OpenWindow()
        {
            DoOpenWindow(true);
        }
        internal static void DoOpenWindow(bool setInputManager)
        {
            EditorWindow window = CreateInstance<GamepadMappingDiscoveryWindow>();
            window.ShowUtility();
            window.titleContent = new GUIContent("Gamepad Mapping Discovery");
            if (setInputManager)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                Object inputManager = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/InputManager.asset");
                string presetPath = AssetDatabase.GetAssetPath(InputPreset);
                presetPath = presetPath.Substring(0, presetPath.Length - 13) + "BackupPreset.preset";
                AssetDatabase.CreateAsset(new Preset(inputManager), presetPath);
                AssetDatabase.ImportAsset(presetPath);
                InputPreset.ApplyTo(inputManager);
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(InputScene));
                EditorApplication.EnterPlaymode();
            }
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                DoOpenWindow(false);
            }
            else
            {
                Object inputManager = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/InputManager.asset");
                BackupPreset.ApplyTo(inputManager);
            }
        }

        private void OnGUI()
        {
            maxSize = new Vector2(340f, 82f);
            minSize = new Vector2(340f, 82f);
            EditorGUILayout.HelpBox("The InputManager settings has been modified temporarily.\nClosing this window will set the previous settings back.", MessageType.Warning);
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("You must exit PlayMode before closing this window.", MessageType.Warning);
            }
        }
    }
}
