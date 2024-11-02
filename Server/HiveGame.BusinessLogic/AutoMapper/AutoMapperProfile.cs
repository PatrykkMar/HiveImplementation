using AutoMapper;
using HiveGame.BusinessLogic.Models.Board;
using HiveGame.BusinessLogic.Models.Insects;
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
                .ForMember(dest => dest.x, opt => opt.MapFrom(src => src.X))  
                .ForMember(dest => dest.y, opt => opt.MapFrom(src => src.Y)) 
                .ForMember(dest => dest.highlighted, opt => opt.MapFrom(src => src.IsEmpty));
        }
    }
}
