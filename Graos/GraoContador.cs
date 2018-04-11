using System;
using Orleans;
using System.Threading.Tasks;
using OrleansDotNet.GraosInterfaces;

namespace OrleansDotNet.Graos
{
    public class GraoContador: Grain, IGraoContador
    {
        private int _contador;
        
        public Task Incremento(int incremento)
        {
            _contador += incremento;
            return Task.CompletedTask;
        }

        public Task<int> GetContador()
        {
            return Task.FromResult(_contador);
        }
    }
}
