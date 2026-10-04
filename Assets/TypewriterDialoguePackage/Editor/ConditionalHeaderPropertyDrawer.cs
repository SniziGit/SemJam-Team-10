#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom property drawer for ConditionalHeader attribute.
/// </summary>
[CustomPropertyDrawer(typeof(ConditionalHeaderAttribute))]
public class ConditionalHeaderPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Skip drawing the field itself if the header should be hidden
        if (!ShouldShow(property))
        {
            // If hiding header, also need to handle the field visibility
            return;
        }

        // Draw the actual field
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
        {
            // When header is hidden, we need to check if the field itself should also be hidden
            // Look for ConditionalHide attributes on the same field
            var conditionalHideAttrs = fieldInfo.GetCustomAttributes(typeof(ConditionalHideAttribute), false);
            if (conditionalHideAttrs.Length > 0)
            {
                // Use ConditionalHide logic to determine if field should be hidden
                var drawer = new ConditionalHidePropertyDrawer();
                return drawer.GetPropertyHeight(property, label);
            }
            return 0f;
        }

        // Return normal height for the field
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    private bool ShouldShow(SerializedProperty property)
    {
        ConditionalHeaderAttribute[] condHAtts = (ConditionalHeaderAttribute[])fieldInfo.GetCustomAttributes(typeof(ConditionalHeaderAttribute), true);

        foreach (ConditionalHeaderAttribute condHAtt in condHAtts)
        {
            if (!GetConditionalHeaderAttributeResult(condHAtt, property))
            {
                return false;
            }
        }

        return true;
    }

    private bool GetConditionalHeaderAttributeResult(ConditionalHeaderAttribute condHAtt, SerializedProperty property)
    {
        string propertyPath = property.propertyPath;
        string conditionPath = propertyPath.Replace(property.name, condHAtt.conditionFieldName);
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null)
        {
            object currentValue = GetValue(sourcePropertyValue);
            foreach (object showValue in condHAtt.showValues)
            {
                if (currentValue.Equals(showValue))
                {
                    return true;
                }
            }
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHeaderAttribute but no matching SourcePropertyValue found in object: " + condHAtt.conditionFieldName + ", conditionPath = " + conditionPath);
        }

        return false;
    }

    private object GetValue(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                return property.intValue;
            case SerializedPropertyType.Boolean:
                return property.boolValue;
            case SerializedPropertyType.Float:
                return property.floatValue;
            case SerializedPropertyType.String:
                return property.stringValue;
            case SerializedPropertyType.Color:
                return property.colorValue;
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue;
            case SerializedPropertyType.LayerMask:
                return property.intValue;
            case SerializedPropertyType.Enum:
                return property.enumValueIndex;
            case SerializedPropertyType.Vector2:
                return property.vector2Value;
            case SerializedPropertyType.Vector3:
                return property.vector3Value;
            case SerializedPropertyType.Vector4:
                return property.vector4Value;
            case SerializedPropertyType.Rect:
                return property.rectValue;
            case SerializedPropertyType.ArraySize:
                return property.intValue;
            case SerializedPropertyType.Character:
                return (char)property.intValue;
            case SerializedPropertyType.AnimationCurve:
                return property.animationCurveValue;
            case SerializedPropertyType.Bounds:
                return property.boundsValue;
            case SerializedPropertyType.Gradient:
                return property.gradientValue;
            case SerializedPropertyType.Quaternion:
                return property.quaternionValue;
            default:
                return null;
        }
    }
}
#endif
