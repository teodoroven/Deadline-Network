using System;
using Microsoft.AspNet.SignalR;
using Server.App.Services;

namespace Server.App.Hubs;
public class AuthorizeHub : Hub
{
    public AuthorizeHub(ILogin loginService, IRegister registerService)
    {
        
    }
}
