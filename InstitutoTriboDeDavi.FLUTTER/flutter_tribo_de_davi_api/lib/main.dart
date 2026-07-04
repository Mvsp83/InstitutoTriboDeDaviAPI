import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/api/auth_service.dart';
import 'package:flutter_tribo_de_davi_api/views/inicial.dart';
import 'package:flutter_tribo_de_davi_api/views/login.dart';

void main() {
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      // Chaves globais usadas pelo AuthService para redirecionar ao login
      // e mostrar aviso quando a sessão expira no meio de uma requisição
      navigatorKey: AuthService.navigatorKey,
      scaffoldMessengerKey: AuthService.scaffoldMessengerKey,
      home: const SessionGate(),
    );
  }
}

/// Decide a tela inicial: se já existe sessão válida (token não expirado),
/// vai direto para a home; senão, para o login.
class SessionGate extends StatelessWidget {
  const SessionGate({super.key});

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<String?>(
      future: AuthService.restoreSession(),
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return const Scaffold(
            backgroundColor: AppTheme.primaryColor,
            body: Center(
              child: CircularProgressIndicator(color: AppTheme.accentColor),
            ),
          );
        }

        final userName = snapshot.data;
        return userName == null
            ? const LoginPage()
            : HomePage(userName: userName);
      },
    );
  }
}
