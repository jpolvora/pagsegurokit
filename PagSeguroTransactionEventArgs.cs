using System;

namespace PagSeguroKit
{
    public class PagSeguroTransactionEventArgs : PagSeguroRetornoEventArgs
    {
        /// <summary>
        /// Utilizar para consultar transa��o client.ConsultaTransaction()
        /// </summary>
        public string TransactionId { get; set; }

        public PagSeguroTransactionEventArgs(string transactionId, string ambiente, Action<string> feedbackAction) : base(ambiente, feedbackAction)
        {
            TransactionId = transactionId;
        }
    }
}