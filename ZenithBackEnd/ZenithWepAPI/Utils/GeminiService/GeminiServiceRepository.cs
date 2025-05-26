
using GenerativeAI.Models;
using GenerativeAI.Types;
using Newtonsoft.Json;
using System.Globalization;

namespace ZenithWepAPI.Utils.GeminiService
{
    public class GeminiServiceRepository : IGeminiServiceRepository
    {
        private readonly string apiKeyGemini;
        public GenerativeModel modelo { get; set; }

        public GeminiServiceRepository()
        {
            apiKeyGemini = "AIzaSyAECWNn_niKjcpuNyme7s3vizD_54zMjCo";
            modelo = new GenerativeModel(apiKeyGemini, new ModelParams()
            {
                GenerationConfig = new GenerationConfig()
                {
                    Temperature = 0,
                    CandidateCount = 1
                },
                Model = "gemini-1.5-flash"
            });
        }

        public async Task<string> RealisarAnaliseProjeto(InfoProjectSettings informacoesProjeto)
        {
            string promptAnalise = this.GerarPromptAnaliseProjeto(informacoesProjeto);

            var respostaAnalise = await modelo.GenerateContentAsync(promptAnalise);

            return respostaAnalise;
        }

        public async Task<RiskSettings[]> ListarRiscosPelaAnalise(string analise)
        {
            string promptListaRiscos = this.GerarPromptRiscosProjeto(analise);

            var respostaModelo = await modelo.GenerateContentAsync(promptListaRiscos);

            RiskSettings[] arrayRiscosAnalise = JsonConvert.DeserializeObject<RiskSettings[]>(respostaModelo)!;

           return arrayRiscosAnalise;
        }

        public async Task<int[,]> AvaliarRangeComplexidadeProjeto(InfoProjectSettings informacoesProjeto, string analiseProjeto)
        {
            string promt = this.GerarPromptRangeComplexidadeProjeto(informacoesProjeto, analiseProjeto);

            string retornoModelo = await modelo.GenerateContentAsync(promt);

            int[,] matrizRangeComplexidade = JsonConvert.DeserializeObject<int[,]>(retornoModelo)!;

            return matrizRangeComplexidade;
        }

        public async Task<string> RealizarAnaliseAreaDoCargo(string cargo)
        {
            string prompt = this.GerarPromptDefinirAreaCargo(cargo);

            string respostaModelo = await modelo.GenerateContentAsync(prompt);

            return respostaModelo;
        }
        public async Task<int> RealizarAnaliseComplexidadeProjeto(InfoProjectSettings informacoesProjeto, string analiseProjeto, float[] composicaoEquipe)
        {
            string prompt = this.GerarPromptParaDefinicaoComplexidadeProjeto(informacoesProjeto, analiseProjeto, composicaoEquipe);

            string respostaModelo = await modelo.GenerateContentAsync(prompt);

            return int.Parse(respostaModelo);
        }








