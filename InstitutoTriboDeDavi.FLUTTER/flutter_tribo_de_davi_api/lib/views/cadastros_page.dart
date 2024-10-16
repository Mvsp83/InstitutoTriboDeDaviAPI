// import 'package:flutter/material.dart';
// import 'package:flutter_tribo_de_davi_api/entities/usuario/main_page.dart';
// import 'package:flutter_tribo_de_davi_api/views/login_page.dart';

// class CadastrosPage extends StatefulWidget {
//   const CadastrosPage({super.key});

//   @override
//   State<CadastrosPage> createState() => _CadastrosPageState();
// }

// class _CadastrosPageState extends State<CadastrosPage> {
//   void _logout() {
//     Navigator.pushReplacement(
//       context,
//       MaterialPageRoute(builder: (context) => const LoginPage()),
//     );
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       appBar: AppBar(
//         title: const Text("Instituto Tribo de Davi"),
//         centerTitle: true,
//         backgroundColor: Colors.black87,
//         foregroundColor: Colors.white,
//         elevation: 4,
//         shadowColor: Colors.black54,
//         toolbarHeight: 100,
//       ),
//       body: Padding(
//         padding: const EdgeInsets.all(16.0),
//         child: GridView.count(
//           crossAxisCount: 2,
//           crossAxisSpacing: 16,
//           mainAxisSpacing: 16,
//           children: <Widget>[
//             // Botão de Cadastros
//             ElevatedButton(
//               style: ElevatedButton.styleFrom(
//                 backgroundColor: Colors.lightBlue,
//                 shape: RoundedRectangleBorder(
//                   borderRadius: BorderRadius.circular(15),
//                 ),
//                 padding: const EdgeInsets.all(20),
//               ),
//               onPressed: () {
//                 Navigator.push(
//                   context,
//                   MaterialPageRoute(builder: (context) => const UsuarioPage()),
//                 );
//               },
//               child: const Column(
//                 mainAxisAlignment: MainAxisAlignment.center,
//                 children: [
//                   Icon(Icons.supervised_user_circle_sharp,
//                       size: 50, color: Colors.white),
//                   SizedBox(height: 10),
//                   Text(
//                     'Usuário',
//                     style: TextStyle(fontSize: 18, color: Colors.white),
//                   ),
//                 ],
//               ),
//             ),
//           ],
//         ),
//       ),
//     );
//   }
// }

import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/entities/aluno/main_page.dart';
import 'package:flutter_tribo_de_davi_api/entities/polo/main_page.dart';
import 'package:flutter_tribo_de_davi_api/entities/usuario/main_page.dart';
import 'package:flutter_tribo_de_davi_api/views/login_page.dart';

class CadastrosPage extends StatefulWidget {
  const CadastrosPage({super.key});

  @override
  State<CadastrosPage> createState() => _CadastrosPageState();
}

class _CadastrosPageState extends State<CadastrosPage> {
  void _logout() {
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(builder: (context) => const LoginPage()),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Instituto Tribo de Davi"),
        centerTitle: true,
        backgroundColor: Colors.black87,
        foregroundColor: Colors.white,
        elevation: 4,
        shadowColor: Colors.black54,
        toolbarHeight: 100,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: GridView.count(
          crossAxisCount: 2,
          crossAxisSpacing: 16,
          mainAxisSpacing: 16,
          children: <Widget>[
            // Botão de Cadastros de Usuário
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.lightBlue,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(15),
                ),
                padding: const EdgeInsets.all(20),
              ),
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const UsuarioPage()),
                );
              },
              child: const Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.supervised_user_circle_sharp,
                      size: 50, color: Colors.white),
                  SizedBox(height: 10),
                  Text(
                    'Usuário',
                    style: TextStyle(fontSize: 18, color: Colors.white),
                  ),
                ],
              ),
            ),
            // Botão de Cadastros de Alunos
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.lightGreen, // Cor do botão de Alunos
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(15),
                ),
                padding: const EdgeInsets.all(20),
              ),
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                      builder: (context) =>
                          const AlunoPage()), // Redirecionar para a página de alunos
                );
              },
              child: const Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.school,
                      size: 50,
                      color: Colors.white), // Ícone representativo de Alunos
                  SizedBox(height: 10),
                  Text(
                    'Alunos',
                    style: TextStyle(fontSize: 18, color: Colors.white),
                  ),
                ],
              ),
            ),
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.purple, // Cor do botão de Alunos
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(15),
                ),
                padding: const EdgeInsets.all(20),
              ),
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                      builder: (context) =>
                          const PoloPage()), // Redirecionar para a página de alunos
                );
              },
              child: const Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.school,
                      size: 50,
                      color: Colors.white), // Ícone representativo de Alunos
                  SizedBox(height: 10),
                  Text(
                    'Polos',
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
