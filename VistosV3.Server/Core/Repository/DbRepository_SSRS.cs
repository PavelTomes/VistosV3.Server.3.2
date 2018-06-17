using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Repository
{
    public partial class DbRepository
    {
        public String GetReports()
        {
            //**List<CatalogItem> catalogItems = null;
            //**
            //**var settings = new SSRSSettings();
            //**settings.LoadSettings();
            //**
            //**using (var rService = new SSRS.ReportingService2005())
            //**{
            //**    rService.Url = settings.ReportServerUrl + "/ReportService2005.asmx";
            //**    if (String.IsNullOrEmpty(settings.ReportServerUserName))
            //**        rService.UseDefaultCredentials = true;
            //**    else
            //**        rService.Credentials = new System.Net.NetworkCredential(settings.ReportServerUserName, settings.ReportServerPassword);
            //**    CatalogItem[] items = rService.ListChildren(settings.ReportsPath, true);
            //**    List<string> ReportAccessRightsPathNames = new List<string>();
            //**    List<CatalogItem> itemsWithRights = new List<CatalogItem>();
            //**    using (var ctx = new VistosApi())
            //**    {
            //**        foreach (var rar in ctx.ReportAccessRights.Where(x => x.Profile_FK == this.userInfo.ProfileId))
            //**        {
            //**            ReportAccessRightsPathNames.Add(settings.ReportsPath + '/' + rar.Name);
            //**        }
            //**        itemsWithRights = items.Where(x => ReportAccessRightsPathNames.Where(rar => rar.Equals(x.Path)).Any() || x.Type == ItemTypeEnum.Folder).ToList();
            //**    }
            //**
            //**    catalogItems = itemsWithRights.Where(i => !i.Hidden
            //**                                 && (i.Type == ItemTypeEnum.Report || i.Type == ItemTypeEnum.Folder))
            //**                                 .OrderBy(x => x.Type)
            //**                               .ToList();
            //**}
            //**
            //**
            //**string json = new JavaScriptSerializer().Serialize(this.GetTreeCatalog(catalogItems));
            //**
            //**return json;
            return "{}";
        }

        //**private List<Item> GetTreeCatalog(List<CatalogItem> catalogItems)
        //**{
        //**    Dictionary<Int32, List<Item>> levels = new Dictionary<Int32, List<Item>>();
        //**    int counter = 0;
        //**    for (int i = 0; i < catalogItems.Count; i++)
        //**    {
        //**        var item = catalogItems[i];
        //**        String[] arrDataTmp = item.Path.Split('/');
        //**        String[] arrData = null;
        //**        if (arrDataTmp.Length > 0 && String.IsNullOrEmpty(arrDataTmp[0]))
        //**        {
        //**            arrData = new string[arrDataTmp.Length - 1];
        //**            arrDataTmp.ToList().CopyTo(1, arrData, 0, arrDataTmp.Length - 1);
        //**        }
        //**        else
        //**        {
        //**            arrData = new string[arrDataTmp.Length];
        //**            arrDataTmp.CopyTo(arrData, 0);
        //**        }
        //**
        //**        for (int l = 0; l < arrData.Length; l++)
        //**        {
        //**            if (!levels.ContainsKey(l))
        //**            {
        //**                levels.Add(l, new List<Item>());
        //**            }
        //**
        //**            var newItem = new Item()
        //**            {
        //**                Id = counter++,
        //**                Subject = arrData[l],
        //**                Name = arrData[l],
        //**                Path = item.Path,
        //**                Type = item.Type.ToString()
        //**            };
        //**
        //**            List<Item> itemsInLevel = levels[l];
        //**            if (!itemsInLevel.Contains(newItem) && itemsInLevel.FirstOrDefault(x => x.Subject == newItem.Subject) == null)
        //**            {
        //**                newItem.Level = l;
        //**                levels[l].Add(newItem);
        //**
        //**                if (levels.Keys.Contains(l - 1))
        //**                {
        //**                    foreach (Item parent in levels[l - 1])
        //**                    {
        //**                        if (newItem.Path.Contains(parent.Path))
        //**                        {
        //**                            newItem.ParentId = parent.Id;
        //**                            break;
        //**                        }
        //**                    }
        //**                }
        //**            }
        //**        }
        //**    }
        //**
        //**    List<Item> result = new List<Item>();
        //**    foreach (Int32 level in levels.Keys)
        //**    {
        //**        result.AddRange(levels[level]);
        //**    }
        //**
        //**    return result;
        //**}

        public class Item
        {
            public Int32 Id { get; set; }
            public Int32 ParentId { get; set; }
            public Int32 Level { get; set; }
            public String Subject { get; set; }
            public String Name { get; set; }
            public String Path { get; set; }
            public String Type { get; set; }
        }

        // TODO: premistit a doplnit o pripadne dalsi fieldy
        public class SSRSSettings
        {
            public String ReportServerUrl { get; set; }
            public String FormsPath { get; set; }
            public String ReportsPath { get; set; }
            public String ReportServerUserName { get; set; }
            public String ReportServerPassword { get; set; }
            public String CRM_Sales_Report_Path { get; set; }

            public void LoadSettings()
            {
                FormsPath = Settings.GetInstance.SystemSettings.ReportServerFormsPath;
                ReportsPath = Settings.GetInstance.SystemSettings.ReportServerReportsPath;
                ReportServerUserName = Settings.GetInstance.SystemSettings.ReportServerUserName;
                ReportServerPassword = Settings.GetInstance.SystemSettings.ReportServerPassword;
                ReportServerUrl = Settings.GetInstance.SystemSettings.ReportServerUrl;
                //CRM_Sales_Report_Path = ctx.SystemSettings.FirstOrDefault(s => !s.Deleted && s.Key == "CRM_Sales_Report_Path").Value;
            }
        }
    }
}