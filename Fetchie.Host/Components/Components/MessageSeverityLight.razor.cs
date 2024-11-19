using Fetchie.Host.Models;
using Microsoft.AspNetCore.Components;

namespace Fetchie.Host.Components.Components
{
    public partial class MessageSeverityLight
    {
        [Parameter]
        public MessageSeverity CurrentSeverity { get; set; }

        
    }
}
