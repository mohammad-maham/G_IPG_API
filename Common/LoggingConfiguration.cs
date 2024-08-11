using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankPay.Services.Global
{
    public static class LoggingConfiguration
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger => (context, configuration) =>
        {
            #region Enriching Logger Context
            var env = context.HostingEnvironment;

            configuration
               .Enrich.FromLogContext()
               .Enrich.WithProperty("ApplicationName", env.ApplicationName)
               .Enrich.WithProperty("Environment", env.EnvironmentName)
               .Enrich.WithExceptionDetails()
               .Enrich.WithProcessId()
               .Enrich.WithProcessName();
            #endregion

            configuration.WriteTo.Console().MinimumLevel.Information();

            #region ElasticSearch Configuration.
            var elasticUrl = context.Configuration.GetValue<string>("Logging:ElasticUrl");

            if (!string.IsNullOrEmpty(elasticUrl))
            {
                configuration.WriteTo.Elasticsearch(
               new ElasticsearchSinkOptions(new Uri(elasticUrl))
               {
                   AutoRegisterTemplate = true,
                   AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                   IndexFormat = "ipg-logs-{0:yyyy.MM}" + ".w" + Helper.GetWeekNumberOfMonth(DateTime.Now),
                   MinimumLogEventLevel = LogEventLevel.Information
               });
            }
            #endregion
        };
    }
}
