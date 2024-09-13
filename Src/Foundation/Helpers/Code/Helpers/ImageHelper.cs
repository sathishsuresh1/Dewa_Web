//using ImageProcessor;
//using ImageProcessor.Imaging;

namespace DEWAXP.Foundation.Helpers
{
    using DEWAXP.Foundation.Integration.Enums;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.WebPages;
    using static DEWAXP.Foundation.Helpers.SystemEnum;

    /// <summary>
    /// Defines the <see cref="ImageHelper" />
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// Defines the ImageMinimumBytes
        /// </summary>
        public const int ImageMinimumBytes = 100000;

        /// <summary>
        /// File Size in Kb
        /// </summary>
        public const int MaxImageOptimizeSize = 250;

        /// <summary>
        /// Defines the MaxImageOptimizeDpiWidth
        /// </summary>
        public const int MaxImageOptimizeDpiWidth = 100;

        /// <summary>
        /// Defines the MaxImageOptimizeDpiHeight
        /// </summary>
        public const int MaxImageOptimizeDpiHeight = 100;

        /// <summary>
        /// ImageMimeDictionary
        /// </summary>
        private static readonly IDictionary<string, string> ImageMimeDictionary = new Dictionary<string, string>
        {
            { ".bmp", "image/bmp" },
            //{ ".dib", "image/bmp" },
            //{ ".gif", "image/gif" },
            //{ ".svg", "image/svg+xml" },
            //{ ".jpe", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".jpg", "image/jpeg" },
            { ".png", "image/png" },
            //{ ".pnz", "image/png" }
        };
        public static string GetAccountImage(BillingClassification billingClassification)
        {
            switch (billingClassification)
            {
                case BillingClassification.Residential:
                    return "/images/accounts/Residential.png";
                case BillingClassification.NonResidential:
                    return "/images/accounts/Business.png";
                case BillingClassification.ElectricVehicle:
                    return "/images/accounts/EV.png";
                default:
                    return "/images/accounts/Customer.png";
            }
        }
        /// <summary>
        /// Validate file is Image with exentions '.bmp','.jpeg','.jpg','.png'
        /// </summary>
        /// <param name="imageValidationRequest"></param>
        /// <returns></returns>
        public static ImageValidationResponse IsImage(ImageValidationRequest imageValidationRequest)
        {
            ImageValidationResponse response = new ImageValidationResponse() { IsValid = true };
            //local variable
            string fileName = imageValidationRequest.FileFullName;

            int imageMinimumBytes = imageValidationRequest.ImageMinimumBytes;
            MemoryStream fileStream = imageValidationRequest.ImageMemoryStream;

            try
            {
                // -------------------------------------------
                //  Check the image mime types && image extension
                //-------------------------------------------
                string extension = Path.GetExtension(fileName);
                if (!ImageMimeDictionary.ContainsKey(extension.ToLower()))
                {
                    response.ErrorMessage += $"{fileName} : only file with extension '.bmp','.jpeg','.jpg','.png' valid.\n";
                }

                if (imageValidationRequest.CheckExtensionOnly)
                {
                    response.IsValid = string.IsNullOrEmpty(response.ErrorMessage);
                    return response;
                }

                long fileLenght = fileStream.Length;

                if (!fileStream.CanRead)
                {
                    response.ErrorMessage += $"{fileName} : File is not readable.\n";
                }

                //------------------------------------------
                //check whether the image size exceeding the limit or not
                //------------------------------------------
                if (fileLenght > imageMinimumBytes && fileLenght <= 0)
                {
                    response.ErrorMessage += $"{fileName} :image size exceeding the limit of 100kb.\n";
                }

                byte[] buffer = new byte[imageMinimumBytes];
                int filestreamread = fileStream.Read(buffer, 0, imageMinimumBytes);
                if (filestreamread == -1)
                {
                    throw new IOException("Error in Reading the file");
                }
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    response.ErrorMessage += "Invalid Image file.\n";
                }

                //using (var mStream = new MemoryStream())
                //{
                //    mStream.Write(fileStream.ToArray(), 0, fileStream.ToArray().Length);
                //    mStream.Seek(0, SeekOrigin.Begin);
                //    Stream myStream = mStream;
                //    if (Image.FromStream(myStream).Size.IsEmpty)
                //    {
                //        response.ErrorMessage += $"{fileName} :Invalid Image file with no size\n";
                //    }
                //    Bitmap bm = new Bitmap(myStream);
                //    if (bm.Size.IsEmpty)
                //    {
                //        response.ErrorMessage += $"{fileName} :Invalid Image file with no size\n";
                //    }
                //}
                //using (Bitmap bitmap = new Bitmap(fileStream))
                //{
                //}
            }
            catch (Exception ex)
            {
                response.ErrorMessage += $"{fileName} :File is not Image file\n : Error : {ex.Message}";
            }

