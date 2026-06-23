using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PRN222_assigment2.Hubs
{
    public class NewsHub : Hub
    {
        public async Task SendNewsUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveNewsUpdate", message);
        }
    }
}
