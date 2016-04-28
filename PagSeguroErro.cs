namespace PagSeguroKit
{
    public class PagSeguroErro
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public PagSeguroErro(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}