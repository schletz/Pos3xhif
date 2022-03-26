using AutoMapper;
using ListDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListDemo.Dto
{
    internal class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<Student, StudentDto>();
            CreateMap<StudentDto, Student>()
                .BeforeMap((src, dst) =>
                {
                    if (string.IsNullOrEmpty(src.Firstname)) { throw new ApplicationException("Invalid firstname."); }
                    if (string.IsNullOrEmpty(src.Lastname)) { throw new ApplicationException("Invalid larstname."); }
                    if (src.Gender is null) { throw new ApplicationException("Invalid gender."); }
                    if (src.Schoolclass is null) { throw new ApplicationException("Invalid schoolclass."); }
                });
        }
    }
}
