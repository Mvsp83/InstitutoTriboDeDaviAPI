import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/entities/aluno/aluno_main.dart';
import 'package:flutter_tribo_de_davi_api/entities/polo/polo_main.dart';
import 'package:flutter_tribo_de_davi_api/entities/usuario/usuario_main.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/views/login.dart';

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
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: const Text(
          "Gerenciamento de Cadastros",
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
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            _buildActionCard(
              context,
              title: "Polos",
              icon: Icons.location_city,
              color: AppTheme.primaryColor,
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const PoloPage()),
                );
              },
            ),
            const SizedBox(height: 20),
            _buildActionCard(
              context,
              title: "Alunos",
              icon: Icons.school,
              color: AppTheme.primaryColor,
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const AlunoPage()),
                );
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildActionCard(BuildContext context,
      {required String title,
      required IconData icon,
      required Color color,
      required VoidCallback onPressed}) {
    return GestureDetector(
      onTap: onPressed,
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 24, horizontal: 16),
        decoration: AppTheme.cardDecoration.copyWith(
          color: AppTheme.primaryColor,
          border: Border.all(color: AppTheme.borderColor, width: 2),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(icon, size: 40, color: AppTheme.iconColor),
            const SizedBox(width: 15),
            Text(
              title,
              style: AppTheme.bodyTextStyle.copyWith(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppTheme.textColor,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
