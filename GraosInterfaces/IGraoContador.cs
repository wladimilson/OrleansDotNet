using System;
using Orleans;
using System.Threading.Tasks;

namespace OrleansDotNet.GraosInterfaces
{
    public interface IGraoContador: IGrainWithStringKey
    {
        Task Incremento(int incremento);
        Task<int> GetContador();
    }
}
