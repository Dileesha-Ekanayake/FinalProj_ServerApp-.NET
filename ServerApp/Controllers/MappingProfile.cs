using AutoMapper;

namespace ServerApp.Controllers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, Employee>()
                .ForMember(emp => emp.GenderId, opt => opt.Ignore())
                .ForMember(emp => emp.DesignationId, opt => opt.Ignore())
                .ForMember(emp => emp.EmpstatusId, opt => opt.Ignore())
                .ForMember(emp => emp.EmptypeId, opt => opt.Ignore());

            CreateMap<User, User>()
                //.ForMember(usr => usr.Id, opt => opt.Ignore())
                .ForMember(usr => usr.UsestatusId, opt => opt.Ignore())
                .ForMember(usr => usr.UsetypeId, opt => opt.Ignore())
                .ForMember(usr => usr.EmployeeId, opt => opt.Ignore())
                .ForMember(usr => usr.Userroles, opt => opt.Ignore());


        }
    }
}
