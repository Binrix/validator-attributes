using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ValidatorAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ValidateItemsInDictAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value == null)
            return new ValidationResult("Property is null");

        var valueType = value.GetType();
        var isDict = valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Dictionary<,>);

        if (!isDict)
            return new ValidationResult("Property is not a dictionary");

        var castedValue = value as IDictionary;
        var results = new List<ValidationResult>();

        foreach (var key in castedValue!.Keys)
        {
            var dictEntry = castedValue[key];

            if(dictEntry is not IList list)
                return new ValidationResult("The item in the dictionary is not a list");

            var items = list.Cast<object>();

            foreach (var item in items)
            {
                var validationContext = new ValidationContext(item, null, null);
                Validator.TryValidateObject(item, validationContext, results, true);
            }
        }

        return results.Count > 0 ? new ValidationResult(results[^1].ErrorMessage) : null;
    }
}