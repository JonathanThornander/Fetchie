using Fetchie.Host.MediatR.Requests.Messages;
using Fetchie.Host.MediatR.Requests.Queues;
using Fetchie.Host.Models;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fetchie.Host.Components.Pages.Home.Component
{
    public partial class SendMessageDialog
    {
        [CascadingParameter]
        public required MudDialogInstance MudDialog { get; init; }

        [Inject]
        public required IMediator Mediator { get; init; }

        public string Queue { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public bool Replace { get; set; }

        public DateTime? ExpirationDateUtc { get; set; } = DateTime.UtcNow.Date;

        public TimeSpan? ExpirationTimeUtc { get; set; } = DateTime.UtcNow.TimeOfDay.Add(TimeSpan.FromMinutes(1));

        public MessageSeverity Severity { get; set; }

        private async Task<IEnumerable<string>> SearchQueues(string input, CancellationToken token)
        {
            var queues = await Mediator.Send(new ListQueuesRequest());

            return queues.Where(queue => queue.Contains(input, StringComparison.OrdinalIgnoreCase)).Prepend(input);
        }

        private bool CanSubmit()
        {
            return !string.IsNullOrWhiteSpace(Title);
        }

        private async Task Submit()
        {
            if (!CanSubmit())
            {
                return;
            }

            DateTime? expirationDateUtc = ExpirationDateUtc?.Add(ExpirationTimeUtc ?? TimeSpan.Zero);

            await Mediator.Send(new SendMessageRequest
            {
                Title = Title,
                Body = Body,
                Queue = Queue,
                Severity = Severity,
                Replace = Replace,
                ExpirationUtc = expirationDateUtc
            });

            MudDialog.Close(DialogResult.Ok(true));
        }
    }
}
