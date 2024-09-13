using System.Collections.Generic;

namespace DEWAXP.Feature.ScrapSale.Helpers
{
    public class CustomFileUtility
    {
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

                if (ms != null && ms.Length > 0)
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
}