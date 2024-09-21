import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/views/tela_login.dart';

void main() {
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      home: LoginPage(), //MainPage(),
    );
  }
}
