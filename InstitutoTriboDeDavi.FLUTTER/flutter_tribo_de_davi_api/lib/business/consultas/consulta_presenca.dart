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
  String _searchQuery = '';

  @override
  void initState() {
    super.initState();
    _loadAulas();
  }

  Future<void> _loadAulas() async {
    setState(() {
      _isLoading = true;
    });

    try {
      List<Aula> todasAulas = await ApiHandler<Aula>(
        baseUri: ApiRoutes.entity("aula"),
        fromJson: (json) => Aula.fromJson(json),
      ).getData();

      setState(() {
        _aulas = todasAulas.where((aula) => aula.presencaSalva).toList();
        _filteredAulas = _aulas;
        _isLoading = false;
      });
    } catch (error) {
      setState(() {
        _isLoading = false;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Erro ao carregar aulas: $error")),
      );
    }
  }

  void _filterAulas(String query) {
    setState(() {
      _searchQuery = query;
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
      appBar: AppBar(
        title: const Text("Consultas de Presenças"),
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
        centerTitle: true,
      ),
      backgroundColor: AppTheme.backgroundColor,
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: TextField(
              decoration: InputDecoration(
                hintText: "Buscar por data, hora ou polo...",
                prefixIcon: const Icon(Icons.search, color: AppTheme.iconColor),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
                focusedBorder: OutlineInputBorder(
                  borderSide: const BorderSide(color: AppTheme.primaryColor),
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              onChanged: _filterAulas,
            ),
          ),
          Expanded(
            child: _isLoading
                ? const Center(
                    child: CircularProgressIndicator(
                      valueColor:
                          AlwaysStoppedAnimation<Color>(AppTheme.primaryColor),
                    ),
                  )
                : _filteredAulas.isEmpty
                    ? const Center(
                        child: Text(
                          "Nenhuma presença registrada.",
                          style: TextStyle(color: AppTheme.textColor),
                        ),
                      )
                    : RefreshIndicator(
                        onRefresh: _loadAulas,
                        color: AppTheme.primaryColor,
                        child: ListView.builder(
                          itemCount: _filteredAulas.length,
                          itemBuilder: (context, index) {
                            var aula = _filteredAulas[index];
                            String formattedDate =
                                DateFormat('dd/MM/yyyy').format(aula.data);
                            String formattedTime =
                                DateFormat('HH:mm').format(aula.data);
                            return Card(
                              margin: const EdgeInsets.symmetric(
                                  vertical: 8, horizontal: 16),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(12),
                              ),
                              color: AppTheme.primaryColor,
                              child: ListTile(
                                contentPadding: const EdgeInsets.symmetric(
                                    vertical: 8, horizontal: 16),
                                title: Text(
                                  "Aula em $formattedDate às $formattedTime - Polo ${aula.poloId}",
                                  style: const TextStyle(
                                    color: AppTheme.textColor,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                subtitle: Text(
                                  "Presenças registradas",
                                  style: TextStyle(
                                    color: AppTheme.textColor.withOpacity(0.7),
                                  ),
                                ),
                                trailing: const Icon(Icons.info,
                                    color: AppTheme.textColor),
                                onTap: () {
                                  Navigator.push(
                                    context,
                                    MaterialPageRoute(
                                      builder: (context) =>
                                          DetalhesPresencaPage(aula: aula),
                                    ),
                                  );
                                },
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
