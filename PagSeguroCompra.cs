using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PagSeguroKit
{
    public class PagSeguroCompra
    {
        private class Item
        {
            public string CodigoProduto { get; set; }
            public string DescricaoProduto { get; set; }
            public decimal Valor { get; set; }
            public int Quantidade { get; set; }
        }

        public string CodigoCompra { get; set; }
        public string NomeComprador { get; set; }
        public string EmailComprador { get; set; }
        public string CpfComprador { get; set; }
        public string Telefone { get; set; }
        public DateTime DataNascto { get; set; }

        /// <summary>
        /// Url para redirecionar após a compra
        /// </summary>
        public string RedirectUrl { get; set; }

        private readonly List<Item> _items = new List<Item>();

        public PagSeguroCompra(string reference, string redirectUrl)
        {
            CodigoCompra = reference;
            RedirectUrl = redirectUrl;
        }

        public PagSeguroCompra AddItem(string codigoProduto, string descricaoProduto, int qtde, decimal valor)
        {
            var item = new Item
            {
                CodigoProduto = codigoProduto,
                DescricaoProduto = descricaoProduto,
                Quantidade = qtde,
                Valor = valor
            };

            _items.Add(item);

            return this;
        }

        public bool IsValid()
        {
            return _items.Count > 0;
        }

        public IDictionary<string, string> ToDictionary(string email, string token, string returnUrl)
        {
            var dictionary = new Dictionary<string, string>
            {
                {"email", email},
                {"token", token},
                {"currency", "BRL"},
                {"reference", this.CodigoCompra},
                {"senderName", this.NomeComprador},
                {"senderEmail", this.EmailComprador},
                {"senderCPF", this.CpfComprador},
                {"senderBornDate", string.Format("{0:dd/MM/yyyy}", this.DataNascto)},
                {"notificationURL", returnUrl},
                {"redirectURL", this.RedirectUrl}
            };

            if (!string.IsNullOrEmpty(this.Telefone) && this.Telefone.Length > 2)
            {
                dictionary.Add("senderAreaCode", this.Telefone.Substring(0, 2));
                dictionary.Add("senderPhone", this.Telefone.Substring(2));
            }

            for (int i = 1; i <= _items.Count; i++)
            {
                dictionary.Add("itemId" + i, _items[i - 1].CodigoProduto);
                dictionary.Add("itemDescription" + i, _items[i - 1].DescricaoProduto);
                dictionary.Add("itemAmount" + i, _items[i - 1].Valor.ToString("F").Replace(",", "."));
                dictionary.Add("itemQuantity" + i, _items[i - 1].Quantidade.ToString());
            }

            return dictionary;
        }
    }
}