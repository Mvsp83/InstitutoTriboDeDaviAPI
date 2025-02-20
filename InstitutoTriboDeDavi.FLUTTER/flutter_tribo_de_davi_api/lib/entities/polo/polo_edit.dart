import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/polo.dart';
import 'package:form_builder_validators/form_builder_validators.dart';
import 'package:http/http.dart' as http;

class EditPolo extends StatefulWidget {
  final Polo polo;
  const EditPolo({super.key, required this.polo});

  @override
  State<EditPolo> createState() => _EditPoloState();
}

class _EditPoloState extends State<EditPolo> {
  final _formKey = GlobalKey<FormBuilderState>();
  final ApiHandler<Polo> apiHandler = ApiHandler<Polo>(
    baseUri: ApiRoutes.entity("polo"),
    fromJson: (json) => Polo.fromJson(json),
  );

  late http.Response response;
  bool isLoading = false;

  void _updateData() async {
    if (_formKey.currentState!.saveAndValidate()) {
      setState(() => isLoading = true);

      final data = _formKey.currentState!.value;
      final polo = Polo(
        id: widget.polo.id,
        nome: data['nome'],
        informacoes: data['informacoes'],
        endereco: data['endereco'],
        bairro: data['bairro'],
        cidade: data['cidade'],
      );

      response = await apiHandler.updateData(id: widget.polo.id, item: polo);

      setState(() => isLoading = false);

      if (!mounted) return;
      Navigator.pop(context);
    }
  }

  Widget _buildTextField({
    required String name,
    required String labelText,
    required IconData icon,
    String? initialValue,
    FormFieldValidator<String>? validator,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 10),
      child: FormBuilderTextField(
        name: name,
        initialValue: initialValue,
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
        title: const Text("Editar Polo"),
        centerTitle: true,
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
        elevation: 4,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: FormBuilder(
          key: _formKey,
          initialValue: {
            'nome': widget.polo.nome,
            'informacoes': widget.polo.informacoes,
            'endereco': widget.polo.endereco,
            'bairro': widget.polo.bairro,
            'cidade': widget.polo.cidade,
          },
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
          onPressed: isLoading ? null : _updateData,
          style: AppTheme.elevatedButtonStyle.copyWith(
            padding: WidgetStateProperty.all(
              const EdgeInsets.symmetric(vertical: 20),
            ),
          ),
          child: isLoading
              ? const CircularProgressIndicator(color: Colors.white)
              : const Text(
                  'Atualizar',
                  style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                      color: AppTheme.textColor),
                ),
        ),
      ),
    );
  }
}