            response.IsValid = string.IsNullOrEmpty(response.ErrorMessage);
            if (response.IsValid && fileStream != null)
            {
                response.Image = fileStream.ToArray();
            }
            return response;
        }

        /// <summary>
        /// The GetImageMimeType
        /// </summary>
        /// <param name="bytes">The bytes<see cref="byte[]"/></param>
        /// <returns>The <see cref="string"/></returns>
        private static string GetImageMimeType(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image image = Image.FromStream(ms);

                if (image.RawFormat.Equals(ImageFormat.Bmp))
                {
                    return "image/bmp";
                }
                if (image.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    return "image/jpeg";
                }
                if (image.RawFormat.Equals(ImageFormat.Png))
                {
                    return "image/png";
                }
                if (image.RawFormat.Equals(ImageFormat.Tiff))
                {
                    return "image/tiff";
                }
                if (image.RawFormat.Equals(ImageFormat.Icon))
                {
                    return "image/ico";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// The MigrateCustomMasterOptimizedImage
        /// </summary>
        /// <param name="request"></param>
        /// <param name="imageInfoList">The imageInfoList<see cref="List{ImageBaseInfo}"/></param>
        /// <returns></returns>
        public static MigratedMasterOptimizedImageResponse MigrateCustomMasterOptimizedImage(CustomOptimizationSetting request, List<ImageBaseInfo> imageInfoList = null)
        {
            MigratedMasterOptimizedImageResponse response = new MigratedMasterOptimizedImageResponse();
            try
            {
                if (string.IsNullOrEmpty(request.SourceFolderPath) ||
                    (!string.IsNullOrEmpty(request.SourceFolderPath) && !Directory.Exists(request.SourceFolderPath)))
                {
                    return response;
                }

                List<string> imageFiles = Directory.GetFiles(request.SourceFolderPath, $"*.*", SearchOption.AllDirectories).ToList();

                List<ImageBaseInfo> needededOptimazation = null;
                if (imageInfoList != null)
                {
                    needededOptimazation = imageInfoList.OrderByDescending(x => x.FileSize).Where(x => x.FileSize > request.MaxSize).ToList();
                }

                foreach (string imagePath in imageFiles)
                {
                    try
                    {
                        ImageValidationResponse imageValidationResponse = null;
                        string _fileName = Path.GetFileName(imagePath);
                        string filesizeerror = string.Empty;
                        if (request.CheckExtentionOnly)
                        {
                            imageValidationResponse = IsImage(new ImageValidationRequest()
                            {
                                FileFullName = _fileName,
                                CheckExtensionOnly = true,
                            });
                        }
                        else
                        {
                            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                            {
                                if (fs.Length > 0 && fs.Length < 5000000)
                                {
                                    byte[] photo = new byte[fs.Length];
                                    int fsread = fs.Read(photo, 0, Convert.ToInt32(fs.Length));
                                    if (fsread == -1)
                                    {
                                        throw new IOException("Error in Reading the file");
                                    }
                                    using (MemoryStream ImageMS = new MemoryStream(photo))
                                    {
                                        imageValidationResponse = IsImage(new ImageValidationRequest()
                                        {
                                            FileFullName = _fileName,
                                            ImageMinimumBytes = ImageMinimumBytes,
                                            ImageMemoryStream = ImageMS,
                                        });
                                    }
                                }
                                else
                                {
                                    filesizeerror = "check the file size";
                                }
                                fs.Close();
                            }
                        }

                        if (imageValidationResponse != null && imageValidationResponse.IsValid)
                        {
                            request.DestinationFolderPath = FileHelper.IsExitOrCreate(request.DestinationFolderPath);

                            string toFile = Path.Combine(request.DestinationFolderPath, Path.GetFileName(imagePath));
                            bool IsMigrated = false;

                            if (!request.CheckBySorting ||
                                request.CheckBySorting && needededOptimazation != null &&
                                needededOptimazation.FirstOrDefault(x => x.FileName == _fileName) != null)
                            {
                                IsMigrated = ImageHelper.OptimizeImageFromPathAndSaveToPath(imagePath, toFile, request.MaxSize);
                            }
                            else
                            {
                                System.IO.File.Copy(imagePath, toFile, true);
                                IsMigrated = true;
                            }

                            if (IsMigrated && CommonAppSetting.EnableArchivedFileDeletion)
                            {
                                FileHelper.IsExitOrDelete(imagePath);
                            }

                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage += filesizeerror;
                        }
                    }
                    catch (Exception innerEx)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage += innerEx.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage += ex.Message;
            }
            return response;
        }

        /// <summary>
        /// The GetDocumentOptimizationSetting
        /// </summary>
        /// <returns>The <see cref="List{OptimizationSetting}"/></returns>
        public static List<OptimizationSetting> GetDocumentOptimizationSetting()
        {
            List<OptimizationSetting> setting = new List<OptimizationSetting>();
            string _from = FileHelper.PathResolver(CommonAppSetting.SourceImgPath);
            string _to = FileHelper.PathResolver(CommonAppSetting.DestinationImgPath);
            setting.Add(new OptimizationSetting()
            {
                AttachmentType = AttachmentType.Profile,
                Width = 150,
                Height = 200,
                SourceFolderPath = _from + AttachmentType.Profile.ToString(),
                DestinationFolderPath = _to + AttachmentType.Profile.ToString(),
                MinSize = 40,
                MaxSize = 50,
            });
            setting.Add(new OptimizationSetting()
            {
                AttachmentType = AttachmentType.DEWAID,
                Width = 880,
                Height = 1250,
                SourceFolderPath = _from + AttachmentType.DEWAID.ToString(),
                DestinationFolderPath = _to + AttachmentType.DEWAID.ToString(),
                MinSize = 100,
                MaxSize = 100,
            });
            setting.Add(new OptimizationSetting()
            {
                AttachmentType = AttachmentType.Passport,
                Width = 880,
                Height = 1250,
                SourceFolderPath = _from + AttachmentType.Passport.ToString(),
                DestinationFolderPath = _to + AttachmentType.Passport.ToString(),
                MinSize = 100,
                MaxSize = 100,
            });
            setting.Add(new OptimizationSetting()
            {
                AttachmentType = AttachmentType.VISA,
                Width = 880,
                Height = 625,
                SourceFolderPath = _from + AttachmentType.VISA.ToString(),
                DestinationFolderPath = _to + AttachmentType.VISA.ToString(),
                MaxSize = 50,
                MinSize = 120,
            });
            setting.Add(new OptimizationSetting()
            {
                AttachmentType = AttachmentType.EID,
                Width = 300,
                Height = 180,
                SourceFolderPath = _from + AttachmentType.EID.ToString(),
                DestinationFolderPath = _to + AttachmentType.EID.ToString(),
                MinSize = 100,
                MaxSize = 130,
            });
            setting.Add(new OptimizationSetting()
            {
                AttachmentType = AttachmentType.Mulkiya,
                Width = 300,
                Height = 180,
                SourceFolderPath = _from + AttachmentType.Mulkiya.ToString(),
                DestinationFolderPath = _to + AttachmentType.Mulkiya.ToString(),
                MinSize = 100,
                MaxSize = 140,
            });
            setting.Add(new OptimizationSetting()
            {
                AttachmentType = AttachmentType.TradingLicense,
                Width = 2500,
                Height = 3510,
                SourceFolderPath = _from + AttachmentType.TradingLicense.ToString(),
                DestinationFolderPath = _to + AttachmentType.TradingLicense.ToString(),
                MinSize = 250,
                MaxSize = 250,
            });
            setting.Add(new OptimizationSetting()
            {
                AttachmentType = AttachmentType.DrivingLicense,
                Width = 600,
                Height = 380,
                SourceFolderPath = _from + AttachmentType.DrivingLicense.ToString(),
                DestinationFolderPath = _to + AttachmentType.DrivingLicense.ToString(),
                MinSize = 50,
                MaxSize = 220,
            });
            return setting;
        }

        /// <summary>
        /// The GetImageInfoFileStream
        /// </summary>
        /// <param name="fileStream">The fileStream<see cref="Stream"/></param>
        /// <param name="path">The path<see cref="string"/></param>
        /// <param name="toPath">The toPath<see cref="string"/></param>
        /// <returns>The <see cref="ImageInfo"/></returns>
        public static ImageInfo GetImageInfoFileStream(Stream fileStream, string path, string toPath)
        {
            ImageInfo imageInfo = new ImageInfo();
            Image img = Image.FromStream(fileStream);
            ImageFormat format = img.RawFormat;
            imageInfo.FullName = Path.GetFileName(path);
            imageInfo.FileFormat = format.ToString();
            imageInfo.Width = img.Width;
            imageInfo.Height = img.Height;
            imageInfo.DpiWidth = img.VerticalResolution;
            imageInfo.DpiHeight = img.HorizontalResolution;
            imageInfo.Resolution = (img.VerticalResolution * img.HorizontalResolution);
            imageInfo.ImagePixelDepth = Image.GetPixelFormatSize(img.PixelFormat);
            imageInfo.FileSize = FileHelper.ConvertMemoryByUnit(fileStream.Length);
            imageInfo.FileLocation = path;
            imageInfo.ToFileLocation = toPath;
            return imageInfo;
        }

        /// <summary>
        /// The OptimizeImage
        /// </summary>
        /// <param name="fromStream">The fromStream<see cref="Stream"/></param>
        /// <param name="toStream">The toStream<see cref="Stream"/></param>
        /// <param name="actualImageInfo">The actualImageInfo<see cref="ImageInfo"/></param>
        /// <param name="maxImageOptimizeSize">The maxImageOptimizeSize<see cref="int"/></param>
        /// <param name="repeatCount">The repeatCount<see cref="float"/></param>
        public static void OptimizeImage(Stream fromStream,
                                        Stream toStream,
                                        ImageInfo actualImageInfo,
                                        int maxImageOptimizeSize = ImageHelper.MaxImageOptimizeSize,
                                        float repeatCount = 0)
        {
            using (Image image = Image.FromStream(fromStream))
            {
                int newWidth = image.Width;
                int newHeight = image.Height;

                //if fileSize is not as per requirement image reduction can be done only till 50 % not more
                if (repeatCount > 0 && repeatCount <= 5)
                {
                    float perCent = (CommonAppSetting.BaseOptimizeUnitPercent * repeatCount / 100);
                    newWidth = (int)(image.Width - (image.Width * perCent));
                    newHeight = (int)(image.Height - (image.Height * perCent));
                }

                using (Bitmap thumbnailBitmap = new Bitmap(newWidth, newHeight))
                {
                    using (Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap))
                    {
                        //validate DPI & File size then optimize the quality
                        if (actualImageInfo != null &
                            (actualImageInfo.FileSize > MaxImageOptimizeSize ||
                            actualImageInfo.DpiHeight > MaxImageOptimizeDpiHeight ||
                            actualImageInfo.DpiWidth > MaxImageOptimizeDpiWidth))
                        {
                            thumbnailGraph.CompositingQuality = CompositingQuality.HighSpeed;
                            thumbnailGraph.SmoothingMode = SmoothingMode.HighSpeed;
                            thumbnailGraph.InterpolationMode = InterpolationMode.NearestNeighbor;
                        }
                        Rectangle imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                        thumbnailGraph.DrawImage(image, imageRectangle);
                        thumbnailBitmap.Save(toStream, image.RawFormat);

                        ImageInfo d = ImageHelper.GetImageInfoFileStream(toStream, actualImageInfo.ToFileLocation, null);
                        if (d.FileSize > maxImageOptimizeSize)
                        {
                            //remove old file stream inorder to Alter content in runtime.
                            toStream.Dispose();
                            toStream.Close();

                            //creating the new temp file with current created | migrated file value
                            string tempDirName = Path.GetDirectoryName(actualImageInfo.ToFileLocation);
                            //Thread.Sleep(1000);
                            string tempFileName = Convert.ToString(Guid.NewGuid()) + Path.GetFileName(actualImageInfo.ToFileLocation);
                            string fromFilePath = Path.GetFullPath(Path.Combine(tempDirName, tempFileName));

                            using (FileStream fromFile = System.IO.File.Create(fromFilePath))
                            {
                                thumbnailBitmap.Save(fromFile, image.RawFormat);

                                using (FileStream toFile = System.IO.File.Create(actualImageInfo.ToFileLocation))
                                {
                                    //reoptimizing the file.
                                    ImageInfo tempFileInfo = ImageHelper.GetImageInfoFileStream(fromFile, fromFilePath, actualImageInfo.ToFileLocation);
                                    ImageHelper.OptimizeImage(fromFile, toFile, tempFileInfo, maxImageOptimizeSize, repeatCount + 1);
                                }
                            }
                            //Delete the temp Optimize file
                            FileHelper.IsExitOrDelete(fromFilePath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The OptimizeImageFromPathAndSaveToPath
        /// </summary>
        /// <param name="fromFilePath">The fromFilePath<see cref="string"/></param>
        /// <param name="toSaveFilePath">The toSaveFilePath<see cref="string"/></param>
        /// <param name="minImageOptimizeSize">The minImageOptimizeSize<see cref="int"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool OptimizeImageFromPathAndSaveToPath(string fromFilePath,
                                                              string toSaveFilePath,
                                                              int minImageOptimizeSize = ImageHelper.MaxImageOptimizeSize)
        {
            try
            {
                //if (System.IO.File.Exists(toSaveFilePath))
                //    System.IO.File.Delete(toSaveFilePath);
                FileHelper.IsExitOrDelete(toSaveFilePath);

                using (FileStream fromFile = System.IO.File.OpenRead(fromFilePath))
                {
                    using (FileStream toFile = System.IO.File.Create(toSaveFilePath))
                    {
                        ImageInfo d = ImageHelper.GetImageInfoFileStream(fromFile, fromFilePath, toSaveFilePath);
                        OptimizeImage(fromFile, toFile, d, minImageOptimizeSize);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Get Document Optimization  Setting
        /// </summary>
        /// <param name="stream">The stream<see cref="Stream"/></param>
        /// <param name="destPath">The destPath<see cref="string"/></param>
        public static void CopyStream(Stream stream, string destPath)
        {
            using (FileStream fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// The ImageInfoFromFile
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/></param>
        /// <returns>The <see cref="ImageInfo"/></returns>
        public static ImageInfo ImageInfoFromFile(string fileName)
        {
            ImageInfo ImageInfo = new ImageInfo
            {
                FileLocation = fileName
            };

            Image img = Image.FromFile(fileName);
            ImageFormat format = img.RawFormat;
            ImageInfo.FullName = Path.GetFileName(fileName);
            ImageInfo.FileFormat = format.ToString();
            ImageInfo.Width = img.Width;
            ImageInfo.Height = img.Height;
            ImageInfo.DpiWidth = img.VerticalResolution;
            ImageInfo.DpiHeight = img.HorizontalResolution;
            ImageInfo.Resolution = (img.VerticalResolution * img.HorizontalResolution);
            ImageInfo.ImagePixelDepth = Image.GetPixelFormatSize(img.PixelFormat);

            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                ImageInfo.FileSize = FileHelper.ConvertMemoryByUnit(file.Length);
            }
            return ImageInfo;
        }

        /// <summary>
        /// Validate Image
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="minBytes"></param>
        /// <param name="checkExtensionOnly"></param>
        /// <returns></returns>
        public static ImageValidationResponse ValidateImageFile(HttpPostedFileBase postedFile, int minBytes = 0, bool checkExtensionOnly = false)
        {
            ImageValidationResponse validationResponse = null;
            if (checkExtensionOnly)
            {
                validationResponse = ImageHelper.IsImage(new ImageValidationRequest()
                {
                    FileFullName = postedFile.FileName,
                    ImageMinimumBytes = minBytes < 0 ? ImageHelper.ImageMinimumBytes : minBytes,
                    CheckExtensionOnly = checkExtensionOnly
                });
            }
            else
            {
                //convert into MemoryStream
                using (var imageMS = new MemoryStream())
                {
                    postedFile.InputStream.CopyTo(imageMS);
                    postedFile.InputStream.Position = 0;
                    validationResponse = ImageHelper.IsImage(new ImageValidationRequest()
                    {
                        FileFullName = postedFile.FileName,
                        ImageMinimumBytes = minBytes < 0 ? ImageHelper.ImageMinimumBytes : minBytes,
                        ImageMemoryStream = imageMS
                    });

                    imageMS.Close();
                    imageMS.Dispose();
                }
            }
            return validationResponse;
        }

        /// <summary>
        /// Upload Images Attachment Or Single
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ImageFileUploaderResponse UploadImagesAttachmentOrSingle(ImageFileUploaderRequest request)
        {
            ImageFileUploaderResponse response = new ImageFileUploaderResponse();

            try
            {
                response.StartDate = DateTime.Now;

                OptimizationSetting imageSetting = ImageHelper.GetDocumentOptimizationSetting()
                       .FirstOrDefault(x => x.AttachmentType == request.AttachmentType);
                string extractPath = imageSetting.SourceFolderPath;
                string ToMigratePath = imageSetting.DestinationFolderPath;
                extractPath = FileHelper.IsExitOrCreate(extractPath);
                ToMigratePath = FileHelper.IsExitOrCreate(ToMigratePath);

                bool IsExtractionCompleted = true;

                if (request.PostedFile != null && request.PostedFile.ContentLength > 0)
                {
                    //if the attachment is Zip File.
                    if (request.PostedFile.FileName.Contains(".zip"))
                    {
                        using (ZipArchive archive = new ZipArchive(request.PostedFile.InputStream))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                ImageValidationResponse validationResponse = null;

                                using (MemoryStream imageMS = new MemoryStream())
                                {
                                    entry.Open().CopyTo(imageMS);
                                    validationResponse = IsImage(new ImageValidationRequest()
                                    {
                                        FileFullName = entry.FullName,
                                        ImageMinimumBytes = ImageHelper.ImageMinimumBytes,
                                        ImageMemoryStream = imageMS
                                    });

                                    imageMS.Close();
                                    imageMS.Dispose();
                                }

                                if (validationResponse.IsValid)
                                {
                                    // Gets the full path to ensure that relative segments are removed.
                                    string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                                    //overwrite if file of same name existed.
                                    FileHelper.IsExitOrDelete(destinationPath);

                                    // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                                    // are case-insensitive.
                                    if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                                    {
                                        entry.ExtractToFile(destinationPath);
                                    }

                                    //var toFilePath = Path.GetFullPath(Path.Combine(ToMigratePath, entry.Name));
                                    //ImageHelper.OptimizeImageFromPathAndSaveToPath(destinationPath, toFilePath);
                                }

                                if (!validationResponse.IsValid)
                                {
                                    IsExtractionCompleted = false;
                                }
                                response.Message += $"\n{validationResponse.ErrorMessage}";
                            }
                        }
                    }
                    else
                    {
                        ImageValidationResponse validationResponse = ValidateImageFile(request.PostedFile, request.MinBytes, request.CheckExtensionOnly);

                        if (validationResponse.IsValid)
                        {
                            // Gets the full path to ensure that relative segments are removed.
                            string destinationPath = Path.GetFullPath(Path.Combine(extractPath, Path.GetFileName(Extensions.StringExtensions.GetSanitizePlainText(request.PostedFile.FileName))));

                            //overwrite if file of same name existed.
                            FileHelper.IsExitOrDelete(destinationPath);

                            // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                            // are case-insensitive.
                            if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                            {
                                request.PostedFile.SaveAs(destinationPath);
                            }

                            //var toFilePath = Path.GetFullPath(Path.Combine(ToMigratePath, request.PostedFile.FileName));

                            //ImageHelper.OptimizeImageFromPathAndSaveToPath(destinationPath, toFilePath);
                            response.SingelFileBytes = File.ReadAllBytes(destinationPath);
                        }

                        IsExtractionCompleted = validationResponse.IsValid;
                        response.Message += $"\n{validationResponse.ErrorMessage}";
                    }
                }

                if (IsExtractionCompleted)
                {
                    CustomOptimizationSetting setting = new CustomOptimizationSetting
                    {
                        AttachmentType = imageSetting.AttachmentType,
                        Width = imageSetting.Width,
                        Height = imageSetting.Height,
                        MinSize = imageSetting.MinSize,
                        MaxSize = imageSetting.MaxSize,
                        SourceFolderPath = imageSetting.SourceFolderPath,
                        DestinationFolderPath = imageSetting.DestinationFolderPath,
                        CheckExtentionOnly = request.CheckExtensionOnly,
                        CheckBySorting = request.CheckBySorting
                    };

                    MigratedMasterOptimizedImageResponse OptimizedImageResponse = ImageHelper.MigrateCustomMasterOptimizedImage(setting);

                    response.IsSucess = OptimizedImageResponse.IsSuccess;
                    response.Message += OptimizedImageResponse.ErrorMessage;
                    response.Message += "\nFile uploaded";
                }

                response.EndDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                response.EndDate = DateTime.Now;
                response.IsSucess = false;
                response.Message += $"\n{ex.Message}";
            }

            return response;
        }
    }

    /// <summary>
    /// Defines the <see cref="ImageValidationRequest" />
    /// </summary>
    public class ImageValidationRequest
    {
        /// <summary>
        /// Gets or sets the FileFullName
        /// </summary>
        public string FileFullName { get; set; }

        /// <summary>
        /// Gets or sets the ImageMemoryStream
        /// </summary>
        public MemoryStream ImageMemoryStream { get; set; }

        /// <summary>
        /// Gets or sets the ImageMinimumBytes
        /// </summary>
        public int ImageMinimumBytes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CheckExtensionOnly
        /// </summary>
        public bool CheckExtensionOnly { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ImageValidationResponse" />
    /// </summary>
    public class ImageValidationResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether IsValid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the ErrorMessage
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the Image
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the ImageMimeType
        /// </summary>
        public string ImageMimeType { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="OptimizationSetting" />
    /// </summary>
    public class OptimizationSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizationSetting"/> class.
        /// </summary>
        public OptimizationSetting()
        {
            MinSize = 40;
            MaxSize = 40;
        }

        /// <summary>
        /// Gets or sets the AttachmentType
        /// </summary>
        public AttachmentType AttachmentType { get; set; }

        /// <summary>
        /// Gets or sets the Width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the Height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the MinSize
        /// in KB
        /// </summary>
        public int MinSize { get; set; }

        /// <summary>
        /// Gets or sets the MaxSize
        /// in KB
        /// </summary>
        public int MaxSize { get; set; }

        /// <summary>
        /// Gets or sets the SourceFolderPath
        /// </summary>
        public string SourceFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the DestinationFolderPath
        /// </summary>
        public string DestinationFolderPath { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="CustomOptimizationSetting" />
    /// </summary>
    public class CustomOptimizationSetting : OptimizationSetting
    {
        /// <summary>
        /// Gets or sets a value indicating whether CheckExtentionOnly
        /// </summary>
        public bool CheckExtentionOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CheckBySorting
        /// </summary>
        public bool CheckBySorting { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ImageInfo" />
    /// </summary>
    public class ImageInfo
    {
        /// <summary>
        /// Gets or sets the FullName
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the Width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the Height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the DpiWidth
        /// </summary>
        public float DpiWidth { get; set; }

        /// <summary>
        /// Gets or sets the DpiHeight
        /// </summary>
        public float DpiHeight { get; set; }

        /// <summary>
        /// Gets or sets the FileFormat
        /// </summary>
        public string FileFormat { get; set; }

        /// <summary>
        /// Gets or sets the FileSize
        /// FileSize is in KB
        /// </summary>
        public decimal FileSize { get; set; }

        /// <summary>
        /// Gets or sets the Resolution
        /// </summary>
        public float Resolution { get; internal set; }

        /// <summary>
        /// Gets or sets the ImagePixelDepth
        /// </summary>
        public int ImagePixelDepth { get; set; }

        /// <summary>
        /// Gets or sets the FileLocation
        /// </summary>
        public string FileLocation { get; set; }

        /// <summary>
        /// Gets or sets the ToFileLocation
        /// </summary>
        public string ToFileLocation { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="MigratedMasterOptimizedImageResponse" />
    /// </summary>
    public class MigratedMasterOptimizedImageResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether IsSuccess
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the ErrorMessage
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ImageBaseInfo" />
    /// </summary>
    public class ImageBaseInfo
    {
        /// <summary>
        /// Gets or sets the FileName
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the FileSize
        /// </summary>
        public decimal FileSize { get; set; }

        /// <summary>
        /// Gets or sets the FileType
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the FileEntriesCount
        /// </summary>
        public string FileEntriesCount { get; set; }

        /// <summary>
        /// Gets or sets the ResponseTime
        /// </summary>
        public string ResponseTime { get; set; }

        /// <summary>
        /// Gets or sets the OptimzeToKB
        /// </summary>
        public string OptimzeToKB { get; set; }

        /// <summary>
        /// Gets or sets the Message
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ImageFileUploaderRequest" />
    /// </summary>
    public class ImageFileUploaderRequest
    {
        /// <summary>
        /// Gets or sets the PostedFile
        /// </summary>
        public HttpPostedFileBase PostedFile { get; set; }

        /// <summary>
        /// Gets or sets the AttachmentType
        /// </summary>
        public AttachmentType AttachmentType { get; set; }

        /// <summary>
        /// Gets or sets the MinBytes
        /// optional
        /// </summary>
        public int MinBytes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CheckExtensionOnly
        /// Optional
        /// Only will validate by file Extension
        /// </summary>
        public bool CheckExtensionOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CheckBySorting
        /// optional
        /// FilterBy the Max size of file & sort it
        /// </summary>
        public bool CheckBySorting { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ImageFileUploaderResponse" />
    /// </summary>
    public class ImageFileUploaderResponse
    {
        /// <summary>
        /// Gets or sets the StartDate
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the EndDate
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSucess
        /// </summary>
        public bool IsSucess { get; set; }

        /// <summary>
        /// Gets or sets the Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the SingelFileBytes
        /// </summary>
        public byte[] SingelFileBytes { get; set; }
    }
}