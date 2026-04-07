using Microsoft.AspNetCore.SignalR;
using RedisDAL.User;
using SessionService;

namespace Chats.Service;

public class RUserService
{
    private readonly RUsersConnectMessage _rUserConnect;
    private readonly  HubCallerContext _context;
    private readonly ISessionService _session;

    public RUserService(RUsersConnectMessage rUserConnect,  HubCallerContext context,  ISessionService session)
    {
        _rUserConnect = rUserConnect;
        _context =  context;
        _session = session;
    }

    public async Task<string> GetUserId()
    {
        string id = await _rUserConnect.GetUserId(_context.ConnectionId);
        
        if (id == null) return null;
        return id;
    }
}