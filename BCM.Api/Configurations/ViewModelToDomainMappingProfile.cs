using AutoMapper;
using BCM.Models.BCM;
using BCM.Models.BCM.BCMModels.ModelCreation;
using BCM.Models.BCM.BCMModels.ModelModification;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM.DbModel;

namespace BCM.Api.Configurations
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public override string ProfileName => "ViewModelToDomainMappings";
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<RoleModification, TbRole>();
            CreateMap<RoleView, TbRole>();
            CreateMap<RoleCreation, TbRole>();

            CreateMap<UserRoleModification, TbUserRole>();
            CreateMap<UserRoleView, TbUserRole>();
            CreateMap<UserRoleCreation,TbUserRole>();

            CreateMap<RoleModuleModification, TbRoleModule>();
            CreateMap<RoleModuleView, TbRoleModule>();
            CreateMap<RoleModuleCreation, TbRoleModule>();


            CreateMap<ModuleModification, TbModule>();
            CreateMap<ModuleView, TbModule>();
            CreateMap<ModuleCreation, TbModule>();

            CreateMap<LogModification, TbLog>();
            CreateMap<LogView, TbLog>();
            CreateMap<LogCreation, TbLog>();

            CreateMap<UserModification, TbUser>();
            CreateMap<UserView, TbUser>();
            CreateMap<UserCreation, TbUser>();

            CreateMap<CompanyModification, Company>();
            CreateMap<CompanyView, Company>();
            CreateMap<CompanyCreation, Company>();

            CreateMap<EmployeeModification, Employee>();
            CreateMap<EmployeeCreation, Employee>();
            CreateMap<EmployeeView, Employee>();

            CreateMap<GiveCountModification, GiveCount>();
            CreateMap<GiveCountView, GiveCount>();
            CreateMap<GiveCountCreation, GiveCount>();

            CreateMap<GiveMoneyModification, GiveMoney>();
            CreateMap<GiveMoneyView, GiveMoney>();
            CreateMap<GiveMoneyCreation, GiveMoney>();

            CreateMap<RulesModification, Rules>();
            CreateMap<RulesCreation, Rules>();
            CreateMap<RulesView, Rules>();

            CreateMap<UseRecordModification, UseRecord>();
            CreateMap<UseRecordView, UseRecord>();
            CreateMap<UseRecordCreation, UseRecord>();

            CreateMap<DictionaryModification, TbDictionary>();
            CreateMap<DictionaryView, TbDictionary>();
            CreateMap<DictionaryCreation, TbDictionary>();

        }
    }
}
