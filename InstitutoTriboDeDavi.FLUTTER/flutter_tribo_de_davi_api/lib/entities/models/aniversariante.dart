class Aniversariante {
  final String nome;
  final DateTime? dataNascimento;
  final bool jaComemorado;

  const Aniversariante(
      {required this.nome, this.dataNascimento, required this.jaComemorado});

  const Aniversariante.empty(
      {this.nome = '', this.dataNascimento, this.jaComemorado = false});

  factory Aniversariante.fromJson(Map<String, dynamic> json) => Aniversariante(
      nome: json['nome'],
      dataNascimento: json['dataNascimento'] != null
          ? DateTime.parse(json['dataNascimento'])
          : null,
      jaComemorado: json['jaComemorado']);

  Map<String, dynamic> toJson() => {
        "nome": nome,
        "dataNascimento": dataNascimento,
        "jaComemorado": jaComemorado
      };
}
