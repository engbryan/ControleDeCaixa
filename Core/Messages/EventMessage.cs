
using System;

namespace EntryControl.Core.Messages
{
    public class EventMessage<T> where T : class, new()
    {
        public T Content { get; set; }
        public string MessageType { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        public EventMessage(T content)
        {
            Content = content;
            MessageType = GetType().Name;
            Timestamp = DateTime.UtcNow;
        }
    }
}
