class ApiRoutes {
  /// URL da API, configurável por ambiente no build/run:
  ///
  ///   flutter run --dart-define=API_BASE_URL=http://192.168.0.10:7030/api   (aparelho físico na rede local)
  ///   flutter build apk --dart-define=API_BASE_URL=https://api.exemplo.com/api   (produção)
  ///
  /// Sem --dart-define, usa o localhost visto pelo emulador Android (dev).
  static const String baseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: 'http://10.0.2.2:7030/api',
  );

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
    return '$baseUrl/${entity(endpoint)}/get-por-polo?$turmasQuery';
  }

  static String alunosPendentes() => '$baseUrl/aluno/pendentes';
  static String atribuirTurma() => '$baseUrl/aluno/atribuir-turma';
}
