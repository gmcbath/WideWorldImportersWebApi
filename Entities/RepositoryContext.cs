using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using NLog;

namespace Entities
{
    public class RepositoryContext : WideWorldImportersDbContext
    {
        private readonly IOptions<AppConfigSettings> _appConfigSettings;

        public RepositoryContext(IOptions<AppConfigSettings> appConfigSettings) { _appConfigSettings = appConfigSettings; }

        public RepositoryContext(DbContextOptions<WideWorldImportersDbContext> options, IOptions<AppConfigSettings> appConfigSettings)
            : base(options)
        {
            _appConfigSettings = appConfigSettings;
        }

        public RepositoryContext(DbContextOptions<WideWorldImportersDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ILogger logger = LogManager.GetCurrentClassLogger();

            if (_appConfigSettings.Value.LogSqlServer)
            {
                optionsBuilder.LogTo(logger.Debug);
                optionsBuilder.EnableSensitiveDataLogging();

                /*

                    No relationship from 'Application_City' to 'Application_SystemParameter' has been configured by convention 
                    because there are multiple properties on one entity type {'Application_SystemParameters_DeliveryCityId', 
                    'Application_SystemParameters_PostalCityId'} that could be matched with the properties on the other entity type 
                    {'DeliveryCity', 'PostalCity'}. This message can be disregarded if explicit configuration has been specified in 
                    'OnModelCreating'.  CoreLoggerExtensions.MultipleNavigationProperties => IDiagnosticsLogger.DispatchEventData => FormattingDbContextLogger.Log
                  
                    https://github.com/dotnet/efcore/issues/11249
                    These are apparently due to multiple foreign keys on one table referencing the same table.
                    
                    For example :
                   
                    Customer -> What user created the customer (references User)
                    Customer -> What user modified the customer (references User)

                    The fix:
                    https://stackoverflow.com/questions/56419481/suppress-ef-core-3-0-x-initialized-msg

                */

                optionsBuilder.ConfigureWarnings(warnings => warnings
                                                     .Ignore(CoreEventId.MultipleNavigationProperties));

            }
        }
    }
}