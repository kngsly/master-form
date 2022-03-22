using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace master_form_blazor_server.Data
{
    public class Experiment
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        [MaxLength(100)]
        public string Slug { 
            get { return Slug; } 
            set { Slug = new Regex("[^a-zA-Z0-9-]").Replace(value ?? "", ""); } 
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
            var Record = Storage.ReadFromJsonFile<Experiment>($"Storage/Experiments/{Slug}.json"); // In production: will be stored

            return Record;
        }

            // Assumption: This class environment is protected under some type of authentication for experiment creators.
        public async Task<Result> WriteExperimentToFile(Experiment experiment)
        {
            var validateInputs = Validation.GetValdationErrors(experiment); // Validate inputs
            if (validateInputs.Ok.Equals(false)) // Validation failed
                return validateInputs; // Return <Result> error

            // Check if this slug is unique
            // In production, this will utilize some form of centralize database storage
            Storage.WriteToJsonFile($"Storage/Experiments/{experiment.Slug}.json", experiment, false);

            return new Result()
            {
                Ok = true,
                Message = "Created!"
            };
        }
    }
}