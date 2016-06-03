using System;
using System.Collections.Generic;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace RedBear.Log4View.AzureServiceBus.Target
{
    [Target("Log4ViewTarget")]
    public sealed class Log4ViewTarget : TargetWithLayout
    {
        private TopicClient _client;

        [RequiredParameter]
        public string ConnectionString { get; set; }

        [RequiredParameter]
        public string Topic { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            CreateClient();
            _client.Send(ProcessEvent(logEvent));
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            try
            {
                CreateClient();
                _client.Send(ProcessEvent(logEvent.LogEvent));
                logEvent.Continuation(null);
            }
            catch (Exception ex)
            {
                logEvent.Continuation(ex);
            }
        }

        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            var messages = new List<BrokeredMessage>();
            var pendingContinuations = new List<AsyncContinuation>();
            Exception lastException = null;

            foreach (var logEvent in logEvents)
            {
                messages.Add(ProcessEvent(logEvent.LogEvent));
                pendingContinuations.Add(logEvent.Continuation);
            }

            try
            {
                CreateClient();
                _client.SendBatch(messages);
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            foreach (var cont in pendingContinuations)
            {
                cont(lastException);
            }

            pendingContinuations.Clear();
        }

        private BrokeredMessage ProcessEvent(LogEventInfo logInfo)
        {
            var item = new AzureLogEvent
            {
                Message = logInfo.FormattedMessage,
                Level = logInfo.Level.ToString(),
                Logger = logInfo.LoggerName,
                Exception = logInfo.Exception?.ToString(),
                StackTrace = logInfo.StackTrace?.ToString(),
                LogSource = Topic,
                OriginalTime = logInfo.TimeStamp,
                Key = logInfo.SequenceID,
                Host = Environment.MachineName
            };

            return new BrokeredMessage(JsonConvert.SerializeObject(item));
        }

        private void CreateClient()
        {
            if (_client == null)
            {
                var nsm = NamespaceManager.CreateFromConnectionString(ConnectionString);

                if (!nsm.TopicExists(Topic))
                {
                    var td = new TopicDescription(Topic) { DefaultMessageTimeToLive = TimeSpan.FromMinutes(1) };
                    nsm.CreateTopic(td);
                }

                _client = TopicClient.CreateFromConnectionString(ConnectionString, Topic);
            }
        }
    }
}