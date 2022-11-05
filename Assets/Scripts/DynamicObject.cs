using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class DynamicObject : ISerializationCallbackReceiver {
    public Resizable resizable;
    public string classificationLabel = "";

    [SerializeField] private int _editorClassificationIndex;

    public void OnBeforeSerialize() {
    }

    public void OnAfterDeserialize() {
        if (classificationLabel != "") {
            int IndexOf(string label, IEnumerable<string> collection) {
                var index = 0;
                foreach (var item in collection) {
                    if (item == label) {
                        return index;
                    }

                    index++;
                }

                return -1;
            }

            // We do this every time we deserialize in case the classification options have been updated
            // This ensures that the label displayed
            _editorClassificationIndex = IndexOf(
                classificationLabel,
                OVRSceneManager.Classification.List
            );

            if (_editorClassificationIndex < 0) {
                Debug.LogError(
                    $"[{nameof(Spawnable)}] OnAfterDeserialize() " + classificationLabel +
                    " not found. The Classification list in OVRSceneManager has likely changed"
                );
            }
        } else {
            // No classification was selected, so we can just assign a default
            // This typically happens this object was just created
            _editorClassificationIndex = 0;
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DynamicObject))]
internal class DynamicObjectEditor : PropertyDrawer {
    private static readonly string[] classificationList = OVRSceneManager.Classification.List.ToArray();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return base.GetPropertyHeight(
            property,
            label
        ) * 2.2f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var labelProperty = property.FindPropertyRelative(nameof(DynamicObject.classificationLabel));
        var editorClassificationIndex = property.FindPropertyRelative("_editorClassificationIndex");
        var prefab = property.FindPropertyRelative(nameof(DynamicObject.resizable));

        EditorGUI.BeginProperty(
            position,
            label,
            property
        );

        var y = position.y;
        var h = position.height / 2;

        var rect = new Rect(
            position.x,
            y,
            position.width,
            h
        );
        if (editorClassificationIndex.intValue == -1) {
            var list = new List<string> {
                labelProperty.stringValue + " (invalid)"
            };
            list.AddRange(OVRSceneManager.Classification.List);
            editorClassificationIndex.intValue = EditorGUI.Popup(
                rect,
                0,
                list.ToArray()
            ) - 1;
        } else {
            editorClassificationIndex.intValue = EditorGUI.Popup(
                rect,
                editorClassificationIndex.intValue,
                classificationList
            );
        }

        if (editorClassificationIndex.intValue >= 0 &&
            editorClassificationIndex.intValue < classificationList.Length) {
            labelProperty.stringValue = OVRSceneManager.Classification.List[editorClassificationIndex.intValue];
        }

        EditorGUI.ObjectField(
            new Rect(
                position.x,
                y + EditorGUI.GetPropertyHeight(labelProperty),
                position.width,
                h
            ),
            prefab
        );
        EditorGUI.EndProperty();
    }
}
#endif