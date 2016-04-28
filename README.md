# pagsegurokit
Integração customizada com o PagSeguro

Exemplo de Utilização:

            var compra = new PagSeguroCompra("REF1234:", redirectUrl: "http://minhaloja.com.br/finazar.html");
            compra.AddItem(codigoProduto: "001", descricaoProduto: "Notebook Core I7", qtde: 1, valor: 1999);
            
            if (compra.IsValid())
            {
                var client = new PagSeguroClient(ambiente: "sandbox");
                CheckoutResponse result = await client.CheckoutAsync(compra);

                if (result.Success)
                {
                    Console.WriteLine(result.LinkPagamento);
                    Console.ReadLine();
                    //digitar o número da transação
                    Console.WriteLine("Cole o número da transação:");
                    var transacao = Console.ReadLine();
                    if (!string.IsNullOrEmpty(transacao))
                    {
                        //consultar a transação
                        var status = await client.ConsultaTransacaoAsync(transacao);
                        Console.WriteLine(status.Status);
                        Console.ReadLine();
                    }
                }
                else
                {
                    //errors
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Key + ": " + error.Value);
                    }

                    Console.ReadLine();
                }
            }
