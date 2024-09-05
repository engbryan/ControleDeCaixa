
using Amazon.Lambda.SQSEvents;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace EntryControl.Core.Messages
{
    public static class EventMessageExtensions
    {
        public static T ExtractToType<T>(this SQSEvent.SQSMessage message)
            where T : class, new()
        {
            var messageTypeAttr = message.MessageAttributes.GetValueOrDefault("MessageType");
            var messageType = Type.GetType(messageTypeAttr.StringValue);

            return JsonSerializer.Deserialize<T>(message.Body);

        }
    }
}
