import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aluno.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/faixa.dart';
import 'package:form_builder_validators/form_builder_validators.dart';
import 'package:intl/intl.dart';

class AlunoUpdatePage extends StatefulWidget {
  final Aluno aluno;

  const AlunoUpdatePage({super.key, required this.aluno});

  @override
  State<AlunoUpdatePage> createState() => _AlunoUpdatePageState();
}

class _AlunoUpdatePageState extends State<AlunoUpdatePage> {
  final _formKey = GlobalKey<FormBuilderState>();
  final ApiHandler<Aluno> apiHandler = ApiHandler<Aluno>(
    baseUri: ApiRoutes.entity("aluno"),
    fromJson: (json) => Aluno.fromJson(json),
  );

  bool isLoading = false;

  void _updateData() async {
    if (_formKey.currentState!.saveAndValidate()) {
      setState(() => isLoading = true);

      final data = _formKey.currentState!.value;
      final alunoAtualizado = widget.aluno.copyWith(
        peso: data['peso'] != null
            ? double.tryParse(data['peso'].toString())
            : null,
        faixa: data['faixa'],
        turma: data['turma'] != null
            ? int.tryParse(data['turma'].toString())
            : null,
      );

      try {
        await apiHandler.updateData(
            id: alunoAtualizado.id, item: alunoAtualizado);

        setState(() => isLoading = false);

        if (!mounted) return;
        Navigator.pop(context, alunoAtualizado);
      } catch (e) {
        setState(() => isLoading = false);

        showDialog(
          context: context,
          builder: (context) => AlertDialog(
            backgroundColor: AppTheme.surfaceColor,
            title: const Text(
              "Erro",
              style: TextStyle(color: AppTheme.textColor),
            ),
            content: Text(
              "Não foi possível atualizar o aluno. Detalhes: $e",
              style: const TextStyle(color: AppTheme.textMutedColor),
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.of(context).pop(),
                child: const Text(
                  "Fechar",
                  style: TextStyle(color: AppTheme.accentColor),
                ),
              ),
            ],
          ),
        );
      }
    }
  }

  // ─── Campo somente leitura no padrão AppTheme ─────────────────────────────
  Widget _buildReadOnlyField(String labelText, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        initialValue: value,
        readOnly: true,
        style: const TextStyle(color: AppTheme.textMutedColor, fontSize: 14),
        decoration: InputDecoration(
          labelText: labelText,
          labelStyle: const TextStyle(
            color: AppTheme.textMutedColor,
            fontSize: 14,
          ),
          floatingLabelStyle: const TextStyle(
            color: AppTheme.textMutedColor,
            fontSize: 12,
            fontWeight: FontWeight.w500,
          ),
          filled: true,
          fillColor: AppTheme.surfaceColor.withOpacity(0.5),
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide:
                const BorderSide(color: AppTheme.borderColor, width: 0.5),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide:
                const BorderSide(color: AppTheme.borderColor, width: 0.5),
          ),
          contentPadding:
              const EdgeInsets.symmetric(vertical: 16, horizontal: 16),
        ),
      ),
    );
  }

  // ─── Campo editável no padrão AppTheme ───────────────────────────────────
  Widget _buildTextField({
    required String name,
    required String labelText,
    IconData? icon,
    String? initialValue,
    TextInputType keyboardType = TextInputType.text,
    FormFieldValidator<String>? validator,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: FormBuilderTextField(
        name: name,
        initialValue: initialValue,
        style: const TextStyle(color: AppTheme.textColor, fontSize: 14),
        cursorColor: AppTheme.accentColor,
        keyboardType: keyboardType,
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
          prefixIcon: icon != null
              ? Icon(icon, color: AppTheme.accentColor, size: 20)
              : null,
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
          focusedErrorBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: Colors.redAccent, width: 1),
          ),
          contentPadding:
              const EdgeInsets.symmetric(vertical: 16, horizontal: 16),
        ),
        validator: validator,
      ),
    );
  }

  // ─── Dropdown no padrão AppTheme ─────────────────────────────────────────
  Widget _buildDropdownField({
    required String name,
    required String labelText,
    required IconData icon,
    required List<Map<String, dynamic>> options,
    int? initialValue,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: FormBuilderDropdown<int>(
        name: name,
        initialValue: initialValue,
        style: const TextStyle(color: AppTheme.textColor, fontSize: 14),
        dropdownColor: AppTheme.surfaceColor,
        iconEnabledColor: AppTheme.accentColor,
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
        items: options
            .map((option) => DropdownMenuItem<int>(
                  value: option['value'],
                  child: Text(
                    option['label'],
                    style: const TextStyle(
                        color: AppTheme.textColor, fontSize: 14),
                  ),
                ))
            .toList(),
      ),
    );
  }

  // ─── Separador de seção no padrão AppTheme ────────────────────────────────
  Widget _buildSectionHeader(String title) {
    return Padding(
      padding: const EdgeInsets.only(top: 16, bottom: 4),
      child: Text(
        title,
        style: const TextStyle(
          color: AppTheme.accentColor,
          fontSize: 12,
          fontWeight: FontWeight.w500,
          letterSpacing: 0.8,
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
          "Informações do Aluno",
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
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: FormBuilder(
          key: _formKey,
          initialValue: {
            'peso': widget.aluno.peso != null
                ? widget.aluno.peso!.toStringAsFixed(2)
                : null,
            'faixa': widget.aluno.faixa.toInt(),
            'turma': widget.aluno.turma?.toString(),
          },
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // ── Dados pessoais (somente leitura) ──
              _buildSectionHeader('DADOS PESSOAIS'),
              _buildReadOnlyField('Nome', widget.aluno.nome ?? ''),
              _buildReadOnlyField(
                'Data de Nascimento',
                widget.aluno.dataNascimento != null
                    ? DateFormat('dd/MM/yyyy')
                        .format(widget.aluno.dataNascimento!)
                    : 'Não informado',
              ),

              // ── Dados editáveis ──
              _buildSectionHeader('DADOS EDITÁVEIS'),
              _buildTextField(
                name: 'peso',
                labelText: 'Peso (kg)',
                icon: Icons.monitor_weight_outlined,
                keyboardType:
                    const TextInputType.numberWithOptions(decimal: true),
                validator: (value) =>
                    value == null || value.isEmpty ? 'Campo obrigatório' : null,
              ),
              _buildDropdownField(
                name: 'faixa',
                labelText: 'Faixa',
                icon: Icons.emoji_events_outlined,
                options: Faixa.listaFaixas
                    .map((f) => {'label': f['label'], 'value': f['value']})
                    .toList(),
                initialValue: widget.aluno.faixa.toInt(),
              ),
              _buildTextField(
                name: 'turma',
                labelText: 'Turma',
                icon: Icons.class_outlined,
                keyboardType: TextInputType.number,
                validator: FormBuilderValidators.compose([
                  FormBuilderValidators.required(),
                  FormBuilderValidators.numeric(),
                ]),
              ),

              // ── Contatos e endereços (somente leitura) ──
              _buildSectionHeader('CONTATOS E ENDEREÇOS'),
              _buildReadOnlyField(
                  'Responsável', widget.aluno.responsavel ?? ''),
              _buildReadOnlyField('Celular', widget.aluno.celular ?? ''),
              _buildReadOnlyField('Endereço', widget.aluno.endereco ?? ''),
              _buildReadOnlyField('Bairro', widget.aluno.bairro ?? ''),
              _buildReadOnlyField('Cidade', widget.aluno.cidade ?? ''),

              const SizedBox(height: 8),
            ],
          ),
        ),
      ),
      bottomNavigationBar: Padding(
        padding: const EdgeInsets.fromLTRB(20, 12, 20, 24),
        child: SizedBox(
          height: 52,
          child: ElevatedButton(
            onPressed: isLoading ? null : _updateData,
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
                : const Text('ATUALIZAR', style: AppTheme.buttonTextStyle),
          ),
        ),
      ),
    );
  }
}
