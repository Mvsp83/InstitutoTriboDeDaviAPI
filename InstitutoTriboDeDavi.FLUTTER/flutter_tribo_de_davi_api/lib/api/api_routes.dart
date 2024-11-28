class ApiRoutes {
  static const String baseUrl = "http://10.0.2.2:7030";

  static String entity(String endpoint) {
    return endpoint;
  }

  static String getAll(String endpoint) {
    return '$baseUrl/${entity(endpoint)}/get-all';
  }

  static String getById(String endpoint, int id) {
    return '$baseUrl/${entity(endpoint)}/get/$id';
  }

  static String create(String endpoint) {
    return '$baseUrl/${entity(endpoint)}/create';
  }

  static String update(String endpoint) {
    return '$baseUrl/${entity(endpoint)}/update';
  }

  static String delete(String endpoint, int id) {
    return '$baseUrl/${entity(endpoint)}/delete/$id';
  }

  static String getByNome(String endpoint, String nome) {
    return '$baseUrl/${entity(endpoint)}/get-by-nome?nome=$nome';
  }

  static String searchByNome(String endpoint, String nome) {
    return '$baseUrl/${entity(endpoint)}/search-by-nome?nome=$nome';
  }

  static String fetchTotalAlunos(String endpoint, int poloId) {
    return '$baseUrl/${entity(endpoint)}/total';
  }

  static String getAll2(String endpoint) {
    return '$baseUrl/${entity(endpoint)}';
  }

  static String getById2(String endpoint, int id) {
    return '$baseUrl/${entity(endpoint)}';
  }
}
