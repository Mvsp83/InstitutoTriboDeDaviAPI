import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/polo.dart';
import 'package:form_builder_validators/form_builder_validators.dart';

class AddPolo extends StatefulWidget {
  const AddPolo({super.key});

  @override
  State<AddPolo> createState() => _AddPoloState();
}

class _AddPoloState extends State<AddPolo> {
  final _formKey = GlobalKey<FormBuilderState>();
  final ApiHandler<Polo> apiHandler = ApiHandler<Polo>(
    baseUri: ApiRoutes.entity("polo"),
    fromJson: (json) => Polo.fromJson(json),
  );

  bool isLoading = false;

  void _addPolo() async {
    if (_formKey.currentState!.saveAndValidate()) {
      setState(() => isLoading = true);

      final data = _formKey.currentState!.value;
      final polo = Polo(
        id: 0,
        nome: data['nome'],
        informacoes: data['informacoes'],
        endereco: data['endereco'],
        bairro: data['bairro'],
        cidade: data['cidade'],
      );

      try {
        await apiHandler.addData(item: polo);
        if (!mounted) return;
        Navigator.pop(context);
      } catch (e) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text("Erro ao adicionar polo: $e")),
        );
      } finally {
        setState(() => isLoading = false);
      }
    }
  }

  Widget _buildTextField({
    required String name,
    required String labelText,
    required IconData icon,
    FormFieldValidator<String>? validator,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 10),
      child: FormBuilderTextField(
        name: name,
        decoration: InputDecoration(
          labelText: labelText,
          labelStyle: const TextStyle(color: Colors.black54),
          floatingLabelStyle: const TextStyle(
            color: AppTheme.primaryColor,
            fontWeight: FontWeight.bold,
          ),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          prefixIcon: Icon(icon, color: AppTheme.primaryColor),
          filled: true,
          fillColor: Colors.white,
        ),
        validator: validator,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: const Text("Adicionar Polo"),
        centerTitle: true,
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
        elevation: 4,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: FormBuilder(
          key: _formKey,
          child: Column(
            children: [
              _buildTextField(
                name: 'nome',
                labelText: 'Nome do Polo',
                icon: Icons.account_balance,
                validator: FormBuilderValidators.required(),
              ),
              _buildTextField(
                name: 'informacoes',
                labelText: 'Informações',
                icon: Icons.info_outline,
              ),
              _buildTextField(
                name: 'endereco',
                labelText: 'Endereço',
                icon: Icons.location_on,
              ),
              _buildTextField(
                name: 'bairro',
                labelText: 'Bairro',
                icon: Icons.home,
              ),
              _buildTextField(
                name: 'cidade',
                labelText: 'Cidade',
                icon: Icons.location_city,
              ),
            ],
          ),
        ),
      ),
      bottomNavigationBar: Padding(
        padding: const EdgeInsets.all(16),
        child: ElevatedButton(
          onPressed: isLoading ? null : _addPolo,
          style: AppTheme.elevatedButtonStyle.copyWith(
            padding: WidgetStateProperty.all(
              const EdgeInsets.symmetric(vertical: 20),
            ),
          ),
          child: isLoading
              ? const CircularProgressIndicator(color: Colors.white)
              : const Text(
                  'Salvar',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: AppTheme.textColor,
                  ),
                ),
        ),
      ),
    );
  }
}
