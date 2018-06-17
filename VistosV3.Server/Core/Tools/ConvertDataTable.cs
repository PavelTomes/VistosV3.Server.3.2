using System;
using System.Data;

namespace Core.Tools
{
    public static  class ConvertDataTable
    {

        public static void ConvertToDate(this DataColumn column,String userCulture)
        {
            foreach (DataRow row in column.Table.Rows)
            {
                DateTime d;
                if(row[column] != DBNull.Value)
                {
                    switch (userCulture)
                    {
                        case "cs-CZ": row[column] = (DateTime.TryParse((string)row[column], out d) ? d.ToString("dd.MM.yyy hh:ss") : row[column]);
                            break;
                        case "en-US":
                            row[column] = (DateTime.TryParse((string)row[column], out d) ? d.ToString("MM/dd/yyyy hh:mm") : row[column]);
                            break;
                    }
                    
                }
            }
        }

        public static void Convert(this DataColumn column, String userCulture)
        {
            foreach (DataRow row in column.Table.Rows)
            {
                if (row[column] != DBNull.Value)
                {
                    switch (userCulture)
                    {
                        case "cs-CZ":
                            row[column] = row[column].ToString() == "True" ? "Ano" : "Ne";
                            break;
                        case "en-US":
                            row[column] = row[column].ToString() == "True" ? "Yes" : "No";
                            break;
                    }
                   
                }
            }
        }
    }
}