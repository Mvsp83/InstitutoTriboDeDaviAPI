import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aula.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:shared_preferences/shared_preferences.dart';

class CriarAulaPage extends StatefulWidget {
  const CriarAulaPage({super.key});

  @override
  State<CriarAulaPage> createState() => _CriarAulaPageState();
}

class _CriarAulaPageState extends State<CriarAulaPage> {
  DateTime? selectedDate;
  TimeOfDay? horaInicio;
  TimeOfDay? horaFim;
  bool isLoading = false;

  int semanasParaReplicar = 1;
  int? poloId;
  int? turma;
  final TextEditingController turmaController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _loadPoloIdFromToken();
  }

  Future<void> _loadPoloIdFromToken() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString('auth_token');

      if (token != null) {
        final payload = _decodeJWT(token);

        final poloIdString = payload['PoloId'];
        poloId = int.tryParse(poloIdString.toString());

        if (poloId == null) {
          throw Exception("PoloId não encontrado no token.");
        }
      } else {
        throw Exception("Token não encontrado.");
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Erro ao carregar PoloId: $e')),
      );
    }
    setState(() {});
  }

  Map<String, dynamic> _decodeJWT(String token) {
    final parts = token.split('.');
    if (parts.length != 3) {
      throw Exception("Token inválido.");
    }

    final payload =
        utf8.decode(base64Url.decode(base64Url.normalize(parts[1])));
    return json.decode(payload) as Map<String, dynamic>;
  }

  Future<void> saveAula() async {
    if (selectedDate == null ||
        horaInicio == null ||
        horaFim == null ||
        poloId == null ||
        turma == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Por favor, preencha todos os detalhes da aula.'),
        ),
      );
      return;
    }

    setState(() {
      isLoading = true;
    });

    List<DateTime> datasAulas = [];
    for (int semana = 0; semana < semanasParaReplicar; semana++) {
      DateTime novaData = selectedDate!.add(Duration(days: semana * 7));
      datasAulas.add(novaData);
    }

    for (var data in datasAulas) {
      final aula = Aula(
          id: 0,
          poloId: poloId!,
          data: data,
          horaInicio:
              "${horaInicio!.hour.toString().padLeft(2, '0')}:${horaInicio!.minute.toString().padLeft(2, '0')}:00",
          horaFim:
              "${horaFim!.hour.toString().padLeft(2, '0')}:${horaFim!.minute.toString().padLeft(2, '0')}:00",
          turma: turma!);

      try {
        final response = await ApiHandler<Aula>(
          baseUri: ApiRoutes.entity("aula"),
          fromJson: (json) => Aula.fromJson(json),
        ).addData(item: aula);

        if (response.statusCode != 200) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Erro ao criar a aula.')),
          );
        }
      } catch (e) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Erro de conexão: $e')),
        );
      }
    }

    setState(() {
      isLoading = false;
    });

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Aulas criadas com sucesso!')),
    );
    Navigator.pop(context);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: const Text("Criar Aula"),
        centerTitle: true,
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
      ),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    poloId == null
                        ? const Center(child: CircularProgressIndicator())
                        : Column(
                            children: [
                              _buildPickerButton(
                                label: selectedDate == null
                                    ? 'Selecionar Data'
                                    : 'Data: ${selectedDate!.toLocal()}'
                                        .split(' ')[0],
                                icon: Icons.calendar_today,
                                onPressed: () async {
                                  selectedDate = await showDatePicker(
                                    context: context,
                                    initialDate: DateTime.now(),
                                    firstDate: DateTime(2000),
                                    lastDate: DateTime(2100),
                                  );
                                  setState(() {});
                                },
                              ),
                              const SizedBox(height: 20),
                              _buildPickerButton(
                                label: horaInicio == null
                                    ? 'Selecionar Hora Início'
                                    : 'Início: ${horaInicio!.format(context)}',
                                icon: Icons.access_time,
                                onPressed: () async {
                                  horaInicio = await showTimePicker(
                                    context: context,
                                    initialTime: TimeOfDay.now(),
                                  );
                                  setState(() {});
                                },
                              ),
                              const SizedBox(height: 20),
                              _buildPickerButton(
                                label: horaFim == null
                                    ? 'Selecionar Hora Fim'
                                    : 'Fim: ${horaFim!.format(context)}',
                                icon: Icons.access_time_filled,
                                onPressed: () async {
                                  horaFim = await showTimePicker(
                                    context: context,
                                    initialTime: TimeOfDay.now(),
                                  );
                                  setState(() {});
                                },
                              ),
                              const SizedBox(height: 20),
                              DropdownButton<int>(
                                value: semanasParaReplicar,
                                onChanged: (int? newValue) {
                                  setState(() {
                                    semanasParaReplicar = newValue!;
                                  });
                                },
                                items: List.generate(4, (index) => index + 1)
                                    .map<DropdownMenuItem<int>>((int value) {
                                  return DropdownMenuItem<int>(
                                    value: value,
                                    child: Text('$value semanas'),
                                  );
                                }).toList(),
                              ),
                              TextField(
                                controller: turmaController,
                                keyboardType: TextInputType.number,
                                decoration: const InputDecoration(
                                  labelText: 'Digite a Turma (1 ou 2)',
                                  border: OutlineInputBorder(),
                                ),
                                onChanged: (value) {
                                  turma = int.tryParse(value);
                                },
                              ),
                              const SizedBox(height: 40),
                              _buildActionCard(
                                context,
                                title: "Salvar Aulas",
                                icon: Icons.save,
                                color: AppTheme.primaryColor,
                                onPressed: saveAula,
                              ),
                            ],
                          ),
                  ],
                ),
              ),
            ),
    );
  }

  Widget _buildPickerButton({
    required String label,
    required IconData icon,
    required VoidCallback onPressed,
  }) {
    return GestureDetector(
      onTap: onPressed,
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 12),
        decoration: AppTheme.cardDecoration,
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(label, style: AppTheme.bodyTextStyle),
            Icon(icon, color: AppTheme.iconColor),
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
          color: color,
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
