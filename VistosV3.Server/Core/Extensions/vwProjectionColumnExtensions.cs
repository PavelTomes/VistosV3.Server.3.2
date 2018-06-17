using Core.Models;
using Core.VistosDb.Objects;
using Newtonsoft.Json.Linq;

namespace Core.Extensions
{
    public static class vwProjectionColumnExtensions
    {
        public static JObject ToJsonObject(this vwProjectionColumn column)
        {
            dynamic json = new JObject();
            string type = "string";

            switch (column.DbColumnType_Id)
            {
                case (int)DbColumnTypeEnum.Bit:
                case (int)DbColumnTypeEnum.BitIcon:
                    type = "boolean";
                    break;
                case (int)DbColumnTypeEnum.Int:
                case (int)DbColumnTypeEnum.Entity:
                case (int)DbColumnTypeEnum.EntityEnumeration:
                case (int)DbColumnTypeEnum.Enumeration:
                case (int)DbColumnTypeEnum.Signature:
                case (int)DbColumnTypeEnum.BigInt:
                    type = "integer";
                    break;
                case (int)DbColumnTypeEnum.Float:
                case (int)DbColumnTypeEnum.Money:
                    type = "number";
                    break;
                case (int)DbColumnTypeEnum.String:
                case (int)DbColumnTypeEnum.Text:
                case (int)DbColumnTypeEnum.TextSimple:
                case (int)DbColumnTypeEnum.WidgetHtml:
                case (int)DbColumnTypeEnum.IconSelect:
                case (int)DbColumnTypeEnum.Guid:
                case (int)DbColumnTypeEnum.XML:
                case (int)DbColumnTypeEnum.EmailAddress:
                case (int)DbColumnTypeEnum.WebUrl:
                case (int)DbColumnTypeEnum.PhoneNumber:
                    type = "string";
                    break;
                case (int)DbColumnTypeEnum.Date:
                    type = "string";
                    json.format = "date";
                    break;
                case (int)DbColumnTypeEnum.DateTime:
                case (int)DbColumnTypeEnum.DateCheckBox:
                    type = "string";
                    json.format = "date-time";
                    break;
                case (int)DbColumnTypeEnum.Time:
                    type = "string";
                    json.format = "time";
                    break;

                case (int)DbColumnTypeEnum.MultiEnumeration:
                    type = "array";
                    break;
                case (int)DbColumnTypeEnum.Geography:
                    type = "number";
                    break;
                case (int)DbColumnTypeEnum.MultiStateBox:
                case (int)DbColumnTypeEnum.JSON:
                    break;
                case (int)DbColumnTypeEnum.SystemChart:
                case (int)DbColumnTypeEnum.UserChart:
                case (int)DbColumnTypeEnum.Varbinary:
                case (int)DbColumnTypeEnum.Image:
                    return null;
                default:
                    type = "string";
                    break;
            }
            JArray types = new JArray() { type };
            if (column.Column_IsNullable)
            {
                types.Add("null");
            }
            json.type = types;
            if (column.Column_StringMaxLength.HasValue && column.Column_StringMaxLength.Value > 0)
            {
                json.maxLength = column.Column_StringMaxLength.Value;
            }
            if (!string.IsNullOrEmpty(column.Column_LocalizationKey))
            {
                json.title = column.Column_LocalizationKey;
            }
            return json;

        }
    }
}