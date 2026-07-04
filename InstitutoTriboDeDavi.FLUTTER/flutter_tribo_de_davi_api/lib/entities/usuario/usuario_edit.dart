import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/usuario.dart';
import 'package:form_builder_validators/form_builder_validators.dart';
import 'package:http/http.dart' as http;

class EditUsuario extends StatefulWidget {
  final Usuario usuario;
  const EditUsuario({super.key, required this.usuario});

  @override
  State<EditUsuario> createState() => _EditUsuarioState();
}

class _EditUsuarioState extends State<EditUsuario> {
  final _formKey = GlobalKey<FormBuilderState>();
  final ApiHandler<Usuario> apiHandler = ApiHandler<Usuario>(
    baseUri: ApiRoutes.entity("usuario"),
    fromJson: (json) => Usuario.fromJson(json),
  );

  late http.Response response;
  bool isLoading = false;

  void _updateData() async {
    if (_formKey.currentState!.saveAndValidate()) {
      setState(() => isLoading = true);

      final data = _formKey.currentState!.value;
      final usuario = Usuario(
        id: widget.usuario.id,
        email: data['email'],
        login: data['login'],
        password: data['password'] ?? '',
      );

      response =
          await apiHandler.updateData(id: widget.usuario.id, item: usuario);

      setState(() => isLoading = false);

      if (!mounted) return;
      Navigator.pop(context);
    }
  }

  Widget _buildTextField({
    required String name,
    required String labelText,
    required IconData icon,
    bool obscureText = false,
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
        obscureText: obscureText,
        validator: validator,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: const Text("Editar Usuário"),
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
            'email': widget.usuario.email,
            'login': widget.usuario.login,
          },
          child: Column(
            children: [
              _buildTextField(
                name: 'email',
                labelText: 'Email',
                icon: Icons.email,
                validator: FormBuilderValidators.compose([
                  FormBuilderValidators.required(),
                  FormBuilderValidators.email(),
                ]),
              ),
              _buildTextField(
                name: 'login',
                labelText: 'Login',
                icon: Icons.person,
                validator: FormBuilderValidators.required(),
              ),
              _buildTextField(
                name: 'password',
                labelText: 'Nova senha (deixe em branco para manter a atual)',
                icon: Icons.lock,
                obscureText: true,
                validator: FormBuilderValidators.conditional(
                  (value) => value != null && value.toString().isNotEmpty,
                  FormBuilderValidators.minLength(3),
                ),
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
