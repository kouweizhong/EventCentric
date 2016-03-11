﻿using Microsoft.Practices.Unity;
using System;

namespace PersistenceBenchmark.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            DbManager.CreateDbs();
            var mainContainer = UnityConfig.GetConfiguredContainer(true);

            var user1App = UnityConfig.UserContainer1.Resolve<UserAppService>();
            var user2App = UnityConfig.UserContainer2.Resolve<UserAppService>();

            // Holding in memory messages
            // Expected: Events: waves * users. Inbox: waves * users
            user1App.StressWithWavesOfConcurrentUsers(wavesCount: 10, concurrentUsers: 500);
            user2App.StressWithWavesOfConcurrentUsers(wavesCount: 10, concurrentUsers: 500);

            Console.WriteLine("Press any key to stop");
            Console.ReadLine();
            DbManager.DropDbs();
        }
    }
}
