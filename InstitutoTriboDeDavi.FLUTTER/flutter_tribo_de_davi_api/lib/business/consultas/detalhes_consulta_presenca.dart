import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aula.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/presenca.dart';

class DetalhesPresencaPage extends StatelessWidget {
  final Aula aula;

  const DetalhesPresencaPage({super.key, required this.aula});

  Future<List<Presenca>> _getPresencasPorAula(int aulaId) async {
    try {
      final apiHandler = ApiHandler<Presenca>(
        baseUri: ApiRoutes.entity("presenca/aula"),
        fromJson: (json) => Presenca.fromJson(json),
      );

      return await apiHandler.getDataListById(aulaId);
    } catch (e) {
      return [];
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Presenças - Aula ${aula.id}"),
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
        centerTitle: true,
      ),
      backgroundColor: AppTheme.backgroundColor,
      body: FutureBuilder<List<Presenca>>(
        future: _getPresencasPorAula(aula.id),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(
              child: CircularProgressIndicator(
                valueColor:
                    AlwaysStoppedAnimation<Color>(AppTheme.primaryColor),
              ),
            );
          } else if (snapshot.hasError) {
            return Center(
              child: Text(
                "Erro: ${snapshot.error}",
                style: const TextStyle(color: AppTheme.textColor),
              ),
            );
          } else if (snapshot.data == null || snapshot.data!.isEmpty) {
            return const Center(
              child: Text(
                "Nenhuma presença encontrada.",
                style: TextStyle(color: AppTheme.textColor),
              ),
            );
          }

          var presencas = snapshot.data!;
          presencas.sort((a, b) =>
              a.nomeAluno!.compareTo(b.nomeAluno!)); // Ordenação alfabética

          return ListView.builder(
            itemCount: presencas.length,
            itemBuilder: (context, index) {
              var presenca = presencas[index];
              return Card(
                margin: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
                color: presenca.estaPresente
                    ? AppTheme.primaryColor
                    : AppTheme.secondaryColor,
                child: ListTile(
                  title: Text(
                    presenca.nomeAluno!, // Exibe apenas o nome do aluno
                    style: const TextStyle(color: AppTheme.textColor),
                  ),
                  subtitle: Text(
                    presenca.estaPresente ? "Presente" : "Ausente",
                    style: TextStyle(
                      color: presenca.estaPresente
                          ? AppTheme.primaryColor
                          : AppTheme.secondaryColor,
                    ),
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }
}
