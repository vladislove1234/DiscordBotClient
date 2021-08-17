using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;
namespace DiscordBot.Models.Db
{
    class ServersContext : DbContext
    {
        public DbSet<Server> Servers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public ServersContext() : base("DbConnection") { }

    }
    class Bans
    {
        public ulong Guild;
        public DateTime End;
    }
    class Server
    {
        public ulong Id { get; set; }
        public List<int> UsersIds { get; set; }
        public List<int> RolesIds { get; set; }
        public string Prefix { get; set; }
    }
    class User
    {
        public int Id { get; set; }
        public List<Role> Roles { get; set; }
    }
    class Role
    {
        public int Id { get; set; }
        public ulong RoleId { get; set; }
    }
}