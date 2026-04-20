import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/polo.dart';
import 'package:flutter_tribo_de_davi_api/entities/polo/polo_add.dart';
import 'package:flutter_tribo_de_davi_api/entities/polo/polo_edit.dart';

class PoloPage extends StatefulWidget {
  const PoloPage({super.key});

  @override
  State<PoloPage> createState() => _PoloPageState();
}

class _PoloPageState extends State<PoloPage> {
  ApiHandler<Polo> apiHandler = ApiHandler<Polo>(
    baseUri: ApiRoutes.entity("polo"),
    fromJson: (json) => Polo.fromJson(json),
  );
  List<Polo> data = [];
  List<Polo> filteredData = [];
  bool isLoading = false;
  String searchQuery = '';

  @override
  void initState() {
    super.initState();
    getPolo();
  }

  Future<void> getPolo() async {
    setState(() => isLoading = true);
    try {
      List<Polo> allPolos = await apiHandler.getData();
      setState(() {
        data = allPolos;
        filteredData = allPolos;
        isLoading = false;
      });
    } catch (e) {
      setState(() => isLoading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Erro ao carregar polos: $e")),
      );
    }
  }

  void _filterPolos(String query) {
    setState(() {
      searchQuery = query;
      if (query.isEmpty) {
        filteredData = data;
      } else {
        filteredData = data.where((polo) {
          return polo.nome.toLowerCase().contains(query.toLowerCase()) ||
              polo.cidade!.toLowerCase().contains(query.toLowerCase()) ||
              polo.bairro!.toLowerCase().contains(query.toLowerCase());
        }).toList();
      }
    });
  }

  Future<void> deletePolo(int id) async {
    await apiHandler.deleteData(id: id);
    setState(() {
      data.removeWhere((polo) => polo.id == id);
      _filterPolos(searchQuery);
    });
  }

  void _confirmDelete(int id) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Confirmar Exclusão"),
        content: const Text("Tem certeza que deseja excluir este polo?"),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text("Cancelar"),
          ),
          ElevatedButton(
            onPressed: () {
              deletePolo(id);
              Navigator.of(context).pop();
            },
            style: AppTheme.elevatedButtonStyle,
            child: const Text("Excluir"),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: const Text(
          "Gestão de Polos",
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
          // Campo de Busca
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: TextField(
              decoration: InputDecoration(
                hintText: "Buscar por nome, cidade ou bairro...",
                prefixIcon: const Icon(Icons.search, color: AppTheme.iconColor),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
                focusedBorder: OutlineInputBorder(
                  borderSide: const BorderSide(color: AppTheme.primaryColor),
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              onChanged: _filterPolos,
            ),
          ),
          Expanded(
            child: isLoading
                ? const Center(
                    child: CircularProgressIndicator(color: Colors.teal),
                  )
                : filteredData.isEmpty
                    ? const Center(
                        child: Text(
                          'Nenhum polo encontrado.',
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            color: Colors.redAccent,
                          ),
                        ),
                      )
                    : ListView.builder(
                        itemCount: filteredData.length,
                        itemBuilder: (context, index) {
                          final polo = filteredData[index];
                          return GestureDetector(
                            onTap: () => Navigator.push(
                              context,
                              MaterialPageRoute(
                                  builder: (context) => EditPolo(polo: polo)),
                            ),
                            child: Container(
                              margin: const EdgeInsets.symmetric(vertical: 10),
                              padding: const EdgeInsets.all(20.0),
                              decoration: BoxDecoration(
                                color: AppTheme.primaryColor,
                                border: Border.all(
                                    color: AppTheme.accentColor, width: 2),
                                borderRadius: BorderRadius.circular(12),
                              ),
                              child: Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                                children: [
                                  Text(
                                    polo.nome,
                                    style: AppTheme.bodyTextStyle.copyWith(
                                      fontSize: 18,
                                      fontWeight: FontWeight.bold,
                                      color: Colors.white,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          );
                        },
                      ),
          ),
        ],
      ),
    );
  }
}
