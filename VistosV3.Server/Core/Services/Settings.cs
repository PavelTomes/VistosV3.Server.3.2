using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Core.Models;
using Core.Extensions;
using Core.VistosDb.Objects;
using Core.VistosDb;
using Microsoft.Extensions.Configuration;

namespace Core.Services
{
    public sealed class Settings
    {
        private static string dbConnectionString = null;
        private static volatile Settings instance;
        private static object syncRoot = new Object();
        public const Int32 PROFILE_SYS_ADMIN_ID = 1;

        private static ConcurrentDictionary<string, List<Enumeration>> enumerationDict = null;

        private static List<vwProjectionRelation> vwProjectionRelationList = null;
        private static List<vwProjectionAction> vwProjectionActionList = null;
        private static List<vwProjectionActionColumnMapping> vwProjectionActionColumnMappingList = null;
        private static List<vwProjection> vwProjectionList = null;
        private static List<CrmEntity> crmEntityList = null;
        private static List<vwProjectionColumn> vwProjectionColumnList = null;
        private static List<ProjectionReport> projectionReportList = null;

        private static ConcurrentDictionary<int, List<vwProjection>> vwProjectionDict = null;
        private static ConcurrentDictionary<int, ConcurrentDictionary<string, List<vwProjectionColumn>>> vwProjectionColumnDict = null;
        private static ConcurrentDictionary<string, CrmEntity> crmEntityDict = null;
        private static ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>>> queryBuilderScripts = null;

        private static ConcurrentDictionary<int, string> schemaDict = null;
        private static ConcurrentDictionary<int, string> menuDict = null;

        private static vwSystemSettings systemSettings = null;
        private static List<vwUserAuthToken> vwUserAuthTokenList = null;
        private static List<vwUser> vwUserList = null;
        private static List<vwRole> vwRoleList = null;
        private static List<vwPohodaDbObjectConfiguration> vwPohodaDbObjectConfigurationList = null;
        private static List<vwBusinessUnit> vwBusinessUnitList = null;
        private static List<LocalizationLanguage> localizationLanguageList = null;
        private static List<vwNumberingSequence> vwNumberingSequence = null;

        private Settings()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddJsonFile("appsettings.json")
              .Build();
            dbConnectionString = configuration.GetConnectionString("DefaultConnection");

            queryBuilderScripts = new ConcurrentDictionary<string, ConcurrentDictionary<int, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>>>();
            vwProjectionDict = new ConcurrentDictionary<int, List<vwProjection>>();
            crmEntityDict = new ConcurrentDictionary<string, CrmEntity>();
            vwProjectionColumnDict = new ConcurrentDictionary<int, ConcurrentDictionary<string, List<vwProjectionColumn>>>();
            enumerationDict = new ConcurrentDictionary<string, List<Enumeration>>();
            vwLocalizationDict = new ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, string>>>();

            schemaDict = new ConcurrentDictionary<int, string>();
            menuDict = new ConcurrentDictionary<int, string>();

            using (VistosDbContext ctx = new VistosDbContext())
            {
                projectionReportList = ctx.ProjectionReport.ToList().Distinct().ToList();
                vwProjectionActionList = ctx.vwProjectionAction.ToList().Distinct().ToList();
                vwProjectionRelationList = ctx.vwProjectionRelation.ToList().Distinct().ToList();
                vwProjectionActionColumnMappingList = ctx.vwProjectionActionColumnMapping.ToList().Distinct().ToList();
                vwProjectionList = ctx.vwProjection.ToList().Distinct().ToList();
                crmEntityList = ctx.CrmEntity.ToList().Distinct().ToList();
                vwProjectionColumnList = ctx.vwProjectionColumn.ToList().Distinct().ToList();
                systemSettings = ctx.vwSystemSettings.First();
                localizationLanguageList = ctx.LocalizationLanguage.ToList().Distinct().ToList();

                vwUserAuthTokenList = ctx.vwUserAuthToken.ToList().Distinct().ToList();
                vwUserList = ctx.vwUser.ToList().Distinct().ToList();
                vwRoleList = ctx.vwRole.ToList().Distinct().ToList();
                vwBusinessUnitList = ctx.vwBusinessUnit.ToList().Distinct().ToList();
                vwNumberingSequence = ctx.vwNumberingSequence.ToList().Distinct().ToList();
                if (this.SystemSettings.PohodaConnectorEnabled)
                {
                    vwPohodaDbObjectConfigurationList = ctx.vwPohodaDbObjectConfiguration.ToList().Distinct().ToList();
                }
            }
        }

