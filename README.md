# MemCleaner

## Descrição
O MemCleaner é um pequeno aplicativo em C# que visa modificar a memória de processos em execução para substituir sequências de bytes específicas por outras. Ele utiliza a biblioteca `Memory.dll` para manipulação de memória e as APIs do Windows para enumerar processos e ajustar proteções de memória.

## Funcionalidade
O programa varre os processos em execução em busca do processo `lsass.exe` com o maior número de threads e tenta encontrar ocorrências de um padrão de bytes especificado (`originalBytes`) na memória desse processo. Ao encontrar o padrão, substitui-o pelo conjunto de bytes especificado (`replacementBytes`). A substituição é realizada em todas as ocorrências encontradas.

## Componentes principais do código
- **Form1.cs:** Contém a interface gráfica do programa, onde o usuário pode interagir para iniciar a limpeza de memória.
- **Mem.cs:** Biblioteca `Memory.dll` para manipulação de memória.
- **Métodos principais:**
  - `GetProcessID()`: Utiliza funções do Windows para obter o identificador do processo `lsass.exe` com mais threads.
  - `CleanMemory(string originalBytes, string replacementBytes)`: Realiza a varredura da memória do processo identificado, substituindo as sequências de bytes especificadas.

## Instruções de uso
1. **Requisitos:**
   - Compilador C# (Visual Studio recomendado).
   - Conhecimento básico de C# e manipulação de memória.

2. **Como usar:**
   - Compile o projeto no Visual Studio.
   - Execute o aplicativo resultante (`MemCleaner.exe`).
   - Insira o padrão de bytes que deseja buscar e substituir no campo de entrada.
   - Clique no botão para iniciar o processo de limpeza de memória.

## Avisos
- Este programa é puramente educativo e não deve ser usado para modificar a memória de processos de forma maliciosa ou sem autorização explícita.
- A substituição de bytes na memória de processos pode causar falhas ou instabilidade nos sistemas operacionais.

## Contribuições
Contribuições são bem-vindas via pull requests. Certifique-se de seguir as diretrizes de código e boas práticas ao contribuir para este projeto
