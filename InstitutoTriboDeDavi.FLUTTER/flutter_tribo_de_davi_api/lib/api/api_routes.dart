class ApiRoutes {
  static const String baseUrl = "http://localhost";

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
}
