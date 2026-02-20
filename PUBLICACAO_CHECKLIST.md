# Checklist para Tornar o Repositório Público

## Segurança e privacidade
- [ ] Verificar se não há caminhos internos sensíveis em código, docs ou commits.
- [ ] Verificar se não há nomes reais de servidores/empresas em exemplos ou prints.
- [ ] Confirmar que não existe `settings.json` versionado.
- [ ] Confirmar que não há tokens, senhas, chaves ou segredos no histórico.

## Conteúdo do repositório
- [ ] `README.md` claro para usuário final.
- [ ] `LICENSE` definida (MIT).
- [ ] `CHANGELOG.md` criado.
- [ ] `.gitignore` cobrindo `bin/`, `obj/` e arquivos de IDE.

## Qualidade mínima
- [ ] Build local passando (`dotnet build`).
- [ ] Fluxo principal testado: configurar > selecionar EXEs > atualizar.
- [ ] Teste de erro validado (servidor indisponível, origem inexistente).

## Publicação
- [ ] Criar tag inicial (`v1.0.0`) opcional.
- [ ] Publicar release com notas resumidas.
- [ ] Marcar repositório como público no GitHub.
