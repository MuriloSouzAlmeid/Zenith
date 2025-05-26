using Microsoft.EntityFrameworkCore;
using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Context
{
    public class ZenithContext : DbContext
    {
        public string connectionStringSenai = "Server=NOTE07-SALA19;Initial Catalog=ZenithDatabase;User ID=sa;Password=Senai@134;TrustServerCertificate=True;";

        public string connectionStringCasaMurilo = "Server=NOTEBOOKFAMILIA;Initial Catalog=ZenithDatabase;User ID=sa;Password=Murilo12$;TrustServerCertificate=True;";

        public string connectionStringAzure = "Server=tcp:projectzenithserver.database.windows.net,1433;Initial Catalog=ZenithBD;User ID=adminzenith;Password=Senai@134;Encrypt=True;TrustServerCertificate=False;;";


        public DbSet<TipoProjeto> TiposProjeto { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<TechSkill> TechSkill { get; set;}
        public DbSet<Tecnologia> Tecnologia { get; set; }
        public DbSet<TecnologiasProjeto> TecnologiasProjeto { get; set; }
        public DbSet<Projeto> Projeto { get; set; }
        public DbSet<Funcionalidade> Funcionalidade { get; set; }
        public DbSet<FuncionalidadesProjeto> FuncionalidadesProjeto { get; set; }
        public DbSet<Equipe> Equipe { get; set; }
        public DbSet<EquipeColaboradores> EquipeColaboradores { get; set; }
        public DbSet<Colaborador> Colaborador { get; set;}
        public DbSet<ColaboradorTechSkills> ColaboradorTechSkills { get; set; }
        public DbSet<CargoUsuario> CargoUsuario { get; set; }
        public DbSet<AnaliseProjeto> AnaliseProjeto { get; set; }
        public DbSet<Risco> Risco { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionStringAzure);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
