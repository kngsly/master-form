using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace master_form_blazor_server.Data
{
    public class ExperimentFormInput
    {
        public string Label { get; set; } = "";
        public string Value { get; set; } = ""; // User input..
        public string Hint { get; set; } = "";
        public InputType InputType { get; set; }
        public List<string> Options { get; set; } = new List<string>(); // Used for InputValidation.Options
    }

    public enum InputType
    {
        Text,
        TextMulti,
        Password,
        Email,
        Phone,
        Dropdown
    }

    public class ExperimentFormBuilderService
    {
        public static List<ExperimentFormInput> DefaultInputs()
        {
            return new List<ExperimentFormInput>()
            {
                new ExperimentFormInput() {
                    Label = "Name",
                    InputType = InputType.Text,
                    Hint = "Your name"
                },
                new ExperimentFormInput() {
                    Label = "Email address",
                    InputType = InputType.Email,
                },
                new ExperimentFormInput(){
                    Label = "Phone",
                    InputType= InputType.Phone,
                }
            };
        }
    }
}