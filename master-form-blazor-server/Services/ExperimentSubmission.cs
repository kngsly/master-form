using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace master_form_blazor_server.Data
{
    public partial class ExperimentService : ControllerBase
    {
        // Rest API
        [HttpPost]
        [Route("experiment/submit/{Slug}")]
        public async Task<IActionResult> MeAsync(string Slug, [FromBody] List<ExperimentFormInput> Inputs)
        {
            var Response = await SubmitExperimentForm(Slug, Inputs);

            if(Response.Ok)
                return Ok(Response);
            else
                return BadRequest(Response);
        }

        /// <summary>
        /// Takes client input, validates some stuff and then appends the client input to the current state of the experiment.
        /// In production, we would have better data integrity which would allow us to use primary/foreign keys for references
        /// This approach allows us to have full context of the experiment, while also validating user inputs.
        /// </summary>
        /// <param name="Slug"></param>
        /// <param name="Inputs"></param>
        /// <returns>Result: true = Submitted successfully, false = Submission failed, contains an error message</returns>
        public async Task<Data.Result> SubmitExperimentForm(string Slug, List<ExperimentFormInput> Inputs)
        {
            var Experiment = await GetExperiment(Slug, false); // Get experiment data

            if (Experiment == null) return new Result() { Message = "This form is not available.", Ok = false };

            if (Experiment.Inputs.Any()) // Has custom/existing inputs
                Experiment.Inputs.InsertRange(0, ExperimentFormBuilderService.DefaultInputs()); // Add default inputs to start of form (should match /Pages/Experiment.razor
            else
                Experiment.Inputs = ExperimentFormBuilderService.DefaultInputs(); // Inherit default inputs

            // Error: the client input and experiment input counts do not match
            if (Inputs.Count != Experiment.Inputs.Count)
                return new Result() { Message = "The form you have submitted has changed, please refresh and fill in the form again.", Ok = false };

            for (var i = 0; i < Experiment.Inputs.Count; i++) // Read inputs
            {
                var SourceInput = Experiment.Inputs[i]; // The experiment's inputs
                var ClientInput = Inputs[i]; // The client/user input

                // Validate inputs
                var ValidationErrors = GetValidationError(SourceInput, ClientInput);
                if (ValidationErrors != null)
                    return new Result() { Message = ValidationErrors, Ok = false };
                // Validation passed

                // Append input to experiment data
                Experiment.Inputs[i].Value = ClientInput.Value;
            }

            // Saving inputs (This would be an SQL insert)
            // Storing in "Storage/Responses/slug_%.json"
            var TargetPath = Storage.GetUniquePath(Storage.AppendResponseDirectory(Slug));

            Storage.WriteToJsonFile<Experiment>(TargetPath.FullName, Experiment);

            // Success, finished.
            return new Result() { Ok = true, Message = "Success!" };
        }

        public string? GetValidationError(ExperimentFormInput Source, ExperimentFormInput Client)
        {
            // Error: the input type and labels do not match (data integrity)
            if (Source.InputType != Client.InputType || Source.Label != Client.Label)
                return "The target form has new changes, please refresh and fill in the form again.";

            // Validate select/dropdown options
            if (Source.InputType == InputType.Dropdown)
            {
                if (!Source.Options.Contains(Client.Value)) // Invalid option
                    return $"Please select valid option for {Source.Label}, available options: ({string.Join(",", Source.Options)})";
            }

            if (Source.InputType == InputType.Email)
            {
                if (!string.IsNullOrEmpty(Source.Value)) // If there is input
                {
                    if (!(new EmailAddressAttribute()).IsValid(Client.Value))
                        return $"{Client.Value} is not a valid email address.";
                }
            }

            return null;
        }
    }
}