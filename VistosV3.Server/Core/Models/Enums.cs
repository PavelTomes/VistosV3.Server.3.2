using System.ComponentModel;
using System.Xml.Linq;

namespace Core.Models
{
    public enum ResponseStatusCode
    {
        OK = 200,
        Warning = 220,
        Error = 210,
        NotFound = 212,
        InvalidLogin = 213,
        Unauthenticated = 214,
        Unauthorized = 215,
        InvalidJson = 216,
        NotAllowed = 0 // TODO: smazat nebo definovat!
    }

    public enum ExportType
    {
        Excel,
        Word,
        PDF,
        HTML,
        MHTML,
        HTMLOWC,
        ImagePNG,
        ImageJPG,

    }

    public enum GridSettingsType
    {
        GridSettings = 1,
        FormatSettings = 2
    }
    public enum EnumNotificationType
    {
        [Description("Discussion")]
        Discussion,
        [Description("TrackChanges")]
        TrackChanges,
        [Description("Reminder")]
        Reminder,
        [Description("Notification")]
        Notification,
    }

    public enum EnumerationType
    {
        [Description("NotificationType")]
        NotificationType,

        [Description("FrequencySendNotify")]
        FrequencySendNotify
    }

    public enum EmailMessageFlags
    {
        // Summary:
        //     None.
        None = 0,
        //
        // Summary:
        //     Message has been read.
        Seen = 1,
        //
        // Summary:
        //     Message has been answered.
        Answered = 2,
        //
        // Summary:
        //     Message is flagged for special attention.
        Flagged = 4,
        //
        // Summary:
        //     Message is marked as deleted for removal by Rebex.Net.Imap.Purge().
        Deleted = 8,
        //
        // Summary:
        //     Message is a draft - not fully composed yet.
        Draft = 16,
        //
        // Summary:
        //     Message has recently arrived and this is the first and only session notified
        //     about this.  
        Recent = 32,
        //
        // Summary:
        //     Message may have keyword (custom flags).
        Keywords = 4096,
    }

    public enum FolderTypeFlags
    {
        None = 0,

        Sent = 1,

        Draft = 2,

        Inbox = 4,

        Deleted = 8
    }

    public enum EmailAccountEncryption
    {
        None = 0,
        SSL = 1,
        TLS = 2
    }

    public enum EmailSmtpAuthentication
    {
        Auto = 0,
        Plain = 1,
        DigestMD5 = 2,
        CramMD5 = 3,
        Login = 4,
        Ntlm = 7,
        GssApi = 9
    }

    public enum EmailIncAuthentication
    {
        Auto = 0,
        Plain = 1,
        DigestMD5 = 2,
        CramMD5 = 3,
        Login = 4,
        ClearText = 5,
        APop = 6, // Pouze POP3
        Ntlm = 7,
        External = 8, // Pouze IMAP
        GssApi = 9
    }

    public enum EmailAccountType
    {
        IMAP = 0,
        POP3 = 1
    }

    public enum ProjectionMethodMode
    {
        QueryBuilder = 1,
        StoredProcedure = 2,
        StoredProcedureAfterQueryBuilder = 3,
        PlugIn = 4,
        PlugInAfterQueryBuilder = 5
    }

    public enum ProjectionActionResultType
    {
        EntityJson = 1,
        EntityId = 2,
        EntityList = 3
    }

    public enum ProjectionActionType
    {
        New = 1,
        CreateFrom = 2,
        Duplicate = 3,
        Generate = 4,
        Reply = 5,
        ReplyAll = 6,
        Forward = 7
    }

    public enum Message
    {
        [Description("User has not set a reference to profile")]
        USER_HAS_NOT_SET_REFERENCE_TO_PROFILE
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug,
        Fatal,
        JsonValidationError,
        ErrorGetSpData

    }

    public enum DbColumnTypeEnum
    {
        Bit = 1,
        Int = 2,
        Float = 3,
        Money = 4,
        String = 5,
        Text = 6,
        Date = 7,
        Enumeration = 8,
        MultiEnumeration = 9,
        Entity = 10,
        Time = 11,
        DateTime = 12,
        Signature = 13,
        Guid = 14,
        XML = 15,
        DateCheckBox = 16,
        BigInt = 17,
        Geography = 18,
        Varbinary = 19,
        Image = 20,
        EntityEnumeration = 21,
        TextSimple = 22,
        WidgetHtml = 23,
        SystemChart = 24,
        UserChart = 25,
        IconSelect = 26,
        MultiStateBox = 27,
        JSON = 28,
        DashboardTable = 29,
        BitIcon = 30,
        Password = 31,
        Color = 32,
        Participant = 33,
        EmailAddress = 34,
        PhoneNumber = 35,
        WebUrl = 36
    }