        // Prompts para o modelo
        public string GerarPromptAnaliseProjeto(InfoProjectSettings informacoesProjeto)
        {
            string funcionalidadesTexto = "";

            foreach(string funcionalidade in informacoesProjeto.Funcionalidades)
            {
                funcionalidadesTexto += $"- {funcionalidade}\n";
            }

            string tecnologiasTexto = "";

            foreach (string tecnologia in informacoesProjeto.Tecnologias)
            {
                tecnologiasTexto += $", {tecnologia}";
            }

            return $@"
Estou planejando um projeto de implementação de sistemas e preciso realizar uma análise detalhada a respeito de quais riscos de desenvolvimento o meu projeto possui. O seu trabalho será receber como entrada diversas informações do projeto como nome, descrição, tipo de projeto, data de início, data de finalização e, por fim, funcionalidades e tecnologias presentes no projeto. Após receber tais parâmetros, analise-os cuidadosamente e retorne uma análise geral em texto corrido detalhando o máximo possível sobre quais riscos estão presentes no projeto fornecendo os seguintes atributos para os riscos: descrição, área em que o risco atinge (exemplo: front-end, back-end, marketing, design, etc...), qual a tech skill (habilidade técnica) necessária para lidar com o risco (exemplo: HTML, JavaScript, Figma, SQL, etc...) **importante: inclua apenas uma tech skill para cada risco listado, caso o risco possua mais que uma liste a mais importante para mitigá-lo**, qual a probabilidade deste risco se concretizar durante o desenvolvimento do projeto e qual o impacto que este risco causaria para todo o desenvolvimento do projeto em si caso o risco se concretize **muito importante: para a classificação da probabilidade e do impacto, indique-os apenas como baixo, médio ou alto, evite informações a mais a este respeito**. Faça uma análise profunda a respeito dos riscos presentes no desenvolvimento do projeto, listando o máximo de riscos possíveis em todas as probabilidades e impactos para todas as áreas técnicas, funcionais e de ambiente do projeto. Lembre-se que deve ser em texto corridos, sendo separados em 1 ou mais parágrafos caso necessário. Evite o retorno de listas na resposta.

Exemplo 1
Informações do Projeto:
Nome: Projeto Zenith

Descrição: O Zenith é um sistema que tem como objetivo realizar a análise de riscos de um projeto de implantação de sistemas, gerando relatórios sobre os mesmos e alocando uma equipe otimizada para a realização do mesmo. Em projetos de implantação de sistemas, é crucial antecipar e gerenciar riscos para garantir o sucesso do projeto. Uma alocação estratégica de recursos humanos, considerando a experiência e especialização de cada membro, pode minimizar estes riscos e otimizar os resultados. Desenvolver um algoritmo ou processo que realize a análise de riscos em projetos de implantação de sistemas, gerando uma alocação otimizada de profissionais de diferentes níveis de senioridade (júnior, pleno, sênior) para mitigar riscos identificados. Tendo em vista este cenário, se faz necessário uma plataforma que:

- Analise riscos de um projeto: a partir do escopo e cronograma que determinado projeto de implantação de sistema possua, analisar os riscos atrelados ao mesmo e listar tais riscos, assim como qual seus impactos e quais as qualificações necessárias para mitigá-los.
- Alocação de equipes para o projeto: com base na análise de riscos realizada, definir equipes recomendadas levando em consideração parâmetros como soft skills, hard skills e tech skills (junto da senioridade do mesmo) na definição de uma equipe otimizada e ideal para a realização do projeto
- Gerenciamento de análises e equipes: após a realização de análises e alocação de equipes ideais, a plataforma permitirá gerenciamento de análises de projetos realizadas na plataforma e gerenciamento de equipes definidas para os mesmos.

Tipo de projeto: Implementação de software na empresa

Data de início: 23/01/2025

Data de finalização: 05/07/2025

Funcionalidades: 
- O gestor, previamente cadastrado, poderá efetuar seu login pelo seu email e senha cadastrado, ou pelo email do Google e/ou Microsoft, contanto que o mesmo tenha sido previamente cadastrado na plataforma
- O usuário poderá efetuar o logout da plataforma conforme necessário
- O administrador da plataforma poderá realizar o cadastro de um gestor ou de um funcionário a partir de uma página privada apenas para administradores
- O usuário poderá recuperar a senha de acesso caso a tenha esquecido por meio de seu email cadastrado
- O gestor poderá cadastrar uma análise de um projeto a partir de seu escopo e conograma fornecido
- O gestor poderá visualizar as informações de determinada análise realizada na plataforma, verificando os riscos identificados, como mitigá-los durante o desenvolvimento do projeto e quais são os possíveis funcionários que irão compor a equipe ideal para a realização do projeto
- O gestor poderá visualizar o histórico de todas as análises de projetos realizadas na plataforma, com ou sem filtros de pesquisa. Caso opte por utilizar filtros de pesquisa, poderá filtrar por nome do projeto, data do projeto ou escopo geral do mesmo
- Ao terminar a análise, caberá ao gestor definir a equipe ideal para a realização do projeto, podendo ser feito de maneira pessoal com a lista de funcionários geral ou sugeridos para o projeto, ou pederá ser feito por meio da equipe ideal sugerida pela própria plataforma.
- O gestor poderá editar a equipe definida para a realização de determinado projeto de acordo com a necessidade.
- O gestor poderá excluir análises de projetos do histórico de análises realizadas caso necessário
- O usuário poderá visualizar a lista de funcionários que foram cadastrados na plataforma pelo mesmo
- O usuário poderá visualizar as informações gerais de determinado funcionário cadastrado na plataforma, assim como em quais projetos e equipes o mesmo foi alocado em análises realizadas na plataforma
- O administrador poderá editar determinadas informações do funcionário, atualizando suas habilidades ou alterando informações pessoais caso necessário
- O administrador poderá excluir determinado funcionário cadastrado na plataforma caso necessário
- O administrador poderá atualizar informações do escopo e cronograma do projeto a fim de realizar uma nova análise acerca do mesmo.
- O usuário poderá filtrar a visualização de análises realizadas na plataforma por parâmetros diversos como cronograma, nome e escopo e funcionários envolvidos
- O usuário poderá filtrar a visualização de funcionários por parâmetros diversos como nome, techskills e participação em projetos.

Tecnologias: Next.JS, ASPNET.CORE, EntityFramework, React, Azure, SQL Database, API REST Full, Gemini AI, C#, HTML, CSS, JavaScript 

Saída do exemplo 1:
O Projeto Zenith, com seu objetivo ambicioso de analisar riscos e otimizar alocação de equipes em projetos de implantação de sistemas, apresenta diversos riscos de desenvolvimento.  A complexidade da integração entre as diferentes tecnologias e funcionalidades, aliada ao curto prazo de desenvolvimento (de 23/01/2025 a 05/07/2025, aproximadamente 5 meses), aumenta significativamente a probabilidade de problemas.

Um risco significativo reside na integração do Gemini AI com o restante da plataforma. A dependência de uma API externa para a análise de riscos implica em uma alta probabilidade de problemas de compatibilidade, performance e disponibilidade da API.  A habilidade técnica crucial para mitigar esse risco é o conhecimento profundo em API REST Full, garantindo a comunicação eficiente e robusta entre o sistema e o Gemini AI. A probabilidade desse risco é alta, e o impacto no projeto seria igualmente alto, podendo atrasar significativamente o cronograma e comprometer a funcionalidade principal do sistema.

Outro risco reside na complexidade do algoritmo de alocação de equipes.  Desenvolver um algoritmo que considere soft skills, hard skills e tech skills, além da senioridade, para otimizar a alocação de recursos humanos é um desafio considerável.  A habilidade técnica mais importante para lidar com este risco é o conhecimento em C#, dada a sua utilização no ASP.NET Core, que será a base da lógica de negócio. A probabilidade deste risco é média, e o impacto também seria médio, podendo levar a atrasos e a uma alocação de equipes subótima.

A integração entre o front-end (React e Next.JS) e o back-end (ASP.NET Core e Entity Framework) também apresenta riscos.  Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades podem ocorrer. A habilidade técnica mais importante para mitigar este risco é o conhecimento em JavaScript, essencial para o desenvolvimento e depuração do front-end e para a comunicação com o back-end. A probabilidade é média, e o impacto seria médio, podendo resultar em bugs e problemas de usabilidade.

A segurança da plataforma é outro ponto crítico.  A implementação de mecanismos robustos de autenticação (login com email/senha, Google e Microsoft) e autorização, além da proteção contra ataques de injeção de SQL e outros tipos de vulnerabilidades, é fundamental.  A habilidade técnica mais importante para mitigar este risco é o conhecimento em SQL, para garantir a segurança do banco de dados. A probabilidade é média, e o impacto seria alto, podendo resultar em vazamento de dados e comprometimento da reputação do projeto.

Por fim, a gestão do projeto em si também apresenta riscos.  A falta de comunicação entre a equipe, atrasos na entrega de funcionalidades e problemas de escopo podem comprometer o sucesso do projeto.  Embora não haja uma única tech skill para mitigar este risco, a experiência em gestão de projetos ágeis é crucial. A probabilidade é média, e o impacto é alto, podendo levar ao fracasso do projeto como um todo.  A utilização do Azure para hospedagem também introduz riscos de dependência de um serviço externo, com potenciais problemas de performance e custos.  A habilidade em Azure é crucial para mitigar este risco, com probabilidade e impacto médios.


Exemplo 2:
Informações do Projeto:
Nome: FreeJobs

Descrição: Divulgue e contrate serviços autônomos através do nosso site, sempre que precisar estaremos aqui! Nossa plataforma FreeJobs trata-se de uma plataforma onde os usuários que a utilizar consigam com facilidade fazer a divulgação/contratação de serviços e trabalhos autônomos de maneira simples, prática e intuitiva se adequando ao uso e necessidade do usuário. Visando a situação atual do Brasil que sofre altas taxas de desemprego e a da perda de postos de trabalho para a tecnologia, nosso projeto visa ajudar qualquer trabalhador autônomo de baixa renda para que ele possa se sustentar financeiramente, mesmo que sem um emprego fixo. Assim oferecendo aos usuários de nosso site outros meios e opções de se sustentar financeiramente, que por vezes não conseguem vagas por conta da alta concorrência e do avanço da tecnologia ou apenas quer trabalhar de maneira autônoma e para si mesma. Nosso projeto foi desenvolvido acima de tudo na experiência que traria para os nossos usuários, portanto foi de extrema importância a atenção em tópicos como a segurança e a acessibilidade de nosso site. Para cumprir com tais requisitos usamos diversos conceitos e ferramentas das áreas da segurança de informação e acessibilidade digital como as técnicas e cuidados que tomamos ao coletar os dados dos usuários cadastrados e as normas e diretrizes de acessibilidade estipuladas pelo W3C (WCAG) e o Governo Digital Eletrônico (eMAG).

Tipo de projeto: Divulgação de Trabalho e Serviços pela Internet

Data de início: 14/02/2022

Data de finalização: 16/12/2022

Funcionalidades:
- Login e Logout sem token na plataforma
- Cadastro na plataforma
- Cadastro de serviço para perfil de prestador de serviços
- Solicitação de serviço para perfil de contratante de serviços
- Gerenciar meus serviços prestados
- Gerenciar meus serviços solicitados
- Visualizar todos os serviços
- Visualizar serviços pela busca
- Visualizar perfil de usuário
- Avaliar serviço prestado
- Chat entre prestador de serviço e quem o solicitou

Tecnologias: HTML, CSS e Javascript, PHP, SQL, phpMyAdmin.

Saída do exemplo 2:
O projeto FreeJobs, apesar de seu objetivo nobre de auxiliar trabalhadores autônomos, apresenta alguns riscos inerentes ao desenvolvimento de uma plataforma web. O prazo de desenvolvimento, de fevereiro a dezembro de 2022, embora mais extenso que o projeto Zenith, ainda requer gerenciamento cuidadoso para evitar atrasos. A ausência de um framework robusto no back-end, utilizando apenas PHP e SQL diretamente, aumenta a complexidade de manutenção e escalabilidade da plataforma a longo prazo. A dependência de um banco de dados SQL, sem menção a mecanismos robustos de segurança, representa um risco de vulnerabilidades, especialmente em relação a injeções SQL. A habilidade técnica mais importante para mitigar este risco é o conhecimento em SQL, visando a implementação de técnicas de prevenção contra essas vulnerabilidades. A probabilidade deste risco é média e o impacto é alto, podendo resultar em vazamento de dados dos usuários.

A implementação do sistema de login e logout sem tokens expõe a plataforma a riscos de segurança, uma vez que informações de sessão podem ser facilmente comprometidas. A adoção de um sistema de autenticação mais robusto, utilizando tokens JWT por exemplo, é crucial. A habilidade técnica mais importante aqui é o conhecimento em Javascript, essencial para gerenciamento de estado e segurança no lado cliente. A probabilidade deste risco é alta, e o impacto também é alto, podendo levar a um comprometimento da segurança dos dados e funcionalidades do sistema.

A funcionalidade de chat entre prestador e contratante demanda uma arquitetura bem planejada para lidar com a escalabilidade e a simultaneidade. Problemas de performance e estabilidade podem surgir com um grande número de usuários simultâneos. A habilidade em Javascript é crucial aqui, sendo a base para a construção de um sistema de chat eficiente e responsivo. A probabilidade deste risco é média, e o impacto também é médio, podendo gerar problemas de usabilidade e frustração aos usuários.

A falta de menção a testes de usabilidade e acessibilidade, apesar da menção aos princípios WCAG e eMAG, implica em um risco considerável de que a plataforma não atenda às necessidades dos usuários e não seja acessível a todos. A habilidade técnica mais importante para mitigar esse risco é o conhecimento em Javascript, fundamental para o desenvolvimento de testes e implementação de recursos de acessibilidade. A probabilidade é média, e o impacto é alto, podendo afetar significativamente o alcance e a usabilidade da plataforma, prejudicando sua adoção pelo público alvo. Por fim, a ausência de tecnologias mais modernas no front-end, como React ou Angular, pode dificultar a manutenção e a escalabilidade da interface no futuro, representando um risco de longo prazo em relação à atualização e modernização da plataforma. A habilidade em HTML é fundamental para garantir que o site se adapte aos diversos dispositivos e tamanhos de telas, garantindo a boa experiência do usuário. A probabilidade desse risco é baixa, mas o impacto é médio a longo prazo, podendo exigir um esforço de refatoração significativa no futuro.

Exemplo 3
Informações do Projeto:
Nome: RealFood

Descrição: Trata-se de uma plataforma que permite gerenciar busca de comidas em restaurantes disponíveis para entrega na região, seu principal objetivo é o de facilitar as interações entre cliente e restaurante.
Para isto a plataforma contará com o cadastro dos usuários com duas possíveis configurações, ou o usuário tem um perfil de cliente que realiza os pedidos para os restaurantes, ou tem um perfil de restaurante que irá ofertar os pratos disponíveis e será responsável, também, por realizar a entrega dos pedidos realizados pelos usuários na plataforma.
O sistema terá também diversas outras opções referentes ao gerenciamento do pedido como avaliação do pedido e da entrega, gerenciamento do status da entrega, salvar pedidos e/ou restaurantes favoritos, etc.

Tipo de projeto: Desenvolvimento e Implantação de Software

Data de início: 12/09/2024

Data de finalização: 23/11/2024

Funcionalidades:
- Cadastro: O usuário deverá ter a opção de se cadastrar na plataforma, utilizando um e-mail válido e uma nova senha
- Login: O usuário deverá poder se logar utilizando o e-mail e a senha cadastrados na plataforma
- Filtro de Pesquisa: O usuário deverá poder filtrar pela comida, ou restaurante, que estiver procurando na plataforma
- Acompanhar Pedido: O usuário deverá poder acompanhar o pedido realizado, desde seu status de preparação no restaurante até o status de entrega até o endereço solicitado
- Avaliar Pedido: Ao final da entrega, o usuário deverá poder avaliar como foi o pedido, seja a qualidade da comida, entrega, atendimento, etc.

Tecnologias: React Native, Json Server, Styled Components

Saida do exemplo 3:
O projeto RealFood, focado na gestão de pedidos de comida online, apresenta alguns riscos inerentes ao desenvolvimento de aplicativos mobile e plataformas web. O curto prazo de desenvolvimento, de setembro a novembro de 2024, exige um gerenciamento rigoroso para evitar atrasos. A utilização do Json Server como banco de dados, embora adequado para protótipos, representa um risco em termos de escalabilidade e segurança para um produto em produção. A habilidade técnica crucial para mitigar este risco é o conhecimento em SQL, permitindo a migração para um banco de dados relacional robusto e seguro. A probabilidade deste risco é média, e seu impacto é alto, podendo comprometer a performance e a segurança da plataforma com o crescimento do número de usuários.

A complexidade da implementação de funcionalidades como o acompanhamento e a avaliação de pedidos em tempo real requer uma arquitetura bem projetada e eficiente. Problemas de performance e concorrência podem surgir com um grande volume de pedidos simultâneos. A habilidade técnica mais importante para mitigar esse risco é o conhecimento em JavaScript, fundamental para o desenvolvimento de um sistema responsivo e escalável no front-end React Native. A probabilidade desse risco é média, e seu impacto também é médio, podendo levar a problemas de usabilidade e frustração do usuário.

A segurança da plataforma é outro ponto crítico. A proteção contra acessos não autorizados e a garantia da privacidade dos dados dos usuários (como informações de endereço e pagamento, se implementados) são essenciais. Embora não haja uma única tech skill para mitigar completamente esse risco, o conhecimento em JavaScript se mostra crucial, pois ele permite a implementação de medidas de segurança no lado cliente, como validações e criptografia. A probabilidade desse risco é média, e o impacto é alto, podendo resultar em perda de confiança dos usuários e em consequências legais.

A integração entre o front-end (React Native) e o back-end (Json Server) também apresenta riscos. Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades podem ocorrer. A habilidade em JavaScript é crucial para mitigar esse risco, pois é a linguagem principal do front-end e fundamental para garantir a comunicação eficaz com o back-end. A probabilidade é média e o impacto é médio, resultando em bugs e problemas de usabilidade. A falta de um sistema de autenticação robusto, apenas com e-mail e senha, apresenta risco de vulnerabilidades, como ataques de força bruta. Novamente, o JavaScript é importante aqui para a implementação de medidas de segurança no client-side, complementando um sistema de autenticação mais robusto no back-end, caso ele seja implementado. A probabilidade é média e o impacto é alto, resultando em comprometimento de dados e perda de confiança do usuário. Finalmente, a ausência de testes de usabilidade pode resultar em uma interface não intuitiva ou pouco amigável, comprometendo a experiência do usuário. A habilidade em JavaScript se mostra importante mais uma vez, pois facilita a integração de ferramentas e bibliotecas para testes automatizados de interface. A probabilidade é média e o impacto é médio, resultando em baixa adoção da plataforma e problemas de usabilidade.


Entrada:
Informações do Projeto:
Nome: {informacoesProjeto.Nome}

Descrição: {informacoesProjeto.Descricao}

Tipo de projeto: {informacoesProjeto.Tipo}

Data de início: {informacoesProjeto.DataInicial}

Data de finalização: {informacoesProjeto.DataFinal}

Funcionalidades: {funcionalidadesTexto}

Tecnologias: {tecnologiasTexto}

Saída:
";
        }

