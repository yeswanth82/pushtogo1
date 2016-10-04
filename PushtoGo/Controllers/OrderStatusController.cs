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
            return View("Index");
        }

        public ActionResult ActiveOrders(string categoryId)
        {
            OrderInformation orderInformation = new OrderInformation();
            orderInformation.lstorder = new List<string>();
            

          //  ReadOrders(categoryId);
           // foreach (var order in activeOrder)
             //   orderInformation.lstorder.Add(order);

            orderInformation.lstorder.Add(DateTime.Now.ToString()+ DateTime.Now.ToString()+ DateTime.Now.ToString());

            return View(orderInformation);
        }
        public void ReadOrders(string orderstatus)
        {
            activeOrder = new List<string>();
            try
            {
                
                dynamoClient = new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAJOMD6KZKIDIWR3GA",
                    "oJNTQDc647jLVVNC/fgYbIwzf1GnznroAeIE3gaN"),
                    RegionEndpoint.GetBySystemName("us-east-1")
                    );
                Table table = Table.LoadTable(dynamoClient, "ORDER_TABLE0");

                ScanFilter scanFilter = new ScanFilter();
                scanFilter.AddCondition("STATUS", ScanOperator.Equal, orderstatus);
                Search search = table.Scan(scanFilter);

                List<Document> documentList = new List<Document>();

                do
                {
                    documentList = search.GetNextSet();

                    foreach (var document in documentList)
                    {
                        if(document["STATUS"].ToString() == orderstatus && document["HOTEL_ID"].ToString() == Session["Hotleid"].ToString())
                        { 
                        activeOrder.Add(document["ROOM_NO"].ToString()+" "+ document["ORDER_ID"].ToString() +" " + document["CREATE_TIMESTAMP"].ToString());
                            
                        }
                    }
                       
                } while (!search.IsDone);

            }
            catch (Exception exception)
            {

            }
        }

        public void UpdateOrders(string orderid, string status)
        {
            string timestamp = DateTime.Now.ToString();
            switch(status)
                {
                case "ACTIVE":
                    status = "IN-ROUTE";
                    break;
                case "IN-ROUTE":
                    status = "COMPLETED";
                    break;
                //case "COMPLETED":
                //    status = "CLOSED";
                //    break;

            }
            try
            {
                dynamoClient = new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAJOMD6KZKIDIWR3GA",
                    "oJNTQDc647jLVVNC/fgYbIwzf1GnznroAeIE3gaN"),
                    RegionEndpoint.GetBySystemName("us-east-1")
                    );
                Table table = Table.LoadTable(dynamoClient, "ORDER_TABLE0");

                ScanFilter scanFilter = new ScanFilter();
                scanFilter.AddCondition("HOTEL_ID", ScanOperator.NotEqual, "0");

                Search search = table.Scan(scanFilter);
                List<Document> documentList = new List<Document>();
                do
                {
                    documentList = search.GetNextSet();
                    foreach (var document in documentList)
                    {
                        document["STATUS"] = status;
                        document["CREATE_TIMESTAMP"] = timestamp;

                        if (document["ORDER_ID"].AsPrimitive().Value.ToString() == orderid.ToString())
                        {
                            Expression expr = new Expression();
                            expr.ExpressionStatement = "ORDER_ID = :val";
                            expr.ExpressionAttributeValues[":val"] = orderid;
                            // Optional parameters.
                            UpdateItemOperationConfig config = new UpdateItemOperationConfig
                            {
                                ConditionalExpression = expr,
                                ReturnValues = ReturnValues.AllNewAttributes
                            };
                            // table.TryUpdateItem(document, config);
                             table.UpdateItem(document, config);
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