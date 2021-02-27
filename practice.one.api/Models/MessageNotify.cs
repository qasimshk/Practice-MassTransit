using practice.one.component.Abstractions;

namespace practice.one.api.Models
{
    public class MessageNotify : IMessageNotify
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }
}
