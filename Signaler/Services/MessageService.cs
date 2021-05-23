using Microsoft.EntityFrameworkCore;
using Signaler.Data;
using System.Collections.Generic;
using System.Linq;

namespace Signaler.Services
{
    public interface IMessageService
    {
        /// <summary>
        /// Returns all messages for the requested group 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        List<Message> GetGroupMessages(int groupId);
        List<Group> GetUserGroups(string username);

        /// <summary>
        /// Creats a chat between two users if the chat doesn't already exist.
        /// </summary>
        /// <param name="user1"></param>
        /// <param name="user2"></param>
        /// <returns>Chat between users</returns>
        Group CreateChat(string user1, string user2);

        /// <summary>
        /// Creates a group chat between participants
        /// </summary>
        /// <param name="creatorUsername"></param>
        /// <param name="groupName"></param>
        /// <param name="participants"></param>
        /// <returns>Newly created group</returns>
        Group CreateGroup(string creatorUsername, string groupName, List<string> participants);
    }

    public class MessageService : IMessageService
    {
        private readonly SignalerDbContext _dbContext;
        public MessageService(SignalerDbContext dbContext) => _dbContext = dbContext;

        public List<Message> GetGroupMessages(int groupId)
        {
            return _dbContext.Message.Where(m => m.Group.GroupId == groupId).Include(m => m.User).ToList();
        }

        public List<Group> GetUserGroups(string username)
        {
            return _dbContext.Group.Include(g => g.Users).Where(g => g.Users.Any(u => u.Username == username)).ToList();
        }

        public Group CreateChat(string user1, string user2)
        {
            // To make this method more robust, it wouldn't be a bad idea to check if users exist.
            var users = (from user in _dbContext.User
                               where user.Username == user1 || user.Username == user2
                               select user).ToList();

            var group = (from g in _dbContext.Group 
                         where g.Users.Count == 2 && g.Users.Contains(users[0]) && g.Users.Contains(users[1])  && g.Type.Equals("Chat")
                         select g).Include(g => g.Users).FirstOrDefault();
            if (group != null)
            {
                return group;
            }
            else
            {
                Group chat = new Group { Type = "Chat", Users = users };
                _dbContext.Add(chat);
                _dbContext.SaveChanges();

                return chat;
            }
        }

        public Group CreateGroup(string creator, string groupName, List<string> participants)
        {
            var users = (from user in _dbContext.User
                         where participants.Contains(user.Username) 
                         select user).ToList();
            var creatorUser = (from user in _dbContext.User
                               where user.Username.Equals(creator)
                               select user).First();

            users.Add(creatorUser);
            var group = new Group { Type = "Group", UserCreator = creatorUser, Users = users, GroupName = groupName };
            _dbContext.Add(group);
            _dbContext.SaveChanges();
            
            return group;
        }
    }
}