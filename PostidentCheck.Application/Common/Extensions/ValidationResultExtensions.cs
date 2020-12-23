using FluentValidation.Results;
using System;
using System.Linq;

namespace Postident.Application.Common.Extensions
{
    public static class ValidationResultExtensions
    {
        /// <summary>
        /// Returns a complete set of "Property name - Error message" pairs in a single string.
        /// </summary>
        /// <param name="result"></param>
        /// <exception cref="ArgumentNullException">If tried to get information out of <see lang="null"/></exception>
        /// <returns>Single string containing unified list of errors from <see cref="ValidationResult"/></returns>
        public static string CombinedErrors(this ValidationResult result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result), "Tried to get combined errors from a NULL.");

            var output = result.Errors?.Select(e => $"{e?.PropertyName} - {e?.ErrorMessage}");
            return string.Join(" | ", output ?? Array.Empty<string>());
        }
    }
}