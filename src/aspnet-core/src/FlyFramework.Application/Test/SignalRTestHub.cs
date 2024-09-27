using Microsoft.AspNetCore.SignalR;

namespace FlyFramework.Application.Test
{
    public class SignalRTestHub : Hub
    {
        public async Task DoWork()
        {
            //供客户端调用
        }
        public async Task SendMessage(string msg)
        {
            // 当前连接用户的标志,是一个GUID，如：362d3597-041e-4d65-8fdf-e77e98425d38
            string connectionId = Context.ConnectionId;
            // 给所有人发送消息
            await Clients.All.SendAsync(msg);
            // 给组内所有人发送消息
            await Clients.Group(connectionId).SendAsync(msg);
            // 给除去自己其他人发送消息
            await Clients.Others.SendAsync(msg);
            // 给自己发送消息
            await Clients.Caller.SendAsync(msg);
        }

        public async Task WhoIam(string connectionId)
        {
            // 当前连接用户的标志,是一个GUID，如：362d3597-041e-4d65-8fdf-e77e98425d38
            await Groups.AddToGroupAsync(Context.ConnectionId, connectionId);
            await Clients.Group(connectionId).SendAsync(Context.ConnectionId + "已登入");
        }

    }
}
