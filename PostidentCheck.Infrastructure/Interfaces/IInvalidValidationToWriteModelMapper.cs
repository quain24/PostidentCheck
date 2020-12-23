using FluentValidation.Results;

namespace Postident.Infrastructure.Interfaces
{
    public interface IInvalidValidationToWriteModelMapper<out TWriteModelType>
    {
        /// <summary>
        /// Maps an invalid validation result to a write model for database to process
        /// </summary>
        /// <param name="result">Validation result to be mapped</param>
        /// <param name="id">mapped object id</param>
        /// <returns>A db write model created from given validation result and id</returns>
        public TWriteModelType MapInvalidResult(ValidationResult result, string id);
    }
}