using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class CargoUsuarioRepositry : ICargoUsuarioRepository
    {
        private readonly ZenithContext _context;
        public CargoUsuarioRepositry()
        {
            _context = new ZenithContext();
        }

        public CargoUsuario BuscarPorNome(string nomeCargo)
        {
            CargoUsuario cargoBuscado = _context.CargoUsuario.FirstOrDefault(cargo => cargo.Cargo.ToLower() == nomeCargo.ToLower())!;

            return cargoBuscado;
        }


        public void Cadastrar(CargoUsuario novoCargo)
        {
            _context.CargoUsuario.Add(novoCargo);

            _context.SaveChanges();
        }

        public void Deletar(Guid idCargo)
        {
            CargoUsuario cargoBuscado = _context.CargoUsuario.FirstOrDefault(cargo => cargo.Id == idCargo)!;

            if(cargoBuscado != null) 
            {
                _context.Remove(cargoBuscado);
                
                _context.SaveChanges();
            }
        }

        public List<CargoUsuario> ListarTodos()
        {
            return _context.CargoUsuario.Where(c => c.NivelCargo != 0).ToList();
        }
    }
}
