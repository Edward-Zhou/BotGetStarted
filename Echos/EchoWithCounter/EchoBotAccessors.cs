using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoWithCounter
{
    public class EchoBotAccessors
    {
        public EchoBotAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
        }
        public static string CounterStateName { get; } = $"{nameof(EchoBotAccessors)}.CounterState";
        public IStatePropertyAccessor<CounterState> CounterState { get; set; }

        public ConversationState ConversationState { get; }
    }
}
