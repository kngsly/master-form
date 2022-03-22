using System.ComponentModel.DataAnnotations;

namespace master_form_blazor_server.Data
{
    public class Validation
    {
        /// <summary>
        /// Uses System.ComponentModel.DataAnnotations to validate given objects.
        /// </summary>
        /// <param name="data">Any object</param>
        /// <returns>
        /// If it returns null, there are no errors.
        /// If it returns stuff, it will contain a message.
        /// </returns>
        public static Data.Result GetValdationErrors(object data)
        {
            var results = new List<ValidationResult>();

            string? _returnErrors = null;

            if (!Validator.TryValidateObject(data, new ValidationContext(data), results, true))
            {
                foreach (var errors in results)
                {
                    if (!string.IsNullOrEmpty(_returnErrors))
                        _returnErrors += "\r\n";

                    _returnErrors += errors.ErrorMessage ?? "Unknown error.";
                }
            }

            return new Result()
            {
                Ok = string.IsNullOrEmpty(_returnErrors),
                Message = _returnErrors ?? ""
            };
        }
    }
}