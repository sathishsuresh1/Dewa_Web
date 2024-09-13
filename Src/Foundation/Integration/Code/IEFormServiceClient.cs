using DEWAXP.Foundation.Integration.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration
{
    public interface IEFormServiceClient
    {
        ServiceResponse<string> SubmitNewForm(string formXml);
        ServiceResponse<System.Data.DataSet> Query_Ework_DB(string sql);
		
        ServiceResponse<string> SubmitNewForm(string formXml, string mapName, string mapAction);

        ServiceResponse<string> UpdateForm(string eFolderID, string formXml, string mapName, string mapAction);

		//Service to Support old BPM System
		ServiceResponse<string> SubmitNewFormBPM(string formXml, string mapName, string actionName, string formName, int mapVersion);

		ServiceResponse<System.Data.DataSet> Query_Ework_DB_BPM(string sql);
    }
}
