import 'dart:convert';
import 'package:flutter_tribo_de_davi_api/entities/models/presenca.dart';
import 'package:jwt_decode/jwt_decode.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';

typedef FromJson<T> = T Function(Map<String, dynamic> json);

class ApiHandler<T> {
  final String baseUri;
  final FromJson<T> fromJson;

  ApiHandler({required this.baseUri, required this.fromJson});

  static Future<String?> _getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('auth_token');
  }

  Future<Map<String, String>> _getHeaders() async {
    final token = await _getToken();
    return {
      'Content-Type': 'application/json; charset=UTF-8',
      'Authorization': 'Bearer $token',
    };
  }

  Future<List<T>> getData() async {
    List<T> data = [];
    List<int> turmas = [1, 2, 3, 4, 5];
    final uri = Uri.parse(ApiRoutes.getDataPorPolo(baseUri, turmas));

    try {
      final headers = await _getHeaders();
      final response = await http.get(uri, headers: headers);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final jsonData = json.decode(response.body);
        final List<dynamic> items = jsonData['data'];
        data = items.map((json) => fromJson(json)).toList();
      }
    } catch (e) {
      print('Erro ao buscar dados: $e');
    }
    return data;
  }

  Future<List<T>> getDataAll() async {
    List<T> data = [];
    final uri = Uri.parse(ApiRoutes.getAll(baseUri));

    try {
      final headers = await _getHeaders();
      final response = await http.get(uri, headers: headers);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final jsonData = json.decode(response.body);
        final List<dynamic> items = jsonData['data'];
        data = items.map((json) => fromJson(json)).toList();
      }
    } catch (e) {
      print('Erro ao buscar dados: $e');
    }
    return data;
  }

  Future<List<T>> getDataPorPolo(List<int> turma) async {
    List<T> data = [];
    final uri = Uri.parse(ApiRoutes.getDataPorPolo(baseUri, turma));

    try {
      final headers = await _getHeaders();
      final response = await http.get(uri, headers: headers);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final jsonData = json.decode(response.body);
        final List<dynamic> items = jsonData['data'];
        data = items.map((json) => fromJson(json)).toList();
      }
    } catch (e) {
      print('Erro ao buscar dados por polo: $e');
    }
    return data;
  }

  Future<http.Response> updateData({
    required int id,
    required T item,
  }) async {
    final uri = Uri.parse(ApiRoutes.update(baseUri));

    try {
      final headers = await _getHeaders();
      final response = await http.put(
        uri,
        headers: headers,
        body: json.encode(item),
      );
      return response;
    } catch (e) {
      print('Erro ao atualizar dados: $e');
      rethrow;
    }
  }

  Future<http.Response> addData({required T item}) async {
    final uri = Uri.parse(ApiRoutes.create(baseUri));

    try {
      final headers = await _getHeaders();
      final response = await http.post(
        uri,
        headers: headers,
        body: json.encode(item),
      );
      return response;
    } catch (e) {
      print('Erro ao adicionar dados: $e');
      rethrow;
    }
  }

  Future<http.Response> deleteData({required int id}) async {
    final uri = Uri.parse(ApiRoutes.delete(baseUri, id));

    try {
      final headers = await _getHeaders();
      final response = await http.delete(uri, headers: headers);
      return response;
    } catch (e) {
      print('Erro ao deletar dados: $e');
      rethrow;
    }
  }

  Future<void> saveToken(String token) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('auth_token', token);
  }

  Future<void> removeToken() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('auth_token');
  }

  Future<List<T>> getDataUrl() async {
    List<T> data = [];
    final uri = Uri.parse(ApiRoutes.getAllUrl(baseUri));

    try {
      final headers = await _getHeaders();
      final response = await http.get(uri, headers: headers);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final jsonData = json.decode(response.body);
        final List<dynamic> items = jsonData['data'];
        data = items.map((json) => fromJson(json)).toList();
      }
    } catch (e) {
      print('Erro ao buscar dados: $e');
    }
    return data;
  }

  Future<List<T>> getDataListById(int aulaId) async {
    final uri = Uri.parse('${ApiRoutes.baseUrl}/presenca/aula/$aulaId');
    List<T> items = [];

    try {
      final headers = await _getHeaders(); // Obtém os cabeçalhos com o token
      final response = await http.get(uri, headers: headers);

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final jsonResponse = json.decode(response.body);

        // Acesse o campo "data" (ou equivalente) que contém a lista
        if (jsonResponse.containsKey('data')) {
          final List<dynamic> dataList = jsonResponse['data'];
          items = dataList.map((item) => fromJson(item)).toList();
        } else {
          print("A resposta não contém o campo 'data'");
        }
      } else {
        print("Erro: Código de status ${response.statusCode}");
      }
    } catch (e) {
      print("Erro ao buscar dados da API: $e");
    }

    return items;
  }

  Future<http.Response> post({
    required String endpoint,
    required List<Map<String, dynamic>> body,
  }) async {
    final uri = Uri.parse('$baseUri/$endpoint');
    try {
      final headers = await _getHeaders();

      print('URI: $uri');
      print('Headers: $headers');
      print('Body: ${json.encode(body)}');

      final response = await http.post(
        uri,
        headers: headers,
        body: json.encode(body),
      );

      if (response.statusCode < 200 || response.statusCode >= 300) {
        throw Exception(
          'Erro no POST: ${response.statusCode} - ${response.reasonPhrase} - ${response.body}',
        );
      }

      return response;
    } catch (e) {
      throw Exception('Erro ao realizar POST: $e');
    }
  }

  Future<http.Response> put({
    required String endpoint,
    required Map<String, dynamic> body,
  }) async {
    final uri = Uri.parse(ApiRoutes.update(baseUri));
    try {
      final headers = await _getHeaders();
      final response = await http.put(
        uri,
        headers: headers,
        body: json.encode(body),
      );
      return response;
    } catch (e) {
      throw Exception('Erro ao realizar PUT: $e');
    }
  }

  Future<Map<String, dynamic>?> login({
    required String login,
    required String password,
  }) async {
    final uri = Uri.parse('${ApiRoutes.baseUrl}/api/v1/auth/login');
    Map<String, dynamic>? responseData;

    try {
      final response = await http.post(
        uri,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: json.encode({'login': login, 'password': password}),
      );

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        responseData = json.decode(response.body) as Map<String, dynamic>;
      } else {
        print("Erro: Código de status ${response.statusCode}");
      }
    } catch (e) {
      print("Erro ao fazer login: $e");
    }
    return responseData;
  }

  Future<void> enviarPresencas(List<Presenca> presencas) async {
    final url = Uri.parse('${ApiRoutes.baseUrl}/presenca/batch/create1');
    final headers = await _getHeaders();
    final response = await http.post(
      url,
      headers: headers,
      body: jsonEncode(presencas.map((p) => p.toJson()).toList()),
    );

    if (response.statusCode == 200) {
      print("Presenças salvas com sucesso!");
    } else {
      print("Erro ao salvar presenças: ${response.body}");
      throw Exception('Erro ao salvar presenças');
    }
  }

  static Future<String?> getPoloName() async {
    final token = await _getToken();
    if (token != null) {
      try {
        Map<String, dynamic> payload = Jwt.parseJwt(token);
        return payload['PoloNome'] ?? 'Polo não encontradoxxxx';
      } catch (e) {
        print('Erro ao decodificar o token: $e');
        return 'Erro ao obter polo';
      }
    }
    return null;
  }
}
