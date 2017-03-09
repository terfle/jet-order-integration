using jet.classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace jet.helper
{
    class HelperClass
    {
        internal static Boolean AddJetOrderLine(dynamic orderLine, String jetOrderId)
        {
            using (var context = new NetsuiteIntegrationEntities())
            {
                try
                {
                    JetOrderLine jetOrderLine = new JetOrderLine
                    {
                        JetOrderId = jetOrderId,
                        JetOrderLineId = orderLine.order_item_id.Value,
                        Price = Convert.ToDecimal(orderLine.item_price.base_price.Value),
                        Sku = orderLine.merchant_sku.Value,
                        ProductName = orderLine.product_title.Value,
                        Quantity = Convert.ToString(orderLine.request_order_quantity.Value)
                    };

                    context.JetOrderLines.Add(jetOrderLine);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    return false;
                }
            }
            return true;
        }



        internal static String OrderToXML(string publicDirectory, dynamic order, FulfillmentNodes fulfillmentNodes)
        {


            String fileType = ".xml";
            String fileName = "jetOrder";
            Random random = new Random();
            String uniqueNum = String.Format("_{0}", random.Next(0, 100000));

            String fileDir = publicDirectory + fileName + order.reference_order_id + uniqueNum + fileType;

            XmlTextWriter writer = new XmlTextWriter(fileDir, null);

            writer.WriteStartDocument();
            writer.WriteComment("Jet order");

            writer.WriteStartElement("orders"); // <orders>
            writer.WriteStartElement("order"); // <order>

            writer.WriteStartElement("fulfilment");
            if (order.fulfillment_node == fulfillmentNodes.fulfillmentNode[0].fulfillmentNodeId)
            {
                writer.WriteString(fulfillmentNodes.fulfillmentNode[0].locationName);
            }
            else
            {
                writer.WriteString(fulfillmentNodes.fulfillmentNode[1].locationName);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("ponumber");
            writer.WriteString("");
            writer.WriteEndElement();

            writer.WriteStartElement("customerid");
            writer.WriteString("");
            writer.WriteEndElement();

            writer.WriteStartElement("number");
            writer.WriteString(order.reference_order_id.Value);
            writer.WriteEndElement();

            //tranid
            writer.WriteStartElement("tranid", "");
            writer.WriteString("");
            writer.WriteEndElement();

            writer.WriteStartElement("source", "");
            writer.WriteString("Jet");
            writer.WriteEndElement();

            writer.WriteStartElement("internalid", "");
            writer.WriteString(order.merchant_order_id.Value);
            writer.WriteEndElement();
            DateTime order_placed_date = DateTime.Parse(order.order_placed_date.Value.ToString());

            writer.WriteStartElement("date", "");
            writer.WriteString(order_placed_date.Date.ToShortDateString());
            writer.WriteEndElement();

            writer.WriteStartElement("time", "");
            writer.WriteString(order_placed_date.TimeOfDay.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("salesrep", "");
            writer.WriteString("");
            writer.WriteEndElement();

            writer.WriteStartElement("contact", " "); // <contact type="SHIP TO">
            writer.WriteAttributeString("type", "SHIP TO");

            writer.WriteStartElement("name", " ");
            writer.WriteString(order.shipping_to.recipient.name.Value);
            writer.WriteEndElement();

            String emailAddress = "";
            if (!String.IsNullOrWhiteSpace(order.hash_email.Value))
            {
                emailAddress = order.hash_email.Value;
            }

            writer.WriteStartElement("email", " ");
            writer.WriteString(emailAddress);
            writer.WriteEndElement();

            writer.WriteStartElement("phone", " ");
            writer.WriteString(order.buyer.phone_number.Value);

            writer.WriteEndElement();

            writer.WriteStartElement("address1", " ");
            writer.WriteString(order.shipping_to.address.address1.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("address2", " ");
            writer.WriteString(order.shipping_to.address.address2.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("city", " ");
            writer.WriteString(order.shipping_to.address.city.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("state", " ");
            writer.WriteString(order.shipping_to.address.state.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("zip", " ");
            writer.WriteString(order.shipping_to.address.zip_code.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("country", " ");
            writer.WriteString("USA");
            writer.WriteEndElement();

            writer.WriteEndElement(); // </contact>

            writer.WriteStartElement("contact", " "); // <contact type="BILL TO">
            writer.WriteAttributeString("type", "BILL TO");

            writer.WriteStartElement("name", " ");
            writer.WriteString(order.buyer.name.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("email", " ");
            writer.WriteString(emailAddress);
            writer.WriteEndElement();

            writer.WriteStartElement("phone", " ");
            writer.WriteString(order.buyer.phone_number.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("address1", " ");
            writer.WriteString(order.shipping_to.address.address1.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("address2", " ");
            writer.WriteString(order.shipping_to.address.address2.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("city", " ");
            writer.WriteString(order.shipping_to.address.city.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("state", " ");
            writer.WriteString(order.shipping_to.address.state.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("zip", " ");
            writer.WriteString(order.shipping_to.address.zip_code.Value);
            writer.WriteEndElement();

            writer.WriteStartElement("country", " ");
            writer.WriteString("USA");
            writer.WriteEndElement();

            writer.WriteEndElement(); // </contact>

            writer.WriteStartElement("lines", " ");
            for (var i = 0; i < ((Newtonsoft.Json.Linq.JContainer)(order.order_items)).Count; i++)
            {
                writer.WriteStartElement("line", " ");
                writer.WriteAttributeString("id", order.order_items[i].order_item_id.Value);

                writer.WriteStartElement("sku", " ");
                writer.WriteString(order.order_items[i].merchant_sku.Value);
                writer.WriteEndElement();

                writer.WriteStartElement("productname", " ");
                writer.WriteString(order.order_items[i].product_title.Value);
                writer.WriteEndElement();

                writer.WriteStartElement("quantity", " ");
                writer.WriteString(order.order_items[i].request_order_quantity.Value.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("price", " ");
                writer.WriteString(order.order_items[i].item_price.base_price.Value.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement(); // </line>
            }
            writer.WriteEndElement(); // </lines>

            writer.WriteStartElement("amounts", " "); // <amounts>

            Decimal basePrice = Convert.ToDecimal(order.order_totals.item_price.base_price.Value);
            Decimal shippingCost = Convert.ToDecimal(order.order_totals.item_price.item_shipping_cost.Value);
            Decimal tax = Convert.ToDecimal(order.order_totals.item_price.item_shipping_tax.Value);
            Decimal feeAdjustments = 0;
            if (!String.IsNullOrEmpty(order.order_totals.fee_adjustments))
                feeAdjustments = Convert.ToDecimal(order.order_totals.fee_adjustments.Value);
            Decimal grandTotal = basePrice + shippingCost + tax - feeAdjustments;

            writer.WriteStartElement("subtotal", " ");
            writer.WriteString(basePrice.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("shipping", " ");
            writer.WriteString(shippingCost.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("handling", " ");
            writer.WriteString("0.00");
            writer.WriteEndElement();

            writer.WriteStartElement("tax", " ");
            writer.WriteString(tax.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("discount", " ");
            writer.WriteString(feeAdjustments.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("grandtotal", " ");
            writer.WriteString(grandTotal.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("payment-type", " ");
            writer.WriteString("Credit Card");
            writer.WriteEndElement();

            writer.WriteEndElement(); // </amounts>
            writer.WriteEndElement(); // </order>    
            writer.WriteEndElement(); // </orders>

            writer.Flush();
            writer.Close();

            return fileDir;

        }
        internal static string AuthCheck(string token)
        {

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync("https://merchant-api.jet.com/api/authcheck").Result.Content.ReadAsStringAsync().Result;
                return response.ToString();
            }
        }
        internal static string GetToken(string userName, string password)
        {

            StringContent stringContent = new StringContent("{ \"user\": \"" + userName + "\", \"pass\": \"" + password + "\"}", UnicodeEncoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var response = client.PostAsync("https://merchant-api.jet.com/api/Token", stringContent).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        internal static string AcknowledgeOrder(dynamic order, string token)
        {
            List<String> order_item_acknowledgement = new List<String>();
            for (var i = 0; i < ((Newtonsoft.Json.Linq.JContainer)(order.order_items)).Count; i++)
            {

                String temp = "{" +
                                    "\"order_item_id\": \"" + order.order_items[i].order_item_id.Value + "\"," +
                                    "\"order_item_acknowledgement_status\": \"fulfillable\"," +
                                    "\"alt_order_item_id\": \"" + order.order_items[i].merchant_sku.Value + "\"" +
                                "}";
                order_item_acknowledgement.Add(temp);
            }
            StringContent sc = new StringContent("{\"acknowledgement_status\": \"accepted\", \"order_items\": [" + String.Join(",", order_item_acknowledgement) + "]}");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.PutAsync("https://merchant-api.jet.com/api/orders/" + order.reference_order_id.Value + "/acknowledge", sc).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        internal static string TrackingUpdate(String token, String filename, string publicDirectory)
        {
            String order_id = filename.Replace(".json", "");
            String absoluteFileName = publicDirectory + filename;

            String data = String.Empty;
            Shipments shipments = new Shipments();
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(absoluteFileName))
                {
                    // Read the stream to a string, and write the string to the console.
                    data = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return "The file could not be read:" + e.Message;
            }
            shipments = (Shipments)JsonConvert.DeserializeObject(data, typeof(Shipments));

            List<String> order_item_acknowledgement = new List<String>();
            StringContent sc = new StringContent(data);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.PutAsync("https://merchant-api.jet.com/api/orders/" + order_id + "/shipped", sc).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        internal static dynamic GetOrderDetail(String token, String order_url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync("https://merchant-api.jet.com/api/" + order_url).Result.Content.ReadAsStringAsync().Result;
                dynamic order = JsonConvert.DeserializeObject(response);
                return order;
            }
        }
        internal static OrderUrlList GetOpenOrders(String token)
        {
            OrderUrlList orderUrls = new OrderUrlList();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync("https://merchant-api.jet.com/api/orders/ready").Result.Content.ReadAsStringAsync().Result;
                var order_urls = JsonConvert.DeserializeObject(response);
                orderUrls = (OrderUrlList)JsonConvert.DeserializeObject(response, typeof(OrderUrlList));
            }
            return orderUrls;
        }
        internal static FulfillmentNodes GetFulfillmentNodes(String token)
        {
            FulfillmentNodes fulfillmentNodes = new FulfillmentNodes();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync("https://merchant-api.jet.com/api/setup/fulfillmentNodes").Result.Content.ReadAsStringAsync().Result;
                fulfillmentNodes = (FulfillmentNodes)JsonConvert.DeserializeObject(response, typeof(FulfillmentNodes));
            }

            return fulfillmentNodes;
        }
    }

}