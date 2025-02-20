import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
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

  void _login() async {
    String login = _loginController.text;
    String password = _passwordController.text;

    final apiHandler = ApiHandler(
      baseUri: '',
      fromJson: (json) => json,
    );

    final response = await apiHandler.login(login: login, password: password);

    if (response != null &&
        response['data'] != null &&
        response['data'].containsKey('token')) {
      final token = response['data']['token'];

      await apiHandler.saveToken(token);

      Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (context) => HomePage(userName: login),
        ),
      );
    } else {
      showDialog(
        context: context,
        builder: (context) => AlertDialog(
          title: const Text("Login Falhou"),
          content: const Text("Credenciais inválidas, tente novamente."),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop();
              },
              child: const Text("OK"),
            ),
          ],
        ),
      );
    }
  }

  void _showAboutDialog() {
    showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          backgroundColor: AppTheme.primaryColor,
          title: Text(
            textAlign: TextAlign.center,
            "Sobre o Aplicativo",
            style: AppTheme.bodyTextStyle.copyWith(
              color: AppTheme.textColor,
            ),
          ),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              const SizedBox(height: 20),
              Text(
                "Desenvolvido por:\n",
                style: AppTheme.bodyTextStyle.copyWith(
                  color: AppTheme.textColor,
                ),
              ), // Imagem
              Image.asset(
                'lib/assets/images/emeve83-2.png',
                height: 100,
              ),
              const SizedBox(height: 20),
              Text(
                "Versão:\n v.1.0",
                style: AppTheme.bodyTextStyle.copyWith(
                  color: AppTheme.textColor,
                ),
              ),
              const SizedBox(height: 20),
              Text(
                textAlign: TextAlign.center,
                "Ferramentas Utilizadas:\n - Flutter\n - API em C#",
                style: AppTheme.bodyTextStyle.copyWith(
                  color: AppTheme.textColor,
                ),
              ),
              const SizedBox(height: 20),
              Text(
                "Contato:",
                style: AppTheme.bodyTextStyle.copyWith(
                  color: AppTheme.textColor,
                ),
              ),
              Text(
                "contato@empresa.com",
                style:
                    AppTheme.bodyTextStyle.copyWith(color: AppTheme.textColor),
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop();
              },
              child: Text(
                "Fechar",
                style: AppTheme.bodyTextStyle.copyWith(
                  color: AppTheme.textColor,
                ),
              ),
            ),
          ],
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        backgroundColor: AppTheme.primaryColor,
        body: SingleChildScrollView(
          child: Padding(
            padding:
                const EdgeInsets.symmetric(horizontal: 30.0, vertical: 80.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                Center(
                  child: Image.asset(
                    'lib/assets/images/logo.png',
                    height: 150,
                  ),
                ),
                const SizedBox(height: 10),
                TextField(
                  controller: _loginController,
                  decoration: InputDecoration(
                    labelText: 'Email',
                    prefixIcon: const Icon(Icons.email),
                    filled: true,
                    fillColor: Colors.white,
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(30),
                      borderSide: BorderSide.none,
                    ),
                    contentPadding: const EdgeInsets.symmetric(
                        vertical: 20, horizontal: 20),
                  ),
                  keyboardType: TextInputType.emailAddress,
                ),
                const SizedBox(height: 20),
                TextField(
                  controller: _passwordController,
                  obscureText: !_isPasswordVisible,
                  decoration: InputDecoration(
                    labelText: 'Password',
                    prefixIcon: const Icon(Icons.lock),
                    suffixIcon: IconButton(
                      icon: Icon(
                        _isPasswordVisible
                            ? Icons.visibility
                            : Icons.visibility_off,
                      ),
                      onPressed: () {
                        setState(() {
                          _isPasswordVisible = !_isPasswordVisible;
                        });
                      },
                    ),
                    filled: true,
                    fillColor: Colors.white,
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(30),
                      borderSide: BorderSide.none,
                    ),
                    contentPadding: const EdgeInsets.symmetric(
                        vertical: 20, horizontal: 20),
                  ),
                ),
                const SizedBox(height: 20),
                Align(
                  alignment: Alignment.centerRight,
                  child: Text(
                    'Forgot Password?',
                    style: AppTheme.bodyTextStyle.copyWith(
                      color: AppTheme.textColor,
                    ),
                  ),
                ),
                const SizedBox(height: 40),
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    onPressed: _login,
                    style: AppTheme.elevatedButtonStyle,
                    child: const Text(
                      'Login',
                      style: AppTheme.buttonTextStyle,
                    ),
                  ),
                ),
                const SizedBox(height: 20),
              ],
            ),
          ),
        ),
        bottomNavigationBar: BottomAppBar(
          color: AppTheme.primaryColor,
          child: Padding(
            padding: const EdgeInsets.all(0.0),
            child: Column(mainAxisSize: MainAxisSize.min, children: [
              GestureDetector(
                onTap: _showAboutDialog,
                child: Text(
                  '© ${DateTime.now().year} - eMeVe83 Tech',
                  style: AppTheme.bodyTextStyle.copyWith(
                    color: AppTheme.textColor,
                    fontSize: 14,
                    fontWeight: FontWeight.w400,
                  ),
                  textAlign: TextAlign.center,
                ),
              ),
            ]),
          ),
        ));
  }
}
