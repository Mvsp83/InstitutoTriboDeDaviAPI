import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aluno.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/faixa.dart';
import 'package:http/http.dart' as http;
import 'package:intl/intl.dart';

class AlunoPendentesPage extends StatefulWidget {
  const AlunoPendentesPage({super.key});

  @override
  State<AlunoPendentesPage> createState() => _AlunoPendentesPageState();
}

class _AlunoPendentesPageState extends State<AlunoPendentesPage> {
  final ApiHandler<Aluno> _apiHandler = ApiHandler<Aluno>(
    baseUri: ApiRoutes.entity('aluno'),
    fromJson: (json) => Aluno.fromJson(json),
  );

  List<Aluno> _pendentes = [];
  List<Aluno> _filtrados = [];
  bool _isLoading = false;
  String _searchQuery = '';

  @override
  void initState() {
    super.initState();
    _carregarPendentes();
  }

  Future<void> _carregarPendentes() async {
    setState(() => _isLoading = true);
    try {
      final uri = Uri.parse(ApiRoutes.alunosPendentes());
      final headers = await _apiHandler.getHeaders();
      final response = await http.get(uri, headers: headers);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final jsonData = json.decode(response.body);
        final List<dynamic> items = jsonData['data'] ?? [];
        final lista = items.map((j) => Aluno.fromJson(j)).toList();
        lista.sort((a, b) => a.nome.compareTo(b.nome));
        setState(() {
          _pendentes = lista;
          _filtrados = List.from(lista);
        });
      } else {
        throw ApiException(ApiHandler.mensagemDoServidor(response),
            statusCode: response.statusCode);
      }
    } catch (e) {
      _mostrarErro('Erro ao carregar pendentes: $e');
    } finally {
      setState(() => _isLoading = false);
    }
  }

  void _filtrar(String query) {
    setState(() {
      _searchQuery = query;
      _filtrados = query.isEmpty
          ? List.from(_pendentes)
          : _pendentes
              .where((a) => a.nome.toLowerCase().contains(query.toLowerCase()))
              .toList();
    });
  }

  Future<void> _atribuirTurma(Aluno aluno) async {
    int? turmaSelecionada;

    final confirmou = await showDialog<bool>(
      context: context,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setDialogState) => AlertDialog(
          backgroundColor: AppTheme.surfaceColor,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16),
            side: const BorderSide(color: AppTheme.borderColor),
          ),
          title: const Text(
            'Atribuir Turma',
            style: TextStyle(
                color: AppTheme.textColor,
                fontSize: 16,
                fontWeight: FontWeight.bold),
          ),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                aluno.nome,
                style: const TextStyle(
                    color: AppTheme.accentColor,
                    fontSize: 14,
                    fontWeight: FontWeight.w600),
              ),
              const SizedBox(height: 4),
              Text(
                aluno.dataNascimento != null
                    ? 'Nasc: ${DateFormat('dd/MM/yyyy').format(aluno.dataNascimento!)}'
                    : 'Data de nascimento não informada',
                style: const TextStyle(
                    color: AppTheme.textMutedColor, fontSize: 13),
              ),
              Text(
                'Faixa: ${Faixa.fromId(aluno.faixa).descricao}',
                style: const TextStyle(
                    color: AppTheme.textMutedColor, fontSize: 13),
              ),
              const SizedBox(height: 20),
              const Text(
                'SELECIONE A TURMA',
                style: TextStyle(
                    color: AppTheme.textMutedColor,
                    fontSize: 11,
                    letterSpacing: 0.8),
              ),
              const SizedBox(height: 8),
              Row(
                children: [1, 2, 3].map((t) {
                  final selecionado = turmaSelecionada == t;
                  return Expanded(
                    child: Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 4),
                      child: GestureDetector(
                        onTap: () => setDialogState(() => turmaSelecionada = t),
                        child: AnimatedContainer(
                          duration: const Duration(milliseconds: 200),
                          height: 52,
                          decoration: BoxDecoration(
                            color: selecionado
                                ? AppTheme.accentColor
                                : AppTheme.primaryColor,
                            borderRadius: BorderRadius.circular(10),
                            border: Border.all(
                              color: selecionado
                                  ? AppTheme.accentColor
                                  : AppTheme.borderColor,
                              width: selecionado ? 2 : 0.5,
                            ),
                          ),
                          alignment: Alignment.center,
                          child: Text(
                            'Turma $t',
                            style: TextStyle(
                              color: selecionado
                                  ? AppTheme.buttonTextColor
                                  : AppTheme.textColor,
                              fontWeight: FontWeight.w700,
                              fontSize: 13,
                            ),
                          ),
                        ),
                      ),
                    ),
                  );
                }).toList(),
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(ctx, false),
              child: const Text('Cancelar',
                  style: TextStyle(color: AppTheme.textMutedColor)),
            ),
            ElevatedButton(
              onPressed: turmaSelecionada == null
                  ? null
                  : () => Navigator.pop(ctx, true),
              style: AppTheme.elevatedButtonStyle.copyWith(
                padding: WidgetStateProperty.all(
                  const EdgeInsets.symmetric(horizontal: 20, vertical: 10),
                ),
              ),
              child: const Text('CONFIRMAR', style: AppTheme.buttonTextStyle),
            ),
          ],
        ),
      ),
    );

    if (confirmou != true || turmaSelecionada == null) return;

    await _enviarAtribuicao(aluno, turmaSelecionada!);
  }

  Future<void> _enviarAtribuicao(Aluno aluno, int turma) async {
    setState(() => _isLoading = true);
    try {
      final uri = Uri.parse(ApiRoutes.atribuirTurma());
      final headers = await _apiHandler.getHeaders();
      final response = await http.patch(
        uri,
        headers: headers,
        body: json.encode({'alunoId': aluno.id, 'turma': turma}),
      );

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        setState(() {
          _pendentes.removeWhere((a) => a.id == aluno.id);
          _filtrar(_searchQuery);
        });
        if (!mounted) return;
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Turma $turma atribuída a ${aluno.nome}!'),
            backgroundColor: Colors.green.shade700,
          ),
        );
      } else {
        final body = json.decode(response.body);
        _mostrarErro(body['message'] ?? 'Erro ao atribuir turma.');
      }
    } catch (e) {
      _mostrarErro('Erro: $e');
    } finally {
      setState(() => _isLoading = false);
    }
  }

  void _mostrarErro(String mensagem) {
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(mensagem), backgroundColor: Colors.redAccent),
    );
  }

  Widget _buildCard(Aluno aluno) {
    return GestureDetector(
      onTap: () => _atribuirTurma(aluno),
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 6),
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: AppTheme.surfaceColor,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: AppTheme.borderColor, width: 0.5),
        ),
        child: Row(
          children: [
            Container(
              width: 56,
              height: 56,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                image: DecorationImage(
                  image: AssetImage(Faixa.fromId(aluno.faixa).imagem),
                  fit: BoxFit.fill,
                ),
              ),
            ),
            const SizedBox(width: 14),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    aluno.nome,
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 15,
                      color: AppTheme.textColor,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    aluno.dataNascimento != null
                        ? 'Nasc: ${DateFormat('dd/MM/yyyy').format(aluno.dataNascimento!)}'
                        : 'Data não informada',
                    style: const TextStyle(
                        fontSize: 13, color: AppTheme.textMutedColor),
                  ),
                  Text(
                    Faixa.fromId(aluno.faixa).descricao,
                    style: const TextStyle(
                        fontSize: 13, color: AppTheme.textMutedColor),
                  ),
                ],
              ),
            ),
            const Column(
              children: [
                Icon(Icons.touch_app,
                    color: AppTheme.accentColor, size: 20),
                SizedBox(height: 2),
                Text(
                  'Atribuir',
                  style: TextStyle(color: AppTheme.accentColor, fontSize: 11),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Text(
              'Alunos Pendentes',
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.w500),
            ),
            if (_pendentes.isNotEmpty) ...[
              const SizedBox(width: 8),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                decoration: BoxDecoration(
                  color: AppTheme.accentColor,
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Text(
                  '${_pendentes.length}',
                  style: const TextStyle(
                    color: AppTheme.buttonTextColor,
                    fontSize: 12,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ],
          ],
        ),
        centerTitle: true,
        backgroundColor: AppTheme.surfaceColor,
        foregroundColor: AppTheme.textColor,
        elevation: 0,
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(1),
          child: Container(color: AppTheme.borderColor, height: 0.5),
        ),
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(16),
            child: TextField(
              style: const TextStyle(color: AppTheme.textColor),
              decoration: InputDecoration(
                hintText: 'Buscar por nome...',
                hintStyle: const TextStyle(color: AppTheme.textMutedColor),
                prefixIcon: const Icon(Icons.search, color: AppTheme.iconColor),
                filled: true,
                fillColor: AppTheme.surfaceColor,
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                  borderSide:
                      const BorderSide(color: AppTheme.borderColor, width: 0.5),
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                  borderSide:
                      const BorderSide(color: AppTheme.accentColor, width: 1),
                ),
                contentPadding:
                    const EdgeInsets.symmetric(vertical: 14, horizontal: 16),
              ),
              onChanged: _filtrar,
            ),
          ),
          Expanded(
            child: _isLoading
                ? const Center(
                    child:
                        CircularProgressIndicator(color: AppTheme.accentColor))
                : _filtrados.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            const Icon(Icons.check_circle_outline,
                                color: Colors.green, size: 48),
                            const SizedBox(height: 12),
                            Text(
                              _searchQuery.isEmpty
                                  ? 'Nenhum aluno aguardando turma!'
                                  : 'Nenhum resultado para "$_searchQuery"',
                              style: const TextStyle(
                                  color: AppTheme.textMutedColor, fontSize: 15),
                            ),
                          ],
                        ),
                      )
                    : RefreshIndicator(
                        onRefresh: _carregarPendentes,
                        color: AppTheme.accentColor,
                        child: ListView.builder(
                          padding: const EdgeInsets.symmetric(horizontal: 16),
                          itemCount: _filtrados.length,
                          itemBuilder: (_, i) => _buildCard(_filtrados[i]),
                        ),
                      ),
          ),
        ],
      ),
    );
  }
}
