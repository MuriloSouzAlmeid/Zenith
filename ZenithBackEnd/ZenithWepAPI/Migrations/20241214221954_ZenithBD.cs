using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenithWepAPI.Migrations
{
    /// <inheritdoc />
    public partial class ZenithBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CargoUsuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cargo = table.Column<string>(type: "VARCHAR(120)", nullable: true),
                    NivelCargo = table.Column<int>(type: "INT", nullable: false),
                    Area = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoUsuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funcionalidade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionalidade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechSkill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Skill = table.Column<string>(type: "VARCHAR(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechSkill", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tecnologia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeTecnologia = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnologia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoProjeto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProjeto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "VARCHAR(200)", nullable: true),
                    Email = table.Column<string>(type: "VARCHAR(150)", nullable: true),
                    Senha = table.Column<string>(type: "TEXT", nullable: true),
                    CodRecupSenha = table.Column<string>(type: "VARCHAR(4)", nullable: true),
                    Foto = table.Column<string>(type: "TEXT", nullable: true),
                    NivelSenioridade = table.Column<int>(type: "INT", nullable: true),
                    IdCargoUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_CargoUsuario_IdCargoUsuario",
                        column: x => x.IdCargoUsuario,
                        principalTable: "CargoUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Colaborador",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colaborador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Colaborador_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Projeto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    DataFinal = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    NivelProjeto = table.Column<int>(type: "INT", nullable: true),
                    NivelAnalise = table.Column<int>(type: "INT", nullable: true),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTipoProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projeto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projeto_TipoProjeto_IdTipoProjeto",
                        column: x => x.IdTipoProjeto,
                        principalTable: "TipoProjeto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Projeto_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ColaboradorTechSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdColaborador = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTechSkill = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColaboradorTechSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColaboradorTechSkills_Colaborador_IdColaborador",
                        column: x => x.IdColaborador,
                        principalTable: "Colaborador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ColaboradorTechSkills_TechSkill_IdTechSkill",
                        column: x => x.IdTechSkill,
                        principalTable: "TechSkill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "AnaliseProjeto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DescricaoGeral = table.Column<string>(type: "TEXT", nullable: true),
                    GestoresIdeais = table.Column<double>(type: "FLOAT", nullable: false),
                    SenioresIdeais = table.Column<double>(type: "FLOAT", nullable: false),
                    PlenosIdeais = table.Column<double>(type: "FLOAT", nullable: false),
                    JuniorsIdeais = table.Column<double>(type: "FLOAT", nullable: false),
                    IdProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnaliseProjeto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnaliseProjeto_Projeto_IdProjeto",
                        column: x => x.IdProjeto,
                        principalTable: "Projeto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QtdIntegrantes = table.Column<int>(type: "INT", nullable: true),
                    IdProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipe_Projeto_IdProjeto",
                        column: x => x.IdProjeto,
                        principalTable: "Projeto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "FuncionalidadesProjeto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdFuncionalidade = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncionalidadesProjeto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuncionalidadesProjeto_Funcionalidade_IdFuncionalidade",
                        column: x => x.IdFuncionalidade,
                        principalTable: "Funcionalidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_FuncionalidadesProjeto_Projeto_IdProjeto",
                        column: x => x.IdProjeto,
                        principalTable: "Projeto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TecnologiasProjeto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTecnologia = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TecnologiasProjeto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TecnologiasProjeto_Projeto_IdProjeto",
                        column: x => x.IdProjeto,
                        principalTable: "Projeto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TecnologiasProjeto_Tecnologia_IdTecnologia",
                        column: x => x.IdTecnologia,
                        principalTable: "Tecnologia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Risco",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DescricaoRisco = table.Column<string>(type: "TEXT", nullable: true),
                    AreaRisco = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    ProbabilidadeRisco = table.Column<int>(type: "INT", nullable: true),
                    ImpactoRisco = table.Column<int>(type: "INT", nullable: true),
                    NivelRisco = table.Column<int>(type: "INT", nullable: true),
                    IdAnaliseProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTechSkill = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Risco", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Risco_AnaliseProjeto_IdAnaliseProjeto",
                        column: x => x.IdAnaliseProjeto,
                        principalTable: "AnaliseProjeto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Risco_TechSkill_IdTechSkill",
                        column: x => x.IdTechSkill,
                        principalTable: "TechSkill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "EquipeColaboradores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdEquipe = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdColaborador = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipeColaboradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipeColaboradores_Colaborador_IdColaborador",
                        column: x => x.IdColaborador,
                        principalTable: "Colaborador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_EquipeColaboradores_Equipe_IdEquipe",
                        column: x => x.IdEquipe,
                        principalTable: "Equipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnaliseProjeto_IdProjeto",
                table: "AnaliseProjeto",
                column: "IdProjeto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colaborador_IdUsuario",
                table: "Colaborador",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ColaboradorTechSkills_IdColaborador",
                table: "ColaboradorTechSkills",
                column: "IdColaborador");

            migrationBuilder.CreateIndex(
                name: "IX_ColaboradorTechSkills_IdTechSkill",
                table: "ColaboradorTechSkills",
                column: "IdTechSkill");

            migrationBuilder.CreateIndex(
                name: "IX_Equipe_IdProjeto",
                table: "Equipe",
                column: "IdProjeto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipeColaboradores_IdColaborador",
                table: "EquipeColaboradores",
                column: "IdColaborador");

            migrationBuilder.CreateIndex(
                name: "IX_EquipeColaboradores_IdEquipe",
                table: "EquipeColaboradores",
                column: "IdEquipe");

            migrationBuilder.CreateIndex(
                name: "IX_FuncionalidadesProjeto_IdFuncionalidade",
                table: "FuncionalidadesProjeto",
                column: "IdFuncionalidade");

            migrationBuilder.CreateIndex(
                name: "IX_FuncionalidadesProjeto_IdProjeto",
                table: "FuncionalidadesProjeto",
                column: "IdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_Projeto_IdTipoProjeto",
                table: "Projeto",
                column: "IdTipoProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_Projeto_IdUsuario",
                table: "Projeto",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Risco_IdAnaliseProjeto",
                table: "Risco",
                column: "IdAnaliseProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_Risco_IdTechSkill",
                table: "Risco",
                column: "IdTechSkill");

            migrationBuilder.CreateIndex(
                name: "IX_TecnologiasProjeto_IdProjeto",
                table: "TecnologiasProjeto",
                column: "IdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_TecnologiasProjeto_IdTecnologia",
                table: "TecnologiasProjeto",
                column: "IdTecnologia");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdCargoUsuario",
                table: "Usuario",
                column: "IdCargoUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColaboradorTechSkills");

            migrationBuilder.DropTable(
                name: "EquipeColaboradores");

            migrationBuilder.DropTable(
                name: "FuncionalidadesProjeto");

            migrationBuilder.DropTable(
                name: "Risco");

            migrationBuilder.DropTable(
                name: "TecnologiasProjeto");

            migrationBuilder.DropTable(
                name: "Colaborador");

            migrationBuilder.DropTable(
                name: "Equipe");

            migrationBuilder.DropTable(
                name: "Funcionalidade");

            migrationBuilder.DropTable(
                name: "AnaliseProjeto");

            migrationBuilder.DropTable(
                name: "TechSkill");

            migrationBuilder.DropTable(
                name: "Tecnologia");

            migrationBuilder.DropTable(
                name: "Projeto");

            migrationBuilder.DropTable(
                name: "TipoProjeto");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "CargoUsuario");
        }
    }
}
