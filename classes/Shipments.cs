using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace jet.classes
{
    public class Shipments
    {
        [JsonProperty("shipments")]
        public Shipment[] shipments { get; set; }
    }
    public class Shipment
    {
        [JsonProperty("shipment_tracking_number")]
        public string shipmentTrackingNumber { get; set; }

        [JsonProperty("response_shipment_date")]
        public DateTime responseShipmentDate { get; set; }

        [JsonProperty("response_shipment_method")]
        public string responseShipmentMethod { get; set; }

        [JsonProperty("expected_delivery_date")]
        public DateTime expectDeliveryDate { get; set; }

        [JsonProperty("carrier")]
        public string carrier { get; set; }

        [JsonProperty("shipment_items")]
        public ShipmentItems[] shipmentItems { get; set; }
    }
    public class ShipmentItems
    {
        [JsonProperty("merchant_sku")]
        public string merchantSku { get; set; }

        [JsonProperty("response_shipment_sku_quantity")]
        public string responseShipmentSkuQuantity { get; set; }
    }    
}
