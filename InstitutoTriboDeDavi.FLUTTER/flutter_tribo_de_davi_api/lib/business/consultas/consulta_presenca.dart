import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/business/consultas/detalhes_consulta_presenca.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aula.dart';

class ConsultaPresencaPage extends StatefulWidget {
  const ConsultaPresencaPage({super.key});

  @override
  State<ConsultaPresencaPage> createState() => _ConsultaPresencaPageState();
}

class _ConsultaPresencaPageState extends State<ConsultaPresencaPage> {
  List<Aula> _aulas = [];
  List<Aula> _filteredAulas = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadAulas();
  }

  Future<void> _loadAulas() async {
    setState(() => _isLoading = true);

    try {
      final todasAulas = await ApiHandler<Aula>(
        baseUri: ApiRoutes.entity("aula"),
        fromJson: (json) => Aula.fromJson(json),
      ).getData();

      setState(() {
        _aulas = todasAulas.where((aula) => aula.presencaSalva).toList();
        _filteredAulas = _aulas;
        _isLoading = false;
      });
    } catch (error) {
      setState(() => _isLoading = false);
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Erro ao carregar aulas: $error")),
      );
    }
  }

  void _filterAulas(String query) {
    setState(() {
      if (query.isEmpty) {
        _filteredAulas = _aulas;
      } else {
        _filteredAulas = _aulas.where((aula) {
          final formattedDate = DateFormat('dd/MM/yyyy').format(aula.data);
          final formattedTime = DateFormat('HH:mm').format(aula.data);
          return formattedDate.contains(query) ||
              formattedTime.contains(query) ||
              aula.poloId.toString().contains(query);
        }).toList();
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.primaryColor,
      appBar: AppBar(
        title: const Text(
          "Consulta de Presenças",
          style: TextStyle(fontSize: 16, fontWeight: FontWeight.w500),
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
          // ── Campo de busca ──────────────────────────────────────────────
          Padding(
            padding: const EdgeInsets.fromLTRB(20, 16, 20, 8),
            child: TextField(
              onChanged: _filterAulas,
              style: const TextStyle(color: AppTheme.textColor, fontSize: 14),
              cursorColor: AppTheme.accentColor,
              decoration: InputDecoration(
                hintText: 'Buscar por data, hora ou polo...',
                hintStyle: const TextStyle(
                  color: AppTheme.textMutedColor,
                  fontSize: 14,
                ),
                prefixIcon: const Icon(
                  Icons.search,
                  color: AppTheme.accentColor,
                  size: 20,
                ),
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
            ),
          ),

          // ── Lista ───────────────────────────────────────────────────────
          Expanded(
            child: _isLoading
                ? const Center(
                    child:
                        CircularProgressIndicator(color: AppTheme.accentColor),
                  )
                : _filteredAulas.isEmpty
                    ? const Center(
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(
                              Icons.event_busy_outlined,
                              color: AppTheme.textMutedColor,
                              size: 48,
                            ),
                            SizedBox(height: 12),
                            Text(
                              'Nenhuma presença registrada.',
                              style: TextStyle(
                                color: AppTheme.textMutedColor,
                                fontSize: 14,
                              ),
                            ),
                          ],
                        ),
                      )
                    : RefreshIndicator(
                        onRefresh: _loadAulas,
                        color: AppTheme.accentColor,
                        child: ListView.builder(
                          padding: const EdgeInsets.symmetric(vertical: 8),
                          itemCount: _filteredAulas.length,
                          itemBuilder: (context, index) {
                            final aula = _filteredAulas[index];
                            final formattedDate =
                                DateFormat('dd/MM/yyyy').format(aula.data);
                            final formattedTime =
                                DateFormat('HH:mm').format(aula.data);

                            return Padding(
                              padding: const EdgeInsets.symmetric(
                                  vertical: 6, horizontal: 20),
                              child: InkWell(
                                onTap: () => Navigator.push(
                                  context,
                                  MaterialPageRoute(
                                    builder: (_) =>
                                        DetalhesPresencaPage(aula: aula),
                                  ),
                                ),
                                borderRadius: BorderRadius.circular(10),
                                child: Container(
                                  padding: const EdgeInsets.symmetric(
                                      vertical: 14, horizontal: 16),
                                  decoration: BoxDecoration(
                                    color: AppTheme.surfaceColor,
                                    borderRadius: BorderRadius.circular(10),
                                    border: Border.all(
                                      color: AppTheme.borderColor,
                                      width: 0.5,
                                    ),
                                  ),
                                  child: Row(
                                    children: [
                                      const Icon(
                                        Icons.check_circle_outline,
                                        color: Colors.green,
                                        size: 20,
                                      ),
                                      const SizedBox(width: 14),
                                      Expanded(
                                        child: Column(
                                          crossAxisAlignment:
                                              CrossAxisAlignment.start,
                                          children: [
                                            Text(
                                              '$formattedDate às $formattedTime',
                                              style: const TextStyle(
                                                fontSize: 14,
                                                fontWeight: FontWeight.w500,
                                                color: AppTheme.textColor,
                                              ),
                                            ),
                                            const SizedBox(height: 3),
                                            Text(
                                              'Polo ${aula.poloId}  ·  Turma ${aula.turma}',
                                              style: const TextStyle(
                                                fontSize: 12,
                                                color: AppTheme.textMutedColor,
                                              ),
                                            ),
                                          ],
                                        ),
                                      ),
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
                          },
                        ),
                      ),
          ),
        ],
      ),
    );
  }
}
