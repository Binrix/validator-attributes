using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ValidatorAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class UniqueInDictAttribute : ValidationAttribute
{
    private readonly string _property;
    public UniqueInDictAttribute(string property)
        => _property = property;

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value == null)
            return new ValidationResult("Property is null");

        var valueType = value.GetType();
        var isDict = valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Dictionary<,>);

        if (!isDict)
            return new ValidationResult("Property is not a dictionary");

        var castedValue = value as IDictionary;

        foreach (var key in castedValue!.Keys)
        {
            var dictEntry = castedValue[key];

            if (dictEntry is not IList list)
                return new ValidationResult("The item in the dictionary is not a list");

            var items = list.Cast<object>().Select(e => e.GetType().GetProperty(_property)?.GetValue(e));
            var enumerable = items.ToList();

            if (enumerable.Count != enumerable.Distinct().Count())
                return new ValidationResult($"The {_property} already exists");
        }

        return ValidationResult.Success;
    }
}
