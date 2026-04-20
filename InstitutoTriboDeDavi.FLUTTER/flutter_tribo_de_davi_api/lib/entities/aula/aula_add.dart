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
        if (poloId == null) throw Exception("PoloId não encontrado no token.");
      } else {
        throw Exception("Token não encontrado.");
      }
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Erro ao carregar PoloId: $e')),
      );
    }
    setState(() {});
  }

  Map<String, dynamic> _decodeJWT(String token) {
    final parts = token.split('.');
    if (parts.length != 3) throw Exception("Token inválido.");
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

    setState(() => isLoading = true);

    for (int semana = 0; semana < semanasParaReplicar; semana++) {
      final data = selectedDate!.add(Duration(days: semana * 7));
      final aula = Aula(
        id: 0,
        poloId: poloId!,
        data: data,
        horaInicio:
            "${horaInicio!.hour.toString().padLeft(2, '0')}:${horaInicio!.minute.toString().padLeft(2, '0')}:00",
        horaFim:
            "${horaFim!.hour.toString().padLeft(2, '0')}:${horaFim!.minute.toString().padLeft(2, '0')}:00",
        turma: turma!,
      );

      try {
        final response = await ApiHandler<Aula>(
          baseUri: ApiRoutes.entity("aula"),
          fromJson: (json) => Aula.fromJson(json),
        ).addData(item: aula);

        if (response.statusCode != 200) {
          if (!mounted) return;
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Erro ao criar a aula.')),
          );
        }
      } catch (e) {
        if (!mounted) return;
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Erro de conexão: $e')),
        );
      }
    }

    setState(() => isLoading = false);

    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Aulas criadas com sucesso!')),
    );
    Navigator.pop(context);
  }

  // ─── Campo de data / hora (picker) ───────────────────────────────────────
  Widget _buildPickerField({
    required String labelText,
    required String? value,
    required IconData icon,
    required VoidCallback onTap,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(10),
        child: InputDecorator(
          decoration: InputDecoration(
            labelText: labelText,
            labelStyle: const TextStyle(
              color: AppTheme.textMutedColor,
              fontSize: 14,
            ),
            floatingLabelStyle: const TextStyle(
              color: AppTheme.accentColor,
              fontSize: 12,
              fontWeight: FontWeight.w500,
            ),
            prefixIcon: Icon(icon, color: AppTheme.accentColor, size: 20),
            suffixIcon: const Icon(
              Icons.chevron_right,
              color: AppTheme.textMutedColor,
              size: 20,
            ),
            filled: true,
            fillColor: AppTheme.surfaceColor,
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide:
                  const BorderSide(color: AppTheme.borderColor, width: 0.5),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide:
                  const BorderSide(color: AppTheme.accentColor, width: 1),
            ),
            contentPadding:
                const EdgeInsets.symmetric(vertical: 16, horizontal: 16),
          ),
          child: Text(
            value ?? '—',
            style: TextStyle(
              color:
                  value != null ? AppTheme.textColor : AppTheme.textMutedColor,
              fontSize: 14,
            ),
          ),
        ),
      ),
    );
  }

  // ─── Dropdown de semanas ──────────────────────────────────────────────────
  Widget _buildSemanasDropdown() {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: DropdownButtonFormField<int>(
        value: semanasParaReplicar,
        dropdownColor: AppTheme.surfaceColor,
        iconEnabledColor: AppTheme.accentColor,
        style: const TextStyle(color: AppTheme.textColor, fontSize: 14),
        decoration: InputDecoration(
          labelText: 'Replicar por',
          labelStyle: const TextStyle(
            color: AppTheme.textMutedColor,
            fontSize: 14,
          ),
          floatingLabelStyle: const TextStyle(
            color: AppTheme.accentColor,
            fontSize: 12,
            fontWeight: FontWeight.w500,
          ),
          prefixIcon: const Icon(
            Icons.repeat,
            color: AppTheme.accentColor,
            size: 20,
          ),
          filled: true,
          fillColor: AppTheme.surfaceColor,
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide:
                const BorderSide(color: AppTheme.borderColor, width: 0.5),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: AppTheme.accentColor, width: 1),
          ),
          contentPadding:
              const EdgeInsets.symmetric(vertical: 16, horizontal: 16),
        ),
        items: List.generate(4, (i) => i + 1)
            .map((v) => DropdownMenuItem<int>(
                  value: v,
                  child: Text(
                    '$v ${v == 1 ? 'semana' : 'semanas'}',
                    style: const TextStyle(
                        color: AppTheme.textColor, fontSize: 14),
                  ),
                ))
            .toList(),
        onChanged: (v) => setState(() => semanasParaReplicar = v!),
      ),
    );
  }

  // ─── Campo de turma ───────────────────────────────────────────────────────
  Widget _buildTurmaField() {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextField(
        controller: turmaController,
        keyboardType: TextInputType.number,
        style: const TextStyle(color: AppTheme.textColor, fontSize: 14),
        cursorColor: AppTheme.accentColor,
        onChanged: (v) => turma = int.tryParse(v),
        decoration: InputDecoration(
          labelText: 'Turma',
          hintText: '1 ou 2',
          labelStyle: const TextStyle(
            color: AppTheme.textMutedColor,
            fontSize: 14,
          ),
          hintStyle: const TextStyle(
            color: AppTheme.textMutedColor,
            fontSize: 14,
          ),
          floatingLabelStyle: const TextStyle(
            color: AppTheme.accentColor,
            fontSize: 12,
            fontWeight: FontWeight.w500,
          ),
          prefixIcon: const Icon(
            Icons.class_outlined,
            color: AppTheme.accentColor,
            size: 20,
          ),
          filled: true,
          fillColor: AppTheme.surfaceColor,
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide:
                const BorderSide(color: AppTheme.borderColor, width: 0.5),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: AppTheme.accentColor, width: 1),
          ),
          errorBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: Colors.redAccent, width: 0.5),
          ),
          contentPadding:
              const EdgeInsets.symmetric(vertical: 16, horizontal: 16),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.primaryColor,
      appBar: AppBar(
        title: const Text(
          "Criar Aula",
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
      body: isLoading
          ? const Center(
              child: CircularProgressIndicator(color: AppTheme.accentColor),
            )
          : poloId == null
              ? const Center(
                  child: CircularProgressIndicator(color: AppTheme.accentColor),
                )
              : SingleChildScrollView(
                  padding: const EdgeInsets.all(20),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      _buildPickerField(
                        labelText: 'Data',
                        value: selectedDate != null
                            ? '${selectedDate!.day.toString().padLeft(2, '0')}/'
                                '${selectedDate!.month.toString().padLeft(2, '0')}/'
                                '${selectedDate!.year}'
                            : null,
                        icon: Icons.calendar_today_outlined,
                        onTap: () async {
                          final picked = await showDatePicker(
                            context: context,
                            initialDate: DateTime.now(),
                            firstDate: DateTime(2000),
                            lastDate: DateTime(2100),
                          );
                          if (picked != null)
                            setState(() => selectedDate = picked);
                        },
                      ),
                      _buildPickerField(
                        labelText: 'Hora de Início',
                        value: horaInicio?.format(context),
                        icon: Icons.access_time_outlined,
                        onTap: () async {
                          final picked = await showTimePicker(
                            context: context,
                            initialTime: TimeOfDay.now(),
                          );
                          if (picked != null)
                            setState(() => horaInicio = picked);
                        },
                      ),
                      _buildPickerField(
                        labelText: 'Hora de Fim',
                        value: horaFim?.format(context),
                        icon: Icons.access_time_filled_outlined,
                        onTap: () async {
                          final picked = await showTimePicker(
                            context: context,
                            initialTime: TimeOfDay.now(),
                          );
                          if (picked != null) setState(() => horaFim = picked);
                        },
                      ),
                      _buildSemanasDropdown(),
                      _buildTurmaField(),
                      const SizedBox(height: 8),
                    ],
                  ),
                ),
      bottomNavigationBar: Padding(
        padding: const EdgeInsets.fromLTRB(20, 12, 20, 24),
        child: SizedBox(
          height: 52,
          child: ElevatedButton(
            onPressed: (isLoading || poloId == null) ? null : saveAula,
            style: AppTheme.elevatedButtonStyle,
            child: isLoading
                ? const SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(
                      strokeWidth: 2,
                      color: AppTheme.primaryColor,
                    ),
                  )
                : const Text('SALVAR AULAS', style: AppTheme.buttonTextStyle),
          ),
        ),
      ),
    );
  }
}
