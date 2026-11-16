
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ScreenWipeDropdownAttribute))]
public class ScreenWipeDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Find the ScreenWipeManager in the same scene
        // as the object containing this trigger.
        ScreenWipeManager manager = Resources
            .FindObjectsOfTypeAll<ScreenWipeManager>()
            .FirstOrDefault(m =>
                m.gameObject.scene.IsValid() &&
                m.gameObject.scene == 
                ((MonoBehaviour)property.serializedObject.targetObject).gameObject.scene
            );

        if (manager == null)
        {
            EditorGUI.LabelField(
                position,
                label.text,
                "No ScreenWipeManager found in scene."
            );

            EditorGUI.EndProperty();
            return;
        }

        // Retrieve available wipes directly from the hierarchy.
        // This also includes inactive objects.
        var wipes = manager
            .GetComponentsInChildren<ScreenWipe>(true)
            .ToList();

        string[] options = wipes
            .Select(w => w.WipeName)
            .Distinct()
            .ToArray();

        // Add an empty selection option.
        options = new[] { "None" }.Concat(options).ToArray();

        // Find the currently selected option.
        int selectedIndex = System.Array.IndexOf(
            options,
            string.IsNullOrEmpty(property.stringValue)
                ? "None"
                : property.stringValue
        );

        if (selectedIndex < 0)
            selectedIndex = 0;

        // Draw dropdown.
        EditorGUI.BeginChangeCheck();

        int newIndex = EditorGUI.Popup(
            position,
            label.text,
            selectedIndex,
            options
        );

        if (EditorGUI.EndChangeCheck())
        {
            property.stringValue = newIndex == 0
                ? string.Empty
                : options[newIndex];

            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }
}
