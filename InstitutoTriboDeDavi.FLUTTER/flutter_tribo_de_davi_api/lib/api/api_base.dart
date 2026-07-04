import 'dart:convert';
import 'package:flutter_tribo_de_davi_api/api/auth_service.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';

typedef FromJson<T> = T Function(Map<String, dynamic> json);

/// Erro vindo da API, já com a mensagem apropriada para exibir ao usuário
/// (extraída do envelope ResultViewModel ou derivada do status HTTP).
class ApiException implements Exception {
  final String message;
  final int? statusCode;

  ApiException(this.message, {this.statusCode});

  @override
  String toString() => message;
}

class ApiHandler<T> {
  final String baseUri;
  final FromJson<T> fromJson;

  ApiHandler({required this.baseUri, required this.fromJson});

  static const _erroConexao =
      'Não foi possível conectar ao servidor. Verifique sua conexão.';

  /// Monta os headers autenticados. Se o token não existe ou expirou,
  /// dispara o redirecionamento para o login e aborta a requisição.
  Future<Map<String, String>> _getHeaders() async {
    if (!await AuthService.hasValidToken()) {
      await AuthService.handleSessionExpired();
      throw SessionExpiredException();
    }

    final token = await AuthService.getToken();
    return {
      'Content-Type': 'application/json; charset=UTF-8',
      'Authorization': 'Bearer $token',
    };
  }

  Future<Map<String, String>> getHeaders() => _getHeaders();

  /// Se o servidor respondeu 401, a sessão expirou: dispara o
  /// redirecionamento para o login e interrompe o fluxo da requisição.
  Future<void> _verificarSessao(http.Response response) async {
    if (response.statusCode == 401) {
      await AuthService.handleSessionExpired();
      throw SessionExpiredException();
    }
  }

  /// Extrai a mensagem do envelope ResultViewModel ({message, success, data})
  /// da API; usa mensagens padrão por status quando o corpo não tem mensagem
  /// (ex.: 403 do framework vem com corpo vazio).
  static String mensagemDoServidor(http.Response response) {
    try {
      final body = json.decode(response.body);
      final message = body['message'];
      if (message is String && message.isNotEmpty) return message;
    } catch (_) {
      // corpo vazio ou não-JSON
    }

    switch (response.statusCode) {
      case 403:
        return 'Você não tem permissão para acessar esta informação.';
      case 404:
        return 'Informação não encontrada no servidor.';
      default:
        return 'Erro no servidor (código ${response.statusCode}).';
    }
  }

  /// GET comum a todas as listagens: desembrulha o envelope e converte a
  /// lista. Lança [ApiException] com a mensagem da API em qualquer falha —
  /// lista vazia (data nulo) é retorno normal, não erro.
  Future<List<T>> _getLista(Uri uri) async {
    final headers = await _getHeaders();

    final http.Response response;
    try {
      response = await http.get(uri, headers: headers);
    } catch (_) {
      throw ApiException(_erroConexao);
    }

    await _verificarSessao(response);

    if (response.statusCode < 200 || response.statusCode > 299) {
      throw ApiException(mensagemDoServidor(response),
          statusCode: response.statusCode);
    }

    final jsonData = json.decode(response.body);
    final List<dynamic> items = jsonData['data'] ?? [];
    return items.map((json) => fromJson(json)).toList();
  }

  Future<List<T>> getData() =>
      _getLista(Uri.parse(ApiRoutes.getDataPorPolo(baseUri, [1, 2, 3, 4, 5])));

  Future<List<T>> getDataAll() =>
      _getLista(Uri.parse(ApiRoutes.getAll(baseUri)));

  Future<List<T>> getDataPorPolo(List<int> turma) =>
      _getLista(Uri.parse(ApiRoutes.getDataPorPolo(baseUri, turma)));

  Future<List<T>> getDataUrl() =>
      _getLista(Uri.parse(ApiRoutes.getAllUrl(baseUri)));

  Future<List<T>> getDataListById(int aulaId) =>
      _getLista(Uri.parse('${ApiRoutes.baseUrl}/presenca/aula/$aulaId'));

  Future<http.Response> updateData({
    required int id,
    required T item,
  }) async {
    final uri = Uri.parse(ApiRoutes.update(baseUri));
    final headers = await _getHeaders();

    final http.Response response;
    try {
      response = await http.put(uri, headers: headers, body: json.encode(item));
    } catch (_) {
      throw ApiException(_erroConexao);
    }

    await _verificarSessao(response);
    return response;
  }

  Future<http.Response> addData({required T item}) async {
    final uri = Uri.parse(ApiRoutes.create(baseUri));
    final headers = await _getHeaders();

    final http.Response response;
    try {
      response =
          await http.post(uri, headers: headers, body: json.encode(item));
    } catch (_) {
      throw ApiException(_erroConexao);
    }

    await _verificarSessao(response);
    return response;
  }

  Future<http.Response> deleteData({required int id}) async {
    final uri = Uri.parse(ApiRoutes.delete(baseUri, id));
    final headers = await _getHeaders();

    final http.Response response;
    try {
      response = await http.delete(uri, headers: headers);
    } catch (_) {
      throw ApiException(_erroConexao);
    }

    await _verificarSessao(response);
    return response;
  }

  Future<http.Response> post({
    required String endpoint,
    required List<Map<String, dynamic>> body,
  }) async {
    final uri = Uri.parse('$baseUri/$endpoint');
    final headers = await _getHeaders();

    final http.Response response;
    try {
      response =
          await http.post(uri, headers: headers, body: json.encode(body));
    } catch (_) {
      throw ApiException(_erroConexao);
    }

    await _verificarSessao(response);

    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(mensagemDoServidor(response),
          statusCode: response.statusCode);
    }

    return response;
  }

  Future<http.Response> put({
    required String endpoint,
    required Map<String, dynamic> body,
  }) async {
    final uri = Uri.parse(ApiRoutes.update(baseUri));
    final headers = await _getHeaders();

    final http.Response response;
    try {
      response = await http.put(uri, headers: headers, body: json.encode(body));
    } catch (_) {
      throw ApiException(_erroConexao);
    }

    await _verificarSessao(response);
    return response;
  }

  /// Autentica na API. Retorna o envelope decodificado (com token em 'data'
  /// no sucesso, ou 'message' explicando a falha); null se não houve conexão.
  Future<Map<String, dynamic>?> login({
    required String login,
    required String password,
  }) async {
    final uri = Uri.parse('${ApiRoutes.baseUrl}/v1/auth/login');

    try {
      final response = await http.post(
        uri,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: json.encode({'login': login, 'password': password}),
      );

      return json.decode(response.body) as Map<String, dynamic>;
    } catch (_) {
      return null; // sem conexão ou resposta sem corpo JSON
    }
  }
}
