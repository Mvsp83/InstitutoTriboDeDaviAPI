import 'package:flutter_tribo_de_davi_api/widgets/action_card.dart';
import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/aula/aula_add.dart';
import 'package:flutter_tribo_de_davi_api/business/consultas/consulta_presenca.dart';
import 'package:flutter_tribo_de_davi_api/business/presenca_main.dart';

class AulaPage extends StatelessWidget {
  const AulaPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: const Text(
          "Gerenciamento de Aulas",
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
              title: "Criar Nova Aula",
              icon: Icons.add,
              color: AppTheme.primaryColor,
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                      builder: (context) => const CriarAulaPage()),
                );
              },
            ),
            const SizedBox(height: 20),
            ActionCard(
              title: "Registrar Presença",
              icon: Icons.event_available,
              color: AppTheme.primaryColor,
              onPressed: () {
                Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => const PresencaPage(),
                    ));
              },
            ),
            const SizedBox(height: 20),
            ActionCard(
              title: "Consulta Presença",
              icon: Icons.search,
              color: AppTheme.primaryColor,
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                      builder: (context) => const ConsultaPresencaPage()),
                );
              },
            ),
          ],
        ),
      ),
    );
  }

}
