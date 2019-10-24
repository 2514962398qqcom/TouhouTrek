using System;
using System.Threading.Tasks;

namespace ZMDFQ.Cards
{
    public class CardCallback
    {
        Card card { get; } = null;
        Func<Card, object[], Task> callback { get; } = null;
        public CardCallback(Card card, Func<Card, object[], Task> callback)
        {
            this.card = card;
            this.callback = callback;
        }
        public Task call(object[] args)
        {
            if (callback != null)
                return callback.Invoke(card, args);
            else
                return Task.CompletedTask;
        }
    }
}
