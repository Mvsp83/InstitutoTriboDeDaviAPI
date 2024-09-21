import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_handler.dart';
import 'package:flutter_tribo_de_davi_api/models/model.dart';

class FindUsuario extends StatefulWidget {
  const FindUsuario({super.key});

  @override
  State<FindUsuario> createState() => _FindUsuarioState();
}

class _FindUsuarioState extends State<FindUsuario> {
  ApiHandler apiHandler = ApiHandler();
  Usuario usuario = const Usuario.empty();
  TextEditingController textEditingController = TextEditingController();

  void findUsuario(int id) async {
    usuario = await apiHandler.getUsuarioById(id: id);
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Find Usuario"),
        centerTitle: true,
        backgroundColor: Colors.black87,
        foregroundColor: Colors.white,
      ),
      bottomNavigationBar: MaterialButton(
        color: Colors.teal,
        textColor: Colors.yellow,
        padding: const EdgeInsets.all(20),
        onPressed: () {
          findUsuario(int.parse(textEditingController.text));
        },
        child: const Text('Find'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(10),
        child: Column(
          children: [
            TextField(
              controller: textEditingController,
            ),
            const SizedBox(
              height: 10,
            ),
            ListTile(
              leading: Text("${usuario.id}"),
              title: Text(usuario.email),
              subtitle: Text(usuario.login),
            ),
          ],
        ),
      ),
    );
  }
}
