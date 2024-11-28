class Frequencia {
  final int alunoId;
  final String nome;
  final String faixa;
  final int totalAulas;
  final int totalFaltas;

  const Frequencia(
      {required this.alunoId,
      required this.nome,
      required this.faixa,
      required this.totalAulas,
      required this.totalFaltas});

  const Frequencia.empty(
      {this.alunoId = 0,
      this.nome = '',
      this.faixa = '',
      this.totalAulas = 0,
      this.totalFaltas = 0});

  factory Frequencia.fromJson(Map<String, dynamic> json) => Frequencia(
      alunoId: json['alunoId'],
      nome: json['nome'],
      faixa: json['faixa'],
      totalAulas: json['totalAulas'],
      totalFaltas: json['totalFaltas']);

  Map<String, dynamic> toJson() => {
        "alunoId": alunoId,
        "nome": nome,
        "faixa": faixa,
        "totalAulas": totalAulas,
        "totalFaltas": totalFaltas
      };
}
