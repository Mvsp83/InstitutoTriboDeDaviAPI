import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/auth_service.dart';
import '../api/api_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/views/inicial.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  LoginPageState createState() => LoginPageState();
}

class LoginPageState extends State<LoginPage> {
  final TextEditingController _loginController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();

  bool _isPasswordVisible = false;
  bool _isLoading = false;

  void _login() async {
    String login = _loginController.text;
    String password = _passwordController.text;

    if (!_validateInputs(login, password)) {
      _showErrorDialog("Campos inválidos",
          "Por favor, preencha todos os campos corretamente.");
      return;
    }

    setState(() => _isLoading = true);

    try {
      final apiHandler = ApiHandler(
        baseUri: '',
        fromJson: (json) => json,
      );

      final response = await apiHandler.login(login: login, password: password);

      if (response != null &&
          response['data'] != null &&
          response['data'].containsKey('token')) {
        final token = response['data']['token'];
        await AuthService.saveToken(token);

        Navigator.pushReplacement(
          context,
          MaterialPageRoute(
            builder: (context) => HomePage(userName: login),
          ),
        );
      } else if (response == null) {
        _showErrorDialog("Sem conexão",
            "Não foi possível conectar ao servidor. Verifique sua conexão.");
      } else {
        // Mostra a mensagem enviada pela API (ex.: combinação incorreta)
        _showErrorDialog(
            "Login Falhou",
            (response['message'] as String?) ??
                "Credenciais inválidas, tente novamente.");
      }
    } catch (e) {
      _showErrorDialog("Erro",
          "Ocorreu um erro ao tentar fazer login. Por favor, tente novamente.");
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  bool _validateInputs(String login, String password) {
    return login.isNotEmpty && password.isNotEmpty;
  }

  void _showErrorDialog(String title, String message) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        backgroundColor: AppTheme.surfaceColor,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(16),
          side: const BorderSide(color: AppTheme.borderColor),
        ),
        title:
            Text(title, style: AppTheme.headerTextStyle.copyWith(fontSize: 18)),
        content: Text(message, style: AppTheme.bodyTextStyle),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child:
                const Text("OK", style: TextStyle(color: AppTheme.accentColor)),
          ),
        ],
      ),
    );
  }

  void _showAboutDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        backgroundColor: AppTheme.surfaceColor,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(16),
          side: const BorderSide(color: AppTheme.borderColor),
        ),
        title: Text(
          "Sobre o Aplicativo",
          textAlign: TextAlign.center,
          style: AppTheme.headerTextStyle.copyWith(fontSize: 18),
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            const SizedBox(height: 16),
            const Text("Desenvolvido por:", style: AppTheme.bodyTextStyle),
            const SizedBox(height: 12),
            Image.asset('lib/assets/images/emeve83-2.png', height: 80),
            const SizedBox(height: 16),
            const Text("Versão: v.1.0.1", style: AppTheme.bodyTextStyle),
            const SizedBox(height: 12),
            const Text("Dúvidas e sugestões:", style: AppTheme.bodyTextStyle),
            const Text(
              "marcusviniciussp.dev@gmail.com",
              style: TextStyle(color: AppTheme.textColor, fontSize: 12),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text("Fechar",
                style: TextStyle(color: AppTheme.accentColor)),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.primaryColor,
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 28.0, vertical: 48.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Logo + linha de acento âmbar
              Center(
                child: Column(
                  children: [
                    Image.asset('lib/assets/images/logo.png', height: 130),
                    const SizedBox(height: 12),
                    Container(
                      width: 40,
                      height: 2,
                      decoration: BoxDecoration(
                        color: AppTheme.accentColor,
                        borderRadius: BorderRadius.circular(2),
                      ),
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 48),

              // Campo e-mail
              TextField(
                controller: _loginController,
                keyboardType: TextInputType.emailAddress,
                style: const TextStyle(color: AppTheme.textColor, fontSize: 14),
                cursorColor: AppTheme.accentColor,
                decoration: AppTheme.inputDecoration(
                  label: 'E-mail',
                  prefixIcon: Icons.email_outlined,
                ),
              ),

              const SizedBox(height: 14),

              // Campo senha
              TextField(
                controller: _passwordController,
                obscureText: !_isPasswordVisible,
                style: const TextStyle(color: AppTheme.textColor, fontSize: 14),
                cursorColor: AppTheme.accentColor,
                decoration: AppTheme.inputDecoration(
                  label: 'Senha',
                  prefixIcon: Icons.lock_outline,
                  suffixIcon: IconButton(
                    icon: Icon(
                      _isPasswordVisible
                          ? Icons.visibility_outlined
                          : Icons.visibility_off_outlined,
                      color: AppTheme.textMutedColor,
                      size: 20,
                    ),
                    onPressed: () => setState(
                        () => _isPasswordVisible = !_isPasswordVisible),
                  ),
                ),
              ),

              const SizedBox(height: 14),

              // Esqueci a senha
              Align(
                alignment: Alignment.centerRight,
                child: GestureDetector(
                  onTap: () {
                    // TODO: navegação para recuperação de senha
                  },
                  child: const Text('Esqueci a senha',
                      style: AppTheme.accentTextStyle),
                ),
              ),

              const SizedBox(height: 36),

              // Botão entrar
              SizedBox(
                width: double.infinity,
                height: 52,
                child: ElevatedButton(
                  onPressed: _isLoading ? null : _login,
                  style: AppTheme.elevatedButtonStyle,
                  child: _isLoading
                      ? const SizedBox(
                          width: 20,
                          height: 20,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                            color: AppTheme.primaryColor,
                          ),
                        )
                      : const Text('ENTRAR', style: AppTheme.buttonTextStyle),
                ),
              ),

              const SizedBox(height: 24),
            ],
          ),
        ),
      ),
      bottomNavigationBar: BottomAppBar(
        color: AppTheme.primaryColor,
        elevation: 0,
        child: GestureDetector(
          onTap: _showAboutDialog,
          child: Text(
            '${DateTime.now().year} · eMeVe ©',
            style: const TextStyle(color: AppTheme.textColor, fontSize: 12),
            textAlign: TextAlign.center,
          ),
        ),
      ),
    );
  }
}
