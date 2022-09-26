using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace KevinCastejon.GamepadMappingDiscovery
{
    [CustomEditor(typeof(GamepadMappingDiscoverer))]
    public class GamepadMappingDiscovererEditor : Editor
    {
        private SerializedProperty _axes;
        private SerializedProperty _buttons;
        private SerializedProperty _showOnlyActives;

        private ReorderableList _axesList;
        private ReorderableList _buttonsList;

        private bool _axesFoldout = true;
        private bool _buttonsFoldout = true;

        [MenuItem("Tools/Gamepad Mapping Discovery")]
        internal static void Init()
        {
            SetInputManager();
            GameObject discoverer = new GameObject("Gamepad Mapping Discoverer", new System.Type[] { typeof(GamepadMappingDiscoverer) });
            Selection.activeObject = discoverer;
            EditorApplication.ExecuteMenuItem("Window/General/Game");
        }
        [MenuItem("Tools/Gamepad Mapping Discovery", true)]
        internal static bool InitValidation()
        {
            return EditorApplication.isPlaying;
        }

        private void OnEnable()
        {
            _axes = serializedObject.FindProperty("_axes");
            _buttons = serializedObject.FindProperty("_buttons");
            _showOnlyActives = serializedObject.FindProperty("_showOnlyActives");
            InitializeAxesList();
            InitializeButtonsList();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_showOnlyActives);
            if (EditorGUI.EndChangeCheck())
            {
                InitializeAxesList();
                InitializeButtonsList();
            }
            _axesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_axesFoldout, new GUIContent("Axes", "Axes"));
            if (_axesFoldout)
            {
                _axesList.DoLayoutList();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            _buttonsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_buttonsFoldout, new GUIContent("Buttons", "Buttons"));
            if (_buttonsFoldout)
            {
                _buttonsList.DoLayoutList();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            serializedObject.ApplyModifiedProperties();
            Repaint();
            EditorApplication.ExecuteMenuItem("Window/General/Game");
        }
        private void InitializeAxesList()
        {
            _axesList = new ReorderableList(serializedObject, _axes, false, false, false, false);
            _axesList.elementHeightCallback = (int index) => { return _showOnlyActives.boolValue && Mathf.Approximately(_axes.GetArrayElementAtIndex(index).floatValue, 0f) ? 0f : 16f; };
            _axesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (!_showOnlyActives.boolValue || !Mathf.Approximately(_axes.GetArrayElementAtIndex(index).floatValue, 0f))
                {
                    GUIContent label = EditorGUIUtility.IconContent(!Mathf.Approximately(_axes.GetArrayElementAtIndex(index).floatValue, 0f) ? "greenLight" : "redLight");
                    label.text = "Axis " + (index + 1);
                    EditorGUI.Slider(rect, label, _axes.GetArrayElementAtIndex(index).floatValue, -1f, 1f);
                }
            };
        }

        private void InitializeButtonsList()
        {
            _buttonsList = new ReorderableList(serializedObject, _buttons, false, false, false, false);
            _buttonsList.elementHeightCallback = (int index) => { return _showOnlyActives.boolValue && !_buttons.GetArrayElementAtIndex(index).boolValue ? 0f : 16f; };
            _buttonsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (!_showOnlyActives.boolValue || _buttons.GetArrayElementAtIndex(index).boolValue)
                {
                    GUIContent label = EditorGUIUtility.IconContent(_buttons.GetArrayElementAtIndex(index).boolValue ? "greenLight" : "redLight");
                    label.text = "Button " + index;
                    EditorGUI.LabelField(rect, label);
                }
            };
        }
        private static void SetInputManager()
        {
            SerializedObject inputManager = new SerializedObject(AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/InputManager.asset"));
            SerializedProperty axes = inputManager.FindProperty("m_Axes");
            inputManager.Update();
            for (int i = 0; i < 27; i++)
            {
                axes.InsertArrayElementAtIndex(axes.arraySize);
                SerializedProperty newAxis = axes.GetArrayElementAtIndex(axes.arraySize - 1);
                newAxis.FindPropertyRelative("m_Name").stringValue = "GamepadMappingDiscovery_Axis" + (i + 1);
                newAxis.FindPropertyRelative("negativeButton").stringValue = "";
                newAxis.FindPropertyRelative("altNegativeButton").stringValue = "";
                newAxis.FindPropertyRelative("positiveButton").stringValue = "";
                newAxis.FindPropertyRelative("altPositiveButton").stringValue = "";
                newAxis.FindPropertyRelative("type").enumValueIndex = 2;
                newAxis.FindPropertyRelative("axis").intValue = i;
                newAxis.FindPropertyRelative("joyNum").intValue = 0;
                newAxis.FindPropertyRelative("gravity").floatValue = 0f;
                newAxis.FindPropertyRelative("dead").floatValue = 0.19f;
                newAxis.FindPropertyRelative("sensitivity").floatValue = 1f;
                newAxis.FindPropertyRelative("snap").boolValue = false;
                newAxis.FindPropertyRelative("invert").boolValue = false;
            }
            inputManager.ApplyModifiedProperties();
        }
    }
}
