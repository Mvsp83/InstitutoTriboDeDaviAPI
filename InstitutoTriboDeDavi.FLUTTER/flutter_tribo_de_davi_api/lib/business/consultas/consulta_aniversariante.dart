import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aniversariante.dart';
import 'package:intl/intl.dart';

class AniversariantesPage extends StatelessWidget {
  const AniversariantesPage({super.key});

  Future<List<Aniversariante>> _getAniversariantesDoMes() async {
    int mesAtual = DateTime.now().month;

    List<Aniversariante> aniversariantes = await ApiHandler<Aniversariante>(
      baseUri: ApiRoutes.entity("aniversariantes/$mesAtual"),
      fromJson: (json) => Aniversariante.fromJson(json),
    ).getData2();

    return aniversariantes;
  }

  bool _isAniversarianteDoDia(DateTime? dataNascimento) {
    if (dataNascimento == null) return false;

    final agora = DateTime.now().toUtc().add(const Duration(hours: -3));

    return dataNascimento.day == agora.day &&
        dataNascimento.month == agora.month;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Aniversariantes do Mês"),
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
        centerTitle: true,
      ),
      body: Container(
        color: AppTheme.backgroundColor,
        child: FutureBuilder<List<Aniversariante>>(
          future: _getAniversariantesDoMes(),
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return const Center(child: CircularProgressIndicator());
            } else if (snapshot.hasError) {
              return Center(child: Text("Erro: ${snapshot.error}"));
            } else if (snapshot.data == null || snapshot.data!.isEmpty) {
              return const Center(
                child: Text(
                  "Nenhum aniversariante encontrado para este mês.",
                  style: TextStyle(fontSize: 16, color: Colors.grey),
                ),
              );
            }

            var aniversariantes = snapshot.data!;
            return ListView.builder(
              itemCount: aniversariantes.length,
              itemBuilder: (context, index) {
                var aniversariante = aniversariantes[index];
                return Container(
                  margin:
                      const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
                  padding:
                      const EdgeInsets.symmetric(vertical: 16, horizontal: 12),
                  decoration: BoxDecoration(
                    color: _isAniversarianteDoDia(aniversariante.dataNascimento)
                        ? Colors.green // Verde para aniversariantes do dia
                        : (aniversariante.jaComemorado
                            ? Colors.grey[600]?.withOpacity(
                                0.1) // Transparente para já comemorados
                            : AppTheme.primaryColor), // Cor padrão para outros
                    borderRadius: BorderRadius.circular(15),
                    border: Border.all(color: AppTheme.borderColor, width: 2),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withOpacity(0.2),
                        blurRadius: 8,
                        spreadRadius: 1,
                        offset: const Offset(2, 4),
                      ),
                    ],
                  ),
                  child: Row(
                    children: [
                      const SizedBox(width: 16),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              aniversariante.nome,
                              style: const TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                                color: Colors.white,
                              ),
                            ),
                            const SizedBox(height: 4),
                            Text(
                              aniversariante.dataNascimento != null
                                  ? "Aniversário: ${DateFormat('dd/MM').format(aniversariante.dataNascimento!)}"
                                  : "Data não informada",
                              style: const TextStyle(
                                fontSize: 14,
                                color: Colors.white70,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                );
              },
            );
          },
        ),
      ),
    );
  }
}
