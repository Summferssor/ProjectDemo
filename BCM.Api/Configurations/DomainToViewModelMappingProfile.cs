using AutoMapper;
using BCM.Models.BCM.BCMModels.ModelModification;
using BCM.Models.BCM.BCMModels.ModelCreation;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;

namespace BCM.Api.Configurations
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName => "DomainToViewModelMappings";
        public DomainToViewModelMappingProfile()
        {
            CreateMap<TbRole, RoleModification>();
            CreateMap<TbRole, RoleCreation>();
            CreateMap<TbRole, RoleView>();

            CreateMap<TbUser, UserModification>();
            CreateMap<TbUser, UserCreation>();
            CreateMap<TbUser, UserView>();

            CreateMap<TbDictionary, DictionaryModification>();
            CreateMap<TbDictionary, DictionaryCreation>();
            CreateMap<TbDictionary, DictionaryView>();




            CreateMap<TbLog, LogModification>();
            CreateMap<TbLog, LogCreation>();
            CreateMap<TbLog, LogView>();

            CreateMap<TbModule, ModuleModification>();
            CreateMap<TbModule, ModuleCreation>();
            CreateMap<TbModule, ModuleView>();


            CreateMap<TbRoleModule, RoleModuleModification>();
            CreateMap<TbRoleModule, RoleModuleCreation>();
            CreateMap<TbRoleModule, RoleModuleView>();

            CreateMap<TbUserRole, UserRoleModification>();
            CreateMap<TbUserRole, UserRoleCreation>();
            CreateMap<TbUserRole, UserRoleView>();

            CreateMap<Company, CompanyModification>();
            CreateMap<Company, CompanyCreation>();
            CreateMap<Company, CompanyView>();

            CreateMap<Employee, EmployeeModification>();
            CreateMap<Employee, EmployeeCreation>();
            CreateMap<Employee, EmployeeView>();

            CreateMap<GiveCount, GiveCountModification>();
            CreateMap<GiveCount, GiveCountCreation>();
            CreateMap<GiveCount, GiveCountView>();

            CreateMap<GiveMoney, GiveMoneyModification>();
            CreateMap<GiveMoney, GiveMoneyCreation>();
            CreateMap<GiveMoney, GiveMoneyView>();

            CreateMap<Rules, RulesModification>();
            CreateMap<Rules, RulesCreation>();
            CreateMap<Rules, RulesView>();

            CreateMap<UseRecord, UseRecordModification>();
            CreateMap<UseRecord, UseRecordCreation>();
            CreateMap<UseRecord, UseRecordView>();

        }
    }
}
