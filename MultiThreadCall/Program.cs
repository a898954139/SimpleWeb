// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

await StartAsync(default);
return;

async Task StartAsync(CancellationToken cancellationToken)
{
    for (var spread = 0; spread < 10; spread++)
    {
        var thread = new Thread(BalanceQueueJob.Task)
        {
            IsBackground = true
        };
        thread.Start();
        await Task.Delay(200, cancellationToken);
    }
}

public class BalanceQueueJob
{
    private static int _spread;
    private static IRabbirMqClient _mq;

    public static void Task()
    {
        _mq.ActionEventMessage += MqClient_ActionEventMessage;
        _mq.OnListening(MQMessageType.Balance.ProfitBalanceSpread(_spread));
    }

    private static void MqClient_ActionEventMessage(EventArgs e)
    {
        throw new NotImplementedException();
    }
}

public class MQMessageType
{
    public static Balance Balance { get; set; }
}

public class Balance
{
    public string ProfitBalanceSpread(int spread) => $"ProfitBalanceSpread_{spread}";
}

public interface IRabbirMqClient
{
    public ActionEvent ActionEventMessage { get; set; }
    void OnListening(string queueName);
}

public class RabbirMqClient : IRabbirMqClient
{
    private object _lock;
    private IModel _channel;
    public ActionEvent ActionEventMessage { get; set; }
    public void OnListening(string queueName)
    {
        try
        {
            lock (_lock)
            {
                if (_channel == null)
                {
                    //_channel = MqFactory.Instance.CreateModel();
                }

                _channel.QueueDeclare(queueName, true, false, false, null);
                var consumer = new EventingBasicConsumer(_channel); //创建事件驱动的消费者类型
                //consumer.Received += consumer_Received;
                _channel.BasicQos(0, 1, false); //一次只获取一个消息进行消费
                _channel.BasicConsume(queueName, false, consumer);
            }
        }
        catch (Exception ex)
        {
        }    
    }
}
public delegate void ActionEvent(EventArgs e);