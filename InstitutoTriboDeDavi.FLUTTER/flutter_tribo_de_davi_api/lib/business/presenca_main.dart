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
  List<int> listaTurmasAulas = [0, 1, 2, 3, 4, 5];

  Future<List<Aula>> fetchAulas() async {
    final todasAulas = await ApiHandler<Aula>(
      baseUri: ApiRoutes.entity("aula"),
      fromJson: (json) => Aula.fromJson(json),
    ).getDataPorPolo(listaTurmasAulas);
    return todasAulas.where((aula) => !aula.presencaSalva).toList();
  }

  Future<void> fetchAlunos(Aula aula, List<int> turma) async {
    alunos = await ApiHandler<Aluno>(
      baseUri: ApiRoutes.entity("aluno"),
      fromJson: (json) => Aluno.fromJson(json),
    ).getDataPorPolo(turma);
    alunos.sort((a, b) => a.nome!.compareTo(b.nome!));
    presencaMap = {for (var aluno in alunos) aluno.id: true};
    setState(() {});
  }

  Future<void> savePresencas() async {
    if (selectedAula == null || isSaving) return;

    setState(() => isSaving = true);

    final presencas = presencaMap.entries.map((entry) {
      return Presenca(
        id: 0,
        alunoId: entry.key,
        nomeAluno: '',
        poloId: selectedAula!.poloId,
        data: selectedAula!.data,
        estaPresente: entry.value,
        observacoes: '',
        aulaId: selectedAula!.id,
      );
    }).toList();

    try {
      final response = await ApiHandler<Presenca>(
        baseUri: ApiRoutes.getAllUrl("presenca"),
        fromJson: (json) => Presenca.fromJson(json),
      ).post(
        endpoint: "batch/create",
        body: presencas.map((p) => p.toJson()).toList(),
      );

      if (response.statusCode == 200) {
        await marcarAulaComPresencaSalva(selectedAula!);
        if (!mounted) return;
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
      if (!mounted) return;
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
      'horaInicio': aula.horaInicio.toString(),
      'horaFim': aula.horaFim.toString(),
      "presencaSalva": true,
      "turma": aula.turma,
    };

    await ApiHandler<Aula>(
      baseUri: ApiRoutes.entity("aula"),
      fromJson: (json) => Aula.fromJson(json),
    ).put(endpoint: "aula", body: aulaAtualizada);
  }

  // ─── Card de aula (lista de seleção) ─────────────────────────────────────
  Widget _buildAulaCard(Aula aula) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6, horizontal: 20),
      child: InkWell(
        onTap: aula.presencaSalva
            ? null
            : () {
                setState(() {
                  selectedAula = aula;
                  fetchAlunos(aula, [aula.turma]);
                });
              },
        borderRadius: BorderRadius.circular(10),
        child: Container(
          padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 16),
          decoration: BoxDecoration(
            color: AppTheme.surfaceColor,
            borderRadius: BorderRadius.circular(10),
            border: Border.all(color: AppTheme.borderColor, width: 0.5),
          ),
          child: Row(
            children: [
              // Ícone de status
              Icon(
                aula.presencaSalva
                    ? Icons.check_circle_outline
                    : Icons.radio_button_unchecked,
                color: aula.presencaSalva ? Colors.green : AppTheme.accentColor,
                size: 20,
              ),
              const SizedBox(width: 14),
              // Informações da aula
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      DateFormat('dd/MM/yyyy').format(aula.data),
                      style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w500,
                        color: aula.presencaSalva
                            ? AppTheme.textMutedColor
                            : AppTheme.textColor,
                      ),
                    ),
                    const SizedBox(height: 3),
                    Text(
                      '${aula.horaInicio} – ${aula.horaFim}  ·  Turma ${aula.turma}',
                      style: const TextStyle(
                        fontSize: 12,
                        color: AppTheme.textMutedColor,
                      ),
                    ),
                  ],
                ),
              ),
              // Badge "Salva" ou seta
              if (aula.presencaSalva)
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                  decoration: BoxDecoration(
                    color: Colors.green.withOpacity(0.15),
                    borderRadius: BorderRadius.circular(6),
                    border: Border.all(color: Colors.green, width: 0.5),
                  ),
                  child: const Text(
                    'Salva',
                    style: TextStyle(
                      color: Colors.green,
                      fontSize: 11,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                )
              else
                const Icon(
                  Icons.chevron_right,
                  color: AppTheme.textMutedColor,
                  size: 20,
                ),
            ],
          ),
        ),
      ),
    );
  }

  // ─── Item de presença (lista de alunos) ───────────────────────────────────
  Widget _buildPresencaItem(Aluno aluno) {
    final presente = presencaMap[aluno.id] ?? true;
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6, horizontal: 20),
      child: InkWell(
        onTap: () => setState(() => presencaMap[aluno.id] = !presente),
        borderRadius: BorderRadius.circular(10),
        child: Container(
          padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
          decoration: BoxDecoration(
            color: AppTheme.surfaceColor,
            borderRadius: BorderRadius.circular(10),
            border: Border.all(
              color: presente ? AppTheme.accentColor : AppTheme.borderColor,
              width: presente ? 1 : 0.5,
            ),
          ),
          child: Row(
            children: [
              // Checkbox estilizado
              AnimatedContainer(
                duration: const Duration(milliseconds: 150),
                width: 22,
                height: 22,
                decoration: BoxDecoration(
                  color: presente ? AppTheme.accentColor : Colors.transparent,
                  borderRadius: BorderRadius.circular(5),
                  border: Border.all(
                    color:
                        presente ? AppTheme.accentColor : AppTheme.borderColor,
                    width: 1.5,
                  ),
                ),
                child: presente
                    ? const Icon(Icons.check,
                        size: 14, color: AppTheme.primaryColor)
                    : null,
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      aluno.nome ?? '',
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w500,
                        color: AppTheme.textColor,
                      ),
                    ),
                    const SizedBox(height: 3),
                    Text(
                      'Faixa ${aluno.faixa}'
                      '${aluno.dataNascimento != null ? '  ·  ${DateFormat('dd/MM/yyyy').format(aluno.dataNascimento!)}' : ''}',
                      style: const TextStyle(
                        fontSize: 12,
                        color: AppTheme.textMutedColor,
                      ),
                    ),
                  ],
                ),
              ),
              // Indicador textual
              Text(
                presente ? 'Presente' : 'Falta',
                style: TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w500,
                  color: presente ? AppTheme.accentColor : Colors.redAccent,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.primaryColor,
      appBar: AppBar(
        title: Text(
          selectedAula == null
              ? 'Presenças'
              : DateFormat('dd/MM/yyyy').format(selectedAula!.data),
          style: const TextStyle(fontSize: 16, fontWeight: FontWeight.w500),
        ),
        centerTitle: true,
        backgroundColor: AppTheme.surfaceColor,
        foregroundColor: AppTheme.textColor,
        elevation: 0,
        leading: selectedAula != null
            ? IconButton(
                icon: const Icon(Icons.arrow_back),
                onPressed: () => setState(() => selectedAula = null),
              )
            : null,
        actions: [
          if (selectedAula == null)
            IconButton(
              icon: const Icon(Icons.list_alt_outlined,
                  color: AppTheme.accentColor),
              tooltip: 'Consultar Presenças',
              onPressed: () => Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (_) => const ConsultaPresencaPage(),
                ),
              ),
            ),
        ],
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(1),
          child: Container(color: AppTheme.borderColor, height: 0.5),
        ),
      ),
      body: selectedAula == null
          // ── Lista de aulas ──────────────────────────────────────────────
          ? FutureBuilder<List<Aula>>(
              future: fetchAulas(),
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(
                    child:
                        CircularProgressIndicator(color: AppTheme.accentColor),
                  );
                }
                if (snapshot.hasError) {
                  return Center(
                    child: Text(
                      'Erro ao carregar aulas: ${snapshot.error}',
                      style: const TextStyle(color: Colors.redAccent),
                    ),
                  );
                }
                final aulas = snapshot.data ?? [];
                if (aulas.isEmpty) {
                  return const Center(
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Icon(Icons.event_available_outlined,
                            color: AppTheme.textMutedColor, size: 48),
                        SizedBox(height: 12),
                        Text(
                          'Nenhuma aula pendente',
                          style: TextStyle(
                              color: AppTheme.textMutedColor, fontSize: 14),
                        ),
                      ],
                    ),
                  );
                }
                return ListView.builder(
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  itemCount: aulas.length,
                  itemBuilder: (_, i) => _buildAulaCard(aulas[i]),
                );
              },
            )
          // ── Lista de alunos ─────────────────────────────────────────────
          : alunos.isEmpty
              ? const Center(
                  child: CircularProgressIndicator(color: AppTheme.accentColor),
                )
              : ListView.builder(
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  itemCount: alunos.length,
                  itemBuilder: (_, i) => _buildPresencaItem(alunos[i]),
                ),
      // ── Botão salvar ────────────────────────────────────────────────────
      bottomNavigationBar: selectedAula != null
          ? Padding(
              padding: const EdgeInsets.fromLTRB(20, 12, 20, 24),
              child: SizedBox(
                height: 52,
                child: ElevatedButton(
                  onPressed: isSaving ? null : savePresencas,
                  style: AppTheme.elevatedButtonStyle,
                  child: isSaving
                      ? const SizedBox(
                          width: 20,
                          height: 20,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                            color: AppTheme.primaryColor,
                          ),
                        )
                      : const Text('SALVAR PRESENÇAS',
                          style: AppTheme.buttonTextStyle),
                ),
              ),
            )
          : null,
    );
  }
}
