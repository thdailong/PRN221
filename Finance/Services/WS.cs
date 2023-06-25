using Finance.DAM;
using Finance.Model;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Finance.Services
{
    public class WS : Hub
    {
        public Account Account { get => Identity.Get(Context.GetHttpContext()!); }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task Join(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task RegisteredAccount(string userEmail)
        {
            var acc = await AccountDAM.Get(userEmail);
            await Clients.Group("listenNewUser").SendAsync("NewAccount", acc.DisplayName);
        }

        public async Task SentFeedback(string userEmail)
        {
            await Clients.Group("listenNewFb").SendAsync("NewFeedback", userEmail);
        }
    }
}
