﻿using Microsoft.Practices.Unity;
using System;

namespace PersistenceBenchmark.ConsoleHost
{
    public class Program
    {
        public static bool VerboseIsEnabled = false;

        static void Main(string[] args)
        {
            DbManager.ResetDbs();
            var mainContainer = UnityConfig.GetConfiguredContainer(true);

            var user1App = UnityConfig.UserContainer1.Resolve<UserAppService>();
            var user2App = UnityConfig.UserContainer2.Resolve<UserAppService>();

            // Holding in memory messages
            // Expected: Events: waves * users. Inbox: waves * users

            Console.WriteLine("Press enter to start....");
            Console.ReadLine();

            // THIS MAKE CRASH 
            //user1App.StressWithWavesOfConcurrentUsers(wavesCount: 5, concurrentUsers: 1000);
            //user2App.StressWithWavesOfConcurrentUsers(wavesCount: 5, concurrentUsers: 1000);

            // THIS MAKE CRASH, ALMOST
            user1App.StressWithWavesOfConcurrentUsers(wavesCount: 5, concurrentUsers: 500);
            user2App.StressWithWavesOfConcurrentUsers(wavesCount: 5, concurrentUsers: 500);

            // Light
            //user1App.StressWithWavesOfConcurrentUsers(wavesCount: 1, concurrentUsers: 1);
            //user2App.StressWithWavesOfConcurrentUsers(wavesCount: 1, concurrentUsers: 1);

            Console.WriteLine("Press enter to stop and clean...");
            Console.ReadLine();
            DbManager.DropDb();
        }
    }
}
