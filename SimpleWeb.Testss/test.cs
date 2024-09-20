using System.Globalization;
using Newtonsoft.Json;

namespace SimpleWeb.Testss;

public class test
{
    [Test]
    public void Check_Expiration_Should_Return_Expected_Judgement_When_Receive_Almost_Expire_Product()
    {
        var products = new List<Product>
        {
            new() { BarCode = "AAAAA2023021523"},
            new() { BarCode = "BBBBB2023021522"},
            new() { BarCode = "CCCCC2023021516"},
            new() { BarCode = "DDDDD2023021520"},
            new() { BarCode = "EEEEE2023021523"}
        };
        CheckExpiration(products);
        Console.WriteLine($"系統時間 {DateTime.Now.ToString("yyyy/MM/dd HH:mm")}");
        foreach (var product in products)
        {
            Console.WriteLine($"{product.BarCode}\t\t{product.Status}");
        }
    }

    private void CheckExpiration(IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            DateTime.TryParse("2023/02/15 22:25", out var now);
            
            var diff = product.ExpireTime - now;
            product.Status = diff switch
            {
                _ when diff.TotalMinutes < 5 => "商品已過期",
                _ when diff.TotalHours < 1 => "商品即將到期",
                _ => product.Status
            };
        }
    }
}

public class Product
{
    public string BarCode { get; set; }
    public DateTime ExpireTime
    {
        get
        {
            DateTime.TryParseExact(
                BarCode.AsSpan(5, 10).ToString(),
                "yyyyMMddHH",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var expire);
            return expire;
        }
    }
    public string Status { get; set; } = "";
}
// Constraints:
// 1hr > time > 5min : `商品即將到期`
// time < 5min : `商品已過期`


/* input
 * 1. 一次輸入一筆條碼做檢查
 * 2. 一次輸入五比條碼做檢查
 */

/* OUTPUT
    系統時間 2023/02/15 22:25 
    EEEEE 2023 02 15 23	      商品即將到期
    Datetime.Now.ToString("yyyy/MM/dd HH:mm") 
    {商品條碼}{Tab*2}{判斷}
*/
