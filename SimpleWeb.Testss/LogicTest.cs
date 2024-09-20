namespace SimpleWeb.Testss;

public class LogicTest
{
    [Test]
    public void Test()
    {

        var orders = new List<Order>
        {
            new Order { Sd = "168", Bm = "12", Ct = "大" },
            new Order { Sd = "11", Bm = "12", Ct = "大" },
            new Order { Sd = "12", Bm = "12", Ct = "小" }
        };

        var dic = orders.GroupBy(g => g.Sd);

        Console.WriteLine(dic);
    }
    
    public class Order
    {
        public string Sd { get; set; }
        public string Bm { get; set; }
        public string Ct { get; set; }
    }
}