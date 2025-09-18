using System;
using System.Collections.Generic;
using System.Linq;
using SSOConfigManagerUI.Services;
using SSOConfigManagerUI.Base;

namespace SSOConfigManagerUI.Validation
{
    /// <summary>
    /// Interface for validation strategies
    /// </summary>
    public interface IValidator<T>
    {
        ValidationResult Validate(T value);
    }

    /// <summary>
    /// Validator for application names that checks format and availability
    /// </summary>
    public class ApplicationNameValidator : IValidator<string>
    {
        private readonly IApplicationService _applicationService;

        public ApplicationNameValidator(IApplicationService applicationService)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        }

        public ValidationResult Validate(string applicationName)
        {
            try
            {
                // First validate format
                var formatResult = _applicationService.ValidateApplicationName(applicationName);
                if (!formatResult.Success)
                {
                    return ValidationResult.Invalid(formatResult.Message);
                }

                return ValidationResult.Valid();
            }
            catch (Exception ex)
            {
                return ValidationResult.Invalid($"Validation error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Validator that checks if an application exists
    /// </summary>
    public class ApplicationExistenceValidator : IValidator<string>
    {
        private readonly ISSOService _ssoService;
        private readonly bool _shouldExist;

        public ApplicationExistenceValidator(ISSOService ssoService, bool shouldExist = true)
        {
            _ssoService = ssoService ?? throw new ArgumentNullException(nameof(ssoService));
            _shouldExist = shouldExist;
        }

        public ValidationResult Validate(string applicationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationName))
                {
                    return ValidationResult.Invalid("Application name cannot be empty");
                }

                var existsResult = _ssoService.ApplicationExists(applicationName);
                if (!existsResult.Success)
                {
                    return ValidationResult.Warning("Could not verify application existence", true);
                }

                bool exists = existsResult.Data;
                
                if (_shouldExist && !exists)
                {
                    return ValidationResult.Invalid("Application does not exist");
                }
                
                if (!_shouldExist && exists)
                {
                    return ValidationResult.Invalid("Application already exists");
                }

                return ValidationResult.Valid();
            }
            catch (Exception ex)
            {
                return ValidationResult.Warning($"Could not check application: {ex.Message}", false);
            }
        }
    }

    /// <summary>
    /// Composite validator that runs multiple validators in sequence
    /// </summary>
    public class CompositeValidator<T> : IValidator<T>
    {
        private readonly List<IValidator<T>> _validators;

        public CompositeValidator()
        {
            _validators = new List<IValidator<T>>();
        }

        public CompositeValidator<T> AddValidator(IValidator<T> validator)
        {
            _validators.Add(validator ?? throw new ArgumentNullException(nameof(validator)));
            return this;
        }

        public ValidationResult Validate(T value)
        {
            foreach (var validator in _validators)
            {
                var result = validator.Validate(value);
                if (!result.IsValid)
                {
                    return result;
                }
            }

            return ValidationResult.Valid();
        }
    }

    /// <summary>
    /// Validator for property names
    /// </summary>
    public class PropertyNameValidator : IValidator<string>
    {
        public ValidationResult Validate(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return ValidationResult.Invalid("Property name cannot be empty");
            }

            if (propertyName.Length > 100)
            {
                return ValidationResult.Invalid("Property name cannot exceed 100 characters");
            }

            // Check for invalid characters
            var invalidChars = new[] { '\\', '/', ':', '*', '?', '\"', '<', '>', '|', '\r', '\n', '\t' };
            if (propertyName.Any(c => invalidChars.Contains(c)))
            {
                return ValidationResult.Invalid("Property name contains invalid characters");
            }

            return ValidationResult.Valid();
        }
    }

    /// <summary>
    /// Factory for creating validators with proper dependencies
    /// </summary>
    public static class ValidatorFactory
    {
        public static IValidator<string> CreateApplicationNameValidator(bool shouldExist = true)
        {
            var applicationService = new ApplicationService();
            var ssoService = new SSOService();

            return new CompositeValidator<string>()
                .AddValidator(new ApplicationNameValidator(applicationService))
                .AddValidator(new ApplicationExistenceValidator(ssoService, shouldExist));
        }

        public static IValidator<string> CreateNewApplicationNameValidator()
        {
            return CreateApplicationNameValidator(shouldExist: false);
        }

        public static IValidator<string> CreateExistingApplicationNameValidator()
        {
            return CreateApplicationNameValidator(shouldExist: true);
        }

        public static IValidator<string> CreatePropertyNameValidator()
        {
            return new PropertyNameValidator();
        }
    }
}