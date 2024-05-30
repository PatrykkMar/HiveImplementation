using AutoMapper;
using HiveGame.BusinessLogic.Models.DTOs;
using HiveGame.BusinessLogic.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Vertex, VertexDTO>()
                .ForMember(dest => dest.Insect, opt => opt.MapFrom(src => src.CurrentInsect != null ? (InsectType?)src.CurrentInsect.Type : null));
        }
    }
}
