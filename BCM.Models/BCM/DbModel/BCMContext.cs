using System;
using System.Threading;
using System.Threading.Tasks;
using BCM.Models.BCM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BCM.Models.BCM.DbModel
{
    public partial class BCMContext : DbContext, IUnitOfWork
    {
        public BCMContext()
        {
        }

        public BCMContext(DbContextOptions<BCMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<GiveCount> GiveCount { get; set; }
        public virtual DbSet<GiveMoney> GiveMoney { get; set; }
        public virtual DbSet<Rules> Rules { get; set; }
        public virtual DbSet<TbDictionary> TbDictionary { get; set; }
        public virtual DbSet<TbLog> TbLog { get; set; }
        public virtual DbSet<TbModule> TbModule { get; set; }
        public virtual DbSet<TbRole> TbRole { get; set; }
        public virtual DbSet<TbRoleModule> TbRoleModule { get; set; }
        public virtual DbSet<TbUser> TbUser { get; set; }
        public virtual DbSet<TbUserRole> TbUserRole { get; set; }
        public virtual DbSet<UseRecord> UseRecord { get; set; }
        public DbQuery<Vw_GiveRecord> Vw_GiveRecords { get; set; }
        public DbQuery<Vw_UseRecord> Vw_UseRecords { get; set; }
        public bool Save()
        {
            return SaveChanges() >= 0;
        }
        public bool Save(bool acceptAllChangesOnSuccess)
        {
            return SaveChanges(acceptAllChangesOnSuccess) >= 0;
        }

        public async Task<bool> SaveAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken) >= 0;
        }

        public async Task<bool> SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAsync(cancellationToken) >= 0;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("company");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CountOrNot)
                    .HasColumnName("countOrNot")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ParentId)
                    .HasColumnName("parentId")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Sort).HasColumnName("sort");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.JobId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("employee");

                entity.Property(e => e.JobId)
                    .HasColumnName("jobId")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AddAt)
                    .HasColumnName("addAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.AvailableCount).HasColumnName("availableCount");

                entity.Property(e => e.AvailableMoney).HasColumnName("availableMoney");

                entity.Property(e => e.CompanyId)
                    .HasColumnName("companyId")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);


                entity.Property(e => e.Numbers).HasColumnName("numbers");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QrCodeNumber)
                    .HasColumnName("qrCodeNumber")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QrCodeUrl)
                    .HasColumnName("qrCodeURL")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Sort).HasColumnName("sort");

                entity.Property(e => e.UpdateCountAt)
                    .HasColumnName("updateCountAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdateMoneyAt)
                    .HasColumnName("updateMoneyAt")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<GiveCount>(entity =>
            {
                entity.HasKey(e => e.GiveRecordId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("giveCount");

                entity.Property(e => e.GiveRecordId)
                    .HasColumnName("giveRecordId")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AddAt)
                    .HasColumnName("addAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.GiveCountType)
                    .HasColumnName("giveCountType")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.GiveNumber).HasColumnName("giveNumber");

                entity.Property(e => e.JobId)
                    .HasColumnName("jobId")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GiveMoney>(entity =>
            {
                entity.HasKey(e => e.GiveRecordId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("giveMoney");

                entity.Property(e => e.GiveRecordId)
                    .HasColumnName("giveRecordId")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AddAt)
                    .HasColumnName("addAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.GiveMoneyType)
                    .HasColumnName("giveMoneyType")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.GiveMoney1).HasColumnName("giveMoney");

                entity.Property(e => e.JobId)
                    .HasColumnName("jobId")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Rules>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("rules");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.EndTime)
                    .HasColumnName("endTime")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Sort).HasColumnName("sort");

                entity.Property(e => e.StartTime)
                    .HasColumnName("startTime")
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TbDictionary>(entity =>
            {
                entity.HasKey(e => e.DictionaryId);

                entity.ToTable("tb_Dictionary");

                entity.Property(e => e.DictionaryId)
                    .HasColumnName("dictionaryId")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.DictionaryCode)
                    .HasColumnName("dictionaryCode")
                    .HasMaxLength(50);

                entity.Property(e => e.DictionaryName)
                    .HasColumnName("dictionaryName")
                    .HasMaxLength(50);

                entity.Property(e => e.DictionaryOrderNum).HasColumnName("dictionaryOrderNum");

                entity.Property(e => e.DictionaryRemarks)
                    .HasColumnName("dictionaryRemarks")
                    .HasMaxLength(200);

                entity.Property(e => e.ParentDictionaryCode)
                    .HasColumnName("parentDictionaryCode")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TbLog>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("tb_Log");

                entity.Property(e => e.LogId)
                    .HasColumnName("logID")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.LogOperationIpv4)
                    .HasColumnName("logOperationIPv4")
                    .HasMaxLength(50);

                entity.Property(e => e.LogOperationIpv6)
                    .HasColumnName("logOperationIPv6")
                    .HasMaxLength(50);

                entity.Property(e => e.LogOperationName)
                    .HasColumnName("logOperationName")
                    .HasMaxLength(50);

                entity.Property(e => e.LogOperationTime)
                    .HasColumnName("logOperationTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LogOperationUrl)
                    .HasColumnName("logOperationURL")
                    .HasMaxLength(200);

                entity.Property(e => e.LogOperationUserName)
                    .HasColumnName("logOperationUserName")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LogRemarks)
                    .HasColumnName("logRemarks")
                    .HasMaxLength(200);

                entity.Property(e => e.LogType)
                    .HasColumnName("logType")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TbModule>(entity =>
            {
                entity.HasKey(e => e.ModuleId);

                entity.ToTable("tb_Module");

                entity.Property(e => e.ModuleId)
                    .HasColumnName("moduleId")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ModuleDescription)
                    .HasColumnName("moduleDescription")
                    .HasMaxLength(200);

                entity.Property(e => e.ModuleIcon)
                    .HasColumnName("moduleIcon")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModuleName)
                    .IsRequired()
                    .HasColumnName("moduleName")
                    .HasMaxLength(20);

                entity.Property(e => e.ModuleOrderNum).HasColumnName("moduleOrderNum");

                entity.Property(e => e.ModuleRemarks)
                    .HasColumnName("moduleRemarks")
                    .HasMaxLength(200);

                entity.Property(e => e.ModuleStatus)
                    .HasColumnName("moduleStatus")
                    .HasMaxLength(50);

                entity.Property(e => e.ModuleUrl)
                    .HasColumnName("moduleURL")
                    .HasMaxLength(200);

                entity.Property(e => e.ParentModuleId)
                    .HasColumnName("parentModuleId")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TbRole>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.ToTable("tb_Role");

                entity.Property(e => e.RoleId)
                    .HasColumnName("roleId")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ParentRoleId)
                    .IsRequired()
                    .HasColumnName("parentRoleId")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.RoleDescription)
                    .HasColumnName("roleDescription")
                    .HasMaxLength(200);

                entity.Property(e => e.RoleLevel).HasColumnName("roleLevel");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasColumnName("roleName")
                    .HasMaxLength(50);

                entity.Property(e => e.RoleOrderNum).HasColumnName("roleOrderNum");

                entity.Property(e => e.RoleRemarks)
                    .HasColumnName("roleRemarks")
                    .HasMaxLength(200);

                entity.Property(e => e.RoleStatus)
                    .HasColumnName("roleStatus")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TbRoleModule>(entity =>
            {
                entity.HasKey(e => e.RoleModuleId);

                entity.ToTable("tb_RoleModule");

                entity.Property(e => e.RoleModuleId)
                    .HasColumnName("roleModuleId")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ModuleId)
                    .HasColumnName("moduleId")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId)
                    .HasColumnName("roleId")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TbUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("tb_User");

                entity.Property(e => e.UserId)
                    .HasColumnName("userId")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.UserAddress)
                    .HasColumnName("userAddress")
                    .HasMaxLength(100);

                entity.Property(e => e.UserCellphone)
                    .HasColumnName("userCellphone")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.UserDepartmentId)
                    .HasColumnName("userDepartmentId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserEmail)
                    .HasColumnName("userEmail")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasColumnName("userName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserPassWord)
                    .IsRequired()
                    .HasColumnName("userPassWord")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.UserRealName)
                    .HasColumnName("userRealName")
                    .HasMaxLength(20);

                entity.Property(e => e.UserRemarks)
                    .HasColumnName("userRemarks")
                    .HasMaxLength(200);

                entity.Property(e => e.UserSex)
                    .HasColumnName("userSex")
                    .HasMaxLength(50);

                entity.Property(e => e.UserStatus)
                    .HasColumnName("userStatus")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TbUserRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleId);

                entity.ToTable("tb_UserRole");

                entity.Property(e => e.UserRoleId)
                    .HasColumnName("userRoleId")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasColumnName("roleId")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("userId")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UseRecord>(entity =>
            {
                entity.HasKey(e => e.UseRecordId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("useRecord");

                entity.Property(e => e.UseRecordId)
                    .HasColumnName("useRecordId")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AddAt)
                    .HasColumnName("addAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.GiveCountType)
                    .HasColumnName("giveCountType")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.JobId)
                    .HasColumnName("jobId")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UseMoney).HasColumnName("useMoney");

                entity.Property(e => e.UseNumber).HasColumnName("useNumber");
            });

            modelBuilder.Query<Vw_GiveRecord>(v =>
            {
                v.ToView("vw_giveRecord");
                v.Property(p => p.GiveRecordId).HasColumnName("giveRecordId");
                v.Property(p => p.AddAt).HasColumnName("addAt");
                v.Property(p => p.GiveMoneyOrNumType).HasColumnName("giveMoneyType");
                v.Property(p => p.GiveMoneyOrNum).HasColumnName("giveMoney");
                v.Property(p => p.JobId).HasColumnName("jobId");
                v.Property(p => p.Remarks).HasColumnName("remarks");
                v.Property(p => p.UseType).HasColumnName("useType");
                v.Property(p => p.GiveType).HasColumnName("giveType");
            });

            modelBuilder.Query<Vw_UseRecord>(v =>
            {
                v.ToView("vw_useRecord");
                v.Property(p => p.UseRecordId).HasColumnName("useRecordId");
                v.Property(p => p.AddAt).HasColumnName("addAt");
                v.Property(p => p.GiveCountType).HasColumnName("giveCountType");
                v.Property(p => p.UseMoney).HasColumnName("useMoney");
                v.Property(p => p.JobId).HasColumnName("jobId");
                v.Property(p => p.Remarks).HasColumnName("remarks");
                v.Property(p => p.UseType).HasColumnName("useType");
                v.Property(p => p.UseNumber).HasColumnName("useNumber");
            });
        }
    }
}
