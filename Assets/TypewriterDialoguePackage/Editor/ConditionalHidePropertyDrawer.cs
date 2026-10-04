#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom property drawer for ConditionalHide attribute.
/// </summary>
[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Only draw the field if it should be shown
        if (ShouldShow(property))
        {
            // Check if this field has a TextArea attribute
            bool hasTextArea = fieldInfo.GetCustomAttributes(typeof(TextAreaAttribute), false).Length > 0;
            
            if (hasTextArea && property.propertyType == SerializedPropertyType.String)
            {
                // Handle TextArea manually to ensure proper rendering
                property.stringValue = EditorGUI.TextArea(position, property.stringValue);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
        {
            return 0f;
        }
        
        // Check if this field has a TextArea attribute
        bool hasTextArea = fieldInfo.GetCustomAttributes(typeof(TextAreaAttribute), false).Length > 0;
        
        if (hasTextArea && property.propertyType == SerializedPropertyType.String)
        {
            // Get TextArea attribute to determine height
            var textAreaAttr = (TextAreaAttribute)fieldInfo.GetCustomAttributes(typeof(TextAreaAttribute), false)[0];
            return EditorGUIUtility.singleLineHeight * textAreaAttr.minLines;
        }
        
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    private bool ShouldShow(SerializedProperty property)
    {
        ConditionalHideAttribute[] condHAtts = (ConditionalHideAttribute[])fieldInfo.GetCustomAttributes(typeof(ConditionalHideAttribute), true);

        foreach (ConditionalHideAttribute condHAtt in condHAtts)
        {
            if (!GetConditionalHideAttributeResult(condHAtt, property))
            {
                return false;
            }
        }

        return true;
    }

    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
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
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.conditionFieldName + ", conditionPath = " + conditionPath);
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
