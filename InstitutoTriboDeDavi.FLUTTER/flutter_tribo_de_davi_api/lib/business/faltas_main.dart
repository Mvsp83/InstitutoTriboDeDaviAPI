import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/faixa.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/frequencia.dart';

class AlunosMaisFaltantesPage extends StatefulWidget {
  const AlunosMaisFaltantesPage({super.key});

  @override
  AlunosMaisFaltantesPageState createState() => AlunosMaisFaltantesPageState();
}

class AlunosMaisFaltantesPageState extends State<AlunosMaisFaltantesPage> {
  List<Frequencia> alunos = [];
  List<Frequencia> alunosFiltrados = [];
  bool isLoading = false;
  String filtroNome = '';
  String ordem = 'Faltas';

  @override
  void initState() {
    super.initState();
    fetchAlunosMaisFaltantes();
  }

  Future<void> fetchAlunosMaisFaltantes() async {
    setState(() => isLoading = true);
    try {
      final response = await ApiHandler<Frequencia>(
        baseUri: ApiRoutes.entity("aluno/alunos-mais-faltantes"),
        fromJson: (json) => Frequencia.fromJson(json),
      ).getDataUrl();

      setState(() {
        alunos = response;
        aplicarFiltros();
      });
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Erro ao carregar dados: $e')),
      );
    } finally {
      setState(() => isLoading = false);
    }
  }

  void aplicarFiltros() {
    setState(() {
      alunosFiltrados = alunos
          .where((aluno) =>
              aluno.nome.toLowerCase().contains(filtroNome.toLowerCase()))
          .toList();

      if (ordem == 'Faltas') {
        alunosFiltrados.sort((a, b) => b.totalFaltas.compareTo(a.totalFaltas));
      } else if (ordem == 'Nome') {
        alunosFiltrados.sort((a, b) => a.nome.compareTo(b.nome));
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: const Text("Alunos Mais Faltantes"),
        centerTitle: true,
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
        elevation: 4,
        shadowColor: Colors.black54,
        toolbarHeight: 100,
      ),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  TextField(
                    onChanged: (value) {
                      setState(() {
                        filtroNome = value;
                        aplicarFiltros();
                      });
                    },
                    decoration: InputDecoration(
                      labelText: 'Buscar por nome',
                      prefixIcon: const Icon(Icons.search),
                      border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(15),
                      ),
                      filled: true,
                      fillColor: Colors.white,
                    ),
                  ),
                  const SizedBox(height: 10),
                  DropdownButton<String>(
                    value: ordem,
                    isExpanded: true,
                    icon: const Icon(Icons.arrow_downward),
                    underline: Container(
                      height: 2,
                      color: AppTheme.primaryColor,
                    ),
                    onChanged: (String? newValue) {
                      if (newValue != null) {
                        setState(() {
                          ordem = newValue;
                          aplicarFiltros();
                        });
                      }
                    },
                    items: <String>['Faltas', 'Nome']
                        .map<DropdownMenuItem<String>>((String value) {
                      return DropdownMenuItem<String>(
                        value: value,
                        child: Text(value),
                      );
                    }).toList(),
                  ),
                  const SizedBox(height: 20),
                  Expanded(
                    child: ListView.builder(
                      itemCount: alunosFiltrados.length,
                      itemBuilder: (context, index) {
                        Frequencia aluno = alunosFiltrados[index];
                        double frequencia = 100 -
                            ((aluno.totalFaltas / aluno.totalAulas) * 100);
                        return Container(
                          margin: const EdgeInsets.symmetric(
                              vertical: 8.0, horizontal: 16.0),
                          padding: const EdgeInsets.all(12.0),
                          decoration: AppTheme.cardDecoration,
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                                children: [
                                  Expanded(
                                    child: Text(
                                      aluno.nome,
                                      style: const TextStyle(
                                        fontSize: 18,
                                        fontWeight: FontWeight.bold,
                                        color: AppTheme.textColor,
                                      ),
                                    ),
                                  ),
                                  Container(
                                    width: 50,
                                    height: 50,
                                    decoration: BoxDecoration(
                                      border: Border.all(
                                        color: AppTheme.borderColor,
                                        width: 1,
                                      ),
                                      borderRadius: BorderRadius.circular(5),
                                    ),
                                    child: Image.asset(
                                      aluno.faixa != null
                                          ? Faixa.fromDescricao(aluno.faixa)
                                              .imagem
                                          : 'lib/assets/images/faixa_default.png',
                                      fit: BoxFit.contain,
                                    ),
                                  ),
                                ],
                              ),
                              const SizedBox(height: 8),
                              Text(
                                "Total de Aulas: ${aluno.totalAulas}",
                                style: const TextStyle(
                                  fontSize: 16,
                                  color: Colors.white,
                                ),
                              ),
                              Text(
                                "Total de Faltas: ${aluno.totalFaltas}",
                                style: const TextStyle(
                                  fontSize: 16,
                                  color: Colors.white,
                                ),
                              ),
                              Text(
                                "Frequência: ${frequencia.toStringAsFixed(1)}%",
                                style: TextStyle(
                                  fontSize: 16,
                                  color: frequencia < 75.0
                                      ? Colors.red
                                      : Colors.green,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ],
                          ),
                        );
                      },
                    ),
                  ),
                ],
              ),
            ),
    );
  }
}
