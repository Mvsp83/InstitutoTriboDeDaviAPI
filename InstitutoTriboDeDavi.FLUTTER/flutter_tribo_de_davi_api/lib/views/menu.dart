import '../api/api_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/business/presenca_main.dart';
import 'package:flutter_tribo_de_davi_api/views/cadastros.dart';
import 'package:flutter_tribo_de_davi_api/views/login.dart';

class MenuPage extends StatefulWidget {
  const MenuPage({super.key});

  @override
  State<MenuPage> createState() => _MenuPageState();
}

class _MenuPageState extends State<MenuPage> {
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
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Grid de botões
            Expanded(
              child: GridView.count(
                crossAxisCount: 2,
                crossAxisSpacing: 16,
                mainAxisSpacing: 16,
                children: <Widget>[
                  ElevatedButton(
                    style: AppTheme.elevatedButtonStyle,
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                            builder: (context) => const CadastrosPage()),
                      );
                    },
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(Icons.person_add,
                            size: 50, color: Colors.white),
                        const SizedBox(height: 10),
                        Text(
                          'Cadastros',
                          style: AppTheme.bodyTextStyle
                              .copyWith(fontSize: 18, color: Colors.white),
                        ),
                      ],
                    ),
                  ),
                  ElevatedButton(
                    style: AppTheme.elevatedButtonStyle,
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                            builder: (context) => const PresencaPage(),
                      ));
                    },
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(Icons.check_box,
                            size: 50, color: Colors.white),
                        const SizedBox(height: 10),
                        Text(
                          'Presença',
                          style: AppTheme.bodyTextStyle
                              .copyWith(fontSize: 18, color: Colors.white),
                        ),
                      ],
                    ),
                  ),
                  ElevatedButton(
                    style: AppTheme.elevatedButtonStyle,
                    onPressed: () {},
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(Icons.message,
                            size: 50, color: Colors.white),
                        const SizedBox(height: 10),
                        Text(
                          'Mensagens',
                          style: AppTheme.bodyTextStyle
                              .copyWith(fontSize: 18, color: Colors.white),
                        ),
                      ],
                    ),
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
