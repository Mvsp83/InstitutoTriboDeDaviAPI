import 'package:flutter_tribo_de_davi_api/widgets/action_card.dart';
import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/business/consultas/consulta_aniversariante.dart';
import 'package:flutter_tribo_de_davi_api/business/faltas_main.dart';

class InfoPage extends StatelessWidget {
  const InfoPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: const Text(
          "Informações",
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
              title: "Faltas",
              icon: Icons.flaky_outlined,
              color: AppTheme.primaryColor,
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                      builder: (context) => const AlunosMaisFaltantesPage()),
                );
              },
            ),
            const SizedBox(height: 20),
            ActionCard(
              title: "Aniversariantes do mês",
              icon: Icons.cake,
              color: AppTheme.primaryColor,
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                      builder: (context) => const AniversariantesPage()),
                );
              },
            ),
          ],
        ),
      ),
    );
  }

}
