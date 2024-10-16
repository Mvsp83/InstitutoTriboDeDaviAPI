import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/usuario.dart';

class FindUsuario extends StatefulWidget {
  const FindUsuario({super.key});

  @override
  State<FindUsuario> createState() => _FindUsuarioState();
}

class _FindUsuarioState extends State<FindUsuario> {
  ApiHandler<Usuario> apiHandler = ApiHandler<Usuario>(
    baseUri: ApiRoutes.entity("usuario"),
    fromJson: (json) => Usuario.fromJson(json),
  );
  Usuario usuario = const Usuario.empty();
  TextEditingController textEditingController = TextEditingController();

  void findUsuario(int id) async {
    usuario = (await apiHandler.getDataById(id: id))!;
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Buscar Usuário"),
        centerTitle: true,
        backgroundColor: Colors.black87,
        foregroundColor: Colors.white,
        elevation: 4,
        shadowColor: Colors.black54,
      ),
      bottomNavigationBar: Padding(
        padding: const EdgeInsets.all(16.0),
        child: ElevatedButton(
          onPressed: () {
            findUsuario(int.parse(textEditingController.text));
          },
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.blue,
            padding: const EdgeInsets.symmetric(vertical: 20),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(12),
            ),
            elevation: 5,
          ),
          child: const Text(
            'Buscar',
            style: TextStyle(
              fontSize: 18,
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            TextField(
              controller: textEditingController,
              keyboardType: TextInputType.number,
              decoration: InputDecoration(
                labelText: 'ID do Usuário',
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                prefixIcon: const Icon(Icons.search),
              ),
            ),
            const SizedBox(height: 20),
            if (usuario.id != 0) // Verifica se há um usuário encontrado
              ListTile(
                leading: CircleAvatar(
                  backgroundColor: Colors.blue,
                  child: Text(
                    "${usuario.id}",
                    style: const TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                title: Text(
                  usuario.email,
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                  ),
                ),
                subtitle: Text(
                  usuario.login,
                  style: TextStyle(color: Colors.grey[600]),
                ),
              )
            else
              const Padding(
                padding: EdgeInsets.all(20),
                child: Text(
                  'Nenhum usuário encontrado.',
                  style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    color: Colors.redAccent,
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
