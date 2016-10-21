﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Toolbox.Logstash.Client;
using Toolbox.Logstash.Message;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Toolbox.Logstash
{
    public class Example
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();
            var app = new ApplicationBuilder(services.BuildServiceProvider());
            var factory = new LoggerFactory();
            //TODO ? //factory.MinimumLevel = Microsoft.Extensions.Logging.LogLevel.Debug;
            factory.AddLogstashHttp(app, options => 
                                    {
                                        options.Index = "index";
                                        options.Url = "url";
                                    });

            var logger = factory.CreateLogger("MyLog");
            for (int i = 0; i < 500; i++)
            {
                Task.Run(() => logger.Log(Microsoft.Extensions.Logging.LogLevel.Debug, 100, BuildLogMessage(), null, null));
            }
            System.Console.ReadLine();
        }

        public static LogMessage BuildLogMessage()
        {
            LogMessage message = new LogMessage();
            LogMessageHeader header = new LogMessageHeader();
            LogMessageBody body = new LogMessageBody();

            message.Header = header;
            message.Body = body;

            header.Source.ApplicationId = Guid.NewGuid().ToString();
            header.Correlation = new LogMessageCorrelation();
            header.Correlation.ApplicationId = Guid.NewGuid().ToString();
            header.Correlation.CorrelationId = Guid.NewGuid().ToString();
            header.Index = "new_index";
            header.IPAddress = IPAddress.Loopback.ToString();
            header.Source = new LogMessageSource("appid", "logging_source");
            header.ProcessId = "135";
            header.ThreadId = "15";
            header.TimeStamp = DateTime.Now;
            header.VersionNumber = "v1.0.2";

            body.Level = LogstashLevel.Debug;
            body.Content = "business_content";
            body.User = new LogMessageUser();
            body.User.IPAddress = IPAddress.Loopback.ToString();
            body.User.UserId = @"ICA\EX01913";
            body.VersionNumber = "v1.0.3";

            return message;

        }
    }
}
