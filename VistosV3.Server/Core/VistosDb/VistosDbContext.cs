using System;
using Core.VistosDb.Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Core.VistosDb
{
    public partial class VistosDbContext : DbContext
    {
        public VistosDbContext()
        {
        }

        public VistosDbContext(DbContextOptions<VistosDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Audit> Audit { get; set; }
        public virtual DbSet<BusinessUnit> BusinessUnit { get; set; }
        public virtual DbSet<ConfNotification> ConfNotification { get; set; }
        public virtual DbSet<CrmEntity> CrmEntity { get; set; }
        public virtual DbSet<DbObjectDbObject> DbObjectDbObject { get; set; }
        public virtual DbSet<DbObjectDocument> DbObjectDocument { get; set; }
        public virtual DbSet<DbObjectEmail> DbObjectEmail { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentAttachment> DocumentAttachment { get; set; }
        public virtual DbSet<Email> Email { get; set; }
        public virtual DbSet<EmailAccountFolder> EmailAccountFolder { get; set; }
        public virtual DbSet<EmailAttachment> EmailAttachment { get; set; }
        public virtual DbSet<Enumeration> Enumeration { get; set; }
        public virtual DbSet<EnumerationType> EnumerationType { get; set; }
        public virtual DbSet<LocalizationArea> LocalizationArea { get; set; }
        public virtual DbSet<LocalizationLanguage> LocalizationLanguage { get; set; }
        public virtual DbSet<LocalizationString> LocalizationString { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<NotificationConfiguration> NotificationConfiguration { get; set; }
        public virtual DbSet<ProjectionReport> ProjectionReport { get; set; }
        public virtual DbSet<Reminder> Reminder { get; set; }
        public virtual DbSet<ReminderSettings> ReminderSettings { get; set; }
        public virtual DbSet<ReportAccessRights> ReportAccessRights { get; set; }
        public virtual DbSet<RichTextAttachment> RichTextAttachment { get; set; }
        public virtual DbSet<Signature> Signature { get; set; }
        public virtual DbSet<TextTemplate> TextTemplate { get; set; }
        public virtual DbSet<TrackChanges> TrackChanges { get; set; }
        public virtual DbSet<TrackChangesType> TrackChangesType { get; set; }
        public virtual DbSet<UserAvatar> UserAvatar { get; set; }
        public virtual DbSet<UserEmailAccount> UserEmailAccount { get; set; }
        public virtual DbSet<vwBusinessUnit> vwBusinessUnit { get; set; }
        public virtual DbSet<vwDiscussionMessage> vwDiscussionMessage { get; set; }
        public virtual DbSet<vwLocalization> vwLocalization { get; set; }
        public virtual DbSet<vwNumberingSequence> vwNumberingSequence { get; set; }
        public virtual DbSet<vwParticipant> vwParticipant { get; set; }
        public virtual DbSet<vwPohodaDbObjectConfiguration> vwPohodaDbObjectConfiguration { get; set; }
        public virtual DbSet<vwProjection> vwProjection { get; set; }
        public virtual DbSet<vwProjectionAction> vwProjectionAction { get; set; }
        public virtual DbSet<vwProjectionActionColumnMapping> vwProjectionActionColumnMapping { get; set; }
        public virtual DbSet<vwProjectionColumn> vwProjectionColumn { get; set; }
        public virtual DbSet<vwProjectionColumnLocalization> vwProjectionColumnLocalization { get; set; }
        public virtual DbSet<vwProjectionRelation> vwProjectionRelation { get; set; }
        public virtual DbSet<vwRole> vwRole { get; set; }
        public virtual DbSet<vwSystemSettings> vwSystemSettings { get; set; }
        public virtual DbSet<vwUser> vwUser { get; set; }
        public virtual DbSet<vwUserAuthToken> vwUserAuthToken { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                  .AddJsonFile("appsettings.json")
                  .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Audit>(entity => { entity.ToTable("Audit", "crm"); });
            modelBuilder.Entity<BusinessUnit>(entity => { entity.ToTable("BusinessUnit", "crm"); });
            modelBuilder.Entity<ConfNotification>(entity => { entity.ToTable("ConfNotification", "crm"); });
            modelBuilder.Entity<CrmEntity>(entity => { entity.ToTable("CrmEntity", "crm"); });
            modelBuilder.Entity<DbObjectDbObject>(entity => { entity.ToTable("DbObjectDbObject", "crm"); });
            modelBuilder.Entity<DbObjectDocument>(entity => { entity.ToTable("DbObjectDocument", "crm"); });
            modelBuilder.Entity<DbObjectEmail>(entity => { entity.ToTable("DbObjectEmail", "crm"); });
            modelBuilder.Entity<Document>(entity => { entity.ToTable("Document", "crm"); });
            modelBuilder.Entity<DocumentAttachment>(entity => { entity.ToTable("DocumentAttachment", "crm"); });
            modelBuilder.Entity<Email>(entity => { entity.ToTable("Email", "crm"); });
            modelBuilder.Entity<EmailAccountFolder>(entity => { entity.ToTable("EmailAccountFolder", "crm"); });
            modelBuilder.Entity<EmailAttachment>(entity => { entity.ToTable("EmailAttachment", "crm"); });
            modelBuilder.Entity<Enumeration>(entity => { entity.ToTable("Enumeration", "crm"); });
            modelBuilder.Entity<EnumerationType>(entity => { entity.ToTable("EnumerationType", "crm"); });
            modelBuilder.Entity<LocalizationArea>(entity => { entity.ToTable("LocalizationArea", "crm"); });
            modelBuilder.Entity<LocalizationLanguage>(entity => { entity.ToTable("LocalizationLanguage", "crm"); });
            modelBuilder.Entity<LocalizationString>(entity => { entity.ToTable("LocalizationString", "crm"); });
            modelBuilder.Entity<Log>(entity => { entity.ToTable("Log", "crm"); });
            modelBuilder.Entity<NotificationConfiguration>(entity => { entity.ToTable("NotificationConfiguration", "crm"); });
            modelBuilder.Entity<ProjectionReport>(entity => { entity.ToTable("ProjectionReport", "crm"); });
            modelBuilder.Entity<Reminder>(entity => { entity.ToTable("Reminder", "crm"); });
            modelBuilder.Entity<ReminderSettings>(entity => { entity.ToTable("ReminderSettings", "crm"); });
            modelBuilder.Entity<ReportAccessRights>(entity => { entity.ToTable("ReportAccessRights", "crm"); });
            modelBuilder.Entity<RichTextAttachment>(entity => { entity.ToTable("RichTextAttachment", "crm"); });
            modelBuilder.Entity<Signature>(entity => { entity.ToTable("Signature", "crm"); });
            modelBuilder.Entity<TextTemplate>(entity => { entity.ToTable("TextTemplate", "crm"); });
            modelBuilder.Entity<TrackChanges>(entity => { entity.ToTable("TrackChanges", "crm"); });
            modelBuilder.Entity<TrackChangesType>(entity => { entity.ToTable("TrackChangesType", "crm"); });
            modelBuilder.Entity<UserAvatar>(entity => { entity.ToTable("UserAvatar", "crm"); });
            modelBuilder.Entity<UserEmailAccount>(entity => { entity.ToTable("UserEmailAccount", "crm"); });
            modelBuilder.Entity<vwBusinessUnit>(entity => { entity.ToTable("vwBusinessUnit", "crm"); });
            modelBuilder.Entity<vwDiscussionMessage>(entity => { entity.ToTable("vwDiscussionMessage", "crm"); });
            modelBuilder.Entity<vwLocalization>(entity => { entity.ToTable("vwLocalization", "crm"); });
            modelBuilder.Entity<vwNumberingSequence>(entity => { entity.ToTable("vwNumberingSequence", "crm"); });
            modelBuilder.Entity<vwParticipant>(entity => { entity.ToTable("vwParticipant", "crm"); });
            modelBuilder.Entity<vwPohodaDbObjectConfiguration>(entity => { entity.ToTable("vwPohodaDbObjectConfiguration", "crm"); });
            modelBuilder.Entity<vwProjection>(entity => { entity.ToTable("vwProjection", "crm"); });
            modelBuilder.Entity<vwProjectionAction>(entity => { entity.ToTable("vwProjectionAction", "crm"); });
            modelBuilder.Entity<vwProjectionActionColumnMapping>(entity => { entity.ToTable("vwProjectionActionColumnMapping", "crm"); });
            modelBuilder.Entity<vwProjectionColumn>(entity => { entity.ToTable("vwProjectionColumn", "crm"); });
            modelBuilder.Entity<vwProjectionColumnLocalization>(entity => { entity.ToTable("vwProjectionColumnLocalization", "crm"); });
            modelBuilder.Entity<vwProjectionRelation>(entity => { entity.ToTable("vwProjectionRelation", "crm"); });
            modelBuilder.Entity<vwRole>(entity => { entity.ToTable("vwRole", "crm"); });
            modelBuilder.Entity<vwSystemSettings>(entity => { entity.ToTable("vwSystemSettings", "crm"); });
            modelBuilder.Entity<vwUser>(entity => { entity.ToTable("vwUser", "crm"); });
            modelBuilder.Entity<vwUserAuthToken>(entity => { entity.ToTable("vwUserAuthToken", "crm"); });

            modelBuilder.Entity<vwBusinessUnit>().HasKey(c => new { c.BusinessUnit_Id });
            modelBuilder.Entity<vwDiscussionMessage>().HasKey(c => new { c.Id });
            modelBuilder.Entity<vwLocalization>().HasKey(c => new { c.LocalizationString_ID });
            modelBuilder.Entity<vwNumberingSequence>().HasKey(c => new { c.Id });
            modelBuilder.Entity<vwParticipant>().HasKey(c => new { c.Participant_Id });
            modelBuilder.Entity<vwPohodaDbObjectConfiguration>().HasKey(c => new { c.Conf_Id });
            modelBuilder.Entity<vwProjection>().HasKey(c => new { c.Projection_Id, c.Profile_Id });
            modelBuilder.Entity<vwProjectionAction>().HasKey(c => new { c.Id });
            modelBuilder.Entity<vwProjectionActionColumnMapping>().HasKey(c => new { c.Id });
            modelBuilder.Entity<vwProjectionColumn>().HasKey(c => new { c.ProjectionColumn_Id, c.Profile_Id });
            modelBuilder.Entity<vwProjectionColumnLocalization>().HasKey(c => new { c.ProjectionColumn_Id });
            modelBuilder.Entity<vwProjectionRelation>().HasKey(c => new { c.ProjectionRelation_Id });
            modelBuilder.Entity<vwRole>().HasKey(c => new { c.Role_ID });
            modelBuilder.Entity<vwSystemSettings>().HasKey(c => new { c.VistosUrl });
            modelBuilder.Entity<vwUser>().HasKey(c => new { c.Id });
            modelBuilder.Entity<vwUserAuthToken>().HasKey(c => new { c.Token });

            modelBuilder.Entity<Document>().Property(b => b.CaptionDisplay).ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<Document>().Property(b => b.CaptionSort).ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<Document>().Property(b => b.SearchColumn).ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Audit>().Property(b => b.CaptionDisplay).ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<Audit>().Property(b => b.CaptionSort).ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Log>().Property(b => b.CaptionDisplay).ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<Log>().Property(b => b.CaptionSort).ValueGeneratedOnAddOrUpdate();


            //Audit
            //BusinessUnit
            //ConfNotification
            //CrmEntity
            //DbObjectDbObject
            //DbObjectDocument
            //DbObjectEmail
            //Document
            //DocumentAttachment
            //Email
            //EmailAccountFolder
            //EmailAttachment
            //Enumeration
            //EnumerationType
            //LocalizationArea
            //LocalizationLanguage
            //LocalizationString
            //Log
            //NotificationConfiguration
            //ProjectionReport
            //Reminder
            //ReminderSettings
            //ReportAccessRights
            //RichTextAttachment
            //Signature
            //TextTemplate
            //TrackChanges
            //TrackChangesType
            //UserAvatar
            //UserEmailAccount
            //vwBusinessUnit
            //vwDiscussionMessage
            //vwLocalization
            //vwNumberingSequence
            //vwParticipant
            //vwPohodaDbObjectConfiguration
            //vwProjection
            //vwProjectionAction
            //vwProjectionActionColumnMapping
            //vwProjectionColumn
            //vwProjectionColumnLocalization
            //vwProjectionRelation
            //vwRole
            //vwSystemSettings
            //vwUser
            //vwUserAuthToken





        }
    }
}
