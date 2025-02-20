class ApiRoutes {
  static const String baseUrl = "http://10.0.2.2:7030";

  static String entity(String endpoint) {
    return endpoint;
  }

  static String getAll(String endpoint) {
    return '$baseUrl/${entity(endpoint)}/get-all';
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

  static String getAllUrl(String endpoint) {
    return '$baseUrl/${entity(endpoint)}';
  }

  static String getDataPorPolo(String endpoint, List<int> turmas) {
    String turmasQuery = turmas.map((t) => 'turmas=$t').join('&');
    return '$baseUrl/${entity(endpoint)}/get-por-polo/?$turmasQuery';
  }
}
