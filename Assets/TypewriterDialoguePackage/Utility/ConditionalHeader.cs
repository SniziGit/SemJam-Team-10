using System;
using UnityEngine;

/// <summary>
/// Attribute to conditionally hide a Header in the Inspector based on the value of another field.
/// This should be placed AFTER the [Header] attribute on the first field of a section.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class ConditionalHeaderAttribute : PropertyAttribute
{
    public string conditionFieldName;
    public object[] showValues;

    public ConditionalHeaderAttribute(string conditionFieldName, params object[] showValues)
    {
        this.conditionFieldName = conditionFieldName;
        this.showValues = showValues;
    }
}
