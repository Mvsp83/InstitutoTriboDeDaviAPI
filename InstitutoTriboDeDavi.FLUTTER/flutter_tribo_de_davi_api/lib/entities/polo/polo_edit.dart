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
    IconData? icon,
    String? initialValue,
    FormFieldValidator<String>? validator,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: FormBuilderTextField(
        name: name,
        initialValue: initialValue,
        style: const TextStyle(color: AppTheme.textColor, fontSize: 14),
        cursorColor: AppTheme.accentColor,
        decoration: InputDecoration(
          labelText: labelText,
          // Label recolhido (campo vazio)
          labelStyle: const TextStyle(
            color: AppTheme.textMutedColor,
            fontSize: 14,
          ),
          // Label flutuante (campo em foco ou preenchido) — âmbar, sem sobreposição
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
          // Borda padrão
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide:
                const BorderSide(color: AppTheme.borderColor, width: 0.5),
          ),
          // Borda em foco — destaca em âmbar
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: AppTheme.accentColor, width: 1),
          ),
          // Borda de erro
          errorBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: Colors.redAccent, width: 0.5),
          ),
          focusedErrorBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: Colors.redAccent, width: 1),
          ),
          // Padding interno — garante espaço para o label flutuar sem sobrepor
          contentPadding:
              const EdgeInsets.symmetric(vertical: 16, horizontal: 16),
        ),
        validator: validator,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.primaryColor,
      appBar: AppBar(
        title: const Text(
          "Informações do Polo",
          style: TextStyle(fontSize: 16, fontWeight: FontWeight.w500),
        ),
        centerTitle: true,
        backgroundColor: AppTheme.surfaceColor,
        foregroundColor: AppTheme.textColor,
        elevation: 0,
        // Linha âmbar sutil na base da AppBar
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
                icon: Icons.account_balance_outlined,
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
                icon: Icons.location_on_outlined,
              ),
              _buildTextField(
                name: 'bairro',
                labelText: 'Bairro',
                icon: Icons.home_outlined,
              ),
              _buildTextField(
                name: 'cidade',
                labelText: 'Cidade',
                icon: Icons.location_city_outlined,
              ),
              const SizedBox(height: 8),
            ],
          ),
        ),
      ),
    );
  }
}
