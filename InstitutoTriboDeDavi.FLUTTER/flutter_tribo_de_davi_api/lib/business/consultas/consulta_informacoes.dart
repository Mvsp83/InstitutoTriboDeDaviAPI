import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/polo.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aluno.dart';

class DashboardPage extends StatefulWidget {
  const DashboardPage({super.key});

  @override
  State<DashboardPage> createState() => _DashboardPageState();
}

class _DashboardPageState extends State<DashboardPage> {
  final ApiHandler<Polo> apiPoloHandler = ApiHandler<Polo>(
    baseUri: ApiRoutes.entity("polo"),
    fromJson: (json) => Polo.fromJson(json),
  );

  final ApiHandler<Aluno> apiAlunoHandler = ApiHandler<Aluno>(
    baseUri: ApiRoutes.entity("aluno"),
    fromJson: (json) => Aluno.fromJson(json),
  );

  Polo? selectedPolo;
  List<Polo> polos = [];
  Map<String, int> alunosPorIdade = {};
  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadPolos();
  }

  Future<void> _loadPolos() async {
    polos = await apiPoloHandler.getData();
    setState(() {
      isLoading = false;
    });
  }

  Future<Map<String, dynamic>> _fetchDashboardData() async {
    List<Aluno> alunos;
    List<int> listaTurmasInformacoes = [0, 1, 2, 3, 4, 5];

    if (selectedPolo == null) {
      // Todos os polos
      alunos = await apiAlunoHandler.getDataAll();
    } else {
      // Apenas um polo
      alunos = await apiAlunoHandler.getDataPorPolo(listaTurmasInformacoes);
      alunos =
          alunos.where((aluno) => aluno.poloId == selectedPolo!.id).toList();
    }

    final totalAlunos = alunos.length;
    final Map<int, int> alunosPorIdadeTemp = {};

    for (var aluno in alunos) {
      if (aluno.dataNascimento != null) {
        final idade = _calcularIdade(aluno.dataNascimento!);
        alunosPorIdadeTemp[idade] = (alunosPorIdadeTemp[idade] ?? 0) + 1;
      }
    }

    // Ordenar por idade
    final sortedEntries = alunosPorIdadeTemp.entries.toList()
      ..sort((a, b) => a.key.compareTo(b.key));

    final alunosPorIdadeOrdenado = {
      for (var e in sortedEntries) e.key.toString(): e.value
    };

    return {
      'nome': selectedPolo?.nome ?? 'Todos os polos',
      'endereco': selectedPolo?.endereco ?? 'N/A',
      'informacoes': selectedPolo?.informacoes ?? 'N/A',
      'totalAlunos': totalAlunos,
      'alunosPorIdade': alunosPorIdadeOrdenado,
    };
  }

  int _calcularIdade(DateTime dataNascimento) {
    final hoje = DateTime.now();
    int idade = hoje.year - dataNascimento.year;
    if (hoje.month < dataNascimento.month ||
        (hoje.month == dataNascimento.month && hoje.day < dataNascimento.day)) {
      idade--;
    }
    return idade;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: const Text("Dashboard"),
        centerTitle: true,
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: isLoading
            ? const Center(
                child: CircularProgressIndicator(
                  color: AppTheme.textColor,
                ),
              )
            : Column(
                children: [
                  // Dropdown para selecionar polo
                  DropdownButton<Polo?>(
                    value: selectedPolo,
                    items: [
                      const DropdownMenuItem(
                        value: null,
                        child: Text("Todos os polos"),
                      ),
                      ...polos.map(
                        (polo) => DropdownMenuItem(
                          value: polo,
                          child: Text(polo.nome),
                        ),
                      ),
                    ],
                    onChanged: (value) {
                      setState(() {
                        selectedPolo = value;
                      });
                    },
                    isExpanded: true,
                    hint: const Text("Selecione um polo"),
                  ),
                  const SizedBox(height: 20),
                  // Painel de informações
                  Expanded(
                    child: FutureBuilder<Map<String, dynamic>>(
                      future: _fetchDashboardData(),
                      builder: (context, snapshot) {
                        if (snapshot.connectionState ==
                            ConnectionState.waiting) {
                          return const Center(
                            child: CircularProgressIndicator(
                              color: AppTheme.textColor,
                            ),
                          );
                        }

                        if (snapshot.hasError) {
                          return const Center(
                            child: Text(
                              "Erro ao carregar os dados",
                              style: TextStyle(
                                color: AppTheme.secondaryColor,
                              ),
                            ),
                          );
                        }

                        final data = snapshot.data!;
                        alunosPorIdade =
                            data['alunosPorIdade'] as Map<String, int>;

                        return ListView(
                          children: [
                            if (selectedPolo != null) ...[
                              _buildInfoCard("Nome do Polo", data['nome']),
                              _buildInfoCard("Endereço", data['endereco']),
                              _buildInfoCard(
                                  "Informações", data['informacoes']),
                            ],
                            _buildInfoCard(
                              "Número total de alunos",
                              data['totalAlunos'].toString(),
                            ),
                            const SizedBox(height: 20),
                            _buildInfoCard(
                              "Número de alunos por idade",
                              alunosPorIdade.entries
                                  .map((e) => "${e.key} anos:   ${e.value}\n")
                                  .join("\n"),
                            ),
                          ],
                        );
                      },
                    ),
                  ),
                ],
              ),
      ),
    );
  }

  Widget _buildInfoCard(String title, String value) {
    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      color: AppTheme.primaryColor,
      elevation: 5,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              title,
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                fontSize: 20,
                color: AppTheme.textColor,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              value,
              style: const TextStyle(
                fontSize: 16,
                color: AppTheme.textColor,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
