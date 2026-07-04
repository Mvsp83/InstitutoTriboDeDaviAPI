class Usuario {
  final int id;
  final String login;
  final String password;
  final String email;

  const Usuario(
      {required this.id,
      required this.email,
      required this.login,
      required this.password});

  const Usuario.empty({
    this.id = 0,
    this.email = '',
    this.login = '',
    this.password = '',
  });

  factory Usuario.fromJson(Map<String, dynamic> json) => Usuario(
        id: json['id'],
        email: json['email'],
        login: json['login'],
        // A API não retorna a senha nos responses
        password: json['password'] ?? '',
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "email": email,
        "login": login,
        // Senha vazia é omitida: no update significa "manter a senha atual"
        if (password.isNotEmpty) "password": password,
      };
}
