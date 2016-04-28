using System;
using System.Configuration;
using System.Diagnostics;

namespace PagSeguroKit
{
    public class PagSeguroConfig
    {
        public const string CheckoutUrl = "https://ws.pagseguro.uol.com.br/v2/checkout";
        public const string CheckoutUrlSandBox = "https://ws.sandbox.pagseguro.uol.com.br/v2/checkout";

        public const string PaymentUrl = "https://pagseguro.uol.com.br/v2/checkout/payment.html?code={0}";
        public const string PaymentUrlSandBox = "https://sandbox.pagseguro.uol.com.br/v2/checkout/payment.html?code={0}";

        public const string QueryTransactionUrl = "https://ws.pagseguro.uol.com.br/v3/transactions";
        public const string QueryTransactionUrlSandBox = "https://ws.sandbox.pagseguro.uol.com.br/v3/transactions";

        public const string QueryNotificationUrl = "https://ws.pagseguro.uol.com.br/v3/transactions/notifications";
        public const string QueryNotificationUrlSandBox = "https://ws.sandbox.pagseguro.uol.com.br/v3/transactions/notifications";

        public string Email { get; private set; }
        public string Token { get; private set; }

        /// <summary>
        /// Url onde serão postadas as notificações para as compras
        /// </summary>
        public string ReturnUrl { get; private set; }


        public PagSeguroConfig(string email, string token, string returnUrl)
        {
            Email = email;
            Token = token;
            ReturnUrl = returnUrl;
        }

        public static PagSeguroConfig LoadConfiguration(string ambiente = null)
        {
            try
            {
                string prefixo = "pagseguro:";
                if (!string.IsNullOrEmpty(ambiente))
                {
                    prefixo = prefixo + ambiente + ":";
                }

                PagSeguroConfig config = new PagSeguroConfig(
                    email: ConfigurationManager.AppSettings[prefixo + "email"],
                    token: ConfigurationManager.AppSettings[prefixo + "token"],
                    returnUrl: ConfigurationManager.AppSettings[prefixo + "returnUrl"]
                    );

                return config;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return null;
            }
        }
    }
}