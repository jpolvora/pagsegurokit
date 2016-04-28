using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ServiceStack.Text;

namespace PagSeguroKit
{
    public class PagSeguroClient
    {
        private readonly string _ambiente;
        public PagSeguroConfig Config { get; set; }

        public PagSeguroClient(PagSeguroConfig config)
        {
            Config = config;
        }

        public PagSeguroClient(string ambiente = null)
        {
            _ambiente = ambiente;
            Config = PagSeguroConfig.LoadConfiguration(ambiente);
            if (Config == null)
                throw new InvalidOperationException("Configuração inválida. Corrija Web.Config/App.config ou utilize o construtor que recebe o objeto de configuração");
        }

        public async Task<CheckoutResponse> CheckoutAsync(PagSeguroCompra compra, string charset = null)
        {
            if (!compra.IsValid())
            {
                throw new InvalidOperationException("Compra inválida. Preencha o objeto corretamente.");
            }

            var dictionary = compra.ToDictionary(Config.Email, Config.Token, Config.ReturnUrl);
            var form = new FormUrlEncodedContent(dictionary);

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _ambiente == "sandbox" ? PagSeguroConfig.CheckoutUrlSandBox : PagSeguroConfig.CheckoutUrl)
                {
                    Content = form
                };

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "UTF-8"
                };

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return CheckoutSucess(result);
                }

                var sb = new StringBuilder();
                var errors = ParseError(result);
                foreach (var erro in errors)
                {
                    var str = string.Format("Erro Code {0}, Message: {1}", erro.Code, erro.Message);
                    sb.AppendLine(str);
                    Trace.TraceError(str);
                }

                throw new PagSeguroException("Erros de validação no pagseguro: " + sb, null);
            }
        }

        public string GetLinkPagamento(string codigoCompra)
        {
            return string.Format(_ambiente == "sandbox"
                ? PagSeguroConfig.PaymentUrlSandBox
                : PagSeguroConfig.PaymentUrl,
                codigoCompra);
        }

        public async Task<PagSeguroTransaction> ConsultaTransacaoAsync(string transacao)
        {
            var queryTransactionUrl = _ambiente == "sandbox"
                ? PagSeguroConfig.QueryTransactionUrlSandBox
                : PagSeguroConfig.QueryTransactionUrl;

            var parameters = string.Format("email={0}&token={1}", Config.Email, Config.Token);
            var fmtTarget = string.Format("{0}/{1}?{2}", queryTransactionUrl, transacao, parameters);
            Trace.TraceInformation("ConsultaTransacaoAsync: {0}", fmtTarget);

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(fmtTarget).ConfigureAwait(false);
                var xml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                Trace.TraceInformation("status: {0}", response.StatusCode);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        return CreatePagSeguroTransactionResponse(xml);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.ToString());
                        throw new PagSeguroException("Erro ao criar resposta XMl", ex);
                    }
                }

                var sb = new StringBuilder();
                var errors = ParseError(xml);
                foreach (var erro in errors)
                {
                    var str = string.Format("Erro Code {0}, Message: {1}", erro.Code, erro.Message);
                    sb.AppendLine(str);
                    Trace.TraceError(str);
                }
                throw new PagSeguroException("Erro ao Consultar Transação: StatusCode: {0}, Erros: {1}".Fmt(response.StatusCode, sb), null);
            }
        }

        public async Task<PagSeguroTransaction> ConsultaNotificationAsync(string notificationCode)
        {
            var queryNotificationUrl = _ambiente == "sandbox"
               ? PagSeguroConfig.QueryNotificationUrlSandBox
               : PagSeguroConfig.QueryNotificationUrl;

            var parameters = string.Format("email={0}&token={1}", Config.Email, Config.Token);
            var fmtTarget = string.Format("{0}/{1}?{2}", queryNotificationUrl, notificationCode, parameters);

            Trace.TraceInformation("ConsultaNotificationAsync: {0}", fmtTarget);
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(fmtTarget).ConfigureAwait(false);
                var xml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                Trace.TraceInformation("status: {0}", response.StatusCode);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        return CreatePagSeguroTransactionResponse(xml);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.ToString());
                        throw new PagSeguroException("Erro ao criar resposta XMl", ex);
                    }
                }
                var sb = new StringBuilder();
                var errors = ParseError(xml);
                foreach (var erro in errors)
                {
                    var str = string.Format("Erro Code {0}, Message: {1}", erro.Code, erro.Message);
                    sb.AppendLine(str);
                    Trace.TraceError(str);
                }
                throw new PagSeguroException("Erro ao Consultar Notificação: StatusCode: {0}, erros: {1}".Fmt(response.StatusCode, sb), null);
            }
        }

        #region helpers

        private CheckoutResponse CheckoutSucess(string xmlResult)
        {
            try
            {
                var doc = XDocument.Parse(xmlResult);

                var code = doc.Root.Element("code");
                var date = doc.Root.Element("date");
                return new CheckoutResponse()
                {
                    Code = code.Value,
                    Date = date.Value,
                    LinkPagamento = GetLinkPagamento(code.Value)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static IEnumerable<PagSeguroErro> ParseError(string xmlResult)
        {
            var response = new List<PagSeguroErro>();
            try
            {
                var doc = XDocument.Parse(xmlResult);
                var errors = doc.Root.Elements("error");

                foreach (var error in errors)
                {
                    var code = error.Element("code").Value;
                    var message = error.Element("message").Value;
                    response.Add(new PagSeguroErro(code, message));
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return response;
        }

        private static PagSeguroTransaction CreatePagSeguroTransactionResponse(string xml)
        {
            var doc = XDocument.Parse(xml);
            var code = doc.Root.Element("code").Value;
            var reference = doc.Root.Element("reference").Value;
            var status = Convert.ToInt32(doc.Root.Element("status").Value);
            var paymentMethod = doc.Root.Elements("paymentMethod").First();
            var pmType = paymentMethod.Element("type");
            var pmCode = paymentMethod.Element("code");
            var shipping = doc.Root.Elements("shipping").First();
            var address = shipping.Element("address");
            var street = address.Element("street");
            var number = address.Element("number");
            var complement = address.Element("complement");
            var district = address.Element("district");
            var postalCode = address.Element("postalCode");
            var city = address.Element("city");
            var state = address.Element("state");
            var netAmount = doc.Root.Element("netAmount").Value;

            string pmTypeStr = string.Empty;
            switch (pmType.Value)
            {
                case "1":
                    {
                        pmTypeStr = "Cartão de Crédito";
                        break;
                    }
                case "2":
                    {
                        pmTypeStr = "Boleto";
                        break;
                    }
                case "3":
                    {
                        pmTypeStr = "Débito online (TEF)";
                        break;
                    }
                case "4":
                    {
                        pmTypeStr = "Saldo PagSeguro";
                        break;
                    }
                case "5":
                    {
                        pmTypeStr = "Oi Paggo";
                        break;
                    }
                case "7":
                    {
                        pmTypeStr = "Depósito em conta";
                        break;
                    }
            }

            var statusResult = new PagSeguroTransaction()
            {
                TransacationCode = code,
                Reference = reference,
                Status = (PagSeguroTransactionStatus)status,
                Street = street.Value,
                Number = number.Value,
                Complement = complement.Value,
                District = district.Value,
                PostalCode = postalCode.Value,
                City = city.Value,
                State = state.Value,
                FormaDePagamento = string.Format("PagSeguro ({0})", pmTypeStr),
                NetAmount = decimal.Parse(netAmount)
            };

            return statusResult;
        }

        #endregion
    }
}