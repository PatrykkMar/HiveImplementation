using AutoMapper;
using HiveGame.BusinessLogic.Models.Game.Graph;
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
                .ForMember(dest => dest.insect, opt => opt.MapFrom(src => src.CurrentInsect != null ? (InsectType?)src.CurrentInsect.Type : null));
        }
    }
}
