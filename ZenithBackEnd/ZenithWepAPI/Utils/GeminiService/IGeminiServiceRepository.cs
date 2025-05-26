namespace ZenithWepAPI.Utils.GeminiService
{
    public interface IGeminiServiceRepository
    {
        string GerarPromptAnaliseProjeto(InfoProjectSettings informacoesProjeto);
        Task<string> RealisarAnaliseProjeto(InfoProjectSettings informacoesProjeto);
        string GerarPromptRiscosProjeto(string analise);
        Task<RiskSettings[]> ListarRiscosPelaAnalise(string analise);
        string GerarPromptRangeComplexidadeProjeto(InfoProjectSettings informacoesProjeto, string analiseProjeto);
        Task<int[,]> AvaliarRangeComplexidadeProjeto(InfoProjectSettings informacoesProjeto, string analiseProjeto);
        string GerarPromptParaDefinicaoComplexidadeProjeto(InfoProjectSettings informacoesProjeto, string analiseProjeto, float[] composicaoEquipe);
        string GerarPromptDefinirAreaCargo(string cargoAnalisado);
        Task<string> RealizarAnaliseAreaDoCargo(string cargo);
        Task<int> RealizarAnaliseComplexidadeProjeto(InfoProjectSettings informacoesProjeto, string analiseProjeto, float[] composicaoEquipe);
    }
}
