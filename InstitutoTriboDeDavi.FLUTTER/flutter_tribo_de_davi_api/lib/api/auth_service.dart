import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_tribo_de_davi_api/views/login.dart';
import 'package:jwt_decode/jwt_decode.dart';

/// Lançada quando a sessão expirou e o usuário precisa autenticar de novo.
class SessionExpiredException implements Exception {
  @override
  String toString() => 'Sessão expirada. Faça login novamente.';
}

/// Centraliza a sessão do app: armazenamento do token, leitura de claims,
/// verificação de expiração e o redirecionamento para o login.
class AuthService {
  static const _tokenKey = 'auth_token';

  // Armazenamento criptografado (Keystore/Keychain) — o token não fica
  // mais em texto puro como ficava no SharedPreferences
  static const _storage = FlutterSecureStorage();

  /// Registrados no MaterialApp (main.dart) para permitir navegação e
  /// snackbar globais quando a sessão expira no meio de uma requisição.
  static final GlobalKey<NavigatorState> navigatorKey =
      GlobalKey<NavigatorState>();
  static final GlobalKey<ScaffoldMessengerState> scaffoldMessengerKey =
      GlobalKey<ScaffoldMessengerState>();

  // Evita múltiplos redirecionamentos quando várias requisições
  // falham ao mesmo tempo com sessão expirada.
  static bool _redirecionandoParaLogin = false;

  static Future<void> saveToken(String token) async {
    await _storage.write(key: _tokenKey, value: token);
    _redirecionandoParaLogin = false;
  }

  static Future<String?> getToken() => _storage.read(key: _tokenKey);

  static Future<void> removeToken() => _storage.delete(key: _tokenKey);

  /// Existe token salvo e ele ainda não expirou?
  static Future<bool> hasValidToken() async {
    final token = await getToken();
    if (token == null) return false;
    try {
      return !Jwt.isExpired(token);
    } catch (_) {
      return false; // token malformado conta como inválido
    }
  }

  /// Lê um claim do token atual (ex.: 'PoloId', 'PoloNome', 'unique_name').
  static Future<String?> getClaim(String claim) async {
    final token = await getToken();
    if (token == null) return null;
    try {
      final payload = Jwt.parseJwt(token);
      return payload[claim]?.toString();
    } catch (_) {
      return null;
    }
  }

  static Future<String?> getPoloName() => getClaim('PoloNome');

  // ClaimTypes.Name da API é serializado como 'unique_name' no JWT
  static Future<String?> getUserName() => getClaim('unique_name');

  // ClaimTypes.Role da API é serializado como 'role' no JWT
  static Future<String?> getRole() => getClaim('role');

  static Future<bool> isAdministrador() async =>
      (await getRole()) == 'Administrador';

  /// Retorna o login do usuário se existe sessão válida; null caso contrário.
  /// Usado na inicialização do app para pular a tela de login.
  static Future<String?> restoreSession() async {
    if (!await hasValidToken()) return null;
    return await getUserName() ?? '';
  }

  /// Logout manual: apaga o token e volta ao login limpando a pilha.
  static Future<void> logout() async {
    await removeToken();
    _irParaLogin();
  }

  /// Chamado quando uma requisição detecta token ausente/expirado ou 401.
  static Future<void> handleSessionExpired() async {
    if (_redirecionandoParaLogin) return;
    _redirecionandoParaLogin = true;

    await removeToken();
    scaffoldMessengerKey.currentState?.showSnackBar(
      const SnackBar(content: Text('Sessão expirada. Faça login novamente.')),
    );
    _irParaLogin();
  }

  static void _irParaLogin() {
    navigatorKey.currentState?.pushAndRemoveUntil(
      MaterialPageRoute(builder: (_) => const LoginPage()),
      (route) => false,
    );
  }
}
