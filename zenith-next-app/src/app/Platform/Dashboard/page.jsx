"use client";

import { api } from "@/Services/Service";
import { useContext, useEffect, useState } from "react";
import { UserContext } from "@/context/AuthContext";
import { GraphicRiskLevel } from "@/components/Graphic/Index";
import { KanbanProjects } from "@/components/KanbanProjects";
import { OtherProjects } from "@/components/OtherProjects";
import { GraphicProjectTypeByPeriod } from "@/components/PeriodChart";

export default function DashboardPage() {
  const { userData } = useContext(UserContext);

  const [chartData, setChartData] = useState([
    { typeRisk: "Low", count: 0, fill: "#4D8BFF" },
    { typeRisk: "Medium", count: 0, fill: "#FD9A30" },
    { typeRisk: "High", count: 0, fill: "#E53E3E" },
  ]);

  const [projectsList, setProjectsList] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [chartKey, setChartKey] = useState(0); // Força re-render do gráfico

  const ListProjectsByUser = async () => {
    try {
      setIsLoading(true);

      // Seleciona a URL com base no cargo do usuário
      const url =
        userData.cargo === "Administrador"
          ? `/Projeto/ListarTodos`
          : userData.cargo === "Gerente De Projetos"
            ? `/Projeto/ListarProjetosPeloUsuario/${userData.id}`
            : userData.cargo === "Colaborador"
              ? `/Projeto/ListarPeloColaborador/${userData.colaborador}`
              : null;

      if (!url) {
        console.warn("Cargo não reconhecido:", userData.cargo);
        return;
      }

      console.log(`URL da requisição: ${url}`);
      const response = await api.get(url);

      console.log("Resposta da API:", response.data);
      setProjectsList(Array.isArray(response.data) ? response.data : []);
    } catch (error) {
      console.error(
        "Erro na requisição:",
        error.response?.data || error.request || error.message
      );
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (userData) {
      ListProjectsByUser();
    }
  }, [userData]);

  useEffect(() => {
    const calculateRiskData = () => {
      const riskCounts = { Low: 0, Medium: 0, High: 0 };

      // Contagem dos projetos por nível de risco
      projectsList.forEach((project) => {
        if (project.nivelProjeto === "Baixo") riskCounts.Low += 1;
        if (project.nivelProjeto === "Médio") riskCounts.Medium += 1;
        if (project.nivelProjeto === "Alto") riskCounts.High += 1;
      });

      const newChartData = [
        { typeRisk: "Low", count: riskCounts.Low, fill: "#008000" },
        { typeRisk: "Medium", count: riskCounts.Medium, fill: "#FD9A30" },
        { typeRisk: "High", count: riskCounts.High, fill: "#ff0000" },
      ];

      console.log("Dados de risco calculados:", newChartData); // Verifique os dados de risco calculados
      setChartData(newChartData);
      setChartKey((prevKey) => prevKey + 1); // Atualiza o gráfico
    };

    if (projectsList.length > 0 && !isLoading) {
      calculateRiskData();
    }
  }, [projectsList, isLoading]);

  return (
    <div className="h-screen mt-16 overflow-scroll lg:h-[850px] overflow-x-hidden scrollbar-custom w-full">
      <div className=" flex flex-col ml-[21%] gap-4 max-xl:ml-0 max-0xl:pt-4 h-[950px] lg:h-[500px]">
        <GraphicProjectTypeByPeriod userData={userData} />
        <div className="flex flex-row min-h-screen max-xl:flex-col max-xl:items-center lg:max-xl:justify-center mt-14">
          {/* Usando a chave "key" para forçar a reinicialização do gráfico */}
          <GraphicRiskLevel key={chartKey} chartData={chartData} />
          <KanbanProjects projectList={projectsList} />
        </div>
      </div>

      <div className="flex ml-[200px] lg:ml-[65px] flex-col  max-xl:justify-center max-xl:items-center mt-[500px]">
        <OtherProjects />
      </div>
    </div>
  );
}