        public string GerarPromptRiscosProjeto(string analise)
        {
            return $@"
Estou trabalhando em um projeto onde ao receber uma análise em texto corrido de um setor, tenho de identificar e classificar os riscos retornados pela análise gerando uma lista de riscos para que possamos trabalhar em mitigá-los individualmente. Você receberá uma entrada na qual descreve a análise em formato de texto corrido. Com o texto em mãos identifique e faça a listagem em tópicos dos riscos catalogados na análise. Para cada risco listado, deve conter descrição, área em que o risco atinge (exempro: Front End, Back End, Marketing, Design, etc... Caso o risco afete tanto no Front End quanto no Back End liste sua área como sendo Full Stack), qual a tech skill (habilidade técnica) necessária para lidar com o risco (exemplo: HTML, JavaScript, Figma, SQL, etc...) importante: inclua apenas uma tech skill para cada risco listado, caso o risco possua mais que uma liste a mais importante para mitigá-lo, qual a probabilidade deste risco se concretizar durante o desenvolvimento do projeto e qual o impacto que este risco causaria para todo o desenvolvimento do projeto em si caso o risco se concretize muito importante: para a classificação da probabilidade e do impacto, indique-os apenas como 1 para baixo, 2 para médio ou 3 para alto, evite informações extras a este respeito. Identifique e faça a listagem de todos os riscos presentes na análise, não deixe nenhum de fora. Traga no mínimo 10 riscos a partir da análise elaborada e no mínimo 4 áreas diferentes, no entanto se durante a análise for identificado a existência de um número maior de riscos  liste-os também. Após finalizar a listagem dos riscos me retorne uma lista em JSON que represente os riscos listados, o JSON representará uma lista de objetos e cada objeto deverá conter como propriedades as características os riscos listados. O nome dos atributos entao serao: titulo, descricao, area, techSkill, probabilidade e impacto. Para as chaves de probabilidade e impacto utilize 1 para baixa, 2 para média e 3 para alta, logo um risco que tiver probabilidade média e impacto alto terá os campos ""probabilidade"" : 2 e ""impacto"" : 3.

Importante: Retorne o valor como um texto qualquer, no entanto simulando um json **sem as crases no início e no final** e iniciando e finalizando a resposta com colchetes []
Importante: Retorne o valor como um texto qualquer, no entanto simulando um json **sem as crases no início e no final** e iniciando e finalizando a resposta com colchetes []
Importante: Retorne o valor como um texto qualquer, no entanto simulando um json **sem as crases no início e no final** e iniciando e finalizando a resposta com colchetes []

Exemplo 1
Análise do Projeto:
O Projeto Zenith, com seu objetivo ambicioso de analisar riscos e otimizar alocação de equipes em projetos de implantação de sistemas, apresenta diversos riscos de desenvolvimento.  A complexidade da integração entre as diferentes tecnologias e funcionalidades, aliada ao curto prazo de desenvolvimento (de 23/01/2025 a 05/07/2025, aproximadamente 5 meses), aumenta significativamente a probabilidade de problemas.

Um risco significativo reside na integração do Gemini AI com o restante da plataforma. A dependência de uma API externa para a análise de riscos implica em uma alta probabilidade de problemas de compatibilidade, performance e disponibilidade da API.  A habilidade técnica crucial para mitigar esse risco é o conhecimento profundo em API REST Full, garantindo a comunicação eficiente e robusta entre o sistema e o Gemini AI. A probabilidade desse risco é alta, e o impacto no projeto seria igualmente alto, podendo atrasar significativamente o cronograma e comprometer a funcionalidade principal do sistema.

Outro risco reside na complexidade do algoritmo de alocação de equipes.  Desenvolver um algoritmo que considere soft skills, hard skills e tech skills, além da senioridade, para otimizar a alocação de recursos humanos é um desafio considerável.  A habilidade técnica mais importante para lidar com este risco é o conhecimento em C#, dada a sua utilização no ASP.NET Core, que será a base da lógica de negócio. A probabilidade deste risco é média, e o impacto também seria médio, podendo levar a atrasos e a uma alocação de equipes subótima.

A integração entre o front-end (React e Next.JS) e o back-end (ASP.NET Core e Entity Framework) também apresenta riscos.  Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades podem ocorrer. A habilidade técnica mais importante para mitigar este risco é o conhecimento em JavaScript, essencial para o desenvolvimento e depuração do front-end e para a comunicação com o back-end. A probabilidade é média, e o impacto seria médio, podendo resultar em bugs e problemas de usabilidade.

A segurança da plataforma é outro ponto crítico.  A implementação de mecanismos robustos de autenticação (login com email/senha, Google e Microsoft) e autorização, além da proteção contra ataques de injeção de SQL e outros tipos de vulnerabilidades, é fundamental.  A habilidade técnica mais importante para mitigar este risco é o conhecimento em SQL, para garantir a segurança do banco de dados. A probabilidade é média, e o impacto seria alto, podendo resultar em vazamento de dados e comprometimento da reputação do projeto.

Por fim, a gestão do projeto em si também apresenta riscos.  A falta de comunicação entre a equipe, atrasos na entrega de funcionalidades e problemas de escopo podem comprometer o sucesso do projeto.  Embora não haja uma única tech skill para mitigar este risco, a experiência em gestão de projetos ágeis é crucial. A probabilidade é média, e o impacto é alto, podendo levar ao fracasso do projeto como um todo.  A utilização do Azure para hospedagem também introduz riscos de dependência de um serviço externo, com potenciais problemas de performance e custos.  A habilidade em Azure é crucial para mitigar este risco, com probabilidade e impacto médios.

Saída do exemplo 1:
[
{{
""titulo"": ""Integração com Gemini AI"",
""descricao"": ""Problemas de compatibilidade, performance e disponibilidade da API do Gemini AI."",
""area"": ""Back End"",
""techSkill"": ""API REST Full"",
""Probabilidade"": ""3"",
""Impacto"": ""3""
}},
{{
""titulo"": ""Complexidade do algoritmo de alocação de equipes"",
""descricao"": "" Dificuldade em desenvolver um algoritmo que considere múltiplas habilidades e a senioridade para otimizar a alocação de recursos humanos."",
""area"": ""Back End"",
""techSkill"": ""C#"",
""Probabilidade"": ""2"",
""Impacto"": ""1""
}},
{{
""titulo"": ""Integração front-end e back-end"",
""descricao"": ""Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades."",
""area"": ""Full Stack"",
""techSkill"": ""JavaScript"",
""Probabilidade"": ""2"",
""Impacto"": ""2""
}},
{{
""titulo"": ""Segurança da plataforma"",
""descricao"": ""Vulnerabilidades de segurança, como ataques de injeção de SQL e falhas na autenticação e autorização."",
""area"": ""Back End"",
""techSkill"": ""SQL"",
""Probabilidade"": ""1"",
""Impacto"": ""3""
}},
{{
""titulo"": ""Gestão de Projeto"",
""descricao"": ""Falhas de comunicação, atrasos nas entregas e problemas de escopo."",
""area"": ""Gerenciamento"",
""techSkill"": ""Gestão de Projetos Ágeis"",
""Probabilidade"": ""2"",
""Impacto"": ""3""
}},
{{
""titulo"": ""Dependência do Azure"",
""descricao"": ""Potenciais problemas de performance e custos relacionados à hospedagem no Azure."",
""area"": ""Infraestrutura"",
""techSkill"": ""Azure"",
""Probabilidade"": ""2"",
""Impacto"": ""2""
}}
]

Exemplo 2:
Análise do Projeto:
O projeto FreeJobs, apesar de seu objetivo nobre de auxiliar trabalhadores autônomos, apresenta alguns riscos inerentes ao desenvolvimento de uma plataforma web. O prazo de desenvolvimento, de fevereiro a dezembro de 2022, embora mais extenso que o projeto Zenith, ainda requer gerenciamento cuidadoso para evitar atrasos. A ausência de um framework robusto no back-end, utilizando apenas PHP e SQL diretamente, aumenta a complexidade de manutenção e escalabilidade da plataforma a longo prazo. A dependência de um banco de dados SQL, sem menção a mecanismos robustos de segurança, representa um risco de vulnerabilidades, especialmente em relação a injeções SQL. A habilidade técnica mais importante para mitigar este risco é o conhecimento em SQL, visando a implementação de técnicas de prevenção contra essas vulnerabilidades. A probabilidade deste risco é média e o impacto é alto, podendo resultar em vazamento de dados dos usuários.

A implementação do sistema de login e logout sem tokens expõe a plataforma a riscos de segurança, uma vez que informações de sessão podem ser facilmente comprometidas. A adoção de um sistema de autenticação mais robusto, utilizando tokens JWT por exemplo, é crucial. A habilidade técnica mais importante aqui é o conhecimento em Javascript, essencial para gerenciamento de estado e segurança no lado cliente. A probabilidade deste risco é alta, e o impacto também é alto, podendo levar a um comprometimento da segurança dos dados e funcionalidades do sistema.

A funcionalidade de chat entre prestador e contratante demanda uma arquitetura bem planejada para lidar com a escalabilidade e a simultaneidade. Problemas de performance e estabilidade podem surgir com um grande número de usuários simultâneos. A habilidade em Javascript é crucial aqui, sendo a base para a construção de um sistema de chat eficiente e responsivo. A probabilidade deste risco é média, e o impacto também é médio, podendo gerar problemas de usabilidade e frustração aos usuários.

A falta de menção a testes de usabilidade e acessibilidade, apesar da menção aos princípios WCAG e eMAG, implica em um risco considerável de que a plataforma não atenda às necessidades dos usuários e não seja acessível a todos. A habilidade técnica mais importante para mitigar esse risco é o conhecimento em Javascript, fundamental para o desenvolvimento de testes e implementação de recursos de acessibilidade. A probabilidade é média, e o impacto é alto, podendo afetar significativamente o alcance e a usabilidade da plataforma, prejudicando sua adoção pelo público alvo. Por fim, a ausência de tecnologias mais modernas no front-end, como React ou Angular, pode dificultar a manutenção e a escalabilidade da interface no futuro, representando um risco de longo prazo em relação à atualização e modernização da plataforma. A habilidade em HTML é fundamental para garantir que o site se adapte aos diversos dispositivos e tamanhos de telas, garantindo a boa experiência do usuário. A probabilidade desse risco é baixa, mas o impacto é médio a longo prazo, podendo exigir um esforço de refatoração significativa no futuro.

Saída do exemplo 2:
[
{{
""titulo"": ""Vulnerabilidades de segurança em banco de dados"",
""descricao"": ""Risco de injeções SQL devido à ausência de mecanismos robustos de segurança no banco de dados SQL."",
""area"": ""Back End"",
""techSkill"": ""SQL"",
""probabilidade"": 2,
""impacto"": 3,
""nivel"": 0
}},
{{
""titulo"": ""Sistema de login/logout inseguro"",
""descricao"": ""Risco de comprometimento de informações de sessão devido à ausência de tokens na implementação do sistema de login e logout."",
""area"": ""Full Stack"",
""techSkill"": ""Javascript"",
""probabilidade"": 3,
""impacto"": 3,
""nivel"": 0
}},
{{
""titulo"": ""Escalabilidade e performance do chat"",
""descricao"": ""Risco de problemas de performance e estabilidade no sistema de chat com um grande número de usuários simultâneos."",
""area"": ""Full Stack"",
""techSkill"": ""Javascript"",
""probabilidade"": 2,
""impacto"": 2,
""nivel"": 0
}},
{{
""titulo"": ""Falta de testes de usabilidade e acessibilidade"",
""descricao"": ""Risco de a plataforma não atender às necessidades dos usuários e não ser acessível a todos, devido à ausência de testes de usabilidade e acessibilidade."",
""area"": ""Front End"",
""techSkill"": ""Javascript"",
""probabilidade"": 2,
""impacto"": 3,
""nivel"": 0
}},
{{
""titulo"": ""Falta de framework robusto no back-end"",
""descricao"": ""A utilização apenas de PHP e SQL diretamente aumenta a complexidade de manutenção e escalabilidade da plataforma a longo prazo."",
""area"": ""Back End"",
""techSkill"": ""PHP"",
""probabilidade"": 2,
""impacto"": 2,
""nivel"": 0
}},
{{
""titulo"": ""Falta de tecnologias modernas no front-end"",
""descricao"": ""Ausência de tecnologias como React ou Angular pode dificultar a manutenção e escalabilidade da interface no futuro."",
""area"": ""Front End"",
""techSkill"": ""HTML"",
""probabilidade"": 1,
""impacto"": 2,
""nivel"": 0
}},
{{
""titulo"": ""Gestão de tempo e prazos"",
""descricao"": ""Risco de atrasos no desenvolvimento devido à necessidade de gerenciamento cuidadoso do prazo extenso, apesar de mais longo que o projeto Zenith."",
""area"": ""Gerenciamento"",
""techSkill"": ""Metodologias Ágeis"",
""probabilidade"": 2,
""impacto"": 2,
""nivel"": 0
}},
{{
""titulo"": ""Manutenção e atualização de código PHP"",
""descricao"": ""O uso de PHP sem um framework pode levar à dificuldade na manutenção e atualização do código-fonte, tornando o sistema mais vulnerável e propenso a erros."",
""area"": ""Back End"",
""techSkill"": ""PHP"",
""probabilidade"": 2,
""impacto"": 2,
""nivel"": 0
}},
{{
""titulo"": ""Compatibilidade entre Front-end e Back-end"",
""descricao"": ""Problemas de integração entre o front-end e o back-end podem gerar erros e falhas no funcionamento do sistema, comprometendo a experiência do usuário."",
""area"": ""Full Stack"",
""techSkill"": ""API REST"",
""probabilidade"": 2,
""impacto"": 2,
""nivel"": 0
}},
{{
""titulo"": ""Otimização de consultas SQL"",
""descricao"": ""Consultas SQL mal otimizadas podem gerar lentidão e problemas de performance na aplicação, afetando a experiência do usuário."",
""area"": ""Back End"",
""techSkill"": ""SQL"",
""probabilidade"": 2,
""impacto"": 2,
""nivel"": 0
}},
{{
""titulo"": ""Teste e Qualidade do código"",
""descricao"": ""A ausência de testes unitários e de integração pode levar a erros e bugs que só serão detectados em fases posteriores do desenvolvimento, aumentando custos e tempo."",
""area"": ""Full Stack"",
""techSkill"": ""Testes Unitários"",
""probabilidade"": 2,
""impacto"": 2,
""nivel"": 0
}}
]

Exemplo 3:
O projeto RealFood, focado na gestão de pedidos de comida online, apresenta alguns riscos inerentes ao desenvolvimento de aplicativos mobile e plataformas web. O curto prazo de desenvolvimento, de setembro a novembro de 2024, exige um gerenciamento rigoroso para evitar atrasos. A utilização do Json Server como banco de dados, embora adequado para protótipos, representa um risco em termos de escalabilidade e segurança para um produto em produção. A habilidade técnica crucial para mitigar este risco é o conhecimento em SQL, permitindo a migração para um banco de dados relacional robusto e seguro. A probabilidade deste risco é média, e seu impacto é alto, podendo comprometer a performance e a segurança da plataforma com o crescimento do número de usuários.

A complexidade da implementação de funcionalidades como o acompanhamento e a avaliação de pedidos em tempo real requer uma arquitetura bem projetada e eficiente. Problemas de performance e concorrência podem surgir com um grande volume de pedidos simultâneos. A habilidade técnica mais importante para mitigar esse risco é o conhecimento em JavaScript, fundamental para o desenvolvimento de um sistema responsivo e escalável no front-end React Native. A probabilidade desse risco é média, e seu impacto também é médio, podendo levar a problemas de usabilidade e frustração do usuário.

A segurança da plataforma é outro ponto crítico. A proteção contra acessos não autorizados e a garantia da privacidade dos dados dos usuários (como informações de endereço e pagamento, se implementados) são essenciais. Embora não haja uma única tech skill para mitigar completamente esse risco, o conhecimento em JavaScript se mostra crucial, pois ele permite a implementação de medidas de segurança no lado cliente, como validações e criptografia. A probabilidade desse risco é média, e o impacto é alto, podendo resultar em perda de confiança dos usuários e em consequências legais.

A integração entre o front-end (React Native) e o back-end (Json Server) também apresenta riscos. Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades podem ocorrer. A habilidade em JavaScript é crucial para mitigar esse risco, pois é a linguagem principal do front-end e fundamental para garantir a comunicação eficaz com o back-end. A probabilidade é média e o impacto é médio, resultando em bugs e problemas de usabilidade. A falta de um sistema de autenticação robusto, apenas com e-mail e senha, apresenta risco de vulnerabilidades, como ataques de força bruta. Novamente, o JavaScript é importante aqui para a implementação de medidas de segurança no client-side, complementando um sistema de autenticação mais robusto no back-end, caso ele seja implementado. A probabilidade é média e o impacto é alto, resultando em comprometimento de dados e perda de confiança do usuário. Finalmente, a ausência de testes de usabilidade pode resultar em uma interface não intuitiva ou pouco amigável, comprometendo a experiência do usuário. A habilidade em JavaScript se mostra importante mais uma vez, pois facilita a integração de ferramentas e bibliotecas para testes automatizados de interface. A probabilidade é média e o impacto é médio, resultando em baixa adoção da plataforma e problemas de usabilidade.

Saída do exemplo 3:
[
  {{
    ""titulo"": ""Escalabilidade e segurança do Json Server"",
    ""descricao"": ""O Json Server, embora adequado para protótipos, apresenta riscos de escalabilidade e segurança para um produto em produção."",
    ""area"": ""Back End"",
    ""techSkill"": ""SQL"",
    ""probabilidade"": 2,
    ""impacto"": 3,
    ""nivel"": 0
  }},
  {{
    ""titulo"": ""Performance e Concorrência no Acompanhamento de Pedidos"",
    ""descricao"": ""Problemas de performance e concorrência podem surgir com um grande volume de pedidos simultâneos na funcionalidade de acompanhamento em tempo real."",
    ""area"": ""Full Stack"",
    ""techSkill"": ""JavaScript"",
    ""probabilidade"": 2,
    ""impacto"": 2,
    ""nivel"": 0
  }},
  {{
    ""titulo"": ""Segurança da Plataforma"",
    ""descricao"": ""Riscos relacionados a acessos não autorizados e a garantia da privacidade dos dados do usuário (endereço e pagamento)."",
    ""area"": ""Full Stack"",
    ""techSkill"": ""JavaScript"",
    ""probabilidade"": 2,
    ""impacto"": 3,
    ""nivel"": 0
  }},
  {{
    ""titulo"": ""Integração Front-end (React Native) e Back-end (Json Server)"",
    ""descricao"": ""Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades."",
    ""area"": ""Full Stack"",
    ""techSkill"": ""JavaScript"",
    ""probabilidade"": 2,
    ""impacto"": 2,
    ""nivel"": 0
  }},
  {{
    ""titulo"": ""Sistema de Autenticação Frágil"",
    ""descricao"": ""A autenticação apenas com e-mail e senha apresenta riscos de vulnerabilidades, como ataques de força bruta."",
    ""area"": ""Full Stack"",
    ""techSkill"": ""JavaScript"",
    ""probabilidade"": 2,
    ""impacto"": 3,
    ""nivel"": 0
  }},
  {{
    ""titulo"": ""Falta de Testes de Usabilidade"",
    ""descricao"": ""A ausência de testes de usabilidade pode resultar em uma interface não intuitiva ou pouco amigável."",
    ""area"": ""Front End"",
    ""techSkill"": ""JavaScript"",
    ""probabilidade"": 2,
    ""impacto"": 2,
    ""nivel"": 0
  }},
  {{
    ""titulo"": ""Gestão de Tempo e Prazos"",
    ""descricao"": ""O curto prazo de desenvolvimento (setembro a novembro de 2024) exige gerenciamento rigoroso para evitar atrasos."",
    ""area"": ""Gerenciamento"",
    ""techSkill"": ""Metodologias Ágeis"",
    ""probabilidade"": 2,
    ""impacto"": 3,
    ""nivel"": 0
  }},
  {{
    ""titulo"": ""Escalabilidade do aplicativo mobile"",
    ""descricao"": ""O aplicativo mobile precisa ser escalável para suportar um número crescente de usuários e pedidos simultâneos."",
    ""area"": ""Mobile"",
    ""techSkill"": ""React Native"",
    ""probabilidade"": 2,
    ""impacto"": 2,
    ""nivel"": 0
  }},
    {{
    ""titulo"": ""Manutenção do código React Native"",
    ""descricao"": ""A longo prazo, manter e atualizar o código React Native pode se tornar complexo se não houver um planejamento adequado."",
    ""area"": ""Mobile"",
    ""techSkill"": ""React Native"",
    ""probabilidade"": 1,
    ""impacto"": 2,
    ""nivel"": 0
  }},
  {{
    ""titulo"": ""Integração com Serviços de Pagamento"",
    ""descricao"": ""A integração com gateways de pagamento pode apresentar problemas de segurança e compatibilidade se não for bem planejada."",
    ""area"": ""Back End"",
    ""techSkill"": ""API REST"",
    ""probabilidade"": 2,
    ""impacto"": 3,
    ""nivel"": 0
  }},
  {{
    ""titulo"": ""Compatibilidade entre plataformas"",
    ""descricao"": ""Garantir que o aplicativo funcione corretamente em diferentes dispositivos e sistemas operacionais (iOS e Android)."",
    ""area"": ""Mobile"",
    ""techSkill"": ""React Native"",
    ""probabilidade"": 2,
    ""impacto"": 2,
    ""nivel"": 0
  }}
]

Entrada:
Análise do Projeto:
{analise}

Saída:
";
        }

