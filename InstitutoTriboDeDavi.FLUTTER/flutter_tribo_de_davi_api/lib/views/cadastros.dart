import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/aluno/aluno_main.dart';
import 'package:flutter_tribo_de_davi_api/entities/polo/polo_main.dart';
import 'package:flutter_tribo_de_davi_api/entities/usuario/usuario_main.dart';
import 'package:flutter_tribo_de_davi_api/widgets/action_card.dart';

class CadastrosPage extends StatelessWidget {
  const CadastrosPage({super.key});

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
            ActionCard(
              title: "Polos",
              icon: Icons.location_city,
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const PoloPage()),
                );
              },
            ),
            const SizedBox(height: 20),
            ActionCard(
              title: "Alunos",
              icon: Icons.school,
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const AlunoPage()),
                );
              },
            ),
            const SizedBox(height: 20),
            ActionCard(
              title: "Usuários",
              icon: Icons.manage_accounts,
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const UsuarioPage()),
                );
              },
            ),
          ],
        ),
      ),
    );
  }
}
