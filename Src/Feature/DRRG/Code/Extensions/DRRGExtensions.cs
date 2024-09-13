using DEWAXP.Foundation.CustomDB.DRRGDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.DRRG.Extensions
{
    public static class DRRGExtensions
    {
        public static void Logger(string name, string description, string type, string reference, DateTime date, string userby, string status)
        {
            try
            {
                if (!string.IsNullOrEmpty(reference))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        context.DRRG_ApplicationHistory.Add(new DRRG_ApplicationHistory
                        {
                            Name = !string.IsNullOrWhiteSpace(name) ? name : "Model Name",
                            Reference = reference.Trim(),
                            Status = status,
                            Description = description,
                            Date = date,
                            Type = type,
                            User = userby,
                        });
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}