import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aluno.dart';
import 'package:flutter_tribo_de_davi_api/entities/usuario/add_page.dart';
import 'package:flutter_tribo_de_davi_api/entities/usuario/find_page.dart';

class AlunoPage extends StatefulWidget {
  const AlunoPage({super.key});

  @override
  State<AlunoPage> createState() => _AlunoPageState();
}

class _AlunoPageState extends State<AlunoPage> {
  ApiHandler<Aluno> apiHandler = ApiHandler<Aluno>(
    baseUri: ApiRoutes.entity("aluno"),
    fromJson: (json) => Aluno.fromJson(json),
  );
  late List<Aluno> data = [];

  void getAluno() async {
    data = await apiHandler.getData();
    setState(() {});
  }

  void deleteUsuario(int id) async {
    await apiHandler.deleteData(id: id);
    setState(() {});
  }

  void searchByEmail(String email) async {}

  @override
  void initState() {
    getAluno();
    super.initState();
  }

  String _getInitials(String name) {
    return name.isNotEmpty
        ? name.trim().split(' ').map((e) => e[0]).take(2).join().toUpperCase()
        : '?';
  }

  void _confirmDelete(int id) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text("Confirmar Exclusão"),
          content: const Text("Tem certeza que deseja excluir este usuário?"),
          actions: <Widget>[
            TextButton(
              child: const Text("Cancelar"),
              onPressed: () {
                Navigator.of(context).pop();
              },
            ),
            TextButton(
              child: const Text("Excluir"),
              onPressed: () {
                deleteUsuario(id);
                Navigator.of(context).pop();
              },
            ),
          ],
        );
      },
    );
  }

// Lista de cores
  final List<Color> avatarColors = [
    Colors.blue,
    Colors.green,
    Colors.red,
    Colors.orange,
    Colors.purple,
    Colors.teal,
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Alunos"),
        centerTitle: true,
        backgroundColor: Colors.black87,
        foregroundColor: Colors.white,
        elevation: 4,
        shadowColor: Colors.black54,
      ),
      bottomNavigationBar: MaterialButton(
        color: Colors.blue,
        textColor: Colors.white,
        padding: const EdgeInsets.all(20),
        onPressed: getAluno,
        child: const Text('Atualizar Lista'),
      ),
      floatingActionButton: Column(
        mainAxisAlignment: MainAxisAlignment.end,
        children: [
          FloatingActionButton(
            heroTag: 1,
            backgroundColor: Colors.blue,
            foregroundColor: Colors.white,
            onPressed: () {
              Navigator.push(context,
                  MaterialPageRoute(builder: (context) => const FindUsuario()));
            },
            child: const Icon(Icons.search),
          ),
          const SizedBox(
            height: 10,
          ),
          FloatingActionButton(
            heroTag: 2,
            backgroundColor: Colors.blue,
            foregroundColor: Colors.white,
            onPressed: () {
              Navigator.push(context,
                  MaterialPageRoute(builder: (context) => const AddUsuario()));
            },
            child: const Icon(Icons.add),
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(8.0),
        child: Column(
          children: [
            Expanded(
              child: data.isEmpty
                  ? const Center(
                      child: CircularProgressIndicator(
                        color: Colors.teal,
                      ),
                    )
                  : ListView.builder(
                      itemCount: data.length,
                      itemBuilder: (BuildContext context, int index) {
                        return Card(
                          elevation: 5,
                          margin: const EdgeInsets.symmetric(vertical: 8),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(15),
                          ),
                          child: ListTile(
                            onTap: () {
                              // Navigator.push(
                              //     context,
                              //     MaterialPageRoute(
                              //         builder: (context) =>
                              //             EditUsuario(usuario: data[index])));
                            },
                            leading: CircleAvatar(
                              // A cor alterna com base no índice
                              backgroundColor:
                                  avatarColors[index % avatarColors.length],
                              child: Text(
                                _getInitials(data[index].cpf),
                                style: const TextStyle(
                                    color: Colors.white,
                                    fontWeight: FontWeight.bold),
                              ),
                            ),
                            title: Text(
                              data[index].nome,
                              style: const TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 16,
                              ),
                            ),
                            subtitle: Row(
                              children: [
                                const Icon(Icons.person_outline, size: 16),
                                const SizedBox(width: 5),
                                Text(
                                  data[index].faixa.toString(),
                                  style: TextStyle(color: Colors.grey[600]),
                                ),
                              ],
                            ),
                            trailing: IconButton(
                              icon: const Icon(Icons.delete_outline),
                              onPressed: () {
                                _confirmDelete(data[index].id);
                              },
                            ),
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
