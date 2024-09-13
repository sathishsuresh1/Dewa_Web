namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar
{
    /// <summary>
    /// 
    /// </summary>
    public class MasarChangePasswordRequest
    {
        public MasarChangePasswordRequest() { changepwdinput = new Changepwdinput(); }
        public Changepwdinput changepwdinput { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Changepwdinput
    {
        public string oldpassword { get; set; }
        public string lang { get; set; }
        public string userid { get; set; }
        public string sessionid { get; set; }
        public string type { get; set; }
        public string newpassword { get; set; }
        public string vendorid { get; set; }
    }
}
