// import 'package:flutter/material.dart';
// import 'package:flutter_form_builder/flutter_form_builder.dart';
// import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
// import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
// import 'package:flutter_tribo_de_davi_api/entities/models/usuario.dart';
// import 'package:form_builder_validators/form_builder_validators.dart';
// import 'package:http/http.dart' as http;

// class AddUsuario extends StatefulWidget {
//   const AddUsuario({super.key});

//   @override
//   State<AddUsuario> createState() => _AddUsuarioState();
// }

// class _AddUsuarioState extends State<AddUsuario> {
//   final _formKey = GlobalKey<FormBuilderState>();
//   ApiHandler<Usuario> apiHandler = ApiHandler<Usuario>(
//     baseUri: ApiRoutes.entity("usuario"),
//     fromJson: (json) => Usuario.fromJson(json),
//   );

//   late http.Response response;

//   void addUsuario() async {
//     if (_formKey.currentState!.saveAndValidate()) {
//       final data = _formKey.currentState!.value;

//       final usuario = Usuario(
//           id: 0,
//           email: data['email'],
//           login: data['login'],
//           password: data['password']);

//       response = await apiHandler.addData(item: usuario);
//     }
//     if (!mounted) return;
//     Navigator.pop(context);
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       appBar: AppBar(
//         title: const Text("Add Usuário"),
//         centerTitle: true,
//         backgroundColor: Colors.black87,
//         foregroundColor: Colors.white,
//       ),
//       bottomNavigationBar: MaterialButton(
//         color: Colors.teal,
//         textColor: Colors.yellow,
//         padding: const EdgeInsets.all(20),
//         onPressed: addUsuario,
//         child: const Text('Add'),
//       ),
//       body: Padding(
//           padding: const EdgeInsets.all(10),
//           child: FormBuilder(
//               key: _formKey,
//               child: Column(
//                 children: [
//                   FormBuilderTextField(
//                     name: 'email',
//                     decoration: const InputDecoration(labelText: 'Email'),
//                     validator: FormBuilderValidators.compose([
//                       FormBuilderValidators.required(),
//                     ]),
//                   ),
//                   const SizedBox(
//                     height: 10,
//                   ),
//                   FormBuilderTextField(
//                     name: 'login',
//                     decoration: const InputDecoration(labelText: 'Login'),
//                     validator: FormBuilderValidators.compose([
//                       FormBuilderValidators.required(),
//                     ]),
//                   ),
//                   const SizedBox(
//                     height: 10,
//                   ),
//                   FormBuilderTextField(
//                     name: 'password',
//                     decoration: const InputDecoration(labelText: 'Password'),
//                     validator: FormBuilderValidators.compose([
//                       FormBuilderValidators.required(),
//                     ]),
//                   ),
//                 ],
//               ))),
//     );
//   }
// }

import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/usuario.dart';
import 'package:form_builder_validators/form_builder_validators.dart';
import 'package:http/http.dart' as http;

class AddUsuario extends StatefulWidget {
  const AddUsuario({super.key});

  @override
  State<AddUsuario> createState() => _AddUsuarioState();
}

class _AddUsuarioState extends State<AddUsuario> {
  final _formKey = GlobalKey<FormBuilderState>();
  ApiHandler<Usuario> apiHandler = ApiHandler<Usuario>(
    baseUri: ApiRoutes.entity("usuario"),
    fromJson: (json) => Usuario.fromJson(json),
  );

  late http.Response response;

  void addUsuario() async {
    if (_formKey.currentState!.saveAndValidate()) {
      final data = _formKey.currentState!.value;

      final usuario = Usuario(
          id: 0,
          email: data['email'],
          login: data['login'],
          password: data['password']);

      response = await apiHandler.addData(item: usuario);
    }
    if (!mounted) return;
    Navigator.pop(context);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Adicionar Usuário"),
        centerTitle: true,
        backgroundColor: Colors.black87,
        foregroundColor: Colors.white,
        elevation: 4,
        shadowColor: Colors.black54,
      ),
      bottomNavigationBar: Padding(
        padding: const EdgeInsets.all(16.0),
        child: ElevatedButton(
          onPressed: addUsuario,
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.blue,
            padding: const EdgeInsets.symmetric(vertical: 20),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(12),
            ),
            elevation: 5,
          ),
          child: const Text(
            'Adicionar',
            style: TextStyle(
              fontSize: 18,
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: FormBuilder(
          key: _formKey,
          child: Column(
            children: [
              FormBuilderTextField(
                name: 'email',
                decoration: InputDecoration(
                  labelText: 'Email',
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  prefixIcon: const Icon(Icons.email_outlined),
                ),
                validator: FormBuilderValidators.compose([
                  FormBuilderValidators.required(),
                  FormBuilderValidators.email(),
                ]),
              ),
              const SizedBox(height: 20),
              FormBuilderTextField(
                name: 'login',
                decoration: InputDecoration(
                  labelText: 'Login',
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  prefixIcon: const Icon(Icons.person_outline),
                ),
                validator: FormBuilderValidators.compose([
                  FormBuilderValidators.required(),
                ]),
              ),
              const SizedBox(height: 20),
              FormBuilderTextField(
                name: 'password',
                obscureText: true,
                decoration: InputDecoration(
                  labelText: 'Senha',
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  prefixIcon: const Icon(Icons.lock_outline),
                ),
                validator: FormBuilderValidators.compose([
                  FormBuilderValidators.required(),
                  FormBuilderValidators.minLength(6),
                ]),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