        public string GerarPromptRangeComplexidadeProjeto(InfoProjectSettings informacoesProjeto, string analiseProjeto)
        {
            string funcionalidadesTexto = "";

            foreach (string funcionalidade in informacoesProjeto.Funcionalidades)
            {
                funcionalidadesTexto += $"- {funcionalidade}\n";
            }

            string tecnologiasTexto = "";

            foreach (string tecnologia in informacoesProjeto.Tecnologias)
            {
                tecnologiasTexto += $", {tecnologia}";
            }

            return $@"
Você está realizando um projeto que tem como funcionalidade definir qual a complexidade de um projeto de acordo com seus aspectos e características na área técnica, funcional e de ambiente do escopo do projeto. Para esta tarefa será concedido as principais informações gerais à respeito do projeto como nome, descrição, prazo e etc. Além disso será fornecido também uma análise de riscos atrelados a este projeto junto de suas características como descrição, área de que o risco afeta, teck skill/habilidade técnica necessária para mitigar tal risco, nível do risco e etc. Com estes parâmetros você deve analisar o escopo e as características do projeto a fim de avaliar diversos tópicos dentro das áreas: técnica, funcional e de ambiente do projeto. Sua missão é gerar uma nota de 0 a 10 (sendo 0 nenhuma baixa complexidade e 10 para muito complexo) para cada um dos atributos que informarei a seguir para cada uma das áreas informadas anteriormente:

Áreas:
     Técnica:
           Funcionalidades do Escopo: (nota de 0 a 10)
           Compatibilidade das Soluções: (nota de 0 a 10)
           Aspectos da Infraestrutura: (nota de 0 a 10)
     Funcional:
           Criticidade de Operação: (nota de 0 a 10)
           Benchmark no Negócio: (nota de 0 a 10)
           Paralelismo de Tarefas: (nota de 0 a 10)
     Ambiente:
           Nível de Relacionamento: (nota de 0 a 10)
           Margem Financeira Estreita: (nota de 0 a 10)
           Objetivo Estratégico: (nota de 0 a 10)

**Importante: retorne apenas a nota para cada tópico junto do nome do mesmo, evite a descrição ou explicação do porquê desta nota na avaliação. Se não for fornecido informações suficientes sobre a margem financeira avalie como 5**  

**Retorne um JSON contendo apenas os valores gerados, o JSON representará uma lista de vetorese cada vetor deverá representar uma área analisada (técnica, funcional e ambiente) contendo apenas os 3 valores resultantes da avaliação realizada. Sua estrutura então será:

[
   [avaliacaoTecnico1, avaliacaoTecnico2, avaliacaoTecnico3],
   [avaliacaoFuncional1],[avaliacaoFuncional2],[avaliacaoFuncional3]
   [avaliacaoAmbiente1],[avaliacaoAmbiente2],[avaliacaoAmbiente3]
]

. Importante: Retorne o valor como um texto qualquer, no entanto simulando um json **sem as crases no início e no final** e iniciando e finalizando a resposta com colchetes []

Importante: Retorne o valor como um texto qualquer, no entanto simulando um json **sem as crases no início e no final** e iniciando e finalizando a resposta com colchetes []
Importante: Retorne o valor como um texto qualquer, no entanto simulando um json **sem as crases no início e no final** e iniciando e finalizando a resposta com colchetes []
Importante: Retorne o valor como um texto qualquer, no entanto simulando um json **sem as crases no início e no final** e iniciando e finalizando a resposta com colchetes []


Exemplo 1
Informações do Projeto:
Nome: Projeto Zenith

Descrição: O Zenith é um sistema que tem como objetivo realizar a análise de riscos de um projeto de implantação de sistemas, gerando relatórios sobre os mesmos e alocando uma equipe otimizada para a realização do mesmo. Em projetos de implantação de sistemas, é crucial antecipar e gerenciar riscos para garantir o sucesso do projeto. Uma alocação estratégica de recursos humanos, considerando a experiência e especialização de cada membro, pode minimizar estes riscos e otimizar os resultados. Desenvolver um algoritmo ou processo que realize a análise de riscos em projetos de implantação de sistemas, gerando uma alocação otimizada de profissionais de diferentes níveis de senioridade (júnior, pleno, sênior) para mitigar riscos identificados. Tendo em vista este cenário, se faz necessário uma plataforma que:

- Analise riscos de um projeto: a partir do escopo e cronograma que determinado projeto de implantação de sistema possua, analisar os riscos atrelados ao mesmo e listar tais riscos, assim como qual seus impactos e quais as qualificações necessárias para mitigá-los.
- Alocação de equipes para o projeto: com base na análise de riscos realizada, definir equipes recomendadas levando em consideração parâmetros como soft skills, hard skills e tech skills (junto da senioridade do mesmo) na definição de uma equipe otimizada e ideal para a realização do projeto
- Gerenciamento de análises e equipes: após a realização de análises e alocação de equipes ideais, a plataforma permitirá gerenciamento de análises de projetos realizadas na plataforma e gerenciamento de equipes definidas para os mesmos.

Tipo de projeto: Implementação de software na empresa

Data de início: 23/01/2025

Data de finalização: 05/07/2025

Funcionalidades: 
- O gestor, previamente cadastrado, poderá efetuar seu login pelo seu email e senha cadastrado, ou pelo email do Google e/ou Microsoft, contanto que o mesmo tenha sido previamente cadastrado na plataforma
- O usuário poderá efetuar o logout da plataforma conforme necessário
- O administrador da plataforma poderá realizar o cadastro de um gestor ou de um funcionário a partir de uma página privada apenas para administradores
- O usuário poderá recuperar a senha de acesso caso a tenha esquecido por meio de seu email cadastrado
- O gestor poderá cadastrar uma análise de um projeto a partir de seu escopo e conograma fornecido
- O gestor poderá visualizar as informações de determinada análise realizada na plataforma, verificando os riscos identificados, como mitigá-los durante o desenvolvimento do projeto e quais são os possíveis funcionários que irão compor a equipe ideal para a realização do projeto
- O gestor poderá visualizar o histórico de todas as análises de projetos realizadas na plataforma, com ou sem filtros de pesquisa. Caso opte por utilizar filtros de pesquisa, poderá filtrar por nome do projeto, data do projeto ou escopo geral do mesmo
- Ao terminar a análise, caberá ao gestor definir a equipe ideal para a realização do projeto, podendo ser feito de maneira pessoal com a lista de funcionários geral ou sugeridos para o projeto, ou pederá ser feito por meio da equipe ideal sugerida pela própria plataforma.
- O gestor poderá editar a equipe definida para a realização de determinado projeto de acordo com a necessidade.
- O gestor poderá excluir análises de projetos do histórico de análises realizadas caso necessário
- O usuário poderá visualizar a lista de funcionários que foram cadastrados na plataforma pelo mesmo
- O usuário poderá visualizar as informações gerais de determinado funcionário cadastrado na plataforma, assim como em quais projetos e equipes o mesmo foi alocado em análises realizadas na plataforma
- O administrador poderá editar determinadas informações do funcionário, atualizando suas habilidades ou alterando informações pessoais caso necessário
- O administrador poderá excluir determinado funcionário cadastrado na plataforma caso necessário
- O administrador poderá atualizar informações do escopo e cronograma do projeto a fim de realizar uma nova análise acerca do mesmo.
- O usuário poderá filtrar a visualização de análises realizadas na plataforma por parâmetros diversos como cronograma, nome e escopo e funcionários envolvidos
- O usuário poderá filtrar a visualização de funcionários por parâmetros diversos como nome, techskills e participação em projetos.

Tecnologias: Next.JS, ASPNET.CORE, EntityFramework, React, Azure, SQL Database, API REST Full, Gemini AI, C#, HTML, CSS, JavaScript 

Análise do Projeto:
O Projeto Zenith, com seu objetivo ambicioso de analisar riscos e otimizar alocação de equipes em projetos de implantação de sistemas, apresenta diversos riscos de desenvolvimento.  A complexidade da integração entre as diferentes tecnologias e funcionalidades, aliada ao curto prazo de desenvolvimento (de 23/01/2025 a 05/07/2025, aproximadamente 5 meses), aumenta significativamente a probabilidade de problemas.

Um risco significativo reside na integração do Gemini AI com o restante da plataforma. A dependência de uma API externa para a análise de riscos implica em uma alta probabilidade de problemas de compatibilidade, performance e disponibilidade da API.  A habilidade técnica crucial para mitigar esse risco é o conhecimento profundo em API REST Full, garantindo a comunicação eficiente e robusta entre o sistema e o Gemini AI. A probabilidade desse risco é alta, e o impacto no projeto seria igualmente alto, podendo atrasar significativamente o cronograma e comprometer a funcionalidade principal do sistema.

Outro risco reside na complexidade do algoritmo de alocação de equipes.  Desenvolver um algoritmo que considere soft skills, hard skills e tech skills, além da senioridade, para otimizar a alocação de recursos humanos é um desafio considerável.  A habilidade técnica mais importante para lidar com este risco é o conhecimento em C#, dada a sua utilização no ASP.NET Core, que será a base da lógica de negócio. A probabilidade deste risco é média, e o impacto também seria médio, podendo levar a atrasos e a uma alocação de equipes subótima.

A integração entre o front-end (React e Next.JS) e o back-end (ASP.NET Core e Entity Framework) também apresenta riscos.  Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades podem ocorrer. A habilidade técnica mais importante para mitigar este risco é o conhecimento em JavaScript, essencial para o desenvolvimento e depuração do front-end e para a comunicação com o back-end. A probabilidade é média, e o impacto seria médio, podendo resultar em bugs e problemas de usabilidade.

A segurança da plataforma é outro ponto crítico.  A implementação de mecanismos robustos de autenticação (login com email/senha, Google e Microsoft) e autorização, além da proteção contra ataques de injeção de SQL e outros tipos de vulnerabilidades, é fundamental.  A habilidade técnica mais importante para mitigar este risco é o conhecimento em SQL, para garantir a segurança do banco de dados. A probabilidade é média, e o impacto seria alto, podendo resultar em vazamento de dados e comprometimento da reputação do projeto.

Por fim, a gestão do projeto em si também apresenta riscos.  A falta de comunicação entre a equipe, atrasos na entrega de funcionalidades e problemas de escopo podem comprometer o sucesso do projeto.  Embora não haja uma única tech skill para mitigar este risco, a experiência em gestão de projetos ágeis é crucial. A probabilidade é média, e o impacto é alto, podendo levar ao fracasso do projeto como um todo.  A utilização do Azure para hospedagem também introduz riscos de dependência de um serviço externo, com potenciais problemas de performance e custos.  A habilidade em Azure é crucial para mitigar este risco, com probabilidade e impacto médios.


Saída do exemplo 1:
[
[7, 6, 8],
[9, 7, 6],
[6, 5, 8]
]

(Neste caso foram decididas as seguintes notas:
  Técnica:
           Funcionalidades do Escopo: 7
           Compatibilidade das Soluções: 6
           Aspectos da Infraestrutura: 8
     Funcional:
           Criticidade de Operação: 9
           Benchmark no Negócio: 7
           Paralelismo de Tarefas: 6
     Ambiente:
           Nível de Relacionamento: 6
           Margem Financeira Estreita: 5
           Objetivo Estratégico: 8
)

Exemplo 2:
Informações do Projeto:
Nome: FreeJobs

Descrição: Divulgue e contrate serviços autônomos através do nosso site, sempre que precisar estaremos aqui! Nossa plataforma FreeJobs trata-se de uma plataforma onde os usuários que a utilizar consigam com facilidade fazer a divulgação/contratação de serviços e trabalhos autônomos de maneira simples, prática e intuitiva se adequando ao uso e necessidade do usuário. Visando a situação atual do Brasil que sofre altas taxas de desemprego e a da perda de postos de trabalho para a tecnologia, nosso projeto visa ajudar qualquer trabalhador autônomo de baixa renda para que ele possa se sustentar financeiramente, mesmo que sem um emprego fixo. Assim oferecendo aos usuários de nosso site outros meios e opções de se sustentar financeiramente, que por vezes não conseguem vagas por conta da alta concorrência e do avanço da tecnologia ou apenas quer trabalhar de maneira autônoma e para si mesma. Nosso projeto foi desenvolvido acima de tudo na experiência que traria para os nossos usuários, portanto foi de extrema importância a atenção em tópicos como a segurança e a acessibilidade de nosso site. Para cumprir com tais requisitos usamos diversos conceitos e ferramentas das áreas da segurança de informação e acessibilidade digital como as técnicas e cuidados que tomamos ao coletar os dados dos usuários cadastrados e as normas e diretrizes de acessibilidade estipuladas pelo W3C (WCAG) e o Governo Digital Eletrônico (eMAG).

Tipo de projeto: Divulgação de Trabalho e Serviços pela Internet

Data de início: 14/02/2022

Data de finalização: 16/12/2022

Funcionalidades:
- Login e Logout sem token na plataforma
- Cadastro na plataforma
- Cadastro de serviço para perfil de prestador de serviços
- Solicitação de serviço para perfil de contratante de serviços
- Gerenciar meus serviços prestados
- Gerenciar meus serviços solicitados
- Visualizar todos os serviços
- Visualizar serviços pela busca
- Visualizar perfil de usuário
- Avaliar serviço prestado
- Chat entre prestador de serviço e quem o solicitou

Tecnologias: HTML, CSS e Javascript, PHP, SQL, phpMyAdmin.

Análise do Projeto:
 projeto FreeJobs, apesar de seu objetivo nobre de auxiliar trabalhadores autônomos, apresenta alguns riscos inerentes ao desenvolvimento de uma plataforma web. O prazo de desenvolvimento, de fevereiro a dezembro de 2022, embora mais extenso que o projeto Zenith, ainda requer gerenciamento cuidadoso para evitar atrasos. A ausência de um framework robusto no back-end, utilizando apenas PHP e SQL diretamente, aumenta a complexidade de manutenção e escalabilidade da plataforma a longo prazo. A dependência de um banco de dados SQL, sem menção a mecanismos robustos de segurança, representa um risco de vulnerabilidades, especialmente em relação a injeções SQL. A habilidade técnica mais importante para mitigar este risco é o conhecimento em SQL, visando a implementação de técnicas de prevenção contra essas vulnerabilidades. A probabilidade deste risco é média e o impacto é alto, podendo resultar em vazamento de dados dos usuários.

A implementação do sistema de login e logout sem tokens expõe a plataforma a riscos de segurança, uma vez que informações de sessão podem ser facilmente comprometidas. A adoção de um sistema de autenticação mais robusto, utilizando tokens JWT por exemplo, é crucial. A habilidade técnica mais importante aqui é o conhecimento em Javascript, essencial para gerenciamento de estado e segurança no lado cliente. A probabilidade deste risco é alta, e o impacto também é alto, podendo levar a um comprometimento da segurança dos dados e funcionalidades do sistema.

A funcionalidade de chat entre prestador e contratante demanda uma arquitetura bem planejada para lidar com a escalabilidade e a simultaneidade. Problemas de performance e estabilidade podem surgir com um grande número de usuários simultâneos. A habilidade em Javascript é crucial aqui, sendo a base para a construção de um sistema de chat eficiente e responsivo. A probabilidade deste risco é média, e o impacto também é médio, podendo gerar problemas de usabilidade e frustração aos usuários.

A falta de menção a testes de usabilidade e acessibilidade, apesar da menção aos princípios WCAG e eMAG, implica em um risco considerável de que a plataforma não atenda às necessidades dos usuários e não seja acessível a todos. A habilidade técnica mais importante para mitigar esse risco é o conhecimento em Javascript, fundamental para o desenvolvimento de testes e implementação de recursos de acessibilidade. A probabilidade é média, e o impacto é alto, podendo afetar significativamente o alcance e a usabilidade da plataforma, prejudicando sua adoção pelo público alvo. Por fim, a ausência de tecnologias mais modernas no front-end, como React ou Angular, pode dificultar a manutenção e a escalabilidade da interface no futuro, representando um risco de longo prazo em relação à atualização e modernização da plataforma. A habilidade em HTML é fundamental para garantir que o site se adapte aos diversos dispositivos e tamanhos de telas, garantindo a boa experiência do usuário. A probabilidade desse risco é baixa, mas o impacto é médio a longo prazo, podendo exigir um esforço de refatoração significativa no futuro.


Saída do exemplo 2:

[
[5, 6, 7],
[7, 4, 6],
[5, 5, 6]
]

(Neste caso foram decididas as seguintes notas:
  Técnica:
           Funcionalidades do Escopo: 5
           Compatibilidade das Soluções: 6
           Aspectos da Infraestrutura: 7
     Funcional:
           Criticidade de Operação: 7
           Benchmark no Negócio: 4
           Paralelismo de Tarefas: 6
     Ambiente:
           Nível de Relacionamento: 5
           Margem Financeira Estreita: 5
           Objetivo Estratégico: 6
)

Exemplo 3:
Informações do Projeto:
Nome: RealFood

Descrição: Trata-se de uma plataforma que permite gerenciar busca de comidas em restaurantes disponíveis para entrega na região, seu principal objetivo é o de facilitar as interações entre cliente e restaurante.
Para isto a plataforma contará com o cadastro dos usuários com duas possíveis configurações, ou o usuário tem um perfil de cliente que realiza os pedidos para os restaurantes, ou tem um perfil de restaurante que irá ofertar os pratos disponíveis e será responsável, também, por realizar a entrega dos pedidos realizados pelos usuários na plataforma.
O sistema terá também diversas outras opções referentes ao gerenciamento do pedido como avaliação do pedido e da entrega, gerenciamento do status da entrega, salvar pedidos e/ou restaurantes favoritos, etc.

Tipo de projeto: Desenvolvimento e Implantação de Software

Data de início: 12/09/2024

Data de finalização: 23/11/2024

Funcionalidades:
- Cadastro: O usuário deverá ter a opção de se cadastrar na plataforma, utilizando um e-mail válido e uma nova senha
- Login: O usuário deverá poder se logar utilizando o e-mail e a senha cadastrados na plataforma
- Filtro de Pesquisa: O usuário deverá poder filtrar pela comida, ou restaurante, que estiver procurando na plataforma
- Acompanhar Pedido: O usuário deverá poder acompanhar o pedido realizado, desde seu status de preparação no restaurante até o status de entrega até o endereço solicitado
- Avaliar Pedido: Ao final da entrega, o usuário deverá poder avaliar como foi o pedido, seja a qualidade da comida, entrega, atendimento, etc.

Tecnologias: React Native, Json Server, Styled Components

Análise do Projeto:
O projeto RealFood, focado na gestão de pedidos de comida online, apresenta alguns riscos inerentes ao desenvolvimento de aplicativos mobile e plataformas web. O curto prazo de desenvolvimento, de setembro a novembro de 2024, exige um gerenciamento rigoroso para evitar atrasos. A utilização do Json Server como banco de dados, embora adequado para protótipos, representa um risco em termos de escalabilidade e segurança para um produto em produção. A habilidade técnica crucial para mitigar este risco é o conhecimento em SQL, permitindo a migração para um banco de dados relacional robusto e seguro. A probabilidade deste risco é média, e seu impacto é alto, podendo comprometer a performance e a segurança da plataforma com o crescimento do número de usuários.

A complexidade da implementação de funcionalidades como o acompanhamento e a avaliação de pedidos em tempo real requer uma arquitetura bem projetada e eficiente. Problemas de performance e concorrência podem surgir com um grande volume de pedidos simultâneos. A habilidade técnica mais importante para mitigar esse risco é o conhecimento em JavaScript, fundamental para o desenvolvimento de um sistema responsivo e escalável no front-end React Native. A probabilidade desse risco é média, e seu impacto também é médio, podendo levar a problemas de usabilidade e frustração do usuário.

A segurança da plataforma é outro ponto crítico. A proteção contra acessos não autorizados e a garantia da privacidade dos dados dos usuários (como informações de endereço e pagamento, se implementados) são essenciais. Embora não haja uma única tech skill para mitigar completamente esse risco, o conhecimento em JavaScript se mostra crucial, pois ele permite a implementação de medidas de segurança no lado cliente, como validações e criptografia. A probabilidade desse risco é média, e o impacto é alto, podendo resultar em perda de confiança dos usuários e em consequências legais.

A integração entre o front-end (React Native) e o back-end (Json Server) também apresenta riscos. Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades podem ocorrer. A habilidade em JavaScript é crucial para mitigar esse risco, pois é a linguagem principal do front-end e fundamental para garantir a comunicação eficaz com o back-end. A probabilidade é média e o impacto é médio, resultando em bugs e problemas de usabilidade. A falta de um sistema de autenticação robusto, apenas com e-mail e senha, apresenta risco de vulnerabilidades, como ataques de força bruta. Novamente, o JavaScript é importante aqui para a implementação de medidas de segurança no client-side, complementando um sistema de autenticação mais robusto no back-end, caso ele seja implementado. A probabilidade é média e o impacto é alto, resultando em comprometimento de dados e perda de confiança do usuário. Finalmente, a ausência de testes de usabilidade pode resultar em uma interface não intuitiva ou pouco amigável, comprometendo a experiência do usuário. A habilidade em JavaScript se mostra importante mais uma vez, pois facilita a integração de ferramentas e bibliotecas para testes automatizados de interface. A probabilidade é média e o impacto é médio, resultando em baixa adoção da plataforma e problemas de usabilidade.

Saída do exemplo 3:
[
[6, 6, 7],
[7, 5, 6],
[5, 5, 6]
]

Entrada:
Informações do Projeto:
Nome: {informacoesProjeto.Nome}

Descrição: {informacoesProjeto.Descricao}

Tipo de projeto: {informacoesProjeto.Tipo}

Data de início: {informacoesProjeto.DataInicial}

Data de finalização: {informacoesProjeto.DataFinal}

Funcionalidades: {funcionalidadesTexto}

Tecnologias: {tecnologiasTexto}

Análise do Projeto:
{analiseProjeto}


Saída:

";
        }

