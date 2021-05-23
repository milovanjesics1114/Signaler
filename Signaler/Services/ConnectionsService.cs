using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Signaler.Services
{
    public interface IConnectionsService
    {
        void Add(string username, string connectionId);
        void Remove(string username, string connectionId);

        List<string> GetConnectionIds(string username);
    }

    public class ConnectionsService : IConnectionsService
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> userConnectionsMap;

        public ConnectionsService()
        {
            userConnectionsMap = new ConcurrentDictionary<string, HashSet<string>>();
        }

        public void Add(string username, string connectionId)
        {
            var connections = userConnectionsMap.AddOrUpdate(username, new HashSet<string>(), (key, oldValue) =>
            { 
                oldValue.Add(connectionId);
                return oldValue;
            });
        }

        public void Remove(string username, string connectionId)
        {
            userConnectionsMap.AddOrUpdate(username, new HashSet<string>(), (key, set) =>
            {
                set.Remove(connectionId);
                return set;
            });
        }

        public List<string> GetConnectionIds(string username)
        {
            var set = userConnectionsMap.GetValueOrDefault(username, new HashSet<string>());
            List<string> list = null;
            lock(set)
            {
                list = set.ToList();
            }

            return list;
        }
    }
}