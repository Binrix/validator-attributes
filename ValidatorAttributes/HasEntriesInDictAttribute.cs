using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ValidatorAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class HasEntriesInDictAttribute : ValidationAttribute
{
    private readonly string _noEntryError = String.Empty;

    public HasEntriesInDictAttribute(string errorMessage)
        => _noEntryError = errorMessage;

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value == null)
            return new ValidationResult(_noEntryError);

        var valueType = value.GetType();
        var isDict = valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Dictionary<,>);

        if (isDict)
        {
            if (value is not IDictionary castedValue || castedValue.Count < 1)
                return new ValidationResult(_noEntryError);

            return ValidationResult.Success;
        }

        return new ValidationResult("Type of property is not a dictionary");
    }
}