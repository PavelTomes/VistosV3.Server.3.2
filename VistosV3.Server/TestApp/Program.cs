using System;
using System.Linq;
using System.Collections.Generic;
using Core.VistosDb;
using Core.VistosDb.Objects;
using Core.Services;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (VistosDbContext ctx = new VistosDbContext())
            {
                List<UserAvatar> ts = ctx.UserAvatar.Where(t => t.Deleted == false).ToList();
                List<vwRole> vwRole1 = ctx.vwRole.ToList();
                List<vwUserAuthToken> vwUserAuthToken1 = ctx.vwUserAuthToken.ToList();
                string s = "";
            }
            List<vwRole> vwRole2 = Settings.GetInstance.VwRoleList;
        }
    }
}
