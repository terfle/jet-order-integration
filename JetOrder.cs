//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace jet
{
    using System;
    using System.Collections.Generic;
    
    public partial class JetOrder
    {
        public string JetOrderId { get; set; }
        public string MerchantOrderId { get; set; }
        public System.DateTime OrderDateTime { get; set; }
        public string RecipientName { get; set; }
        public string ShipToEmail { get; set; }
        public string ShipToPhone { get; set; }
        public string ShipToAddress1 { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToState { get; set; }
        public string ShipToZip { get; set; }
        public string ShipToCountry { get; set; }
        public string BillToName { get; set; }
        public string BillToEmail { get; set; }
        public string BillToAddress1 { get; set; }
        public string BillToAddress2 { get; set; }
        public string BillToCity { get; set; }
        public string BillToState { get; set; }
        public string BillToZip { get; set; }
        public string BillToCountry { get; set; }
        public string BillToPhone { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> Shipping { get; set; }
        public Nullable<decimal> Handling { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public string PaymentType { get; set; }
    }
}