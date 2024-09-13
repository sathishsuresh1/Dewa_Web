using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DEWAXP.Foundation.Helpers
{
    public static class CompareObject
    {
        public static bool Compare<T>(T e1, T e2)
        {
            bool flag = true;
            bool match = false;
            int countFirst, countSecond;
            foreach (PropertyInfo propObj1 in e1.GetType().GetProperties())
            {
                var propObj2 = e2.GetType().GetProperty(propObj1.Name);
                if (propObj1.PropertyType.Name.Equals("List`1"))
                {
                    dynamic objList1 = propObj1.GetValue(e1, null);
                    dynamic objList2 = propObj2.GetValue(e2, null);
                    countFirst = objList1.Count;
                    countSecond = objList2.Count;
                    if (countFirst == countSecond)
                    {
                        countFirst = objList1.Count - 1;
                        while (countFirst > -1)
                        {
                            match = false;
                            countSecond = objList2.Count - 1;
                            while (countSecond > -1)
                            {
                                match = Compare(objList1[countFirst], objList2[countSecond]);
                                if (match)
                                {
                                    objList2.Remove(objList2[countSecond]);
                                    countSecond = -1;
                                    match = true;
                                }
                                if (match == false && countSecond == 0)
                                {
                                    return false;
                                }
                                countSecond--;
                            }
                            countFirst--;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (propObj1.PropertyType.Name.Equals("String[]"))
                {
                    string[] objList1 =(string[])propObj1.GetValue(e1, null);
                    string[] objList2 = (string[])propObj2.GetValue(e2, null);
                    var comparelist = objList1.ToList().Except(objList2.ToList());
                    var comparelist2 = objList2.ToList().Except(objList1.ToList());
                    if (comparelist.Any() || comparelist2.Any())
                    {
                        flag = false;
                        return flag;
                    }
                }
                else if (propObj1.GetValue(e1) == null || propObj2.GetValue(e1) == null)
                {
                    if ((propObj1.GetValue(e1) == null && propObj2.GetValue(e1) != null) || (propObj1.GetValue(e1) != null && propObj2.GetValue(e1) == null))
                    {
                        flag = false;
                        return flag;
                    }
                }
                else if (!(propObj1.GetValue(e1, null).Equals(propObj2.GetValue(e2, null))))
                {
                    flag = false;
                    return flag;
                }
            }
            return flag;
        }
    }
}