using RabbitMQ.Client;
using System.Security.Cryptography;
using System.Text;

internal class Send_one
{
    public static async Task Main(string[] args)
    {

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

        const string message = $@"
--ITW1
--RPC19

select
T0.""DocNum"",
T0.""DocEntry"",
T0.""DocDate"",
T1.""ItemCode"",
T1.""Quantity"",
T4.""BinCode"",
T5.""OnHandQty"",
T7.""DistNumber"",
T7.""SysNumber"",
T7.""AbsEntry""

FROM
ORPC T0  
INNER JOIN RPC1 T1 ON T0.""DocEntry"" = T1.""DocEntry""
INNER JOIN OITW  T3  on T1.""ItemCode"" = T3.""ItemCode"" and T1.""WhsCode"" = T3.""WhsCode""
INNER JOIN OBIN  T4  on T3.""WhsCode"" = T4.""WhsCode""
INNER JOIN OIBQ T5 ON T3.""WhsCode""  = T5.""WhsCode"" AND T4.""AbsEntry"" = T5.""BinAbs"" AND T3.""ItemCode"" = T5.""ItemCode"" and T1.""WhsCode"" = T5.""WhsCode"" 
INNER JOIN OBBQ T6 ON T1.""ItemCode"" = T6.""ItemCode"" AND T4.""AbsEntry"" = T6.""BinAbs""  and T1.""WhsCode"" = T6.""WhsCode""
INNER JOIN OBTN T7 ON  T6.""SnBMDAbs"" = T7.""AbsEntry"" and T1.""ItemCode"" = T7.""ItemCode""
INNER JOIN OITM T8 ON T3.""ItemCode"" = T8.""ItemCode"" AND T5.""ItemCode"" = T8.""ItemCode""

WHERE 
T0.""DocEntry"" = '10897'
and T1.""ItemCode"" = 'GEN-CAPD0008'

ORDER BY
T0.""DocNum"" desc";
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);
        Console.WriteLine($" [x] Sent {message}");

        Console.WriteLine(" Saliendo ");
        Task.Delay(1000).Wait();

    }
}