        public string GerarPromptParaDefinicaoComplexidadeProjeto(InfoProjectSettings informacoesProjeto, string analiseProjeto, float[] composicaoEquipe)
        {
            string funcionalidadesTexto = "";

            foreach (string funcionalidade in informacoesProjeto.Funcionalidades)
            {
                funcionalidadesTexto += $"- {funcionalidade}\n";
            }

            string tecnologiasTexto = "";

            foreach (string tecnologia in informacoesProjeto.Tecnologias)
            {
                tecnologiasTexto += $", {tecnologia}";
            }

            return $@"
Você faz parte da equipe de desenvolvimento de uma aplicação que visa definir o nível de complexidade que um determinadi projeto possui. Para isto o algoritmo da aplicação irá receber as informações gerais do projeto em questão, incluindo seu nome, uma descrição do mesmo, suas funcionalides, suas tecnologias, etc; Uma análise a respeito de quais são os principais riscos que este projeto possui durante o seu desenvolvimento, juntamente com as características gerais de tais riscos como descrição, área em que afeta, probabilidade de ocorrer e impacto que pode causar, etc; e, por fim, qual a composição da equipe ideal para a realização de tal projeto mostrando em porcentagem de colaboradores por cada nível de senioridade do mesmo (júnior, pleno, sênior ou gestão).

Partindo de tais informações, o seu trabalho será o de classificar qual o nível de complexidade em baixo, médio ou alto que este projeto possui. Levando em consideração as informações do projeto, a análise do projeto e a configuração em porcentagem da equipe ideal para o mesmo. Tenha em vista que projetos de complexidade baixa possuem um número menor de funcionalidades, tecnologias menos avançadas e riscos em áreas técnicas com impactos não muito graves, assim sendo uma equipe composta majoritariamente por júniors e plenos poderia ser ideal para a realização do mesmo; projetos de complexidade média possuem um número médio de funcionalidades, tecnologias já um pouco mais avançadas e riscos em áreas técnicas com impactos altos, e alguns sendo até graves assim sendo uma equipe composta majoritariamente por plenos e sêniors contendo, se necessário, alguns gestores e/ou júniors poderia ser ideal para a realização do mesmo; já projetos de complexidade alta possuem um número maior de funcionalidades, tecnologias robustas e avançadas e riscos em áreas técnicas com impactos altos ou graves para o desenvolvimento do projeto, assim sendo uma equipe composta majoritariamente por gestores e sêniors, contendo um numero relativo de plenos e quase nenhum júnior, poderia ser ideal para a realização do mesmo.

Com isso classifique o projeto apresentado a seguir em nível de complexidade baixo, médio ou alto. Importante: retorne apenas um dos seguintes valores: 1 para nível baixo, 2 para nível médio e 3 para nível alto. Evite incluir textos a mais como justificativa ou resumo

Exemplo 1:
Informações do Projeto:
Nome: Projeto Zenith

Descrição: O Zenith é um sistema que tem como objetivo realizar a análise de riscos de um projeto de implantação de sistemas, gerando relatórios sobre os mesmos e alocando uma equipe otimizada para a realização do mesmo. Em projetos de implantação de sistemas, é crucial antecipar e gerenciar riscos para garantir o sucesso do projeto. Uma alocação estratégica de recursos humanos, considerando a experiência e especialização de cada membro, pode minimizar estes riscos e otimizar os resultados. Desenvolver um algoritmo ou processo que realize a análise de riscos em projetos de implantação de sistemas, gerando uma alocação otimizada de profissionais de diferentes níveis de senioridade (júnior, pleno, sênior) para mitigar riscos identificados. Tendo em vista este cenário, se faz necessário uma plataforma que:

Analise riscos de um projeto: a partir do escopo e cronograma que determinado projeto de implantação de sistema possua, analisar os riscos atrelados ao mesmo e listar tais riscos, assim como qual seus impactos e quais as qualificações necessárias para mitigá-los.

Alocação de equipes para o projeto: com base na análise de riscos realizada, definir equipes recomendadas levando em consideração parâmetros como soft skills, hard skills e tech skills (junto da senioridade do mesmo) na definição de uma equipe otimizada e ideal para a realização do projeto

Gerenciamento de análises e equipes: após a realização de análises e alocação de equipes ideais, a plataforma permitirá gerenciamento de análises de projetos realizadas na plataforma e gerenciamento de equipes definidas para os mesmos.

Tipo de projeto: Implementação de software na empresa

Data de início: 23/01/2025

Data de finalização: 05/07/2025

Funcionalidades:
- O gestor, previamente cadastrado, poderá efetuar seu login pelo seu email e senha cadastrado, ou pelo email do Google e/ou Microsoft, contanto que o mesmo tenha sido previamente cadastrado na plataforma
- O usuário poderá efetuar o logout da plataforma conforme necessário
- O administrador da plataforma poderá realizar o cadastro de um gestor ou de um funcionário a partir de uma página privada apenas para administradores
- O usuário poderá recuperar a senha de acesso caso a tenha esquecido por meio de seu email cadastrado
- O gestor poderá cadastrar uma análise de um projeto a partir de seu escopo e conograma fornecido
- O gestor poderá visualizar as informações de determinada análise realizada na plataforma, verificando os riscos identificados, como mitigá-los durante o desenvolvimento do projeto e quais são os possíveis funcionários que irão compor a equipe ideal para a realização do projeto
- O gestor poderá visualizar o histórico de todas as análises de projetos realizadas na plataforma, com ou sem filtros de pesquisa. Caso opte por utilizar filtros de pesquisa, poderá filtrar por nome do projeto, data do projeto ou escopo geral do mesmo
- Ao terminar a análise, caberá ao gestor definir a equipe ideal para a realização do projeto, podendo ser feito de maneira pessoal com a lista de funcionários geral ou sugeridos para o projeto, ou pederá ser feito por meio da equipe ideal sugerida pela própria plataforma.
- O gestor poderá editar a equipe definida para a realização de determinado projeto de acordo com a necessidade.
- O gestor poderá excluir análises de projetos do histórico de análises realizadas caso necessário
- O usuário poderá visualizar a lista de funcionários que foram cadastrados na plataforma pelo mesmo
- O usuário poderá visualizar as informações gerais de determinado funcionário cadastrado na plataforma, assim como em quais projetos e equipes o mesmo foi alocado em análises realizadas na plataforma
- O administrador poderá editar determinadas informações do funcionário, atualizando suas habilidades ou alterando informações pessoais caso necessário
- O administrador poderá excluir determinado funcionário cadastrado na plataforma caso necessário
- O administrador poderá atualizar informações do escopo e cronograma do projeto a fim de realizar uma nova análise acerca do mesmo.
- O usuário poderá filtrar a visualização de análises realizadas na plataforma por parâmetros diversos como cronograma, nome e escopo e funcionários envolvidos
- O usuário poderá filtrar a visualização de funcionários por parâmetros diversos como nome, techskills e participação em projetos.

Tecnologias: Next.JS, ASPNET.CORE, EntityFramework, React, Azure, SQL Database, API REST Full, Gemini AI, C#, HTML, CSS, JavaScript

Análise do Projeto:
O Projeto Zenith, com seu objetivo ambicioso de analisar riscos e otimizar alocação de equipes em projetos de implantação de sistemas, apresenta diversos riscos de desenvolvimento.  A complexidade da integração entre as diferentes tecnologias e funcionalidades, aliada ao curto prazo de desenvolvimento (de 23/01/2025 a 05/07/2025, aproximadamente 5 meses), aumenta significativamente a probabilidade de problemas.

Um risco significativo reside na integração do Gemini AI com o restante da plataforma. A dependência de uma API externa para a análise de riscos implica em uma alta probabilidade de problemas de compatibilidade, performance e disponibilidade da API.  A habilidade técnica crucial para mitigar esse risco é o conhecimento profundo em API REST Full, garantindo a comunicação eficiente e robusta entre o sistema e o Gemini AI. A probabilidade desse risco é alta, e o impacto no projeto seria igualmente alto, podendo atrasar significativamente o cronograma e comprometer a funcionalidade principal do sistema.

Outro risco reside na complexidade do algoritmo de alocação de equipes.  Desenvolver um algoritmo que considere soft skills, hard skills e tech skills, além da senioridade, para otimizar a alocação de recursos humanos é um desafio considerável.  A habilidade técnica mais importante para lidar com este risco é o conhecimento em C#, dada a sua utilização no ASP.NET Core, que será a base da lógica de negócio. A probabilidade deste risco é média, e o impacto também seria médio, podendo levar a atrasos e a uma alocação de equipes subótima.

A integração entre o front-end (React e Next.JS) e o back-end (ASP.NET Core e Entity Framework) também apresenta riscos.  Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades podem ocorrer. A habilidade técnica mais importante para mitigar este risco é o conhecimento em JavaScript, essencial para o desenvolvimento e depuração do front-end e para a comunicação com o back-end. A probabilidade é média, e o impacto seria médio, podendo resultar em bugs e problemas de usabilidade.

A segurança da plataforma é outro ponto crítico.  A implementação de mecanismos robustos de autenticação (login com email/senha, Google e Microsoft) e autorização, além da proteção contra ataques de injeção de SQL e outros tipos de vulnerabilidades, é fundamental.  A habilidade técnica mais importante para mitigar este risco é o conhecimento em SQL, para garantir a segurança do banco de dados. A probabilidade é média, e o impacto seria alto, podendo resultar em vazamento de dados e comprometimento da reputação do projeto.

Por fim, a gestão do projeto em si também apresenta riscos.  A falta de comunicação entre a equipe, atrasos na entrega de funcionalidades e problemas de escopo podem comprometer o sucesso do projeto.  Embora não haja uma única tech skill para mitigar este risco, a experiência em gestão de projetos ágeis é crucial. A probabilidade é média, e o impacto é alto, podendo levar ao fracasso do projeto como um todo.  A utilização do Azure para hospedagem também introduz riscos de dependência de um serviço externo, com potenciais problemas de performance e custos.  A habilidade em Azure é crucial para mitigar este risco, com probabilidade e impacto médios.

Composição ideal para a Equipe do Projeto:
Colaboradores com senioridade Gestor: 7%
Colaboradores com senioridade Sênior: 21%
Colaboradores com senioridade Pleno: 42%
Colaboradores com senioridade Júnior: 30%

Saída do exemplo 1:
2

Exemplo 2:
Informações do Projeto:
Nome: FreeJobs

Descrição: Divulgue e contrate serviços autônomos através do nosso site, sempre que precisar estaremos aqui! Nossa plataforma FreeJobs trata-se de uma plataforma onde os usuários que a utilizar consigam com facilidade fazer a divulgação/contratação de serviços e trabalhos autônomos de maneira simples, prática e intuitiva se adequando ao uso e necessidade do usuário. Visando a situação atual do Brasil que sofre altas taxas de desemprego e a da perda de postos de trabalho para a tecnologia, nosso projeto visa ajudar qualquer trabalhador autônomo de baixa renda para que ele possa se sustentar financeiramente, mesmo que sem um emprego fixo. Assim oferecendo aos usuários de nosso site outros meios e opções de se sustentar financeiramente, que por vezes não conseguem vagas por conta da alta concorrência e do avanço da tecnologia ou apenas quer trabalhar de maneira autônoma e para si mesma. Nosso projeto foi desenvolvido acima de tudo na experiência que traria para os nossos usuários, portanto foi de extrema importância a atenção em tópicos como a segurança e a acessibilidade de nosso site. Para cumprir com tais requisitos usamos diversos conceitos e ferramentas das áreas da segurança de informação e acessibilidade digital como as técnicas e cuidados que tomamos ao coletar os dados dos usuários cadastrados e as normas e diretrizes de acessibilidade estipuladas pelo W3C (WCAG) e o Governo Digital Eletrônico (eMAG).

Tipo de projeto: Divulgação de Trabalho e Serviços pela Internet

Data de início: 14/02/2022

Data de finalização: 16/12/2022

Funcionalidades:
- Login e Logout sem token na plataforma
- Cadastro na plataforma
- Cadastro de serviço para perfil de prestador de serviços
- Solicitação de serviço para perfil de contratante de serviços
- Gerenciar meus serviços prestados
- Gerenciar meus serviços solicitados
- Visualizar todos os serviços
- Visualizar serviços pela busca
- Visualizar perfil de usuário
- Avaliar serviço prestado
- Chat entre prestador de serviço e quem o solicitou

Tecnologias: HTML, CSS e Javascript, PHP, SQL, phpMyAdmin.

Análise do Projeto:
O projeto FreeJobs, apesar de seu objetivo nobre de auxiliar trabalhadores autônomos, apresenta alguns riscos inerentes ao desenvolvimento de uma plataforma web. O prazo de desenvolvimento, de fevereiro a dezembro de 2022, embora mais extenso que o projeto Zenith, ainda requer gerenciamento cuidadoso para evitar atrasos. A ausência de um framework robusto no back-end, utilizando apenas PHP e SQL diretamente, aumenta a complexidade de manutenção e escalabilidade da plataforma a longo prazo. A dependência de um banco de dados SQL, sem menção a mecanismos robustos de segurança, representa um risco de vulnerabilidades, especialmente em relação a injeções SQL. A habilidade técnica mais importante para mitigar este risco é o conhecimento em SQL, visando a implementação de técnicas de prevenção contra essas vulnerabilidades. A probabilidade deste risco é média e o impacto é alto, podendo resultar em vazamento de dados dos usuários.

A implementação do sistema de login e logout sem tokens expõe a plataforma a riscos de segurança, uma vez que informações de sessão podem ser facilmente comprometidas. A adoção de um sistema de autenticação mais robusto, utilizando tokens JWT por exemplo, é crucial. A habilidade técnica mais importante aqui é o conhecimento em Javascript, essencial para gerenciamento de estado e segurança no lado cliente. A probabilidade deste risco é alta, e o impacto também é alto, podendo levar a um comprometimento da segurança dos dados e funcionalidades do sistema.

A funcionalidade de chat entre prestador e contratante demanda uma arquitetura bem planejada para lidar com a escalabilidade e a simultaneidade. Problemas de performance e estabilidade podem surgir com um grande número de usuários simultâneos. A habilidade em Javascript é crucial aqui, sendo a base para a construção de um sistema de chat eficiente e responsivo. A probabilidade deste risco é média, e o impacto também é médio, podendo gerar problemas de usabilidade e frustração aos usuários.

A falta de menção a testes de usabilidade e acessibilidade, apesar da menção aos princípios WCAG e eMAG, implica em um risco considerável de que a plataforma não atenda às necessidades dos usuários e não seja acessível a todos. A habilidade técnica mais importante para mitigar esse risco é o conhecimento em Javascript, fundamental para o desenvolvimento de testes e implementação de recursos de acessibilidade. A probabilidade é média, e o impacto é alto, podendo afetar significativamente o alcance e a usabilidade da plataforma, prejudicando sua adoção pelo público alvo. Por fim, a ausência de tecnologias mais modernas no front-end, como React ou Angular, pode dificultar a manutenção e a escalabilidade da interface no futuro, representando um risco de longo prazo em relação à atualização e modernização da plataforma. A habilidade em HTML é fundamental para garantir que o site se adapte aos diversos dispositivos e tamanhos de telas, garantindo a boa experiência do usuário. A probabilidade desse risco é baixa, mas o impacto é médio a longo prazo, podendo exigir um esforço de refatoração significativa no futuro.

Composição ideal para a Equipe do Projeto:
Colaboradores com senioridade Gestor: 6%
Colaboradores com senioridade Sênior: 20%
Colaboradores com senioridade Pleno: 15%
Colaboradores com senioridade Júnior: 59%

Saída do exemplo 2:
1

Exemplo 3:
Informações do Projeto:
Nome: RealFood

Descrição: Trata-se de uma plataforma que permite gerenciar busca de comidas em restaurantes disponíveis para entrega na região, seu principal objetivo é o de facilitar as interações entre cliente e restaurante.
Para isto a plataforma contará com o cadastro dos usuários com duas possíveis configurações, ou o usuário tem um perfil de cliente que realiza os pedidos para os restaurantes, ou tem um perfil de restaurante que irá ofertar os pratos disponíveis e será responsável, também, por realizar a entrega dos pedidos realizados pelos usuários na plataforma.
O sistema terá também diversas outras opções referentes ao gerenciamento do pedido como avaliação do pedido e da entrega, gerenciamento do status da entrega, salvar pedidos e/ou restaurantes favoritos, etc.

Tipo de projeto: Desenvolvimento e Implantação de Software

Data de início: 12/09/2024

Data de finalização: 23/11/2024

Funcionalidades:
- Cadastro: O usuário deverá ter a opção de se cadastrar na plataforma, utilizando um e-mail válido e uma nova senha
- Login: O usuário deverá poder se logar utilizando o e-mail e a senha cadastrados na plataforma
- Filtro de Pesquisa: O usuário deverá poder filtrar pela comida, ou restaurante, que estiver procurando na plataforma
- Acompanhar Pedido: O usuário deverá poder acompanhar o pedido realizado, desde seu status de preparação no restaurante até o status de entrega até o endereço solicitado
- Avaliar Pedido: Ao final da entrega, o usuário deverá poder avaliar como foi o pedido, seja a qualidade da comida, entrega, atendimento, etc.

Tecnologias: React Native, Json Server, Styled Components

Análise do Projeto:
O projeto RealFood, focado na gestão de pedidos de comida online, apresenta alguns riscos inerentes ao desenvolvimento de aplicativos mobile e plataformas web. O curto prazo de desenvolvimento, de setembro a novembro de 2024, exige um gerenciamento rigoroso para evitar atrasos. A utilização do Json Server como banco de dados, embora adequado para protótipos, representa um risco em termos de escalabilidade e segurança para um produto em produção. A habilidade técnica crucial para mitigar este risco é o conhecimento em SQL, permitindo a migração para um banco de dados relacional robusto e seguro. A probabilidade deste risco é média, e seu impacto é alto, podendo comprometer a performance e a segurança da plataforma com o crescimento do número de usuários.

A complexidade da implementação de funcionalidades como o acompanhamento e a avaliação de pedidos em tempo real requer uma arquitetura bem projetada e eficiente. Problemas de performance e concorrência podem surgir com um grande volume de pedidos simultâneos. A habilidade técnica mais importante para mitigar esse risco é o conhecimento em JavaScript, fundamental para o desenvolvimento de um sistema responsivo e escalável no front-end React Native. A probabilidade desse risco é média, e seu impacto também é médio, podendo levar a problemas de usabilidade e frustração do usuário.

A segurança da plataforma é outro ponto crítico. A proteção contra acessos não autorizados e a garantia da privacidade dos dados dos usuários (como informações de endereço e pagamento, se implementados) são essenciais. Embora não haja uma única tech skill para mitigar completamente esse risco, o conhecimento em JavaScript se mostra crucial, pois ele permite a implementação de medidas de segurança no lado cliente, como validações e criptografia. A probabilidade desse risco é média, e o impacto é alto, podendo resultar em perda de confiança dos usuários e em consequências legais.

A integração entre o front-end (React Native) e o back-end (Json Server) também apresenta riscos. Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades podem ocorrer. A habilidade em JavaScript é crucial para mitigar esse risco, pois é a linguagem principal do front-end e fundamental para garantir a comunicação eficaz com o back-end. A probabilidade é média e o impacto é médio, resultando em bugs e problemas de usabilidade. A falta de um sistema de autenticação robusto, apenas com e-mail e senha, apresenta risco de vulnerabilidades, como ataques de força bruta. Novamente, o JavaScript é importante aqui para a implementação de medidas de segurança no client-side, complementando um sistema de autenticação mais robusto no back-end, caso ele seja implementado. A probabilidade é média e o impacto é alto, resultando em comprometimento de dados e perda de confiança do usuário. Finalmente, a ausência de testes de usabilidade pode resultar em uma interface não intuitiva ou pouco amigável, comprometendo a experiência do usuário. A habilidade em JavaScript se mostra importante mais uma vez, pois facilita a integração de ferramentas e bibliotecas para testes automatizados de interface. A probabilidade é média e o impacto é médio, resultando em baixa adoção da plataforma e problemas de usabilidade.

Composição ideal para a Equipe do Projeto:
Colaboradores com senioridade Gestor: 30%
Colaboradores com senioridade Sênior: 60%
Colaboradores com senioridade Pleno: 5%
Colaboradores com senioridade Júnior: 5%

Saída do exemplo 3:
3

Entrada:
Informações do Projeto:
Nome: {informacoesProjeto.Nome}

Descrição: {informacoesProjeto.Descricao}

Tipo de projeto: {informacoesProjeto.Tipo}

Data de início: {informacoesProjeto.DataInicial}

Data de finalização: {informacoesProjeto.DataFinal}

Funcionalidades: {funcionalidadesTexto}

Tecnologias: {tecnologiasTexto}

Análise do Projeto:
{analiseProjeto}

Composição ideal para a Equipe do Projeto:
Colaboradores com senioridade Gestor: {composicaoEquipe[0]*100}%
Colaboradores com senioridade Sênior: {composicaoEquipe[1] * 100}%
Colaboradores com senioridade Pleno: {composicaoEquipe[2] * 100}%
Colaboradores com senioridade Júnior: {composicaoEquipe[3] * 100}%

Saída:
";
        }