        public bool Authorize(UserInfo userInfo, string projectionName, OperationAccessRightsEnum operationAccessRightsEnum)
        {
            return this.GetVwProjectionList(userInfo.ProfileId).Where(x => x.Projection_Name == projectionName
                && (x.AccessRight & (long)operationAccessRightsEnum) == (long)operationAccessRightsEnum).Any();
        }

        public vwSystemSettings SystemSettings
        {
            get
            {
                return systemSettings;
            }
        }

        public List<ProjectionReport> ProjectionReportList
        {
            get
            {
                return projectionReportList;
            }
        }

        public List<vwBusinessUnit> VwBusinessUnitList
        {
            get
            {
                return vwBusinessUnitList;
            }
        }

        public List<vwNumberingSequence> VwNumberingSequence
        {
            get
            {
                return vwNumberingSequence;
            }
        }

        public List<vwUser> VwUserList
        {
            get
            {
                return vwUserList;
            }
        }

        public List<vwUserAuthToken> VwUserAuthTokenList
        {
            get
            {
                return vwUserAuthTokenList;
            }
        }

        public void VwUserAuthTokenListReset()
        {
            using (VistosDbContext ctx = new VistosDbContext())
            {
                vwUserAuthTokenList = ctx.vwUserAuthToken.ToList().Distinct().ToList();
            }
        }

        public List<vwRole> VwRoleList
        {
            get
            {
                return vwRoleList;
            }
        }

        public List<vwPohodaDbObjectConfiguration> VwPohodaDbObjectConfigurationList
        {
            get
            {
                return vwPohodaDbObjectConfigurationList;
            }
        }

        public List<vwProjectionActionColumnMapping> VwProjectionActionColumnMappingList
        {
            get
            {
                return vwProjectionActionColumnMappingList;
            }
        }

        public List<vwProjectionAction> VwProjectionActionList
        {
            get
            {
                return vwProjectionActionList;
            }
        }

        public List<LocalizationLanguage> LocalizationLanguageList
        {
            get
            {
                return localizationLanguageList;
            }
        }

        public List<vwProjectionRelation> VwProjectionRelationList
        {
            get
            {
                return vwProjectionRelationList;
            }
        }

