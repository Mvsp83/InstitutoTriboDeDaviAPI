class Aluno {
  final int id;
  final String nome;
  final String cpf;
  final int faixa;
  //final DateTime dataCadastro;
  //final DateTime dataAtualizacao;

  const Aluno(
      {required this.id,
      required this.nome,
      required this.cpf,
      required this.faixa});

  const Aluno.empty({
    this.id = 0,
    this.nome = '',
    this.cpf = '',
    this.faixa = 0,
  });

  factory Aluno.fromJson(Map<String, dynamic> json) => Aluno(
        id: json['id'],
        nome: json['nome'],
        cpf: json['cpf'],
        faixa: json['faixa'],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "nome": nome,
        "cpf": cpf,
        "faixa": faixa,
      };
}
