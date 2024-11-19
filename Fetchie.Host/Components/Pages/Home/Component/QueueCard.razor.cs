using Dynq;
using Fetchie.Host.Dynq.Messages;
using Fetchie.Host.MediatR.Requests.Queues;
using Fetchie.Host.Models;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fetchie.Host.Components.Pages.Home.Component
{
    public partial class QueueCard : IDisposable
    {
        private List<IDisposable> _subscriptions = new List<IDisposable>();
    
        [EditorRequired]
        [Parameter]
        public required string Queue { get; init; }

        [Inject]
        public required IDialogService DialogService { get; init; }

        [Inject]
        public required IMediator Mediator { get; init; }

        [Inject]
        public required IDynqService Dynq { get; init; }

        public Message? Head { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _subscriptions.Add(Dynq.Subscribe<QueueHeadChangedMessage>(notification => LoadHead()));

            await LoadHead();
        }

        private async Task LoadHead()
        {
            Head = await Mediator.Send(new GetQueueHeadRequest { Queue = Queue });

            await InvokeAsync(StateHasChanged);
        }

        private async Task ClearQueue()
        {
            await Mediator.Send(new ClearQueueRequest { Queue = Queue });
        }

        private async Task DeleteQueue()
        {
            var shouldDelete = await DialogService.ShowMessageBox("Are you sure you want to delete this queue?", "Delete Queue", yesText: "Yes", noText: "No");

            if (shouldDelete != true) return;

            await Mediator.Send(new DeleteQueueRequest { Queue = Queue });
        }

        public void Dispose()
        {
            _subscriptions.ForEach(subscription => subscription.Dispose());
            _subscriptions.Clear();
        }
    }
}
