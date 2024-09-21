import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/entities/Usuario/main_page.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Instituto Tribo de Davi API"),
        centerTitle: true,
        backgroundColor: Colors.black87,
        foregroundColor: Colors.white,
        elevation: 4, // Adicionando sombra na AppBar para profundidade
        shadowColor: Colors.black54,
      ),
      // appBar: AppBar(
      //   title: const Text("Página Inicial"),
      //   backgroundColor: Colors.blueAccent,
      // ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: GridView.count(
          crossAxisCount: 2, // Quantidade de colunas
          crossAxisSpacing: 16,
          mainAxisSpacing: 16,
          children: <Widget>[
            // Botão de Chamada
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.blue,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(15),
                ),
                padding: EdgeInsets.all(20),
              ),
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => MainPage()),
                );
              },
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: const [
                  Icon(Icons.check_box, size: 50, color: Colors.white),
                  SizedBox(height: 10),
                  Text(
                    'Presença',
                    style: TextStyle(fontSize: 18, color: Colors.white),
                  ),
                ],
              ),
            ),
            // Botão de Cadastros
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.orange,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(15),
                ),
                padding: EdgeInsets.all(20),
              ),
              onPressed: () {
                // Ação para a tela de Cadastros
              },
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: const [
                  Icon(Icons.person_add, size: 50, color: Colors.white),
                  SizedBox(height: 10),
                  Text(
                    'Cadastros',
                    style: TextStyle(fontSize: 18, color: Colors.white),
                  ),
                ],
              ),
            ),
            // Botão de Mensagens
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.purple,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(15),
                ),
                padding: EdgeInsets.all(20),
              ),
              onPressed: () {
                // Ação para a tela de Mensagens
              },
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: const [
                  Icon(Icons.message, size: 50, color: Colors.white),
                  SizedBox(height: 10),
                  Text(
                    'Mensagens',
                    style: TextStyle(fontSize: 18, color: Colors.white),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
