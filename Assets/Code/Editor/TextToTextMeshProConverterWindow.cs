using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Code.Editor
{
    public class TextToTextMeshProConverterWindow : EditorWindow
    {
        private GameObject _prefab;
        private TMP_FontAsset _fontAsset;
        private Vector2 _scrollPosition;
        private List<string> _convertedComponents = new List<string>();

        [MenuItem("Tools/Text to TextMeshPro Converter", false, 2003)]
        private static void OpenWindow()
        {
            TextToTextMeshProConverterWindow window = GetWindow<TextToTextMeshProConverterWindow>();
            window.titleContent = new GUIContent("Text to TMP Converter");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Text to TextMeshPro Converter", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Перетащите UI префаб и выберите шрифт TextMeshPro для конвертации", MessageType.Info);
            
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("UI Prefab:", EditorStyles.label);
            _prefab = (GameObject)EditorGUILayout.ObjectField(_prefab, typeof(GameObject), false);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("TextMeshPro Font:", EditorStyles.label);
            _fontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField(_fontAsset, typeof(TMP_FontAsset), false);

            EditorGUILayout.Space(10);

            GUI.enabled = _prefab != null && _fontAsset != null;
            
            if (GUILayout.Button("Convert Text to TextMeshPro", GUILayout.Height(30)))
            {
                ConvertTextToTextMeshPro();
            }

            GUI.enabled = true;

            if (_convertedComponents.Count > 0)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField($"Converted Components ({_convertedComponents.Count}):", EditorStyles.boldLabel);
                
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(200));
                
                foreach (string componentPath in _convertedComponents)
                {
                    EditorGUILayout.LabelField(componentPath, EditorStyles.wordWrappedLabel);
                }
                
                EditorGUILayout.EndScrollView();
            }
        }

        private void ConvertTextToTextMeshPro()
        {
            if (_prefab == null || _fontAsset == null)
            {
                EditorUtility.DisplayDialog("Error", "Пожалуйста, выберите префаб и шрифт", "OK");
                return;
            }

            _convertedComponents.Clear();

            bool isPrefabAsset = PrefabUtility.IsPartOfPrefabAsset(_prefab);
            GameObject targetObject = _prefab;

            Text[] textComponents = targetObject.GetComponentsInChildren<Text>(true);

            if (textComponents.Length == 0)
            {
                EditorUtility.DisplayDialog("Info", "Не найдено компонентов Text для конвертации", "OK");
                return;
            }

            int convertedCount = 0;

            foreach (Text textComponent in textComponents)
            {
                if (textComponent == null)
                    continue;

                GameObject gameObject = textComponent.gameObject;
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

                if (rectTransform == null)
                    continue;

                string text = textComponent.text;
                Color color = textComponent.color;
                int fontSize = textComponent.fontSize;
                FontStyle fontStyle = textComponent.fontStyle;
                TextAnchor alignment = textComponent.alignment;
                bool raycastTarget = textComponent.raycastTarget;

                Undo.RecordObject(gameObject, "Convert Text to TextMeshPro");

                DestroyImmediate(textComponent);

                TextMeshProUGUI tmpComponent = gameObject.AddComponent<TextMeshProUGUI>();
                tmpComponent.text = text;
                tmpComponent.color = color;
                tmpComponent.fontSize = fontSize > 0 ? fontSize : tmpComponent.fontSize;
                tmpComponent.font = _fontAsset;
                tmpComponent.raycastTarget = raycastTarget;

                switch (alignment)
                {
                    case TextAnchor.UpperLeft:
                        tmpComponent.alignment = TextAlignmentOptions.TopLeft;
                        break;
                    case TextAnchor.UpperCenter:
                        tmpComponent.alignment = TextAlignmentOptions.Top;
                        break;
                    case TextAnchor.UpperRight:
                        tmpComponent.alignment = TextAlignmentOptions.TopRight;
                        break;
                    case TextAnchor.MiddleLeft:
                        tmpComponent.alignment = TextAlignmentOptions.MidlineLeft;
                        break;
                    case TextAnchor.MiddleCenter:
                        tmpComponent.alignment = TextAlignmentOptions.Midline;
                        break;
                    case TextAnchor.MiddleRight:
                        tmpComponent.alignment = TextAlignmentOptions.MidlineRight;
                        break;
                    case TextAnchor.LowerLeft:
                        tmpComponent.alignment = TextAlignmentOptions.BottomLeft;
                        break;
                    case TextAnchor.LowerCenter:
                        tmpComponent.alignment = TextAlignmentOptions.Bottom;
                        break;
                    case TextAnchor.LowerRight:
                        tmpComponent.alignment = TextAlignmentOptions.BottomRight;
                        break;
                }

                if ((fontStyle & FontStyle.Bold) != 0)
                {
                    tmpComponent.fontStyle |= FontStyles.Bold;
                }
                if ((fontStyle & FontStyle.Italic) != 0)
                {
                    tmpComponent.fontStyle |= FontStyles.Italic;
                }

                string path = GetGameObjectPath(gameObject, targetObject.transform);
                _convertedComponents.Add(path);
                convertedCount++;
            }

            if (isPrefabAsset)
            {
                PrefabUtility.SavePrefabAsset(targetObject);
            }
            else
            {
                EditorUtility.SetDirty(targetObject);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", $"Конвертировано компонентов: {convertedCount}", "OK");
        }

        private string GetGameObjectPath(GameObject gameObject, Transform root)
        {
            List<string> path = new List<string>();
            Transform current = gameObject.transform;

            while (current != null && current != root.parent)
            {
                path.Insert(0, current.name);
                current = current.parent;
            }

            return string.Join("/", path);
        }
    }
}

