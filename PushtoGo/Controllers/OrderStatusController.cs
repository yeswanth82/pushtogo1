using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using PushtoGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PushtoGo.Controllers
{
    [Authorize]
    public class OrderStatusController : Controller
    {
        private AmazonDynamoDBClient dynamoClient;
        List<string> activeOrder;

        // GET: OrderStatus
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetPage()
        {
            OrderInformation orderInformation = new OrderInformation();
            orderInformation.lstorder = new List<string>();

            ReadActiveOrders();
            foreach (var order in activeOrder)
              orderInformation.lstorder.Add(order);
           
            orderInformation.userid = "For Display";
            
            return View("Index",orderInformation);
        }

        public void ReadActiveOrders()
        {
            activeOrder = new List<string>();
            try
            {
                Console.WriteLine("Read Password");
                dynamoClient = new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAJOMD6KZKIDIWR3GA",
                    "oJNTQDc647jLVVNC/fgYbIwzf1GnznroAeIE3gaN"),
                    RegionEndpoint.GetBySystemName("us-east-1")
                    );
                Table table = Table.LoadTable(dynamoClient, "ORDER_TABLE0");

                ScanFilter scanFilter = new ScanFilter();
                scanFilter.AddCondition("STATUS", ScanOperator.Equal, "ACTIVE");
                Search search = table.Scan(scanFilter);

                List<Document> documentList = new List<Document>();

                do
                {
                    documentList = search.GetNextSet();

                    foreach (var document in documentList)
                    {
                        if(document["STATUS"].ToString() =="ACTIVE" && document["HOTEL_ID"].ToString() == Session["Hotleid"].ToString())
                        { 
                        activeOrder.Add(document["ROOM_NO"].ToString()+" "+ document["ORDER_ID"].ToString());
                        }
                    }
                       
                } while (!search.IsDone);

            }
            catch (Exception exception)
            {

            }
        }
    }
}