using AutoMapper;
using PTM.Application.DTOs.TaskDTOs;
using PTM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Application.Mappings
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<AppTask, TaskResponseDto>();
        }
    }
}
