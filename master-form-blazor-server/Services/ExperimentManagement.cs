using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace master_form_blazor_server.Data
{
    public partial class ExperimentService : ControllerBase
    {
        [HttpPost]
        [Route("api/experiment/create")]
        [Route("api/experiment/create/{initSlug?}")]
        public async Task<IActionResult> WriteExperimentToFileIActionResult([FromBody] Experiment experiment, string? initSlug)
        {
            var Response = await WriteExperimentToFile(experiment, initSlug);

            if (Response != null)
                return Ok(Response);
            else
                return NotFound(Response);
        }

        // Assumption: This class environment is protected under some type of authentication for experiment creators.
        public async Task<Result> WriteExperimentToFile(Experiment experiment, string? initSlug)
        {
            var validateInputs = Validation.GetValdationErrors(experiment); // Validate inputs
            if (validateInputs.Ok.Equals(false)) // Validation failed
                return validateInputs; // Return <Result> error

            if (experiment.Created == default)
                experiment.Created = DateTime.Now; // Add initial creation date
            experiment.Updated = DateTime.Now;

            // Store experiment as slug.json, existing data will be replaced
            Storage.WriteToJsonFile(Storage.AppendSlugDirectory(experiment.Slug), experiment, false);

            if (initSlug != null) // We're editing
            {
                if (experiment.Slug != initSlug) // The slug has changed, there are now multiple versions of this experiment
                {
                    Storage.RemoveFile(Storage.AppendSlugDirectory(initSlug)); // Remove the initial/old slug
                }
            }

            return new Result()
            {
                Ok = true,
                Message = $"Experiment has been {(initSlug != null ? "updated" : "created")}!"
            };
        }

        [HttpPost]
        [Route("api/experiment/disable/{Slug}")]
        public async Task<IActionResult> DisableExperimentIActionResult(string Slug)
        {
            var Response = await DisableExperiment(Slug);

            if (Response != null)
                return Ok(Response);
            else
                return NotFound(Response);
        }

        // Assumption: This class environment is protected under some type of authentication for experiment creators.
        public async Task<Result> DisableExperiment(string Slug)
        {
            var Experiment = await GetExperiment(Slug, true); // Assume we're an admin and can edit any experiment

            if (Experiment != null)
            {
                if(Experiment.Enabled.Equals(false))
                {
                    return new Result() { Message = "This experiment is already disabled.", Ok = false };
                }

                Experiment.Enabled = false; // Set enabled to false

                await WriteExperimentToFile(Experiment, Slug); // Save changes

                return new Result()
                {
                    Ok = true,
                    Message = $"Experiment has been disabled."
                };
            }
            else
            {
                return new Result() { Message = "Experiment not found.", Ok = false };
            }

        }
    }
}