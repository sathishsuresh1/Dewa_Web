using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.WebApi.DMSRequestModels
{
    public class UserModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

    }
    public class UserModelWithRepo : UserModel
    {
        [Required]
        public string DefaultRepo { get; set; }
    }

    public class CoreModel
    {
        [Required]
        public string DMSSessionID { get; set; }
        [Required]
        public string DMSUserID { get; set; }
    }
    public class BaseModel : CoreModel
    {
        [Required]
        public string ObjectId { get; set; }
    }
    public class UpdateAttributeModel : BaseModel
    {

        [Required]
        public string CurrentName { get; set; }
        [Required]
        public string NewName { get; set; }
    }
    public class ChangeTypeModel : BaseModel
    {
        [Required]
        public string CurrentType { get; set; }
        [Required]
        public string NewType { get; set; }

    }
    public class UploadModel : CoreModel
    {
        [Required]
        public string DMSAttributes { get; set; }
        [Required]
        public string DocumentumFolder { get; set; }
        [Required]
        public string DocumentType { get; set; }
        [Required]
        public string FileType { get; set; }
        [Required]
        public Byte[] FileBytesArray { get; set; }
    }

    public class CreateFolderModel : CoreModel
    {
        [Required]
        public string NewFolderPath { get; set; }
    }
    public class DMSFile
    {
        public Byte[] Bytes { get; set; }
        public string Name { get; set; }

    }
    public class SearchInFileModel : CoreModel
    {
        [Required]
        public string DQLTableName { get; set; }
        [Required]
        public string DQLWhere { get; set; }
        [Required]
        public string DQLSelect { get; set; }
    }
}
