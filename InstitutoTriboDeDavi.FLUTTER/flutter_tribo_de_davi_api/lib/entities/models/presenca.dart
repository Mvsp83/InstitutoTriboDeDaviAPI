class Presenca {
  final int id;
  final int alunoId;
  final String? nomeAluno;
  final int poloId;
  final DateTime data;
  final bool estaPresente;
  final String? observacoes;
  final int aulaId;

  const Presenca(
      {required this.id,
      required this.alunoId,
      this.nomeAluno,
      required this.poloId,
      required this.data,
      required this.estaPresente,
      this.observacoes,
      required this.aulaId});

  Presenca.empty()
      : id = 0,
        alunoId = 0,
        nomeAluno = '',
        poloId = 0,
        data = DateTime(1970, 1, 1),
        estaPresente = false,
        observacoes = '',
        aulaId = 0;

  factory Presenca.fromJson(Map<String, dynamic> json) => Presenca(
      id: json['id'],
      alunoId: json['alunoId'],
      nomeAluno: json['nomeAluno'],
      poloId: json['poloId'],
      data: DateTime.parse(json['data']),
      estaPresente: json['estaPresente'],
      observacoes: json['observacoes'],
      aulaId: json['aulaId']);

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'alunoId': alunoId,
      'nomeAluno': nomeAluno,
      'poloId': poloId,
      'data': data.toIso8601String(),
      'estaPresente': estaPresente,
      'observacoes': observacoes,
      'aulaId': aulaId,
    };
  }
}
