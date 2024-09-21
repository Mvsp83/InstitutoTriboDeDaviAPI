import 'dart:convert';
import 'package:flutter_tribo_de_davi_api/models/model.dart';
import 'package:http/http.dart' as http;

class ApiHandler {
  final String baseUri = "http://localhost/usuario";

  Future<List<Usuario>> getUsuarioData() async {
    List<Usuario> data = [];
    final uri = Uri.parse(baseUri + "/get-all");

    try {
      final response = await http.get(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
      );

      // if(response.statusCode >= 200 && response.statusCode <= 299){
      //   final List<dynamic> jsonData = json.decode(response.body);
      //   data = jsonData.map((json) => Usuario.fromJson(json)).toList();
      // }

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        final Map<String, dynamic> jsonData = json.decode(response.body);
        final List<dynamic> usuarios = jsonData['data'];
        data = usuarios.map((json) => Usuario.fromJson(json)).toList();
      }
    } catch (e) {
      return data;
    }
    return data;
  }

  Future<http.Response> updateUsuario(
      {required int id, required Usuario usuario}) async {
    final uri = Uri.parse("$baseUri/update");
    late http.Response response;

    try {
      response = await http.put(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
        body: json.encode(usuario),
      );
    } catch (e) {
      return response;
    }
    return response;
  }

  Future<http.Response> addUsuario({required Usuario usuario}) async {
    final uri = Uri.parse("$baseUri/create");
    late http.Response response;

    try {
      response = await http.post(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
        body: json.encode(usuario),
      );
    } catch (e) {
      return response;
    }

    return response;
  }

  Future<http.Response> deleteUsuario({required int id}) async {
    final uri = Uri.parse("$baseUri/delete/$id");
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

  Future<Usuario> getUsuarioById({required int id}) async {
    //List<Usuario> data = [];
    final uri = Uri.parse("$baseUri/get/$id");
    Usuario? usuario;

    try {
      final response = await http.get(
        uri,
        headers: <String, String>{
          'Content-type': 'application/json; charset=UTF-8'
        },
      );

      // if(response.statusCode >= 200 && response.statusCode <= 299){
      //   final List<dynamic> jsonData = json.decode(response.body);
      //   data = jsonData.map((json) => Usuario.fromJson(json)).toList();
      // }

      if (response.statusCode >= 200 && response.statusCode <= 299) {
        //final Map<String, dynamic> jsonData = json.decode(response.body);
        //usuario = Usuario.fromJson(jsonData);
        //final Map<String, dynamic> jsonData = json.decode(response.body);
        //final List<dynamic> usuarios = jsonData['data'];
        //data = usuarios.map((json) => Usuario.fromJson(json)).toList();

        final Map<String, dynamic> jsonData = json.decode(response.body);

        // Extraindo o campo "data" do JSON
        final Map<String, dynamic> userData = jsonData['data'];

        // Criando o objeto usuario a partir de "data"
        usuario = Usuario.fromJson(userData);
      }
    } catch (e) {
      return usuario!;
    }
    return usuario!;
  }
}
