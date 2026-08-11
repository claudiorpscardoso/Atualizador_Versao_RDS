# 🚀 Manual de Uso - Atualizador de Versão RDS

[![Build Desktop App](https://github.com/claudiorpscardoso/Atualizador_Versao_RDS/actions/workflows/build.yml/badge.svg)](https://github.com/claudiorpscardoso/Atualizador_Versao_RDS/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

English version: [`README.en.md`](README.en.md)

## 📦 Instalação rápida
Pré-requisito: .NET SDK 10 instalado.

```powershell
dotnet build
dotnet test
dotnet run --project .\AtualizadorVersaoRds
```

## 🔐 Aviso de segurança
Use primeiro em ambiente de teste. Sempre valide pastas de origem/destino antes de rodar em produção.

## 🎯 Para que serve
Este aplicativo atualiza arquivos `.exe` em pastas de servidores da rede, usando uma pasta de origem como base.

Em resumo:
1. Você define **de onde** os executáveis serão lidos.
2. Você define **em quais servidores** será feita a atualização.
3. Você escolhe **quais `.exe`** deseja atualizar.
4. O sistema renomeia o antigo para `REMOVER_*` e copia o novo.

---

## ⚙️ O que o sistema faz na atualização
Para cada servidor e para cada executável selecionado:
1. Acessa a pasta do servidor.
2. Copia o executável novo para um arquivo temporário no servidor e confere o tamanho.
3. Se o executável antigo existir, renomeia para backup com prefixo `REMOVER_`.
4. Promove o arquivo temporário para o nome final.

### 🛡️ Por que a cópia vem antes do backup
O arquivo do servidor só é tocado depois que a cópia foi concluída e validada.
Se qualquer etapa falhar — rede caiu, executável em uso, sem permissão — o
processo desfaz o que fez e **a versão que estava no servidor continua intacta**.
O item entra como falha no resumo e o processo segue para os demais.

### 📦 Regra de backup (importante)
Se já existir um arquivo `REMOVER_Nome.exe`, o sistema **não apaga**.
Ele cria com sufixo, como no Windows:
- `REMOVER_Nome.exe`
- `REMOVER_Nome (2).exe`
- `REMOVER_Nome (3).exe`

✅ Isso preserva o histórico de backups.

---

## 🖥️ Tela principal (como usar)
Na tela principal você encontra:
- `Configurações`: abre a tela para cadastrar origem e servidores.
- `Recarregar EXEs`: atualiza a lista de `.exe` da pasta de origem.
- `Exibir log`: mostra/oculta detalhes da execução.
- Lista de executáveis: com checkbox e ícone de cada arquivo.
- `Atualizar selecionados`: inicia o processo (pede confirmação antes).
- `Cancelar`: interrompe a execução em andamento com segurança.
- Barra de progresso: mostra andamento em tempo real.
- Status: mostra o passo atual (servidor, arquivo e ação).

---

## 🧩 Configurando o sistema (primeiro uso)
1. Clique em `Configurações`.
2. Em **Pasta de origem**, selecione a pasta onde estão os `.exe` novos.
3. Em **Pastas de servidores**, adicione 1 ou mais caminhos de destino.
4. Clique em `Salvar`.

💡 Dica: você pode abrir as pastas em rede no Explorer antes para validar acesso.

---

## 🔄 Atualizando executáveis (passo a passo)
1. Na tela principal, clique em `Recarregar EXEs`.
2. Marque os servidores e os executáveis desejados.
3. Clique em `Atualizar selecionados` e confirme o resumo exibido.
4. Acompanhe o status e a barra de progresso.
5. Ao final, confira a mensagem de conclusão — ela informa quantos itens
   tiveram sucesso e quantos falharam.

💡 Clique com o botão direito nas listas para `Marcar todos` / `Desmarcar todos`.

---

## ⏱️ O que acontece durante a execução
Durante a atualização:
- A lista de executáveis fica bloqueada para evitar alterações no meio do processo.
- Botões principais ficam bloqueados temporariamente.
- O app mostra qual servidor e qual arquivo estão sendo processados.
- O progresso da cópia é mostrado em percentual.

---

## 🧾 Mensagens que você pode ver
- `Acessando pasta servidor ...`
- `[Servidor] Renomeando ...`
- `[Servidor] Copiando ... 45%`
- `[Servidor] Cópia concluída ...`
- `ERRO: Pasta de servidor não encontrada`
- `ERRO: Executável de origem não encontrado`

---

## 🛠️ Como interpretar erros
Se ocorrer erro em um arquivo/servidor:
- O sistema registra no status/log.
- O processo continua com os demais itens.
- Você pode corrigir o problema e rodar novamente.

---

## ✅ Boas práticas de operação
Antes de atualizar:
1. Confirme a pasta de origem correta.
2. Confirme se os servidores cadastrados estão certos.
3. Verifique se os executáveis selecionados são os esperados.

Depois de atualizar:
1. Revise o log (se necessário).
2. Valide pelo menos um servidor por amostragem.
3. Se precisar voltar versão, use o arquivo `REMOVER_*` como referência.

---

## ❓ Perguntas rápidas
### O app apaga meus backups antigos?
Não. Ele sempre cria novo nome com `(N)` quando já existe backup.

### Posso atualizar só alguns programas?
Sim. Basta marcar apenas os `.exe` desejados.

### Posso cadastrar mais de um servidor?
Sim. A tela de configurações aceita vários caminhos.

### Onde as configurações ficam salvas?
Em `%APPDATA%\AtualizadorVersaoRds\settings.json`. Se você vem da versão 1.0.x,
o arquivo antigo (ao lado do executável) é migrado automaticamente no primeiro uso.

### Onde fica o log das execuções?
Em `%APPDATA%\AtualizadorVersaoRds\logs\atualizacao-AAAAMMDD.log`, um arquivo por dia.
O painel `Exibir log` mostra o mesmo conteúdo da sessão atual.

### Posso cancelar no meio?
Sim. O botão `Cancelar` interrompe após concluir o arquivo em andamento,
sem deixar executável pela metade no servidor.

### Preciso instalar o .NET?
Depende do pacote baixado na página de releases:
- `...-win-x64.zip`: menor, exige o **.NET Desktop Runtime 10**.
- `...-win-x64-self-contained.zip`: executável único, não exige nada instalado.

---

## 📚 Arquivos úteis do projeto
- `CHANGELOG.md`: histórico de versões.
- `LICENSE`: licença MIT.
- `CONTRIBUTING.md`: guia rápido para contribuir.

---

## 🤝 Contribuição
Contribuições são bem-vindas.

Antes de abrir PR, leia:
- `CONTRIBUTING.md`
