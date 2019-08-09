using BCM.IRepositories;
using BCM.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BCM.Api.Configurations
{
    public static class RepositoryConfiguration
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            #region  system
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserRoleRepository, UserRoleRepository>();
            services.AddTransient<IModuleRepository, ModuleRepository>();
            
            services.AddTransient<IDictionaryRepository, DictionaryRepository>();
            services.AddTransient<ILogRepository, LogRepository>();

            services.AddTransient<IRoleModuleRepository, RoleModuleRepository>();
            #endregion

            #region work
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<IGiveCountRepository, GiveCountRepository>();
            services.AddTransient<IGiveMoneyRepository, GiveMoneyRepository>();
            services.AddTransient<IRulesRepository, RulesRepository>();
            services.AddTransient<IUseRecordRepository, UseRecordRepository>();
            #endregion
        }
    }
}