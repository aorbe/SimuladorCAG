# Simulador CAG

O **Simulador CAG (Central de Água Gelada)** é uma ferramenta educacional desenvolvida para auxiliar no ensino de Sistemas de Supervisão e Redes Industriais. Ele emula o comportamento de um processo industrial de refrigeração, permitindo que alunos desenvolvam e testem soluções de IHM (Interface Homem-Máquina) e SCADA sem a necessidade de equipamentos físicos.

## 📋 Sobre o Projeto
Este software foi desenvolvido para auxiliar alunos do curso de Tecnologia em Automação Industrial do SENAI. O objetivo principal é disponibilizar um canal de comunicação via **Modbus/TCP** com dados simulados, oferecendo uma alternativa leve e de fácil acesso para atividades práticas fora da sala de aula.

### Principais Características
* **Simulação de CAG:** Modela chillers, bombas, fan-coils e setpoints de temperatura.
* **Comunicação Industrial:** Implementa o protocolo **Modbus/TCP** (porta 502).
* **Leveza:** Consumo de memória inferior a 10MB, otimizado para rodar em sistemas legados ou virtualizados.
* **Modos de Operação:** Suporta operação automática (controle via setpoint) e modo manual (controle direto pelo usuário).

## 🛠️ Tecnologias Utilizadas
* **Linguagem:** C# (.NET)
* **IDE:** Microsoft Visual Studio Community
* **Comunicação:** EasyModbusTCP.NET

## 📋 Tabela de Variáveis (Modbus)
O simulador expõe os dados através dos seguintes registradores:

| Endereço | Descrição | Tipo de Acesso |
| :--- | :--- | :--- |
| 40001-40003 | Fan-Coil 1 (Temp. Ent/Sai, Abertura) | Leitura |
| 40004, 40017 | Fan-Coil 1 (Setpoint, Comando Manual) | Leitura/Escrita |
| 40005-40007 | Fan-Coil 2 (Temp. Ent/Sai, Abertura) | Leitura |
| 40008, 40018 | Fan-Coil 2 (Setpoint, Comando Manual) | Leitura/Escrita |
| 40009-40010 | Chiller 1 (Temp. Ent/Sai) | Leitura |
| 40011 | Chiller 1 (Setpoint) | Leitura/Escrita |
| 40012-40013 | Chiller 2 (Temp. Ent/Sai) | Leitura |
| 40014 | Chiller 2 (Setpoint) | Leitura/Escrita |
| 40015 | Comandos (Liga/Desliga, Auto/Man) | Leitura/Escrita |
| 40016 | Indicação de Status | Apenas Leitura |

## 🚀 Como Utilizar
1. **Compilação:** Abra o projeto no Visual Studio Community.
2. **Execução:** Execute a aplicação. A interface gráfica exibirá os valores das variáveis em tempo real, emulando uma tabela de monitoração de um CLP.
3. **Conexão:** Utilize seu software SCADA (ou cliente Modbus) para conectar-se ao endereço IP local na porta `502`.
4. **Operação:** Altere os setpoints e comandos através do seu software supervisório ou interagindo com a interface do simulador.

## 📜 Referências
* **ALMULLA, M. A.** *The Effectiveness of the Project-Based Learning (PBL) Approach as a Way to Engage Students in Learning.*
* **LAMB, F.** *Industrial automation: hands-on.*
* **EasyModbusTCP.NET:** [Repositório GitHub](https://github.com/rossmann-engineering/EasyModbusTCP.NET)

---
*Desenvolvido por: André Luis dos Santos (SENAI)*
