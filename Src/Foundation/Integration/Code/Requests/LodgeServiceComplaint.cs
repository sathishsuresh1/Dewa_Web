namespace DEWAXP.Foundation.Integration.Requests
{
    public class LodgeServiceComplaint
    {
        public LodgeServiceComplaint()
        {
            DocumentData = new byte[0];
        }

        public string MobileNumber{ get; set; }

        public string Name{ get; set; }

        public string City{ get; set; }

        public string ContractAccountNumber{ get; set; }

        public string CodeGroup{ get; set; }

        public string Code{ get; set; }
        
        public byte[] DocumentData{ get; set; }

        public string GpsXCoordinates{ get; set; }

        public string GpsYCoordinates{ get; set; }

        public string Remarks{ get; set; }

    }
}
