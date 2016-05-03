namespace PagSeguroKit
{
    public class PagSeguroTransaction
    {
        public string TransactionCode { get; set; }
        public string Reference { get; set; }
        public PagSeguroTransactionStatus Status { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string District { get; set; } //Bairro
        public string PostalCode { get; set; } //cep
        public string City { get; set; }
        public string State { get; set; }
        public decimal NetAmount { get; set; }
        public string FormaDePagamento { get; set; }

        public string Exception { get; set; }

        public bool Success
        {
            get { return string.IsNullOrEmpty(Exception); }
        }
    }
}