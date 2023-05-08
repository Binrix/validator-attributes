using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ValidatorAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ValidateChildAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)

    {
        if (value == null)
            return new ValidationResult("Property is null");

        var results = new List<ValidationResult>();
        var validationContext = new ValidationContext(value, null, null);

        Validator.TryValidateObject(value, validationContext, results, true);

        if (results.Count > 0)
            return new ValidationResult(results[^1].ErrorMessage);

        return ValidationResult.Success;
    }
}