using Microsoft.EntityFrameworkCore;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class AnaliseProjetoRepository : IAnaliseProjetoRepository
    {
        private readonly ZenithContext _context;

        public AnaliseProjetoRepository()
        {
            _context = new ZenithContext();
        }

        public void AtualizarAnalise(Guid idAnalise, AnaliseProjeto novaAnalise)
        {
            AnaliseProjeto analiseBuscada = _context.AnaliseProjeto.FirstOrDefault(a => a.Id == idAnalise);

            if (analiseBuscada != null) {

                if (novaAnalise.DescricaoGeral != null && novaAnalise.DescricaoGeral != analiseBuscada.DescricaoGeral)
                {
                    analiseBuscada.DescricaoGeral = novaAnalise.DescricaoGeral;
                }

                if (novaAnalise.GestoresIdeais != 0 && novaAnalise.GestoresIdeais != analiseBuscada.GestoresIdeais)
                {
                    analiseBuscada.GestoresIdeais = novaAnalise.GestoresIdeais;
                }

                if (novaAnalise.SenioresIdeais != 0 && novaAnalise.SenioresIdeais != analiseBuscada.SenioresIdeais)
                {
                    analiseBuscada.SenioresIdeais = novaAnalise.SenioresIdeais;
                }

                if (novaAnalise.PlenosIdeais != 0 && novaAnalise.PlenosIdeais != analiseBuscada.PlenosIdeais)
                {
                    analiseBuscada.PlenosIdeais = novaAnalise.PlenosIdeais;
                }

                if (novaAnalise.JuniorsIdeais != 0 && novaAnalise.JuniorsIdeais != analiseBuscada.JuniorsIdeais)
                {
                    analiseBuscada.JuniorsIdeais = novaAnalise.JuniorsIdeais;
                }

                _context.AnaliseProjeto.Update(analiseBuscada);

                _context.SaveChanges();
            }
        }

        public AnaliseProjeto BuscarPeloId(Guid idAnalise)
        {
            AnaliseProjeto analiseBuscada = _context.AnaliseProjeto.FirstOrDefault(a => a.Id == idAnalise);

            return analiseBuscada;
        }

        public AnaliseProjeto BuscarPeloIdProjeto(Guid idProjeto)
        {
            return _context.AnaliseProjeto
                .Include(a => a.Riscos)
                .ThenInclude(r => r.TechSkill)
                .FirstOrDefault(a => a.IdProjeto == idProjeto)!;
        }

        public void CadastrarAnalise(AnaliseProjeto novaAnalise)
        {
            _context.AnaliseProjeto.Add(novaAnalise);

            _context.SaveChanges();
        }

        public void Deletar(Guid idAnalise)
        {
            AnaliseProjeto analiseBuscada = _context.AnaliseProjeto.FirstOrDefault(a => a.Id ==idAnalise);

            if (analiseBuscada != null)
            {
                _context.AnaliseProjeto.Remove(analiseBuscada);

                _context.SaveChanges();
            }
        }
    }
}
