using DEWA.Website.Helpers;
using DEWA.Website.Models.SmartResponseModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using _consumptionComplaint = DEWA.Website.Models.ConsumptionComplaint;

namespace DEWAXP.Foundation.Content.Utils
{
    public class CustomFileUtility
    {
        public static T LoadJson<T>(string filePath, bool isVirtualPath = false)
        {
            T items;
            if (isVirtualPath)
            {
                filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
            }

            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                items = CustomJsonConvertor.DeserializeObject<T>(json);

            }
            return items;
        }


        public static JsonMasterModel LoadSmartDubaiModelJson(string filePath, bool isVirtualPath = false)
        {
            JsonMasterModel items;

            if (string.IsNullOrWhiteSpace(SmartResponse.SessionHelper.SmartResponseJsonText))
            {

                if (isVirtualPath)
                {
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
                }
                using (StreamReader r = new StreamReader(filePath))
                {
                    SmartResponse.SessionHelper.SmartResponseJsonText = r.ReadToEnd();
                    items = JsonMasterModel.FromJson(SmartResponse.SessionHelper.SmartResponseJsonText);
                }
            }
            else
            {
                items = JsonMasterModel.FromJson(SmartResponse.SessionHelper.SmartResponseJsonText);
            }
            return items;
        }

        /// <summary>
        /// Download Multiple Files by Zip Archiving
        /// </summary>
        /// <param name="zipName"></param>
        /// <param name="fileDetails"></param>
        /// <returns></returns>
        public static CustomFileDetail DownloadMultipleFiles(string zipName, List<CustomFileDetail> fileDetails)
        {
            CustomFileDetail data = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (var archive = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    foreach (var file in fileDetails)
                    {
                        var entry = archive.CreateEntry(file.FileNameWithExtension, System.IO.Compression.CompressionLevel.Fastest);
                        using (var zipStream = entry.Open())
                        {
                            zipStream.Write(file.FileBytes, 0, file.FileBytes.Length);
                        }
                    }
                }

                if(ms!=null && ms.Length>0)
                {
                    data = new CustomFileDetail()
                    {
                        FileNameWithExtension = $"{zipName}.zip",
                        MimeTypeExtension = "application/zip",
                        FileBytes = ms.ToArray(),
                    };
                }
                //return File(ms.ToArray(), "application/zip", $"{zipName}.zip");
            }

            return data;
        }

        public static _consumptionComplaint.JsonMasterModel LoadConsumptionComplaintJson(string filePath, bool isVirtualPath = false)
        {
            _consumptionComplaint.JsonMasterModel items;

            if (string.IsNullOrWhiteSpace(ConsumptionComplaint.SessionHelper.CC_JsonText))
            {

                if (isVirtualPath)
                {
                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);
                }
                using (StreamReader r = new StreamReader(filePath))
                {
                    ConsumptionComplaint.SessionHelper.CC_JsonText = r.ReadToEnd();
                    items = _consumptionComplaint.JsonMasterModel.FromJson(ConsumptionComplaint.SessionHelper.CC_JsonText);
                }
            }
            else
            {
                items = _consumptionComplaint.JsonMasterModel.FromJson(ConsumptionComplaint.SessionHelper.CC_JsonText);
            }
            return items;
        }

    }
}

/// <summary>
/// file basic details
/// </summary>
public class CustomFileDetail
{

    public string FileNameWithExtension { get; set; }

    public byte[] FileBytes { get; set; }

    public string MimeTypeExtension { get; set; }
    public string AttachmentNumber { get; set; }
    public string DocumentNumber { get; set; }
}