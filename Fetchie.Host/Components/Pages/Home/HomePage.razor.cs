using Dynq;
using Fetchie.Host.Components.Pages.Home.Component;
using Fetchie.Host.Dynq.Messages;
using Fetchie.Host.MediatR.Requests.Queues;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fetchie.Host.Components.Pages.Home
{
    public partial class HomePage : IDisposable
    {
        private List<IDisposable> _subscriptions = new List<IDisposable>();

        [Inject]
        public required IMediator Mediator { get; init; }

        [Inject]
        public required IDynqService Dynq { get; init; }

        [Inject]
        public required IDialogService DialogService { get; init; }

        public IEnumerable<string> Queues { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _subscriptions.Add(Dynq.Subscribe<QueueHeadChangedMessage>(notification => LoadQueues()));
            _subscriptions.Add(Dynq.Subscribe<QueueDeletedMessage>(notification => LoadQueues()));

            await LoadQueues();
        }

        private async Task LoadQueues()
        {
            Queues = await Mediator.Send(new ListQueuesRequest());

            await InvokeAsync(StateHasChanged);
        }

        private async Task SendMessage()
        {
            var dialog = DialogService.Show<SendMessageDialog>("Send Message");
            await dialog.Result;
        }

        public void Dispose()
        {
            _subscriptions.ForEach(subscription => subscription.Dispose());
            _subscriptions.Clear();
        }
    }
}
