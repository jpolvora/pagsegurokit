using System.Collections.Generic;
using System.Linq;

namespace PagSeguroKit
{
    public class CheckoutResponse
    {
        /// <summary>
        /// Código para redirecionamento na página de pagamentos do PagSeguro
        /// </summary>
        public string Code { get; set; }
        public string Date { get; set; }

        public string LinkPagamento { get; set; }

        public CheckoutResponse()
        {
            Errors = new List<KeyValuePair<string, string>>();
        }

        public bool Success
        {
            get { return !Errors.Any(); }
        }

        public List<KeyValuePair<string, string>> Errors { get; private set; }
    }
}