        public string GetSchema(UserInfo userInfo)
        {
            if (!schemaDict.ContainsKey(userInfo.ProfileId))
            {
                StringBuilder json = new StringBuilder();

                using (SqlConnection conn = new SqlConnection(dbConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_api3_getSchema", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@userId", userInfo.UserId));
                    SqlDataReader rdr = cmd.ExecuteReader();
                    rdr.Read();
                    json.Append(rdr.GetString(0));
                    while (rdr.Read())
                    {
                        json.Append(rdr.GetString(0));
                    }
                }
                JObject obj = JObject.Parse(json.ToString());
                string json1 = obj.ToString(Formatting.None);
                schemaDict.GetOrAdd(userInfo.ProfileId, json1);
            }
            if (schemaDict.ContainsKey(userInfo.ProfileId))
            {
                return schemaDict[userInfo.ProfileId];
            }
            return string.Empty;
        }

        public string GetMenu(int userId)
        {
            if (!menuDict.ContainsKey(userId))
            {
                StringBuilder json = new StringBuilder();

                using (SqlConnection conn = new SqlConnection(dbConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_api3_getMenu", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@userId", userId));
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        json.Append(rdr.GetString(0));
                    }
                }
                menuDict.GetOrAdd(userId, json.ToString());
            }
            if (menuDict.ContainsKey(userId))
            {
                return menuDict[userId];
            }
            return string.Empty;
        }

        public List<vwProjection> GetVwProjectionList(int profileId)
        {
            if (profileId <= 0)
            {
                return new List<vwProjection>();
            }
            if (!vwProjectionDict.ContainsKey(profileId))
            {
                List<vwProjection> list = vwProjectionList.Where(p => p.Profile_Id == profileId).OrderBy(p => p.Projection_Name).ToList();
                vwProjectionDict.GetOrAdd(profileId, list);
            }
            if (vwProjectionDict.ContainsKey(profileId))
            {
                return vwProjectionDict[profileId];
            }
            return new List<vwProjection>();
        }
        public int GetCrmEntityType(string entityName)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                return 0;
            }
            if (!crmEntityDict.ContainsKey(entityName))
            {
                CrmEntity crmEntity = crmEntityList.Where(p => p.Name == entityName).FirstOrDefault();
                crmEntityDict.GetOrAdd(entityName, crmEntity != null && crmEntity.Id > 0 ? crmEntity : null);
            }
            if (crmEntityDict.ContainsKey(entityName))
            {
                CrmEntity crmEntity = crmEntityDict[entityName];
                if (crmEntity != null)
                {
                    return crmEntity.Type;
                }
            }
            return 0;
        }

        public List<vwProjectionColumn> GetVwProjectionColumnList(int profileId, string projectionName)
        {
            if (string.IsNullOrEmpty(projectionName) || profileId <= 0)
            {
                return new List<vwProjectionColumn>();
            }
            if (!vwProjectionColumnDict.ContainsKey(profileId))
            {
                vwProjectionColumnDict.GetOrAdd(profileId, new ConcurrentDictionary<string, List<vwProjectionColumn>>());
            }
            if (vwProjectionColumnDict.ContainsKey(profileId))
            {
                if (!vwProjectionColumnDict[profileId].ContainsKey(projectionName))
                {
                    vwProjectionColumnDict[profileId].GetOrAdd(projectionName, vwProjectionColumnList.Where(p => p.Profile_Id == profileId && p.Projection_Name == projectionName).ToList());
                }
                if (vwProjectionColumnDict[profileId].ContainsKey(projectionName))
                {
                    return vwProjectionColumnDict[profileId][projectionName];
                }
            }
            return new List<vwProjectionColumn>();
        }

        public string GetQueryBuilderScript(string userLanguage, int profileId, string projectionName, string opeartion)
        {
            if (string.IsNullOrEmpty(userLanguage) || string.IsNullOrEmpty(projectionName) || profileId <= 0 || string.IsNullOrEmpty(opeartion))
            {
                return null;
            }
            if (queryBuilderScripts.ContainsKey(userLanguage) && queryBuilderScripts[userLanguage].ContainsKey(profileId) && queryBuilderScripts[userLanguage][profileId].ContainsKey(projectionName) && queryBuilderScripts[userLanguage][profileId][projectionName].ContainsKey(opeartion))
            {
                return queryBuilderScripts[userLanguage][profileId][projectionName][opeartion];
            }
            else
            {
                if (!queryBuilderScripts.ContainsKey(userLanguage))
                {
                    queryBuilderScripts.GetOrAdd(userLanguage, new ConcurrentDictionary<int, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>>());
                }
                if (queryBuilderScripts.ContainsKey(userLanguage))
                {
                    if (!queryBuilderScripts[userLanguage].ContainsKey(profileId))
                    {
                        queryBuilderScripts[userLanguage].GetOrAdd(profileId, new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());
                    }
                    if (queryBuilderScripts[userLanguage].ContainsKey(profileId))
                    {
                        if (!queryBuilderScripts[userLanguage][profileId].ContainsKey(projectionName))
                        {
                            queryBuilderScripts[userLanguage][profileId].GetOrAdd(projectionName, new ConcurrentDictionary<string, string>());
                        }
                        if (queryBuilderScripts[userLanguage][profileId].ContainsKey(projectionName))
                        {
                            if (!queryBuilderScripts[userLanguage][profileId][projectionName].ContainsKey(opeartion))
                            {
                                queryBuilderScripts[userLanguage][profileId][projectionName].GetOrAdd(opeartion, string.Empty);
                            }
                            if (queryBuilderScripts[userLanguage][profileId][projectionName].ContainsKey(opeartion))
                            {
                                return queryBuilderScripts[userLanguage][profileId][projectionName][opeartion];
                            }
                        }
                    }
                }
            }
            return null;
        }

        public bool SetQueryBuilderScript(string userLanguage, int profileId, string projectionName, string opeartion, string script)
        {
            if (string.IsNullOrEmpty(projectionName) || string.IsNullOrEmpty(projectionName) || profileId <= 0 || string.IsNullOrEmpty(opeartion))
            {
                return false;
            }
            if (queryBuilderScripts.ContainsKey(userLanguage) && queryBuilderScripts[userLanguage].ContainsKey(profileId) && queryBuilderScripts[userLanguage][profileId].ContainsKey(projectionName) && queryBuilderScripts[userLanguage][profileId][projectionName].ContainsKey(opeartion))
            {
                queryBuilderScripts[userLanguage][profileId][projectionName][opeartion] = script;
                return true;
            }
            return false;
        }




        public ProjectionMethodMode GetProjectionMethodMode(int profileId, string projectionName, string methodName)
        {
            int id = 0;
            switch (methodName)
            {
                case "GetById":
                    id = this.GetVwProjectionList(profileId).Where(x => x.Projection_Name == projectionName).Select(x => x.Object_MethodMode_GetById_FK).FirstOrDefault();
                    break;
                case "Add":
                    id = this.GetVwProjectionList(profileId).Where(x => x.Projection_Name == projectionName).Select(x => x.Object_MethodMode_Add_FK).FirstOrDefault();
                    break;
                case "Update":
                    id = this.GetVwProjectionList(profileId).Where(x => x.Projection_Name == projectionName).Select(x => x.Object_MethodMode_Update_FK).FirstOrDefault();
                    break;
                case "Remove":
                    id = this.GetVwProjectionList(profileId).Where(x => x.Projection_Name == projectionName).Select(x => x.Object_MethodMode_Remove_FK).FirstOrDefault();
                    break;
                case "GetPage":
                    id = this.GetVwProjectionList(profileId).Where(x => x.Projection_Name == projectionName).Select(x => x.Object_MethodMode_GetPage_FK).FirstOrDefault();
                    break;
                case "NewEntityFrom":
                    id = this.GetVwProjectionList(profileId).Where(x => x.Projection_Name == projectionName).Select(x => x.Object_MethodMode_NewEntityFrom_FK).FirstOrDefault();
                    break;
                case "NewEntity":
                    id = this.GetVwProjectionList(profileId).Where(x => x.Projection_Name == projectionName).Select(x => x.Object_MethodMode_NewEntity_FK).FirstOrDefault();
                    break;
                default:
                    id = 1;
                    break;
            }
            return (ProjectionMethodMode)id;
        }

        private void CheckEnumerationDict(string typeName)
        {
            if (!enumerationDict.ContainsKey(typeName))
            {
                using (VistosDbContext ctx = new VistosDbContext())
                {
                    int typeId = ctx.EnumerationType.Where(t => !t.Deleted && t.Type.ToLower() == typeName.ToLower()).Select(t => t.Id).FirstOrDefault();
                    if (typeId > 0)
                    {
                        enumerationDict.GetOrAdd(typeName, ctx.Enumeration.Where(e => !e.Deleted && e.EnumerationType_FK.Value == typeId).ToList());
                    }
                    else
                    {
                        enumerationDict.GetOrAdd(typeName, new List<Enumeration>());
                    }
                }
            }
        }

        public Enumeration GetEnumerationByName(string typeName, string enumDesc)
        {
            CheckEnumerationDict(typeName);
            if (enumerationDict.ContainsKey(typeName))
            {
                Enumeration enum1 = enumerationDict[typeName].Where(e => e.Description.ToLower() == enumDesc.ToLower()).FirstOrDefault();
                if (enum1 != null && enum1.Id > 0)
                {
                    return enum1;
                }
            }
            return null;
        }

        public Enumeration GetEnumerationByEnumDescription(Enum typeName, Enum enumDesc)
        {
            var strTypeName = typeName.ToDescriptionString();
            var strEnumDesc = enumDesc.ToDescriptionString();
            return GetEnumerationByName(strTypeName, strEnumDesc);
        }

        public Enumeration GetEnumerationById(string typeName, int enumId)
        {
            CheckEnumerationDict(typeName);
            if (enumerationDict.ContainsKey(typeName))
            {
                Enumeration enum1 = enumerationDict[typeName].Where(e => e.Id == enumId).FirstOrDefault();
                if (enum1 != null && enum1.Id > 0)
                {
                    return enum1;
                }
            }
            return null;
        }

        public string GetEnumerationCaptionById(string typeName, int enumId, string userLanguage)
        {
            Enumeration enum1 = GetEnumerationById(typeName, enumId);
            if (enum1 != null && enum1.Id > 0)
            {
                switch (userLanguage.ToLower())
                {
                    case "cs-cz":
                        return enum1.Description_cs_CZ;
                    case "en-us":
                        return enum1.Description_en_US;
                    default:
                        return enum1.Description;
                }
            }
            return null;
        }

        public List<ReminderSettings> GetReminderSettings(Int32? dbObjectId = null)
        {
            Expression<Func<ReminderSettings, bool>> expr = x => !x.Deleted;

            if (dbObjectId.HasValue)
            {
                expr = x => !x.Deleted && x.DbObject_FK.HasValue && (x.DbObject_FK == dbObjectId.Value);
            }

            using (VistosDbContext ctx = new VistosDbContext())
            {
                //                return ctx.ReminderSettings.Include(rs => rs.Reminder && rs.Reminder.de).Where(expr).ToList();
                //**return ctx.ReminderSettings.Include(x => x.Reminder).Where(expr).ToList().Distinct().ToList();
                return ctx.ReminderSettings.Where(expr).ToList().Distinct().ToList();
            }
        }

        //public List<vwProjectionColumnLocalization> GetVwProjectionColumnLocalizations(string projectionName)
        //{
        //    if (string.IsNullOrEmpty(projectionName))
        //    {
        //        return new List<vwProjectionColumnLocalization>();
        //    }
        //    if (!vwProjectionColumnLocalizationDict.ContainsKey(projectionName))
        //    {
        //        using (VistosDbContext ctx = new VistosDbContext())
        //        {
        //            List<vwProjectionColumnLocalization> list = ctx.vwProjectionColumnLocalization.Where(p => p.Projection_Name == projectionName).ToList();
        //            vwProjectionColumnLocalizationDict.GetOrAdd(projectionName, list);
        //        }
        //    }

        //    if (vwProjectionColumnLocalizationDict.ContainsKey(projectionName))
        //    {
        //        return vwProjectionColumnLocalizationDict[projectionName];
        //    }

        //    return new List<vwProjectionColumnLocalization>();
        //}

        private static ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, string>>> vwLocalizationDict = null;

        public Dictionary<string, string> GetGlobalLocalizations(string language)
        {
            return GetLocalizations("Global", language);
        }

        public Dictionary<string, string> GetLocalizations(string areaName, string language)
        {
            if (vwLocalizationDict == null)
            {
                vwLocalizationDict = new ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, string>>>();
            }

            if (!vwLocalizationDict.ContainsKey(areaName) || !vwLocalizationDict[areaName].ContainsKey(language))
            {
                using (VistosDbContext ctx = new VistosDbContext())
                {
                    var projectionColumnLocalizations = ctx.vwLocalization
                                                .Where(x => (x.LocalizationArea_Name == areaName && x.LocalizationLanguage_Name == language)).ToList();

                    if (!vwLocalizationDict.ContainsKey(areaName))
                    {
                        vwLocalizationDict.GetOrAdd(areaName, new ConcurrentDictionary<string, Dictionary<string, string>>());
                    }
                    if (!vwLocalizationDict[areaName].ContainsKey(language))
                    {
                        vwLocalizationDict[areaName].GetOrAdd(language, new Dictionary<string, string>());
                    }

                    // zpracovani vychozich lokalizaci na projekci
                    projectionColumnLocalizations.ForEach(x =>
                    {
                        if (!vwLocalizationDict[areaName][language].ContainsKey(x.LocalizationString_Key))
                        {
                            vwLocalizationDict[areaName][language].Add(x.LocalizationString_Key, x.LocalizationString_Value);
                        }
                    });
                }
            }

            return vwLocalizationDict[areaName][language];
        }

        public List<vwParticipant> GetVwParticipants(int dbObjectId, int recordId)
        {
            using (VistosDbContext ctx = new VistosDbContext())
            {
                return ctx.vwParticipant.Where(x => x.DbObject_Id == dbObjectId && x.Participant_RecordId == recordId).ToList().Distinct().ToList();
            }
        }

        public static Settings GetInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Settings();
                    }
                }

                return instance;
            }
        }

        public static void Restart()
        {
            lock (syncRoot)
            {
                instance = new Settings();
            }
        }
    }
}