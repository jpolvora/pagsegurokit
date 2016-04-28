using System;
using System.Collections.Generic;
using System.Threading;

namespace PagSeguroKit
{
    public static class PagSeguroEvents
    {
        public static event EventHandler<PagSeguroTransactionEventArgs> TransactionReceived;
        public static event EventHandler<PagSeguroNotificationEventArgs> NotificationReceived;
        
        internal static void InvokeNotificationReceived(PagSeguroNotificationEventArgs args)
        {
            Delegate[] subscribers = NotificationReceived?.GetInvocationList() ?? new Delegate[0];
            Execute(subscribers, args);
        }

        internal static void InvokeTransactionReceived(PagSeguroTransactionEventArgs args)
        {
            Delegate[] subscribers = TransactionReceived?.GetInvocationList() ?? new Delegate[0];
            Execute(subscribers, args);
        }

        /// <summary>
        /// Se invocar diretamente o evento, e um handler lançar exception, o httpHandler irá retornar erro 500
        /// </summary>
        private static void Execute<TEventArgs>(IEnumerable<Delegate> subscribers, TEventArgs args) where TEventArgs: PagSeguroRetornoEventArgs
        {
            args.FeedBack("Invoking event handlers on thread {0}", Thread.CurrentThread.ManagedThreadId);

            foreach (var subscriber in subscribers)
            {
                if (args.Handled)
                    break;

                var handler = (EventHandler<TEventArgs>)subscriber;
                try
                {
                    handler(null, args);
                    args.FeedBack("Event was handled successfully");
                }
                catch (Exception ex)
                {
                    args.FeedBack("Handler faulted: " + ex.ToString());
                }
            }
        }
    }
}