        public string GerarPromptDefinirAreaCargo(string cargoAnalisado)
        {
            return $@"
Você faz parte de uma equipe que está desenvolvendo um sistema de gerenciamento de colaboradores em uma empresa. No momento em que o administrador está cadastrando um novo colaborador no sistema ele tem de informar diversas características incluindo o cargo que este colaborador possui na empresa. Um cargo naquela empresa possui uma característica importante para se considerar, a área em que este cargo atua durante o desenvolvimento de projetos na empresa.

O seu trabalho é o de receber o nome do cargo e por meio dele definir qual é a área de atuação deste cargo em um projeto podendo ser possíveis três áreas: Tecnica, Funcional e Ambiente. Sendo assim um cargo na área tecnica tem como responsabilidade o desenvolvimento técnico do projeto como funcionalidades, tecnologias, escopo e o produto em si; um cargo na área funcional tem como responsabilidade o gerenciamento dos procedimentos do projeto como supervisão, paralelismo de tarefas, integração entre as áreas, etc; já a área de Ambiente é responsável pela questão interpessoal no desenvolvimento do projeto como steakholders envolvidos, relacionamento entre equipes, orçamento, marketing e etc.

Tendo isto em consideração categorize apenas com qual a área (Tecnica, Funcional ou Ambiente) o cargo a seguir se relaciona:

Exemplo 1:
Entrada:
Cargo: Desenvolvedor Front-End

Saída do exemplo 1:
Tecnica

Exemplo 2:
Entrada:
Cargo: Gerente de Procedimentos e Homologações

Saída do exemplo 2:
Funcional

Exemplo 3:
Entrada:
Cargo: Avaliador de Relações interpessoais

Saída do exemplo 3:
Ambiente


Entrada Real:
Cargo: {cargoAnalisado}

Saída:
";
        }
    }
}
