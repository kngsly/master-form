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
        public string Slug
        {
            get { return this.slug; }
            set { this.slug = (new Regex("[^a-zA-Z0-9-]").Replace(value ?? "", "-")).TrimEnd('-'); }
        } // Used as a unique reference (url supported)
        public bool Enabled { get; set; } // Allow experiment to be viewed/submitted
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public class ExperimentService
    {
        public async Task<Experiment> GetExperiment(string Slug)
        {
            // Assumption: In production there will be some kind of remote/async request to receive experiment data
            var Record = Storage.ReadFromJsonFile<Experiment>(Storage.AppendSlugDirectory(Slug)); // In production: will be stored

            return Record;
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
                Message = "Experiment has been update!"
            };
        }
    }
}