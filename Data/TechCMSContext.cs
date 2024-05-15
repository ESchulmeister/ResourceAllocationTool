using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ResourceAllocationTool.Data
{
    public partial class TechCMSContext : DbContext
    {
        public TechCMSContext()
        {
        }

        public TechCMSContext(DbContextOptions<TechCMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<HourAllocation> HourAllocations { get; set; }
        public virtual DbSet<Period> Periods { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectUser> ProjectUsers { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSupervisor> UserSupervisors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=TechCMSDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<HourAllocation>(entity =>
            {
                entity.HasKey(e => e.HaId)
                    .HasName("PK_HourAllocations");

                entity.ToTable("HourAllocation");

                entity.Property(e => e.HaId).HasColumnName("haID");

                entity.Property(e => e.HaActualHours).HasColumnName("haActualHours");

                entity.Property(e => e.HaCreatedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("haCreatedBy")
                    .HasDefaultValueSql("('admin')");

                entity.Property(e => e.HaCreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("haCreatedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.HaEstimatedHours).HasColumnName("haEstimatedHours");

                entity.Property(e => e.HaModifiedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("haModifiedBy")
                    .HasDefaultValueSql("('admin')");

                entity.Property(e => e.HaModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("haModifiedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.HaPeriodId).HasColumnName("haPeriodID");

                entity.Property(e => e.HapuId).HasColumnName("hapuID");

                entity.HasOne(d => d.HaPeriod)
                    .WithMany(p => p.HourAllocations)
                    .HasForeignKey(d => d.HaPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HourAllocation_Period");

                entity.HasOne(d => d.Hapu)
                    .WithMany(p => p.HourAllocations)
                    .HasForeignKey(d => d.HapuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HourAllocation_Project_Users");
            });

            modelBuilder.Entity<Period>(entity =>
            {
                entity.HasKey(e => e.PerId);

                entity.ToTable("Period");

                entity.Property(e => e.PerId).HasColumnName("perID");

                entity.Property(e => e.PerActive)
                    .IsRequired()
                    .HasColumnName("perActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PerCreatedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("perCreatedBy");

                entity.Property(e => e.PerCreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("perCreatedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PerModifiedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("perModifiedBy");

                entity.Property(e => e.PerModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("perModifiedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PerName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("perName");

                entity.Property(e => e.PerWorkDays).HasColumnName("perWorkDays");

                entity.Property(e => e.PerWorkHours).HasColumnName("perWorkHours");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.PrjId);

                entity.ToTable("PROJECTS");

                entity.Property(e => e.PrjId)
                    .ValueGeneratedNever()
                    .HasColumnName("PRJ_ID");

                entity.Property(e => e.PrjActive).HasColumnName("PRJ_ACTIVE");

                entity.Property(e => e.PrjApps)
                    .HasColumnName("PRJ_APPS")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PrjArtFileMask)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_ART_FILE_MASK");

                entity.Property(e => e.PrjArtFolder)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_ART_FOLDER");

                entity.Property(e => e.PrjArtNumMask)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_ART_NUM_MASK");

                entity.Property(e => e.PrjCopytoRoot)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_COPYTO_ROOT");

                entity.Property(e => e.PrjCreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_CREATED_BY");

                entity.Property(e => e.PrjCreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("PRJ_CREATED_DATE");

                entity.Property(e => e.PrjCsdbName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_CSDB_NAME");

                entity.Property(e => e.PrjCusId).HasColumnName("PRJ_CUS_ID");

                entity.Property(e => e.PrjDesc)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_DESC");

                entity.Property(e => e.PrjEntityFile)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_ENTITY_FILE");

                entity.Property(e => e.PrjEntityRegex)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_ENTITY_REGEX");

                entity.Property(e => e.PrjFlags).HasColumnName("PRJ_FLAGS");

                entity.Property(e => e.PrjFolderName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_FOLDER_NAME");

                entity.Property(e => e.PrjMasterArtFolder)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_MASTER_ART_FOLDER");

                entity.Property(e => e.PrjModifiedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_MODIFIED_BY");

                entity.Property(e => e.PrjModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("PRJ_MODIFIED_DATE");

                entity.Property(e => e.PrjModifiedTimestamp)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("PRJ_MODIFIED_TIMESTAMP");

                entity.Property(e => e.PrjName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_NAME");

                entity.Property(e => e.PrjPublisherVersion).HasColumnName("PRJ_PUBLISHER_VERSION");

                entity.Property(e => e.PrjRemoteWorkingDir)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_REMOTE_WORKING_DIR");

                entity.Property(e => e.PrjStatus).HasColumnName("PRJ_STATUS");

                entity.Property(e => e.PrjStatusDate)
                    .HasColumnType("datetime")
                    .HasColumnName("PRJ_STATUS_DATE");

                entity.Property(e => e.PrjStatusUsrId).HasColumnName("PRJ_STATUS_USR_ID");

                entity.Property(e => e.PrjTxdId).HasColumnName("PRJ_TXD_ID");

                entity.Property(e => e.PrjWorkingDir)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRJ_WORKING_DIR");
            });

            modelBuilder.Entity<ProjectUser>(entity =>
            {
                entity.HasKey(e => e.PuId)
                    .HasName("PK_Project_Users");

                entity.ToTable("Project_User");

                entity.Property(e => e.PuId).HasColumnName("puID");

                entity.Property(e => e.PuActive)
                    .IsRequired()
                    .HasColumnName("puActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PuCreatedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("puCreatedBy")
                    .HasDefaultValueSql("('admin')");

                entity.Property(e => e.PuCreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("puCreatedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PuModifiedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("puModifiedBy")
                    .HasDefaultValueSql("('admin')");

                entity.Property(e => e.PuModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("puModifiedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PuProjectId).HasColumnName("puProjectID");

                entity.Property(e => e.PuRoleId).HasColumnName("puRoleID");

                entity.Property(e => e.PuUserId).HasColumnName("puUserID");

                entity.HasOne(d => d.PuProject)
                    .WithMany(p => p.ProjectUsers)
                    .HasForeignKey(d => d.PuProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Project_User_PROJECTS");

                entity.HasOne(d => d.PuUser)
                    .WithMany(p => p.ProjectUsers)
                    .HasForeignKey(d => d.PuUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Project_User_USERS");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RId)
                    .HasName("PK_Roles");

                entity.ToTable("Role");

                entity.Property(e => e.RId).HasColumnName("rID");

                entity.Property(e => e.RActive)
                    .IsRequired()
                    .HasColumnName("rActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RAdministrator).HasColumnName("rAdministrator");

                entity.Property(e => e.RCreatedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("rCreatedBy");

                entity.Property(e => e.RCreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("rCreatedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RModifiedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("rModifiedBy");

                entity.Property(e => e.RModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("rModifiedDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("rName");

                entity.Property(e => e.RSupervisor).HasColumnName("rSupervisor");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UsrId);

                entity.ToTable("USERS");

                entity.Property(e => e.UsrId)
                    .ValueGeneratedNever()
                    .HasColumnName("USR_ID");

                entity.Property(e => e.UsrActive)
                    .HasColumnName("USR_ACTIVE")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UsrApps).HasColumnName("USR_APPS");

                entity.Property(e => e.UsrClock).HasColumnName("USR_CLOCK");

                entity.Property(e => e.UsrCreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USR_CREATED_BY");

                entity.Property(e => e.UsrCreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("USR_CREATED_DATE");

                entity.Property(e => e.UsrCsdbName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USR_CSDB_NAME");

                entity.Property(e => e.UsrDefaultRole).HasColumnName("USR_DefaultRole");

                entity.Property(e => e.UsrEmail)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("USR_EMAIL");

                entity.Property(e => e.UsrFirst)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("USR_FIRST");

                entity.Property(e => e.UsrFlags).HasColumnName("USR_FLAGS");

                entity.Property(e => e.UsrFte).HasColumnName("USR_FTE");

                entity.Property(e => e.UsrLast)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("USR_LAST");

                entity.Property(e => e.UsrLogin)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("USR_LOGIN");

                entity.Property(e => e.UsrModifiedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USR_MODIFIED_BY");

                entity.Property(e => e.UsrModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("USR_MODIFIED_DATE");

                entity.Property(e => e.UsrModifiedTimestamp)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("USR_MODIFIED_TIMESTAMP");

                entity.Property(e => e.UsrNetLogin)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USR_NET_LOGIN");

                entity.Property(e => e.UsrPassword)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("USR_PASSWORD");

                entity.Property(e => e.UsrSettings)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("USR_SETTINGS");

                entity.HasOne(d => d.UsrDefaultRoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UsrDefaultRole)
                    .HasConstraintName("FK_USERS_Role");
            });

            modelBuilder.Entity<UserSupervisor>(entity =>
            {
                entity.HasKey(e => e.UsId);

                entity.ToTable("User_Supervisor");

                entity.Property(e => e.UsId).HasColumnName("usID");

                entity.Property(e => e.UsUserId).HasColumnName("usUserID");

                entity.Property(e => e.UsUserSupervisorId).HasColumnName("usUser_SupervisorID");

                entity.HasOne(d => d.UsUser)
                    .WithMany(p => p.UserSupervisorUsUsers)
                    .HasForeignKey(d => d.UsUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Supervisor_USERS");

                entity.HasOne(d => d.UsUserSupervisor)
                    .WithMany(p => p.UserSupervisorUsUserSupervisors)
                    .HasForeignKey(d => d.UsUserSupervisorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Supervisor_Supervisor");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