    public enum DbRelationTypeEnum
    {
        ItemGrid = 1,
        SubGrid = 2,
        ManyGrid = 3,
        ItemMasterGrid = 4,
        Attachment = 5
    }

    public enum OperationAccessRightsEnum
    {
        GetPageParam = 1,
        GetGridIdsParam = 1,
        DeleteParam = 8,
        GetAutocompleteParam = 1,

        GetByIdParam = 1,
        CreateParam = 2,
        SaveParam = 4,
        RemoveParam = 8,

        CreateEntityFromParam = 2,
        GetExportParam = 41,

        MassActionGridParam = 84,
        MassActionUpdateParam = 84,



        GetMerkObjectParam = 1,
        GetEntityFilteredParam = 1,
        GetEntityListParam = 1,
        GetItemsForAutocomplete = 1,
        GetCalendarDataParam = 1,
        CalendarFilterData = 1,
        UpdateCalendarDataParam = 1,
        SaveLayoutParam = 1,

        FilterSettingsParam = 1,
        MethodGetAutocompletePsCodeParam = 1

    }

    public static class SystemEnums
    {
        public static string[] SysColumns = new string[] { "CreatedBy_FK", "Created", "ModifiedBy_FK", "Modified", "Deleted" };
    }

    public class PohodaHelper
    {
        public static XNamespace nsRsp = "http://www.stormware.cz/schema/version_2/response.xsd";
        public static XNamespace nsRdc = "http://www.stormware.cz/schema/version_2/documentresponse.xsd";
        public static XNamespace nsTyp = "http://www.stormware.cz/schema/version_2/type.xsd";
        public static XNamespace nsLst = "http://www.stormware.cz/schema/version_2/list.xsd";
        public static XNamespace nsLStk = "http://www.stormware.cz/schema/version_2/list_stock.xsd";
        public static XNamespace nsLAdb = "http://www.stormware.cz/schema/version_2/list_addBook.xsd";
        public static XNamespace nsAcu = "http://www.stormware.cz/schema/version_2/accountingunit.xsd";
        public static XNamespace nsInv = "http://www.stormware.cz/schema/version_2/invoice.xsd";
        public static XNamespace nsVch = "http://www.stormware.cz/schema/version_2/voucher.xsd";
        public static XNamespace nsInt = "http://www.stormware.cz/schema/version_2/intDoc.xsd";
        public static XNamespace nsStk = "http://www.stormware.cz/schema/version_2/stock.xsd";
        public static XNamespace nsOrd = "http://www.stormware.cz/schema/version_2/order.xsd";
        public static XNamespace nsOfr = "http://www.stormware.cz/schema/version_2/offer.xsd";
        public static XNamespace nsEnq = "http://www.stormware.cz/schema/version_2/enquiry.xsd";
        public static XNamespace nsVyd = "http://www.stormware.cz/schema/version_2/vydejka.xsd";
        public static XNamespace nsPri = "http://www.stormware.cz/schema/version_2/prijemka.xsd";
        public static XNamespace nsBal = "http://www.stormware.cz/schema/version_2/balance.xsd";
        public static XNamespace nsPre = "http://www.stormware.cz/schema/version_2/prevodka.xsd";
        public static XNamespace nsVyr = "http://www.stormware.cz/schema/version_2/vyroba.xsd";
        public static XNamespace nsPro = "http://www.stormware.cz/schema/version_2/prodejka.xsd";
        public static XNamespace nsCon = "http://www.stormware.cz/schema/version_2/contract.xsd";
        public static XNamespace nsAdb = "http://www.stormware.cz/schema/version_2/addressbook.xsd";
        public static XNamespace nsPrm = "http://www.stormware.cz/schema/version_2/parameter.xsd";
        public static XNamespace nsLCon = "http://www.stormware.cz/schema/version_2/list_contract.xsd";
        public static XNamespace nsCtg = "http://www.stormware.cz/schema/version_2/category.xsd";
        public static XNamespace nsIpm = "http://www.stormware.cz/schema/version_2/intParam.xsd";
        public static XNamespace nsStr = "http://www.stormware.cz/schema/version_2/storage.xsd";
        public static XNamespace nsIdp = "http://www.stormware.cz/schema/version_2/individualPrice.xsd";
        public static XNamespace nsSup = "http://www.stormware.cz/schema/version_2/supplier.xsd";
        public static XNamespace nsPrn = "http://www.stormware.cz/schema/version_2/print.xsd";
        public static XNamespace nsAct = "http://www.stormware.cz/schema/version_2/accountancy.xsd";
        public static XNamespace nsBnk = "http://www.stormware.cz/schema/version_2/bank.xsd";
        public static XNamespace nsDat = "http://www.stormware.cz/schema/version_2/data.xsd";
        public static XNamespace nsFtr = "http://www.stormware.cz/schema/version_2/filter.xsd";
    }
}

