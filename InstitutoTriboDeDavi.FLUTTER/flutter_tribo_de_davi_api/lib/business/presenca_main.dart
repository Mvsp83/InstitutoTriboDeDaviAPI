import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/business/consultas/consulta_presenca.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aluno.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aula.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/presenca.dart';
import 'package:intl/intl.dart';

class PresencaPage extends StatefulWidget {
  const PresencaPage({super.key});

  @override
  State<PresencaPage> createState() => _PresencaPageState();
}

class _PresencaPageState extends State<PresencaPage> {
  Aula? selectedAula;
  List<Aluno> alunos = [];
  Map<int, bool> presencaMap = {};
  bool isSaving = false;

  Future<List<Aula>> fetchAulas() async {
    final todasAulas = await ApiHandler<Aula>(
      baseUri: ApiRoutes.entity("aula"),
      fromJson: (json) => Aula.fromJson(json),
    ).getData();
    return todasAulas.where((aula) => !aula.presencaSalva).toList();
  }

  Future<void> fetchAlunos(Aula aula) async {
    alunos = await ApiHandler<Aluno>(
      baseUri: ApiRoutes.entity("aluno"),
      fromJson: (json) => Aluno.fromJson(json),
    ).getData();
    presencaMap = {for (var aluno in alunos) aluno.id: true};
    setState(() {});
  }

  Future<void> savePresencas() async {
    if (selectedAula == null || isSaving) return;

    setState(() => isSaving = true);

    List<Presenca> presencas = presencaMap.entries.map((entry) {
      return Presenca(
        id: 0,
        alunoId: entry.key,
        nomeAluno: '',
        poloId: selectedAula!.poloId,
        aulaId: selectedAula!.id,
        data: selectedAula!.data,
        estaPresente: entry.value,
        observacoes: '',
      );
    }).toList();

    try {
      final response = await ApiHandler.post(
        ApiRoutes.create("presenca/batch"),
        body: presencas.map((p) => p.toJson()).toList(),
      );

      if (response.statusCode == 200) {
        await marcarAulaComPresencaSalva(selectedAula!);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Presenças salvas com sucesso!')),
        );

        setState(() {
          selectedAula!.presencaSalva = true;
          selectedAula = null;
        });
      } else {
        throw Exception('Erro ao salvar presenças.');
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Erro: $e')),
      );
    } finally {
      setState(() => isSaving = false);
    }
  }

  Future<void> marcarAulaComPresencaSalva(Aula aula) async {
    final aulaAtualizada = {
      "id": aula.id,
      "poloId": aula.poloId,
      "data": aula.data.toIso8601String(),
      "presencaSalva": true,
    };

    await ApiHandler.put(
      ApiRoutes.update("aula"),
      body: aulaAtualizada,
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Presenças na Aula"),
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
        centerTitle: true,
        actions: [
          IconButton(
            icon: const Icon(Icons.list),
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => const ConsultaPresencaPage(),
                ),
              );
            },
            tooltip: "Consultar Presenças",
          ),
        ],
      ),
      body: selectedAula == null
          ? FutureBuilder<List<Aula>>(
              future: fetchAulas(),
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(child: CircularProgressIndicator());
                } else if (snapshot.hasError) {
                  return Center(
                    child: Text(
                      "Erro ao carregar aulas: ${snapshot.error}",
                      style: const TextStyle(color: AppTheme.textColor),
                    ),
                  );
                } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
                  return const Center(
                    child: Text("Nenhuma aula disponível."),
                  );
                }

                final aulas = snapshot.data!;
                return ListView.builder(
                  itemCount: aulas.length,
                  itemBuilder: (context, index) {
                    final aula = aulas[index];
                    return _buildAulaCard(aula);
                  },
                );
              },
            )
          : _buildPresencaList(),
      bottomNavigationBar: selectedAula != null
          ? Padding(
              padding: const EdgeInsets.all(16),
              child: ElevatedButton.icon(
                onPressed: isSaving ? null : savePresencas,
                icon: isSaving
                    ? const CircularProgressIndicator(
                        strokeWidth: 2,
                        valueColor: AlwaysStoppedAnimation(Colors.white),
                      )
                    : const Icon(Icons.save, color: AppTheme.textColor),
                label: Text(
                  isSaving ? "Salvando..." : "Salvar Presenças",
                  style: const TextStyle(color: AppTheme.textColor),
                ),
                style: AppTheme.elevatedButtonStyle.copyWith(
                  backgroundColor:
                      WidgetStateProperty.all(AppTheme.primaryColor),
                ),
              ))
          : null,
      backgroundColor: AppTheme.backgroundColor,
    );
  }

  Widget _buildAulaCard(Aula aula) {
    return Card(
      margin: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(10),
      ),
      color:
          aula.presencaSalva ? AppTheme.secondaryColor : AppTheme.primaryColor,
      child: ListTile(
        title: Text(
          "Aula: ${DateFormat('dd/MM').format(aula.data)} - ${aula.horaInicio} : ${aula.horaFim}",
          style: const TextStyle(color: AppTheme.textColor),
        ),
        trailing: aula.presencaSalva
            ? const Icon(Icons.check, color: Colors.white)
            : const Icon(Icons.arrow_forward, color: Colors.white),
        onTap: aula.presencaSalva
            ? null
            : () {
                setState(() {
                  selectedAula = aula;
                  fetchAlunos(aula);
                });
              },
      ),
    );
  }

  Widget _buildPresencaList() {
    return ListView.builder(
      itemCount: alunos.length,
      itemBuilder: (context, index) {
        final aluno = alunos[index];
        return Card(
          margin: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
          color: AppTheme.primaryColor,
          child: CheckboxListTile(
            title: Text(
              aluno.nome!,
              style: const TextStyle(color: AppTheme.textColor),
            ),
            subtitle: Text(
              "Faixa: ${aluno.faixa} - Nascimento: ${aluno.dataNascimento}",
              style: TextStyle(color: AppTheme.textColor.withOpacity(0.7)),
            ),
            value: presencaMap[aluno.id],
            activeColor: AppTheme.secondaryColor,
            onChanged: (bool? value) {
              setState(() {
                presencaMap[aluno.id] = value ?? false;
              });
            },
          ),
        );
      },
    );
  }
}
