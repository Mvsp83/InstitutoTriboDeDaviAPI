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
            title: const Text("Erro"),
            content: Text("Não foi possível atualizar o aluno. Detalhes: $e"),
            actions: [
              TextButton(
                onPressed: () => Navigator.of(context).pop(),
                child: const Text("Fechar"),
              ),
            ],
          ),
        );
      }
    }
  }

  Widget _buildReadOnlyField(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 10),
      child: TextFormField(
        initialValue: value,
        readOnly: true,
        decoration: InputDecoration(
          labelText: label,
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
          filled: true,
          fillColor: Colors.grey.shade200,
        ),
      ),
    );
  }

  Widget _buildEditableField({
    required String name,
    required String label,
    required IconData icon,
    dynamic initialValue,
    required TextInputType keyboardType,
    FormFieldValidator<String>? validator,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 10),
      child: FormBuilderTextField(
        name: name,
        initialValue: initialValue?.toString(),
        decoration: InputDecoration(
          labelText: label,
          prefixIcon: Icon(icon),
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
        ),
        keyboardType: keyboardType,
        validator: validator,
      ),
    );
  }

  Widget _buildEditableFieldDouble({
    required String name,
    required String label,
    required IconData icon,
    double? initialValue,
    required TextInputType keyboardType,
    FormFieldValidator<String>? validator,
    required void Function(double?) onChanged,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 10),
      child: FormBuilderTextField(
        name: name,
        initialValue:
            initialValue != null ? initialValue.toStringAsFixed(2) : null,
        decoration: InputDecoration(
          labelText: label,
          prefixIcon: Icon(icon),
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
        ),
        keyboardType: keyboardType,
        validator: validator,
        onChanged: (value) {
          double? parsedValue = double.tryParse(value ?? '');
          onChanged(parsedValue);
        },
      ),
    );
  }

  Widget _buildDropdownField({
    required String name,
    required String label,
    required List<Map<String, dynamic>> options,
    int? initialValue,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 10),
      child: FormBuilderDropdown<int>(
        name: name,
        initialValue: initialValue,
        decoration: InputDecoration(
          labelText: label,
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
        ),
        items: options
            .map((option) => DropdownMenuItem<int>(
                  value: option['value'],
                  child: Text(option['label']),
                ))
            .toList(),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Editar Aluno'),
        backgroundColor: AppTheme.primaryColor,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: FormBuilder(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                'Informações do Aluno',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 10),
              _buildReadOnlyField('Nome', widget.aluno.nome ?? ''),
              _buildReadOnlyField(
                'Data de Nascimento',
                widget.aluno.dataNascimento != null
                    ? DateFormat('dd/MM/yyyy')
                        .format(widget.aluno.dataNascimento!)
                    : 'Não informado',
              ),
              _buildEditableFieldDouble(
                name: 'peso',
                label: 'Peso',
                icon: Icons.monitor_weight,
                initialValue: widget.aluno.peso?.toDouble(),
                keyboardType: TextInputType.numberWithOptions(decimal: true),
                validator: (value) =>
                    value!.isEmpty ? 'Campo obrigatório' : null,
                onChanged: (value) {
                  print('Novo valor: $value');
                },
              ),
              _buildDropdownField(
                name: 'faixa',
                label: 'Faixa',
                options: Faixa.listaFaixas
                    .map((faixa) => {
                          'label': faixa['label'],
                          'value':
                              faixa['value'], // Usa o ID da faixa como value
                        })
                    .toList(),
                initialValue: widget.aluno.faixa.toInt(),
              ),
              _buildEditableField(
                name: 'turma',
                label: 'Turma',
                icon: Icons.class_,
                initialValue: widget.aluno.turma,
                keyboardType: TextInputType.number,
                validator: FormBuilderValidators.compose([
                  FormBuilderValidators.required(),
                  FormBuilderValidators.numeric(),
                ]),
              ),
              const SizedBox(height: 20),
              const Text(
                'Contatos e Endereços',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 10),
              _buildReadOnlyField(
                  'Responsável', widget.aluno.responsavel ?? ''),
              _buildReadOnlyField('Celular', widget.aluno.celular ?? ''),
              _buildReadOnlyField('Endereço', widget.aluno.endereco ?? ''),
              _buildReadOnlyField('Bairro', widget.aluno.bairro ?? ''),
              _buildReadOnlyField('Cidade', widget.aluno.cidade ?? ''),
            ],
          ),
        ),
      ),
      bottomNavigationBar: Padding(
        padding: const EdgeInsets.all(16),
        child: ElevatedButton(
          onPressed: isLoading ? null : _updateData,
          style: ElevatedButton.styleFrom(
            backgroundColor: AppTheme.primaryColor,
            padding: const EdgeInsets.symmetric(vertical: 16),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(12),
            ),
          ),
          child: isLoading
              ? const CircularProgressIndicator(color: Colors.white)
              : const Text('Atualizar', style: TextStyle(fontSize: 18)),
        ),
      ),
    );
  }
}
