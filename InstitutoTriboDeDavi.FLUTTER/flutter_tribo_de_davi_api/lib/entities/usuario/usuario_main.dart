import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/usuario.dart';
import 'package:flutter_tribo_de_davi_api/entities/usuario/usuario_add.dart';
import 'package:flutter_tribo_de_davi_api/entities/usuario/usuario_edit.dart';

class UsuarioPage extends StatefulWidget {
  const UsuarioPage({super.key});

  @override
  State<UsuarioPage> createState() => _UsuarioPageState();
}

class _UsuarioPageState extends State<UsuarioPage> {
  ApiHandler<Usuario> apiHandler = ApiHandler<Usuario>(
    baseUri: ApiRoutes.entity("usuario"),
    fromJson: (json) => Usuario.fromJson(json),
  );
  List<Usuario> data = [];
  List<Usuario> filteredData = [];
  bool isLoading = false;
  String searchQuery = '';

  @override
  void initState() {
    super.initState();
    getUsuarios();
  }

  Future<void> getUsuarios() async {
    setState(() => isLoading = true);
    try {
      List<Usuario> allUsuarios = await apiHandler.getData();
      setState(() {
        data = allUsuarios;
        filteredData = allUsuarios; // Inicialmente, ambas listas são iguais.
        isLoading = false;
      });
    } catch (e) {
      setState(() => isLoading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Erro ao carregar usuários: $e")),
      );
    }
  }

  void _filterUsuarios(String query) {
    setState(() {
      searchQuery = query;
      if (query.isEmpty) {
        filteredData = data;
      } else {
        filteredData = data.where((usuario) {
          return usuario.login.toLowerCase().contains(query.toLowerCase()) ||
              usuario.email.toLowerCase().contains(query.toLowerCase());
        }).toList();
      }
    });
  }

  Future<void> deleteUsuario(int id) async {
    await apiHandler.deleteData(id: id);
    setState(() {
      data.removeWhere((usuario) => usuario.id == id);
      _filterUsuarios(searchQuery);
    });
  }

  void _confirmDelete(int id) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Confirmar Exclusão"),
        content: const Text("Tem certeza que deseja excluir este usuário?"),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text("Cancelar"),
          ),
          ElevatedButton(
            onPressed: () {
              deleteUsuario(id);
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
        title: const Text("Gestão de Usuários"),
        centerTitle: true,
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
        elevation: 4,
      ),
      body: Column(
        children: [
          // Campo de Busca
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: TextField(
              decoration: InputDecoration(
                hintText: "Buscar por login ou email...",
                prefixIcon: const Icon(Icons.search, color: AppTheme.iconColor),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
                focusedBorder: OutlineInputBorder(
                  borderSide: BorderSide(color: AppTheme.primaryColor),
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              onChanged: _filterUsuarios,
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
                          'Nenhum usuário encontrado.',
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
                          final usuario = filteredData[index];
                          return GestureDetector(
                            onTap: () => Navigator.push(
                              context,
                              MaterialPageRoute(
                                  builder: (context) =>
                                      EditUsuario(usuario: usuario)),
                            ),
                            child: Container(
                              margin: const EdgeInsets.symmetric(vertical: 10),
                              padding: const EdgeInsets.all(16.0),
                              decoration: BoxDecoration(
                                color: AppTheme.primaryColor,
                                border:
                                    Border.all(color: Colors.white, width: 2),
                                borderRadius: BorderRadius.circular(12),
                              ),
                              child: Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                                children: [
                                  Text(
                                    usuario.login,
                                    style: AppTheme.bodyTextStyle.copyWith(
                                      fontSize: 18,
                                      fontWeight: FontWeight.bold,
                                      color: Colors.white,
                                    ),
                                  ),
                                  IconButton(
                                    icon: const Icon(Icons.delete_outline),
                                    color: Colors.white,
                                    onPressed: () => _confirmDelete(usuario.id),
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
      floatingActionButton: FloatingActionButton(
        onPressed: () => Navigator.push(
          context,
          MaterialPageRoute(builder: (context) => const AddUsuario()),
        ),
        backgroundColor: AppTheme.primaryColor,
        child: const Icon(
          Icons.add,
          color: AppTheme.textColor,
        ),
      ),
    );
  }
}
