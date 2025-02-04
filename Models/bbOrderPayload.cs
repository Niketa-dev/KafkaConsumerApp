using System;

public class bbOrderPayload
{
    public string OrderKey { get; set; }
    public string SubmittedDate { get; set; }
    public string SourceSystem { get; set; }
    public string ReferenceText { get; set; }
    public decimal TotalOrderValue { get; set; }
    public List<OrderLineItem> LineItems { get; set; }
}