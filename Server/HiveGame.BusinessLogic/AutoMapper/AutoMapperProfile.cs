using AutoMapper;
using HiveGame.BusinessLogic.Models.Game.Graph;
using HiveGame.BusinessLogic.Models.Graph;
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
                .ForMember(dest => dest.z, opt => opt.MapFrom(src => src.Z))
                //.ForMember(dest => dest.insect, opt => opt.MapFrom(src => src.CurrentInsect != null ? src.CurrentInsect.Type : InsectType.Nothing))
                .ForMember(dest => dest.highlighted, opt => opt.MapFrom(src => src.IsEmpty));
        }
    }
}
