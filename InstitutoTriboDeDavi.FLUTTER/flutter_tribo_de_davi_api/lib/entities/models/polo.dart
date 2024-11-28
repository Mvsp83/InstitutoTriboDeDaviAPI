class Polo {
  final int id;
  final String nome;
  final String informacoes;
  final String endereco;
  final String? bairro;
  final String? cidade;

  const Polo(
      {required this.id,
      required this.nome,
      required this.informacoes,
      required this.endereco,
      this.bairro,
      this.cidade});

  const Polo.empty(
      {this.id = 0,
      this.nome = '',
      this.informacoes = '',
      this.endereco = '',
      this.bairro = '',
      this.cidade = ''});

  factory Polo.fromJson(Map<String, dynamic> json) => Polo(
      id: json['id'],
      nome: json['nome'],
      informacoes: json['informacoes'],
      endereco: json['endereco'],
      bairro: json['bairro'],
      cidade: json['cidade']);

  Map<String, dynamic> toJson() => {
        "id": id,
        "nome": nome,
        "informacoes": informacoes,
        "endereco": endereco,
        "bairro": bairro,
        "cidade": cidade
      };
}
