using HiveGame.BusinessLogic.Models.Graph;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Insect
    {
        public Insect()
        {
        }

        public Insect(InsectType type)
        {
            Type = type;
        }

        public InsectType Type { get; set; }

        public virtual IList<Vertex> GetAllEmptyVertices(Vertex moveFrom, IList<Vertex> vertices)
        {
            var toRemove = moveFrom.GetAdjacentVerticesList(vertices)
                .Where(x => x.GetAdjacentVerticesList(vertices).Count == 1); //cannot move on empty vertex which would be deleted after move

            foreach(var v in toRemove) 
                vertices.Remove(v);

            vertices.Remove(moveFrom); //cannot stay during move

            return vertices;
        }
    }

    public enum InsectType
    {
        Queen, Ant, Spider, Grasshopperm, Beetle
    }
}
