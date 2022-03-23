using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace master_form_blazor_server.Data
{
    public class Experiment
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        private string slug; // used to hold and modify slug regex
        [Required(ErrorMessage = "URL is required.")]
        [MinLength(1, ErrorMessage = "URL must be atleast 1 character.")]
        [MaxLength(200, ErrorMessage = "URL must be less than 200 characters.")]
        public string Slug // Used as a unique reference
        {
            get { return this.slug; }
            set { this.slug = (new Regex("[^a-zA-Z0-9-]").Replace(value ?? "", "-")).TrimEnd('-'); }
        } // Format slug to be more URL friendly by removing all non alphamerical characters, except for -

        public bool Enabled { get; set; } = true; // Allow experiment to be viewed/submitted
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public List<ExperimentFormInput> Inputs { get; set; } = new List<ExperimentFormInput>();
    }

    public partial class ExperimentService : ControllerBase
    {
        [HttpGet]
        [Route("api/experiment/{Slug}")]
        public async Task<IActionResult> GetExperimentIActionResult(string Slug, bool IsAdmin = false)
        {
            var Response = await GetExperiment(Slug, IsAdmin);

            if (Response != null)
                return Ok(Response);
            else
                return NotFound(Response);
        }

        public async Task<Experiment> GetExperiment(string Slug, bool IsAdmin = false)
        {
            // Assumption: In production there will be some kind of remote/async request to receive experiment data
            var Record = Storage.ReadFromJsonFile<Experiment>(Storage.AppendSlugDirectory(Slug)); // In production: will be stored

            if (Record != null) // Is actually found
            {
                if (!IsAdmin && Record.Enabled.Equals(false)) // User is not an admin & the record is disabled
                {
                    return default; // Return not found
                }
            }

            return Record;
        }
    }
}