using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Signaler.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Signaler.Services;
using System;
using Newtonsoft.Json;

/// <summary>
/// Use cases: <br/>
/// 1. Posalji poruku korisniku <br/>
/// 2. Primi poruku od korisnika <br/>
/// 3. Kreiraj grupu <br/>
/// 4. 
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly SignalerDbContext _dbContext;

    private readonly IMessageService _messageService;

    private readonly IConnectionsService _connectionsService;
    public ChatHub(SignalerDbContext dbContext, IMessageService messageService, IConnectionsService connectionsService)
    {
        _messageService = messageService;
        _dbContext = dbContext;
        _connectionsService = connectionsService;
    }

    public override Task OnConnectedAsync()
    {
        var groups = _dbContext.Group
            .Include(g => g.Users)
            .Where(g => g.Users.Any(u => u.Username.Equals(Context.User.Identity.Name)))
            .ToList();

        _connectionsService.Add(Context.User.Identity.Name, Context.ConnectionId);
        groups.ForEach(async g => await Groups.AddToGroupAsync(Context.ConnectionId, g.GroupId.ToString()));        

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        _connectionsService.Remove(Context.User.Identity.Name, Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    // Message sent CLIENT -> Server
    public async Task SendMessage(int groupId, string message)
    {
        Message messageObj = new Message();

        messageObj.MessageText = message;
        messageObj.Group = _dbContext.Group.Where(g => g.GroupId == groupId).First();
        messageObj.User = _dbContext.User.Where(u => u.Username == Context.User.Identity.Name).First();

        _dbContext.Add(messageObj);
        await _dbContext.SaveChangesAsync();
        await Clients.GroupExcept(groupId.ToString(), Context.ConnectionId).SendAsync("ReceiveMessage", groupId, message, Context.User.Identity.Name);
    }

    public Task CreateGroup(string groupName, List<string> usernames)
    {
        var group = _messageService.CreateGroup(Context.User.Identity.Name, groupName, usernames);
        usernames.ForEach(u =>
        {
            _connectionsService.GetConnectionIds(u).ForEach(async cid => await Groups.AddToGroupAsync(cid, group.GroupId.ToString()));
        });

        return Task.CompletedTask;
    }

    public Task CreateChat(string username)
    {
        var group = _messageService.CreateChat(Context.User.Identity.Name, username);
        _connectionsService.GetConnectionIds(username).ForEach(async cid =>  await Groups.AddToGroupAsync(cid, group.GroupId.ToString()));
        return Task.CompletedTask;
    }
}