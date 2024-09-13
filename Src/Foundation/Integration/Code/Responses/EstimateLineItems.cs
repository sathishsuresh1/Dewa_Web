
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class EstimateLineItems
{

    private byte responseCodeField;

    private string descriptionField;

    private EstimateLineItemsEstimateItem[] estimateItemField;

    /// <remarks/>
    public byte ResponseCode
    {
        get
        {
            return this.responseCodeField;
        }
        set
        {
            this.responseCodeField = value;
        }
    }

    /// <remarks/>
    public string Description
    {
        get
        {
            return this.descriptionField;
        }
        set
        {
            this.descriptionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("EstimateItem")]
    public EstimateLineItemsEstimateItem[] EstimateItem
    {
        get
        {
            return this.estimateItemField;
        }
        set
        {
            this.estimateItemField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class EstimateLineItemsEstimateItem
{


    //private uint sales_Distribution_DocField;
    private string sales_Distribution_DocField;

    private string sales_Document_TypeField;

    //private uint sold_To_PartyField;
    private string sold_To_PartyField;

    private string customer_PO_NumberField;

    //private uint cA_NumberField;
    private string cA_NumberField;

    private string cityField;

    private decimal netValue1Field;

    private decimal netValue2Field;

    private decimal netValue3Field;

    private decimal PartialPaymentField;

    //private uint ownerNumField;
    private string ownerNumField;

    private string ownerNameField;

    //private uint estimateNoField;
    private string estimateNoField;

    private System.DateTime estimateValidFromDateField;

    private System.DateTime estimateValidToDateField;

    private string commentField;

    //private ushort plotField;
    private string plotField;

    private string areaField;

    //private uint consultantNoField;
    private string consultantNoField;

    private string con_NameField;

    private string con_EMailField;

    /// <remarks/>
    //public uint Sales_Distribution_Doc
    public string Sales_Distribution_Doc
    {
        get
        {
            return this.sales_Distribution_DocField;
        }
        set
        {
            this.sales_Distribution_DocField = value;
        }
    }

    /// <remarks/>
    public string Sales_Document_Type
    {
        get
        {
            return this.sales_Document_TypeField;
        }
        set
        {
            this.sales_Document_TypeField = value;
        }
    }

    /// <remarks/>
    //public uint Sold_To_Party
    public string Sold_To_Party
    {
        get
        {
            return this.sold_To_PartyField;
        }
        set
        {
            this.sold_To_PartyField = value;
        }
    }

    /// <remarks/>
    public string Customer_PO_Number
    {
        get
        {
            return this.customer_PO_NumberField;
        }
        set
        {
            this.customer_PO_NumberField = value;
        }
    }

    /// <remarks/>
    //public uint CA_Number
    public string CA_Number
    {
        get
        {
            return this.cA_NumberField;
        }
        set
        {
            this.cA_NumberField = value;
        }
    }

    /// <remarks/>
    public string City
    {
        get
        {
            return this.cityField;
        }
        set
        {
            this.cityField = value;
        }
    }

    /// <remarks/>
    public decimal NetValue1
    {
        get
        {
            return this.netValue1Field;
        }
        set
        {
            this.netValue1Field = value;
        }
    }

    /// <remarks/>
    public decimal NetValue2
    {
        get
        {
            return this.netValue2Field;
        }
        set
        {
            this.netValue2Field = value;
        }
    }

    /// <remarks/>
    public decimal NetValue3
    {
        get
        {
            return this.netValue3Field;
        }
        set
        {
            this.netValue3Field = value;
        }
    }

    /// <remarks/>
    public decimal PartialPayment
    {
        get
        {
            return this.PartialPaymentField;
        }
        set
        {
            this.PartialPaymentField = value;
        }
    }

    /// <remarks/>
    //public uint OwnerNum
    public string OwnerNum
    {
        get
        {
            return this.ownerNumField;
        }
        set
        {
            this.ownerNumField = value;
        }
    }

    /// <remarks/>
    public string OwnerName
    {
        get
        {
            return this.ownerNameField;
        }
        set
        {
            this.ownerNameField = value;
        }
    }

    /// <remarks/>
    public string EstimateNo
    {
        get
        {
            return this.estimateNoField;
        }
        set
        {
            this.estimateNoField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
    public System.DateTime EstimateValidFromDate
    {
        get
        {
            return this.estimateValidFromDateField;
        }
        set
        {
            this.estimateValidFromDateField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
    public System.DateTime EstimateValidToDate
    {
        get
        {
            return this.estimateValidToDateField;
        }
        set
        {
            this.estimateValidToDateField = value;
        }
    }

    /// <remarks/>
    public string Comment
    {
        get
        {
            return this.commentField;
        }
        set
        {
            this.commentField = value;
        }
    }

    /// <remarks/>
    //public ushort Plot
    public string Plot
    {
        get
        {
            return this.plotField;
        }
        set
        {
            this.plotField = value;
        }
    }

    /// <remarks/>
    public string Area
    {
        get
        {
            return this.areaField;
        }
        set
        {
            this.areaField = value;
        }
    }

    /// <remarks/>
    //public uint ConsultantNo
    public string ConsultantNo
    {
        get
        {
            return this.consultantNoField;
        }
        set
        {
            this.consultantNoField = value;
        }
    }

    /// <remarks/>
    public string Con_Name
    {
        get
        {
            return this.con_NameField;
        }
        set
        {
            this.con_NameField = value;
        }
    }

    /// <remarks/>
    public string Con_EMail
    {
        get
        {
            return this.con_EMailField;
        }
        set
        {
            this.con_EMailField = value;
        }
    }
}

