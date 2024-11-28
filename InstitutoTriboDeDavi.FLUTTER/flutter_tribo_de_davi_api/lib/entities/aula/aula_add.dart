import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aula.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';

class CriarAulaPage extends StatefulWidget {
  const CriarAulaPage({super.key});

  @override
  State<CriarAulaPage> createState() => _CriarAulaPageState();
}

class _CriarAulaPageState extends State<CriarAulaPage> {
  DateTime? selectedDate;
  String? selectedPolo;
  TimeOfDay? horaInicio;
  TimeOfDay? horaFim;
  bool isLoading = false;

  int semanasParaReplicar = 1;

  Future<void> saveAula() async {
    if (selectedDate == null ||
        selectedPolo == null ||
        horaInicio == null ||
        horaFim == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
            content: Text('Por favor, preencha todos os detalhes da aula.')),
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
        poloId: int.parse(selectedPolo!),
        data: data,
        horaInicio:
            "${horaInicio!.hour.toString().padLeft(2, '0')}:${horaInicio!.minute.toString().padLeft(2, '0')}:00",
        horaFim:
            "${horaFim!.hour.toString().padLeft(2, '0')}:${horaFim!.minute.toString().padLeft(2, '0')}:00",
      );

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
                    _buildFormField(
                      label: 'Polo',
                      onChanged: (value) => selectedPolo = value,
                    ),
                    const SizedBox(height: 20),
                    _buildPickerButton(
                      label: selectedDate == null
                          ? 'Selecionar Data'
                          : 'Data: ${selectedDate!.toLocal()}'.split(' ')[0],
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
              ),
            ),
    );
  }

  Widget _buildFormField(
      {required String label, required ValueChanged<String> onChanged}) {
    return TextField(
      decoration: InputDecoration(
        labelText: label,
        border: const OutlineInputBorder(),
        fillColor: Colors.white,
        filled: true,
      ),
      onChanged: onChanged,
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
