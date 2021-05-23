using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Signaler.Data;
using Signaler.Services;
using System.Collections.Generic;

namespace Signaler.Controllers
{
    public class MessagesViewModel
    {
        public List<Message> Messages { get; set; }
        public string Username { get; set; }
    }


    public class ChatsIndexViewModel
    {
        public List<Group> Groups { get; set; }
        public List<User> Users { get; set; }
    }

    [Authorize]
    public class ChatsController : Controller
    {
        private readonly IMessageService _messageService;

        private readonly IUserService _userService;

        public ChatsController(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            List<Group> groups = _messageService.GetUserGroups(HttpContext.User.Identity.Name);
            List<User> users = _userService.All();
            return View(new ChatsIndexViewModel { Groups = groups, Users = users });
        }

        [HttpGet]
        public IActionResult GroupMessages(int groupId)
        {
            string username = HttpContext.User.Identity.Name;
            var messages = _messageService.GetGroupMessages(groupId);

            return View("Messages",new MessagesViewModel { Messages = messages, Username = username });
        }

        [HttpGet]
        public IActionResult Groups()
        {
            var groups = _messageService.GetUserGroups(HttpContext.User.Identity.Name);

            return View("Chats", groups);
        }

        [HttpPost]
        public IActionResult CreateGroupChat(string groupName, List<string> usernames)
        {
            return null;
        }
    }
}