import 'dart:convert';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:http/http.dart' as http;

typedef FromJson<T> = T Function(Map<String, dynamic> json);

class ApiHandler<T> {
  final String baseUri;
  final FromJson<T> fromJson;

  ApiHandler({required this.baseUri, required this.fromJson});

  Future<List<T>> getData() async {
    List<T> data = [];
    final uri = Uri.parse(ApiRoutes.getAll(baseUri));

    try {
      final response = await http.get(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
      );

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final Map<String, dynamic> jsonData = json.decode(response.body);
        final List<dynamic> items = jsonData['data'];
        data = items.map((json) => fromJson(json)).toList();
      }
    } catch (e) {
      return data;
    }
    return data;
  }

  Future<List<T>> getData2() async {
    List<T> data = [];
    final uri = Uri.parse(ApiRoutes.getAll2(baseUri));

    try {
      final response = await http.get(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
      );

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final Map<String, dynamic> jsonData = json.decode(response.body);
        final List<dynamic> items = jsonData['data'];
        data = items.map((json) => fromJson(json)).toList();
      }
    } catch (e) {
      return data;
    }
    return data;
  }

  Future<T?> getByNome(String nome) async {
    final uri = Uri.parse(ApiRoutes.getByNome(baseUri, nome));
    T? item;

    try {
      final response = await http.get(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
      );

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final Map<String, dynamic> jsonData = json.decode(response.body);
        final Map<String, dynamic> itemData = jsonData['data'];

        item = fromJson(itemData);
      }
    } catch (e) {
      return item;
    }
    return item;
  }

  Future<http.Response> updateData({
    required int id,
    required T item,
  }) async {
    final uri = Uri.parse(ApiRoutes.update(baseUri));
    late http.Response response;

    try {
      response = await http.put(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
        body: json.encode(item),
      );
    } catch (e) {
      return response;
    }
    return response;
  }

  Future<http.Response> addData({required T item}) async {
    final uri = Uri.parse(ApiRoutes.create(baseUri));
    late http.Response response;

    try {
      response = await http.post(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
        body: json.encode(item),
      );
    } catch (e) {
      return response;
    }

    return response;
  }

  Future<http.Response> deleteData({required int id}) async {
    final uri = Uri.parse(ApiRoutes.delete(baseUri, id));
    late http.Response response;

    try {
      response = await http.delete(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
      );
    } catch (e) {
      return response;
    }
    return response;
  }

  Future<List<T>> getDataListById(int aulaId) async {
    final uri = Uri.parse('${ApiRoutes.baseUrl}/presenca/aula/$aulaId');
    List<T> items = [];

    try {
      final response = await http.get(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8',
        },
      );

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final Map<String, dynamic> jsonResponse = json.decode(response.body);

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

  Future<List<T>> searchByNome(String nome) async {
    List<T> data = [];
    final uri = Uri.parse(ApiRoutes.searchByNome(baseUri, nome));

    try {
      final response = await http.get(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
      );

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final Map<String, dynamic> jsonData = json.decode(response.body);
        final List<dynamic> items = jsonData['data'];
        data = items.map((json) => fromJson(json)).toList();
      }
    } catch (e) {
      return data;
    }
    return data;
  }

  static Future<http.Response> post(String url,
      {required List<Map<String, dynamic>> body}) async {
    final response = await http.post(
      Uri.parse(url),
      headers: {
        'Content-Type': 'application/json',
      },
      body: jsonEncode(body),
    );
    return response;
  }

  Future<int> fetchTotalAlunos(int poloId) async {
    final uri = Uri.parse(ApiRoutes.fetchTotalAlunos(baseUri, poloId));
    final response = await http.get(uri);

    if (response.statusCode == 200) {
      return json.decode(response.body)['data'];
    } else {
      throw Exception('Falha ao carregar total de alunos');
    }
  }

  static Future<http.Response> put(String url,
      {required Map<String, dynamic> body}) async {
    final response = await http.put(
      Uri.parse(url),
      headers: {
        'Content-Type': 'application/json',
      },
      body: jsonEncode(body),
    );
    return response;
  }
}
