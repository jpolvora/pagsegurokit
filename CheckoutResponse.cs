using System.Collections.Generic;
using System.Linq;

namespace PagSeguroKit
{
    public class CheckoutResponse
    {
        /// <summary>
        /// C�digo para redirecionamento na p�gina de pagamentos do PagSeguro
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