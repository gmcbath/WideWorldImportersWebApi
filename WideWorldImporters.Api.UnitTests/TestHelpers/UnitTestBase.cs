using System;
using System.Collections.Generic;
using System.IO;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WideWorldImporters.Api.Utility;

namespace WideWorldImporters.Api.UnitTests.TestHelpers
{
    /// <summary>
    ///     https://www.meziantou.net/quick-introduction-to-xunitdotnet.htm
    ///     https://stackoverflow.com/questions/12976319/xunit-net-global-setup-teardown/16590641
    /// </summary>
    public class UnitTestBase : IDisposable
    {
        public UnitTestBase()
        {
            // Called once before running all tests
            AppConfigSettings = CreateIOptionsObject();
            ConfigurationRoot = CreateConfigurationBuilderObject();
            InMemoryDatabase = CreateInMemoryDatabase();
        }

        public void Dispose()
        {
            // Called once after running all tests
        }

        internal IOptions<AppConfigSettings> AppConfigSettings { get; }
        private IConfigurationRoot ConfigurationRoot { get; }
        internal DbContextOptions<WideWorldImportersDbContext> InMemoryDatabase { get; }

        /// <summary>
        ///     Create in memory DbContext
        /// </summary>
        /// <returns></returns>
        private static DbContextOptions<WideWorldImportersDbContext> CreateInMemoryDatabase()
        {
            return new DbContextOptionsBuilder<WideWorldImportersDbContext>()
                   .UseInMemoryDatabase("RepositoryDataBase")
                   .Options;
        }

        /// <summary>
        ///     Build IOption
        ///     https://stackoverflow.com/questions/40876507/net-core-unit-testing-mock-ioptionst
        ///     https://stackoverflow.com/questions/41399526/how-to-initialize-ioptionappsettings-for-unit-testing-a-net-core-mvc-service
        /// </summary>
        /// <returns></returns>
        private static IOptions<AppConfigSettings> CreateIOptionsObject()
        {
            IOptions<AppConfigSettings> appConfig = Options.Create(new AppConfigSettings());
            appConfig.Value.DeleteLog = true;
            appConfig.Value.LogSqlServer = true;

            return appConfig;
        }

        /// <summary>
        ///     Build Configuration
        ///     https://weblog.west-wind.com/posts/2018/Feb/18/Accessing-Configuration-in-NET-Core-Test-Projects
        /// </summary>
        /// <param name="useInMemoryCollection"></param>
        /// <returns></returns>
        private static IConfigurationRoot CreateConfigurationBuilderObject(bool useInMemoryCollection = false)
        {
            IConfigurationRoot configuration = null;

            if (useInMemoryCollection)
            {
                // https://stackoverflow.com/questions/55497800/populate-iconfiguration-for-unit-tests
                var myConfiguration = new Dictionary<string, string>
                                      {
                                          {"AllowedHosts", "*"},
                                          {"AppConfigSettings:LogSqlServer", "true"},
                                          {"AppConfigSettings:DeleteLog", "false"}
                                      };

                configuration = new ConfigurationBuilder()
                                .AddInMemoryCollection(myConfiguration)
                                .Build();
            }
            else
            {
                Environment.SetEnvironmentVariable("myVariableName", "myVariableValue");

                // todo - resolve this bug
                // UtilityHelpers.WriteDebugString($"myVariableName = {Environment.GetEnvironmentVariable("myVariableValue")}");

                string assemblyLocation = UtilityHelpers.AssemblyDirectory;
                string appsettingsFileName = Path.Combine(assemblyLocation, "appsettings.json");

                var builder = new ConfigurationBuilder()
                              .SetBasePath(assemblyLocation)
                              .AddJsonFile(appsettingsFileName, false, true);

                configuration = builder.Build();
            }

            return configuration;
        }

        //public Task InitializeAsync()
        //{
        //    // Called once before running all tests in UnitTest1
        //}

        //public Task DisposeAsync()
        //{
        //    // Called once after running all tests in UnitTest1
        //}
    }
}