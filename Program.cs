using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using System.Xml;

using jet.classes;
using System.Web.Mail;
using System.Configuration;
using jet.helper;

namespace jet
{
    class Program
    {

        public static FulfillmentNodes fulfillmentNodes { get; set; }

        public static String publicDirectory = ConfigurationManager.AppSettings["PublicDirectory"];       
        public static String user = ConfigurationManager.AppSettings["UserName"];
        public static String password = ConfigurationManager.AppSettings["Password"];
        public static String emailList = ConfigurationManager.AppSettings["EmailList"];
        public static String emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
        public static String smtp = ConfigurationManager.AppSettings["SmtpServer"];

        static void Main(string[] args)
        {


            BearerToken tokenResponse = new BearerToken();
            String line = String.Empty;
            Int32 value;

            Dictionary<int, string> menuList = new Dictionary<int, string>();
            menuList.Add(1, "Check for open orders");
            menuList.Add(2, "Acknowledge Orders");
            menuList.Add(3, "Add Tracking Number");
            menuList.Add(4, "Download Order XML, acknowledge order");

            Console.WriteLine("-------------------------------------\r\r");
            Console.WriteLine("\rPlease select:\r");
            Console.WriteLine("-------------------------------------\r\r");

            foreach (KeyValuePair<int, string> menuItem in menuList)
            {
                Console.WriteLine("{0}) {1} \r", menuItem.Key, menuItem.Value);
            }
            Console.WriteLine("9) Quit \r");
            Console.WriteLine("\r\r");

            if (args.Length > 0)
            {
                line = args[0];
                Console.WriteLine(args[0]);
            }
            else
            {
                Console.Write(">");
                line = Console.ReadLine();
            }
            if (int.TryParse(line, out value))
            {

                if (menuList.ContainsKey(value))
                {
                    /* Get token object, convert to custom object type Bearer Token */
                    String token = HelperClass.GetToken(user, password);
                    tokenResponse = (BearerToken)JsonConvert.DeserializeObject(token, typeof(BearerToken));
                    Console.WriteLine("Token expires " + tokenResponse.ExpiresOn.ToString());

                    /* Check for authentication */
                    String isAuthenticated = HelperClass.AuthCheck(tokenResponse.Token);
                    Console.WriteLine(isAuthenticated);
                }
                switch (value)
                {
                    case 1:
                        fulfillmentNodes = HelperClass.GetFulfillmentNodes(tokenResponse.Token);
                        Console.WriteLine(String.Format("Retrieved {0} fulfillment nodes.", fulfillmentNodes.fulfillmentNode.Count()));
                        OrderUrlList orderUrls = HelperClass.GetOpenOrders(tokenResponse.Token);

                        Console.WriteLine(String.Format("There are {0} open orders", orderUrls.OrderUrls.Count));
                        foreach (string order_url in orderUrls.OrderUrls)
                        {
                            dynamic order = HelperClass.GetOrderDetail(tokenResponse.Token, order_url);
                            String filename = HelperClass.OrderToXML(publicDirectory, order, fulfillmentNodes);
                        }
                        break;
                    case 2:

                        String order_id = String.Empty;
                        if (args.Length == 2)
                        {
                            order_id = args[1];
                            Console.WriteLine(args[1]);
                        }
                        else
                        {
                            Console.WriteLine("Please enter order id: \r");
                            Console.WriteLine("\r\r");
                            Console.Write(">");
                            order_id = Console.ReadLine();
                        }
                        dynamic orderAck = HelperClass.GetOrderDetail(tokenResponse.Token, "orders/withoutShipmentDetail/" + order_id);
                        String ackResult = HelperClass.AcknowledgeOrder(orderAck, tokenResponse.Token);
                        Console.WriteLine(ackResult);
                        break;
                    case 3:

                        String trackingFilename = string.Empty;
                        if (args.Length == 2)
                        {
                            order_id = args[1];
                            Console.WriteLine(args[1]);
                        }
                        else
                        {
                            Console.WriteLine("Please enter file name: \r");
                            Console.WriteLine("\r\r");
                            Console.Write(">");
                            trackingFilename = Console.ReadLine();
                        }
                        String trackingResult = HelperClass.TrackingUpdate(tokenResponse.Token, trackingFilename, publicDirectory);
                        Console.WriteLine(trackingResult);
                        break;
                    case 4:

                        fulfillmentNodes = HelperClass.GetFulfillmentNodes(tokenResponse.Token);
                        Console.WriteLine(String.Format("Retrieved {0} fulfillment nodes.", fulfillmentNodes.fulfillmentNode.Count()));
                        OrderUrlList orderUrls2 = HelperClass.GetOpenOrders(tokenResponse.Token);

                        Console.WriteLine(String.Format("There are {0} open orders", orderUrls2.OrderUrls.Count));
                        foreach (string order_url in orderUrls2.OrderUrls)
                        {
                            dynamic order = HelperClass.GetOrderDetail(tokenResponse.Token, order_url);
                            Boolean successful = AddJetOrder(order);
                            HelperClass.AcknowledgeOrder(order, tokenResponse.Token);
                        }

                        if (orderUrls2.OrderUrls.Count > 0)
                        {
                            MailMessage mail = new MailMessage();
                            mail.To = emailList;
                            mail.From = emailFrom;
                            mail.Subject = orderUrls2.OrderUrls.Count + " new order(s) on Jet.com";

                            mail.Body = orderUrls2.OrderUrls.Count + " new order(s) on Jet.com";

                            SmtpMail.SmtpServer = smtp;
                            SmtpMail.Send(mail);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        static Boolean AddJetOrder(dynamic order)
        {
            using (var context = new NetsuiteIntegrationEntities())
            {
                try
                {
                    String emailAddress = "";
                    if (!String.IsNullOrWhiteSpace(order.hash_email.Value))
                    {
                        emailAddress = order.hash_email.Value;
                    }

                    Decimal basePrice = Convert.ToDecimal(order.order_totals.item_price.base_price.Value);
                    Decimal shippingCost = Convert.ToDecimal(order.order_totals.item_price.item_shipping_cost.Value);
                    Decimal tax = Convert.ToDecimal(order.order_totals.item_price.item_shipping_tax.Value);
                    Decimal feeAdjustments = 0;
                    if (!String.IsNullOrEmpty(order.order_totals.fee_adjustments))
                        feeAdjustments = Convert.ToDecimal(order.order_totals.fee_adjustments.Value);
                    Decimal grandTotal = basePrice + shippingCost + tax - feeAdjustments;

                    JetOrder jetOrder = new JetOrder
                    {
                        JetOrderId = order.reference_order_id.Value,
                        BillToName = order.buyer.name.Value,
                        BillToAddress1 = order.shipping_to.address.address1.Value,
                        BillToAddress2 = order.shipping_to.address.address2.Value,
                        BillToCity = order.shipping_to.address.city.Value,
                        BillToCountry = "USA",
                        BillToEmail = emailAddress,
                        BillToPhone = order.buyer.phone_number.Value,
                        BillToState = order.shipping_to.address.state.Value,
                        BillToZip = order.shipping_to.address.zip_code.Value,
                        RecipientName = order.shipping_to.recipient.name.Value,
                        ShipToAddress1 = order.shipping_to.address.address1.Value,
                        ShipToAddress2 = order.shipping_to.address.address2.Value,
                        ShipToCity = order.shipping_to.address.city.Value,
                        ShipToCountry = "USA",
                        ShipToEmail = emailAddress,
                        ShipToPhone = order.buyer.phone_number.Value,
                        ShipToState = order.shipping_to.address.state.Value,
                        ShipToZip = order.shipping_to.address.zip_code.Value,
                        Discount = feeAdjustments,
                        GrandTotal = grandTotal,
                        Handling = 0,
                        OrderDateTime = order.order_placed_date,
                        PaymentType = "Credit Card",
                        SubTotal = basePrice,
                        Shipping = shippingCost,
                        Tax = tax,
                        MerchantOrderId = order.merchant_order_id.Value
                    };

                    for (var i = 0; i < ((Newtonsoft.Json.Linq.JContainer)(order.order_items)).Count; i++)
                    {
                        Boolean ret = HelperClass.AddJetOrderLine(order.order_items[i], order.reference_order_id.Value);
                        if (!ret)
                        {
                            throw new Exception("Order Line Failed");
                        }
                    }
                    context.JetOrders.Add(jetOrder);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }
        }
    
    

        private String XMLStringCheck(String txt)
        {
            if (String.IsNullOrEmpty(txt))
                return "";
            else
                return txt.ToString();

        }
    }
}
