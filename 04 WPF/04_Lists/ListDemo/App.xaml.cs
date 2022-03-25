using AutoMapper;
using ListDemo.Dto;
using ListDemo.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ListDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly IMapper Mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Student, StudentDto>();
            cfg.CreateMap<StudentDto, Student>();
        }).CreateMapper();
    }
}
