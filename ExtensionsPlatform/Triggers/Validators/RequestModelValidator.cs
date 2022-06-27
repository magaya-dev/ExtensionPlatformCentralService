using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ExtensionsPlatform.Triggers.Validators
{
    public interface IRequestModelValidator
    {
        string ValidateRequestModel(object requestModel);
    }

    public class RequestModelValidator : IRequestModelValidator
    {
        public string ValidateRequestModel(object requestModel)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(requestModel, serviceProvider: null, items: null);
            Validator.TryValidateObject(requestModel, context, validationResults, true);

            return string.Join(",", validationResults);
        }
    }


    public class ValidateObjectAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var context = validationContext;
            if (value != null)
            {
                context = new ValidationContext(value, null, null);
                Validator.TryValidateObject(value, context, results, true);
            }
                
            if (results.Count != 0)
            {
                var compositeResults = new CompositeValidationResult(results[0].ErrorMessage);
                results.ForEach(compositeResults.AddResult);

                return compositeResults;
            }

            return ValidationResult.Success;
        }
    }

    public class CompositeValidationResult : ValidationResult
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();

        public IEnumerable<ValidationResult> Results
        {
            get
            {
                return _results;
            }
        }

        public CompositeValidationResult(string errorMessage) : base(errorMessage) { }
        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }
        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

        public void AddResult(ValidationResult validationResult)
        {
            _results.Add(validationResult);
        }
    }

    public class CheckChildrenAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GeniusDMValidationResult result = new GeniusDMValidationResult();
            result.ErrorMessage = "";

            IEnumerable list = value as IEnumerable;
            if (list == null)
            {
                // Single Object
                if (value != null)
                {
                    List<ValidationResult> results = new List<ValidationResult>();
                    Validator.TryValidateObject(value, validationContext, results, true);
                    result.NestedResults = results;
                    return result;
                }
                
                return ValidationResult.Success;
            }
            else
            {
                List<ValidationResult> recursiveResultList = new List<ValidationResult>();

                // List Object
                var i = 0;
                foreach (var item in list)
                {
                    List<ValidationResult> nestedItemResult = new List<ValidationResult>();
                    ValidationContext context = new ValidationContext(item, null, null);

                    GeniusDMValidationResult nestedParentResult = new GeniusDMValidationResult();

                    Validator.TryValidateObject(item, context, nestedItemResult, true);
                    nestedParentResult.NestedResults = nestedItemResult;
                    nestedParentResult.ErrorMessage = string.Join(",", nestedItemResult);
                    result.ErrorMessage = !string.IsNullOrEmpty(nestedParentResult.ErrorMessage) ?
                        (string.IsNullOrEmpty(result.ErrorMessage) ?
                        $"Error at {validationContext.DisplayName}[{i++}] - {string.Join(",", nestedItemResult)}"
                        : $"{result.ErrorMessage} | Error at {validationContext.DisplayName}[{i++}] - {string.Join(",", nestedItemResult)}") 
                        : "";
                    if (nestedParentResult.NestedResults.Count > 0)
                        recursiveResultList.Add(nestedParentResult);
                }

                if (recursiveResultList.Count == 0) return ValidationResult.Success;

                result.NestedResults = recursiveResultList;
                return result;
            }
        }
    }

    public class GeniusDMValidationResult : ValidationResult
    {
        public GeniusDMValidationResult() : base("")
        {

        }
        public IList<ValidationResult> NestedResults { get; set; }
    }
}