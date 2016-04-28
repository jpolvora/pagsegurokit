using System;

namespace PagSeguroKit
{
    public class PagSeguroNotificationEventArgs : PagSeguroRetornoEventArgs
    {
        /// <summary>
        /// Utilizar para consultar notificação client.ConsultaNOtification()
        /// </summary>
        public string NotificationCode { get; set; }

        public PagSeguroNotificationEventArgs(string notificationCode, string ambiente, Action<string> feedbackAction) :
            base(ambiente, feedbackAction)
        {
            NotificationCode = notificationCode;
        }
    }
}