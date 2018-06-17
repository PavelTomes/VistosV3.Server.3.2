using Core.Models;
using Core.Services;
using Core.VistosDb.Objects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions
{
    public static class JObjectExtensions
    {
        private static void CleanProperties(this JObject jobject, List<vwProjectionColumn> columnsWithoutDbColumn, List<vwProjectionRelation> gridItems, UserInfo userInfo)
        {
            Settings settings = Settings.GetInstance;

            List<string> propertiesCaptionList = jobject.Properties().Where(p => p.Name.EndsWith("_Caption")).Select(p => p.Name).ToList();
            foreach (string propertyCaption in propertiesCaptionList)
            {
                jobject.Remove(propertyCaption);
            }
            foreach (string propertyCaption in new List<string>() { "Version", "Caption", "CreatedBy_FK", "CreatedBy_FK_Caption", "Created", "ModifiedBy_FK", "ModifiedBy_FK_Caption", "Modified" })
            {
                jobject.Remove(propertyCaption);
            }
            foreach (string propertyCaption in columnsWithoutDbColumn.Select(c => c.ProjectionColumn_Name))
            {
                jobject.Remove(propertyCaption);
            }

            if (gridItems != null && gridItems.Count > 0)
            {
                JObject definitions = new JObject();
                foreach (vwProjectionRelation relation in gridItems)
                {
                    List<vwProjectionColumn> childColumns = settings.GetVwProjectionColumnList(userInfo.ProfileId, relation.ProjectionRelation_ChildProjectionName).Where(x =>
                        x.AppColumnType_Id != 1
                        && !x.Column_IsReadOnly
                        && !x.Column_IsPrimaryKey
                        && x.ProjectionColumn_Id != relation.ProjectionColumn1_Id
                        && (x.Column_IsVisibleOnItemGrid || x.Column_HiddenData)
                        && x.AccessRightsType_Id == 2
                    ).ToList();
                    vwProjection chuildVwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Id == relation.ProjectionRelation_ChildProjection_FK);

                    if (jobject[relation.ProjectionRelation_ChildProjectionName] != null
                        && jobject[relation.ProjectionRelation_ChildProjectionName].HasValues
                        && jobject[relation.ProjectionRelation_ChildProjectionName].Type == JTokenType.Array
                        && jobject[relation.ProjectionRelation_ChildProjectionName].Count() > 0)
                    {
                        for (int i = 0; i < jobject[relation.ProjectionRelation_ChildProjectionName].Count(); i++)
                        {
                            JObject row = ((JObject)jobject[relation.ProjectionRelation_ChildProjectionName][i]);
                            List<string> propertiesCaptionListChild = row.Properties().Where(p => p.Name.EndsWith("_Caption")).Select(p => p.Name).ToList();
                            foreach (string propertyCaption in propertiesCaptionListChild)
                            {
                                row.Remove(propertyCaption);
                            }
                            foreach (string propertySystem in new List<string>() { "Version", "Caption", "CreatedBy_FK", "CreatedBy_FK_Caption", "Created", "ModifiedBy_FK", "ModifiedBy_FK_Caption", "Modified", "Total_WithoutTax_WithoutDiscounts", "Total_Tax_WithoutDiscounts", "Total_WithoutDiscounts", "Total_WithoutTax_WithDiscounts", "Total_Tax_WithDiscounts", "Total_WithDiscounts" })
                            {
                                row.Remove(propertySystem);
                            }
                        }
                    }
                }
            }
        }

        private static JObject ToSchemaProperties(vwProjection vwProjection, List<vwProjectionColumn> columns, List<vwProjectionRelation> gridItems)
        {
            JObject jsonObject = new JObject();

            {
                dynamic json1 = new JObject();
                json1.Type = new JArray() { "integer" };
                jsonObject["ID"] = json1;
            }
            {
                dynamic json1 = new JObject();
                json1.Type = new JArray() { "boolean" };
                jsonObject["Deleted"] = json1;
            }
            if (!string.IsNullOrEmpty(vwProjection.DbPrimaryColumn_Name))
            {
                dynamic json1 = new JObject();
                json1.Type = new JArray() { "integer", "null" };
                jsonObject[vwProjection.DbPrimaryColumn_Name] = json1;
            }

            if (vwProjection.Object_DocumentEnabled)
            {
                dynamic json2 = new JObject();
                json2.Type = new JArray() { "array", "null" };
                jsonObject["Document"] = json2;
            }
            if (vwProjection.Object_ParticipantEnabled)
            {
                dynamic json2 = new JObject();
                json2.Type = new JArray() { "array", "null" };
                jsonObject["Participant"] = json2;
            }
            //if (vwProjection.Object_CategoriesEnabled)
            {
                dynamic json2 = new JObject();
                json2.Type = new JArray() { "array", "null" };
                jsonObject["EntityCategories"] = json2;
            }

            foreach (vwProjectionColumn column in columns)
            {
                switch (column.DbColumnType_Id)
                {
                    case (int)DbColumnTypeEnum.Geography:
                        JObject json2a = column.ToJsonObject();
                        if (json2a != null)
                        {
                            jsonObject[column.ProjectionColumn_Name + "_Lat"] = json2a;
                        }
                        JObject json2b = column.ToJsonObject();
                        if (json2b != null)
                        {
                            jsonObject[column.ProjectionColumn_Name + "_Long"] = json2b;
                        }
                        break;
                    default:
                        JObject json2 = column.ToJsonObject();
                        if (json2 != null)
                        {
                            jsonObject[column.ProjectionColumn_Name] = json2;
                        }
                        break;
                }

            }

            if (gridItems != null && gridItems.Count > 0)
            {
                foreach (vwProjectionRelation gridItem in gridItems)
                {
                    switch (gridItem.ProjectionRelation_Type_FK)
                    {
                        case (int)DbRelationTypeEnum.ItemGrid:
                        case (int)DbRelationTypeEnum.ItemMasterGrid:
                        case (int)DbRelationTypeEnum.Attachment:
                            {
                                dynamic json4 = new JObject();
                                json4["$ref"] = $"#/definitions/{gridItem.ProjectionRelation_ChildProjectionName}";

                                dynamic json3 = new JObject();
                                json3.type = new JArray() { "array", "null" };
                                json3.items = json4;

                                jsonObject[gridItem.ProjectionRelation_ChildProjectionName] = json3;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return jsonObject;
        }

        private static JArray ToSchemaRequired(List<vwProjectionColumn> columns)
        {
            JArray jsonArray = new JArray();

            List<string> requiredColumns = columns.Where(c => !c.Column_IsNullable).Select(c => c.ProjectionColumn_Name).ToList();
            foreach (string column in requiredColumns)
            {
                jsonArray.Add(column);
            }

            return jsonArray;
        }

        public static bool Validate(this JObject jobject, string projectionName, UserInfo userInfo, out IList<ValidationError> errList)
        {
            try
            {
                Settings settings = Settings.GetInstance;
                vwProjection vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
                List<vwProjectionColumn> columnsAll = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                    x.AppColumnType_Id != 1
                    && !x.Column_IsReadOnly
                    && !x.Column_IsPrimaryKey
                    && (x.Column_IsVisibleOnForm || x.Column_HiddenData)
                    && x.AccessRightsType_Id == 2
                ).ToList();
                List<vwProjectionRelation> gridItems = settings.VwProjectionRelationList.Where(x =>
                      x.ProjectionRelation_ParentProjection_FK == vwProjection.Projection_Id
                      && x.ProjectionRelation_Type_FK == 1
                      && x.ProjectionRelation_ChildProjectionName != "Participant"
                      && x.ProjectionRelation_ChildProjectionProfile_Id == userInfo.ProfileId
                      && ((x.ProjectionRelation_ChildProjectionAccessRight & (long)6) == (long)6)
                      ).ToList();

                dynamic jsonSchema = new JObject();
                #region make JSON Schema definition
                jsonSchema.type = "object";
                jsonSchema.properties = ToSchemaProperties(vwProjection, columnsAll, gridItems);
                jsonSchema.required = ToSchemaRequired(columnsAll);

                if (gridItems != null && gridItems.Count > 0)
                {
                    JObject definitions = new JObject();
                    foreach (vwProjectionRelation relation in gridItems)
                    {
                        List<vwProjectionColumn> childColumns = settings.GetVwProjectionColumnList(userInfo.ProfileId, relation.ProjectionRelation_ChildProjectionName).Where(x =>
                            x.AppColumnType_Id != 1
                            && !x.Column_IsReadOnly
                            && !x.Column_IsPrimaryKey
                            && x.ProjectionColumn_Id != relation.ProjectionColumn1_Id
                            && (x.Column_IsVisibleOnItemGrid || x.Column_HiddenData)
                            && x.AccessRightsType_Id == 2
                        ).ToList();
                        vwProjection childVwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Id == relation.ProjectionRelation_ChildProjection_FK);

                        dynamic jsonChild = new JObject();
                        jsonChild.type = "object";
                        jsonChild.properties = ToSchemaProperties(childVwProjection, childColumns, null);
                        jsonChild.required = ToSchemaRequired(childColumns);
                        definitions[relation.ProjectionRelation_ChildProjectionName] = jsonChild;
                    }
                    jsonSchema["definitions"] = definitions;
                }
                #endregion

                string jsonSchemaString = jsonSchema.ToString();
                JSchema schema = JSchema.Parse(jsonSchemaString);

                #region setup JSchema
                schema.Type = JSchemaType.Object;
                //schema.AllowAdditionalProperties = false;
                //schema.AllowAdditionalItems = false;
                //if (gridItems != null && gridItems.Count > 0)
                //{
                //    foreach (vwProjectionRelation relation in gridItems)
                //    {
                //        schema.Properties[relation.ProjectionRelation_ChildProjectionName].AllowAdditionalProperties = false;
                //        schema.Properties[relation.ProjectionRelation_ChildProjectionName].AllowAdditionalItems = false;
                //        if (schema.Properties[relation.ProjectionRelation_ChildProjectionName].Items != null && schema.Properties[relation.ProjectionRelation_ChildProjectionName].Items.Count > 0)
                //        {
                //            for (int i = 0; i < schema.Properties[relation.ProjectionRelation_ChildProjectionName].Items.Count; i++)
                //            {
                //                schema.Properties[relation.ProjectionRelation_ChildProjectionName].Items[i].AllowAdditionalProperties = false;
                //                schema.Properties[relation.ProjectionRelation_ChildProjectionName].Items[i].AllowAdditionalItems = false;
                //            }
                //        }
                //    }
                //}
                #endregion

                bool isValid = jobject.IsValid(schema, out errList);

                return isValid;
            }
            catch (Exception ex)
            {
                Logger.SaveLogError(LogLevel.Error, "JObjectExtensions Validate", ex, null, userInfo);

                errList = null;
                return true;
            }
        }
    }
}