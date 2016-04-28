namespace PagSeguroKit
{
    public enum PagSeguroTransactionStatus
    {
        Erro = -1,
        Pendente = 0,
        Aguardando_Pagamento = 1,
        Em_Análise = 2,
        Paga = 3,
        Disponível = 4,
        Em_Disputa = 5,
        Devolvida = 6,
        Cancelada = 7
    }
}