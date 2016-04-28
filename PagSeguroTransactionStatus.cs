namespace PagSeguroKit
{
    public enum PagSeguroTransactionStatus
    {
        Erro = -1,
        Pendente = 0,
        Aguardando_Pagamento = 1,
        Em_An�lise = 2,
        Paga = 3,
        Dispon�vel = 4,
        Em_Disputa = 5,
        Devolvida = 6,
        Cancelada = 7
    }
}