using System;
using System.ComponentModel.DataAnnotations;

namespace ValidatorAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ExactlyOnePropertyRequiredAttribute : ValidationAttribute
{
    private readonly string[] _propertyNames;

    public ExactlyOnePropertyRequiredAttribute(string[] propertyNames)
        => _propertyNames = propertyNames;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var doesOneExist = false;

        foreach (string propertyName in _propertyNames)
        {
            var property = validationContext.ObjectType.GetProperty(propertyName);
            if (property == null) continue;

            if(property.GetValue(validationContext.ObjectInstance) != null)
            {
                if (doesOneExist)
                    return new ValidationResult("Only one property at a time is allowed");

                doesOneExist = true;
            }
        }

        if (!doesOneExist)
            return new ValidationResult("At least one property must be set!");

        return ValidationResult.Success;
    }
}