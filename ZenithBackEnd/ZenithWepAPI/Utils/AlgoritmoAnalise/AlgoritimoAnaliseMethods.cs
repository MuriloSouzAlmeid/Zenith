using ZenithWepAPI.Utils.GeminiService;
using ZenithWepAPI.ViewModels;

namespace ZenithWepAPI.Utils.AlgoritmoAnalise
{
    public class AlgoritimoAnaliseMethods
    {
        private readonly IGeminiServiceRepository _geminiService;

        public AlgoritimoAnaliseMethods()
        {
            _geminiService = new GeminiServiceRepository();
        }

        public int ClassificarNivelDoRisco(int probabilidade, int impacto)
        {
            int somaProbabilidadeImpacto = probabilidade + impacto;

            return (somaProbabilidadeImpacto == 2) ? 1 : ((somaProbabilidadeImpacto <= 4) ? 2 : ((somaProbabilidadeImpacto < 6) ? 3 : 4));
        }

        public async Task<AnaliseProjetoViewModel> AnalisarRiscosProjeto(InfoProjectSettings informacoesProjeto)
        {
            AnaliseProjetoViewModel analise = new AnaliseProjetoViewModel();

            List<RiskSettings> listaDeRiscos = new List<RiskSettings>();

            string analiseProjeto = await _geminiService.RealisarAnaliseProjeto(informacoesProjeto);

            analise.AnaliseGeral = analiseProjeto;

            RiskSettings[] arrayListaDeRiscos = await _geminiService.ListarRiscosPelaAnalise(analiseProjeto);

            if (arrayListaDeRiscos.Length != 0)
            {
                foreach (RiskSettings riscoAnalise in arrayListaDeRiscos)
                {
                    riscoAnalise.Nivel = ClassificarNivelDoRisco(riscoAnalise.Probabilidade, riscoAnalise.Impacto);
                    listaDeRiscos.Add(riscoAnalise);
                }
            }

            analise.RiscosAnalise = listaDeRiscos;

            return analise;
        }


        public float[] DefinirComposicaoDaEquipe(int[,] arrayQuantidadeColaboradores, int[,] arrayNotasPorAreaProjeto)
        {
            // Matriz de notas, médias e ponderamento
            float[,] matrizNotasMediasPonderamento = PreencherMatrizPrincipal(arrayNotasPorAreaProjeto);

            // Array com as médias já ponderadas
            float[] arrayMediasPonderadas = GerarArrayComNotasPonderadas(matrizNotasMediasPonderamento);


            float[,] matrizComposicaoEquipe = CalcularComposicaoEquipe(arrayMediasPonderadas, arrayQuantidadeColaboradores);

            float[] arrayDeComposicaoEquipe = GerarArrayComComposicaoDaEquipe(matrizComposicaoEquipe);

            return arrayDeComposicaoEquipe;
        }



        static float MediaDasNotas(float somatorio)
        {
            return somatorio / 3;
        }

        static float DefinirPonderacaoMedia(float media, string area)
        {
            if (area == "Técnica")
            {
                if (media < 3)
                {
                    return 1f;
                }
                else if (media < 5)
                {
                    return 0.9f;
                }
                else if (media < 7)
                {
                    return 0.8f;
                }
                else if (media < 9)
                {
                    return 0.7f;
                }
                else
                {
                    return 0.6f;
                }
            }
            else if (area == "Funcional")
            {
                if (media < 3)
                {
                    return 0f;
                }
                else if (media < 5)
                {
                    return 0.1f;
                }
                else if (media < 7)
                {
                    return 0.15f;
                }
                else if (media < 9)
                {
                    return 0.2f;
                }
                else
                {
                    return 0.25f;
                }
            }
            else
            {
                if (media < 3)
                {
                    return 0f;
                }
                else if (media < 5)
                {
                    return 0.05f;
                }
                else if (media < 7)
                {
                    return 0.1f;
                }
                else if (media < 9)
                {
                    return 0.15f;
                }
                else
                {
                    return 0.2f;
                }
            }
        }

        static float[,] PreencherMatrizPrincipal(int[,] matrizInicial)
        {
            // Matriz a ser trabalhada
            float[,] matrizValores = new float[3, 5];

            for (int i = 0; i < matrizValores.GetLength(0); i++)
            {
                string area = (i == 0) ? "Técnica" : ((i == 1) ? "Funcional" : "Ambiente");
                float somatorioArea = 0f;

                for (int j = 0; j < 3; j++)
                {
                    matrizValores[i, j] = matrizInicial[i, j];
                    somatorioArea += matrizValores[i, j];
                }

                matrizValores[i, 3] = MediaDasNotas(somatorioArea);

                matrizValores[i, 4] = DefinirPonderacaoMedia(matrizValores[i, 3], area);
            }

            return matrizValores;
        }

        static float[] GerarArrayComNotasPonderadas(float[,] matrizNotasPonderamento)
        {
            float[] arrayNotasPonderadas = new float[3];

            for (int i = 0; i < arrayNotasPonderadas.GetLength(0); i++)
            {
                arrayNotasPonderadas[i] = matrizNotasPonderamento[i, 3] * matrizNotasPonderamento[i, 4];
            };

            return arrayNotasPonderadas;
        }

        static float[,] CalcularComposicaoEquipe(float[] mediasPonderadas, int[,] colaboradores)
        {
            float[,] matrizComposicaoEquipe = new float[5, 4];

            for (int area = 0; area < 3; area++)
            {
                for (int senrioridade = 0; senrioridade < matrizComposicaoEquipe.GetLength(1); senrioridade++)
                {
                    matrizComposicaoEquipe[area, senrioridade] = mediasPonderadas[area] * colaboradores[area, senrioridade];
                };

            };

            float somatoriaGeral = 0;
            for (int senrioridade = 0; senrioridade < matrizComposicaoEquipe.GetLength(1); senrioridade++)
            {
                float somatoria = 0;
                for (int area = 0; area < 3; area++)
                {
                    somatoria += matrizComposicaoEquipe[area, senrioridade];
                };
                matrizComposicaoEquipe[3, senrioridade] = somatoria;
                somatoriaGeral += somatoria;
            };

            for (int senrioridade = 0; senrioridade < matrizComposicaoEquipe.GetLength(1); senrioridade++)
            {
                float parcelaSomatoria = matrizComposicaoEquipe[3, senrioridade] / somatoriaGeral;
                matrizComposicaoEquipe[4, senrioridade] = parcelaSomatoria;
            };

            return matrizComposicaoEquipe;
        }

        static float[] GerarArrayComComposicaoDaEquipe(float[,] matrizComposicaoEquipe)
        {
            float[] arrayComposicao = new float[4];

            for (int i = 0; i < arrayComposicao.GetLength(0); i++)
            {
                arrayComposicao[i] = matrizComposicaoEquipe[4, i];
            }

            return arrayComposicao;
        }
    }
}
