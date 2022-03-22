using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace master_form_blazor_server.Data
{
    public class AuthenticationContext
    {
        public bool IsAnAdmin { get; set; }
    }
}