using GenerativeAI.Models;
using GenerativeAI.Types;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

string apiKeyGemini = "AIzaSyAECWNn_niKjcpuNyme7s3vizD_54zMjCo";
var modelo = new GenerativeModel(apiKeyGemini, new ModelParams()
{
    GenerationConfig = new GenerationConfig()
    {
        Temperature = 0,
        CandidateCount = 1
    },
    Model = "gemini-1.5-flash"
});

string prompt = $@"
Estou trabalhando em um projeto onde ao receber uma análise em texto corrido de um setor, tenho de identificar e classificar os riscos retornados pela análise gerando uma lista de riscos para que possamos trabalhar em mitigá-los individualmente. Você receberá uma entrada na qual descreve a análise em formato de texto corrido. Com o texto em mãos identifique e faça a listagem em tópicos dos riscos catalogados na análise. Para cada risco listado, deve conter descrição, área em que o risco atinge (exempro: Front End, Back End, Marketing, Design, etc... Caso o risco afete tanto no Front End quanto no Back End liste sua área como sendo Full Stack), qual a tech skill (habilidade técnica) necessária para lidar com o risco (exemplo: HTML, JavaScript, Figma, SQL, etc...) importante: inclua apenas uma tech skill para cada risco listado, caso o risco possua mais que uma liste a mais importante para mitigá-lo, qual a probabilidade deste risco se concretizar durante o desenvolvimento do projeto e qual o impacto que este risco causaria para todo o desenvolvimento do projeto em si caso o risco se concretize muito importante: para a classificação da probabilidade e do impacto, indique-os apenas como 1 para baixo, 2 para médio ou 3 para alto, evite informações extras a este respeito. Identifique e faça a listagem de todos os riscos presentes na análise, não deixe nenhum de fora. Traga no mínimo 10 riscos a partir da análise elaborada e no mínimo 4 áreas diferentes, no entanto se durante a análise for identificado a existência de um número maior de riscos  liste-os também. Após finalizar a listagem dos riscos me retorne uma lista em JSON que represente os riscos listados, o JSON representará uma lista de objetos e cada objeto deverá conter como propriedades as características os riscos listados. O nome dos atributos entao serao: titulo, descricao, area, techSkill, probabilidade e impacto. Para as chaves de probabilidade e impacto utilize 1 para baixa, 2 para média e 3 para alta, logo um risco que tiver probabilidade média e impacto alto terá os campos ""probabilidade"" : 2 e ""impacto"" : 3.

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

Entrada:
O projeto RealFood, focado na gestão de pedidos de comida online, apresenta alguns riscos inerentes ao desenvolvimento de aplicativos mobile e plataformas web. O curto prazo de desenvolvimento, de setembro a novembro de 2024, exige um gerenciamento rigoroso para evitar atrasos. A utilização do Json Server como banco de dados, embora adequado para protótipos, representa um risco em termos de escalabilidade e segurança para um produto em produção. A habilidade técnica crucial para mitigar este risco é o conhecimento em SQL, permitindo a migração para um banco de dados relacional robusto e seguro. A probabilidade deste risco é média, e seu impacto é alto, podendo comprometer a performance e a segurança da plataforma com o crescimento do número de usuários.

A complexidade da implementação de funcionalidades como o acompanhamento e a avaliação de pedidos em tempo real requer uma arquitetura bem projetada e eficiente. Problemas de performance e concorrência podem surgir com um grande volume de pedidos simultâneos. A habilidade técnica mais importante para mitigar esse risco é o conhecimento em JavaScript, fundamental para o desenvolvimento de um sistema responsivo e escalável no front-end React Native. A probabilidade desse risco é média, e seu impacto também é médio, podendo levar a problemas de usabilidade e frustração do usuário.

A segurança da plataforma é outro ponto crítico. A proteção contra acessos não autorizados e a garantia da privacidade dos dados dos usuários (como informações de endereço e pagamento, se implementados) são essenciais. Embora não haja uma única tech skill para mitigar completamente esse risco, o conhecimento em JavaScript se mostra crucial, pois ele permite a implementação de medidas de segurança no lado cliente, como validações e criptografia. A probabilidade desse risco é média, e o impacto é alto, podendo resultar em perda de confiança dos usuários e em consequências legais.

A integração entre o front-end (React Native) e o back-end (Json Server) também apresenta riscos. Problemas de comunicação entre as camadas, falhas na comunicação de dados e dificuldades na implementação de funcionalidades podem ocorrer. A habilidade em JavaScript é crucial para mitigar esse risco, pois é a linguagem principal do front-end e fundamental para garantir a comunicação eficaz com o back-end. A probabilidade é média e o impacto é médio, resultando em bugs e problemas de usabilidade. A falta de um sistema de autenticação robusto, apenas com e-mail e senha, apresenta risco de vulnerabilidades, como ataques de força bruta. Novamente, o JavaScript é importante aqui para a implementação de medidas de segurança no client-side, complementando um sistema de autenticação mais robusto no back-end, caso ele seja implementado. A probabilidade é média e o impacto é alto, resultando em comprometimento de dados e perda de confiança do usuário. Finalmente, a ausência de testes de usabilidade pode resultar em uma interface não intuitiva ou pouco amigável, comprometendo a experiência do usuário. A habilidade em JavaScript se mostra importante mais uma vez, pois facilita a integração de ferramentas e bibliotecas para testes automatizados de interface. A probabilidade é média e o impacto é médio, resultando em baixa adoção da plataforma e problemas de usabilidade.

Saída
";

string respostaModelo = await modelo.GenerateContentAsync(prompt);

Console.WriteLine(respostaModelo);