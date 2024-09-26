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

  Future<T?> getDataById({required int id}) async {
    final uri = Uri.parse(ApiRoutes.getById(baseUri, id));
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
}
