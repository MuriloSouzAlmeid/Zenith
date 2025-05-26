using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;
using ZenithWepAPI.Utils;

namespace ZenithWepAPI.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ZenithContext _context;

        public UsuarioRepository()
        {
            _context = new ZenithContext();
        }

        public void AtualizarFotoUsuario(string novaUrl, Guid idUsuario)
        {
            Usuario usuarioBuscado = _context.Usuario.FirstOrDefault(u => u.Id == idUsuario)!;

            if(novaUrl != null)
            {
                usuarioBuscado.Foto = novaUrl;

                _context.Usuario.Update(usuarioBuscado);

                _context.SaveChanges();
            }
        }

        public void AtualizarUsuario(Guid idUsuario, Usuario usuarioAtualizado)
        {
            Usuario usuarioBuscado = _context.Usuario.FirstOrDefault(u => u.Id == idUsuario)!;

            if (usuarioAtualizado.Nome != usuarioBuscado.Nome && usuarioAtualizado.Nome != null)
            {
                usuarioBuscado.Nome = usuarioAtualizado.Nome;
            }

            if (usuarioAtualizado.Email != usuarioBuscado.Email && usuarioAtualizado.Email != null)
            {
                usuarioBuscado.Email = usuarioAtualizado.Email;
            }

            if (usuarioAtualizado.NivelSenioridade != usuarioBuscado.NivelSenioridade && usuarioAtualizado.NivelSenioridade != null)
            {
                usuarioBuscado.NivelSenioridade = usuarioAtualizado.NivelSenioridade;
            }

            if (usuarioAtualizado.IdCargoUsuario != usuarioBuscado.IdCargoUsuario)
            {
                usuarioBuscado.IdCargoUsuario = usuarioAtualizado.IdCargoUsuario;
            }

            if(usuarioAtualizado.Foto != usuarioBuscado.Foto && usuarioAtualizado.Foto != null)
            {
                usuarioBuscado.Foto = usuarioAtualizado.Foto;
            }

            _context.Usuario.Update(usuarioBuscado);

            _context.SaveChanges();

        }

        public Usuario BuscarPorEmail(string email)
        {
            Usuario usuarioBuscado = _context.Usuario.FirstOrDefault(usuario => usuario.Email == email)!;

            return usuarioBuscado;
        }

        public Usuario BuscarPorId(Guid id)
        {
            Usuario usuarioBuscado = _context.Usuario
                .Select(usuario => new Usuario()
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    NivelSenioridade = usuario.NivelSenioridade,
                    Nome = usuario.Nome,
                    Foto = usuario.Foto,
                    IdCargoUsuario = usuario.IdCargoUsuario,
                    CargoUsuario = new CargoUsuario()
                    {
                        Id = usuario.CargoUsuario!.Id,
                        Cargo = usuario.CargoUsuario.Cargo,
                        NivelCargo = usuario.CargoUsuario.NivelCargo,
                        Area = usuario.CargoUsuario.Area
                    }
                })
                .FirstOrDefault(usuario => usuario.Id == id)!;

            return usuarioBuscado;
        }

        public Usuario BuscarPorLogin(string email, string senha)
        {
            Usuario usuarioBuscado = _context.Usuario
                .Include(u => u.CargoUsuario)
                .FirstOrDefault(usuario => usuario.Email == email)!;

            if(usuarioBuscado != null && Criptografia.CompararHash(senha, usuarioBuscado.Senha!)) 
            {
                return usuarioBuscado;
            }

            return null;
        }

        public void Cadastrar(Usuario novoUsuario)
        {
            _context.Usuario.Add(novoUsuario);

            _context.SaveChanges();
        }

        public void Deletar(Guid idUsuario)
        {
            Usuario usuarioBuscado = BuscarPorId(idUsuario);

            _context.Usuario.Remove(usuarioBuscado);

            _context.SaveChanges();
        }

        public List<Usuario> ListarColaboradores()
        {
            return _context.Usuario.Select(usuario => new Usuario()
            {
                Id = usuario.Id,
                Email = usuario.Email,
                NivelSenioridade = usuario.NivelSenioridade,
                Nome = usuario.Nome,
                Foto = usuario.Foto,
                IdCargoUsuario = usuario.IdCargoUsuario,
                CargoUsuario = new CargoUsuario()
                {
                    Id = usuario.CargoUsuario!.Id,
                    Cargo = usuario.CargoUsuario.Cargo,
                    NivelCargo = usuario.CargoUsuario.NivelCargo,
                    Area = usuario.CargoUsuario.Area
                }
            }).Where(u => u.CargoUsuario!.NivelCargo == 2).ToList();
        }

        public List<Usuario> ListarNaoColaboradores()
        {
            return _context.Usuario.Select(usuario => new Usuario()
            {
                Id = usuario.Id,
                Email = usuario.Email,
                NivelSenioridade = usuario.NivelSenioridade,
                Nome = usuario.Nome,
                Foto = usuario.Foto,
                IdCargoUsuario = usuario.IdCargoUsuario,
                CargoUsuario = new CargoUsuario()
                {
                    Id = usuario.CargoUsuario!.Id,
                    Cargo = usuario.CargoUsuario.Cargo,
                    NivelCargo = usuario.CargoUsuario.NivelCargo,
                    Area = usuario.CargoUsuario.Area
                }
            }).Where(u => u.CargoUsuario!.NivelCargo != 2).ToList();
        }

        public List<Usuario> ListarPorAreaCargo(string area)
        {
            return _context.Usuario
                .Select(usuario => new Usuario()
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    NivelSenioridade = usuario.NivelSenioridade,
                    Nome = usuario.Nome,
                    Foto = usuario.Foto,
                    IdCargoUsuario = usuario.IdCargoUsuario,
                    CargoUsuario = new CargoUsuario()
                    {
                        Id = usuario.CargoUsuario!.Id,
                        Cargo = usuario.CargoUsuario.Cargo,
                        NivelCargo = usuario.CargoUsuario.NivelCargo,
                        Area = usuario.CargoUsuario.Area
                    }
                })
                .Where(u => u.CargoUsuario.Area == area)
                .ToList();
        }

        public List<Usuario> ListarPorCargo(string cargoUsuario)
        {
            return _context.Usuario.Select(usuario => new Usuario()
            {
                Id = usuario.Id,
                Email = usuario.Email,
                NivelSenioridade = usuario.NivelSenioridade,
                Nome = usuario.Nome,
                Foto = usuario.Foto,
                IdCargoUsuario = usuario.IdCargoUsuario,
                CargoUsuario = new CargoUsuario()
                {
                    Id = usuario.CargoUsuario!.Id,
                    Cargo = usuario.CargoUsuario.Cargo,
                    NivelCargo = usuario.CargoUsuario.NivelCargo,
                    Area = usuario.CargoUsuario.Area
                }
            }).Where(u => EF.Functions.Like(u.CargoUsuario.Cargo, $"%{cargoUsuario}%")).ToList();
        }

        public List<Usuario> ListarTodos()
        {
            return _context.Usuario.Select(usuario => new Usuario()
            {
                Id = usuario.Id,
                Email = usuario.Email,
                NivelSenioridade = usuario.NivelSenioridade,
                Nome = usuario.Nome,
                Foto = usuario.Foto,
                IdCargoUsuario = usuario.IdCargoUsuario,
                CargoUsuario = new CargoUsuario()
                {
                    Id = usuario.CargoUsuario!.Id,
                    Cargo = usuario.CargoUsuario.Cargo,
                    NivelCargo = usuario.CargoUsuario.NivelCargo,
                    Area = usuario.CargoUsuario.Area
                }
            }).ToList();
        }
    }
}
