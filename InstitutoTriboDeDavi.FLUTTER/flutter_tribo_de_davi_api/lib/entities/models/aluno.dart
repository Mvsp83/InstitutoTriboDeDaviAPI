class Aluno {
  final int id;
  final String? nome;
  final String? rg;
  final String? cpf;
  final DateTime? dataNascimento;
  final String? peso;
  int faixa;
  final String? endereco;
  final String? bairro;
  final String? cidade;
  final String? celular;
  final String? responsavel;
  int parentesco;
  final String? rgResponsavel;
  final String? cpfResponsavel;
  final String? escola;
  final String? periodo;
  final int? poloId;

  Aluno(
      {required this.id,
      this.nome,
      this.rg,
      this.cpf,
      this.dataNascimento,
      this.peso,
      required this.faixa,
      this.endereco,
      this.bairro,
      this.cidade,
      this.celular,
      this.responsavel,
      required this.parentesco,
      this.rgResponsavel,
      this.cpfResponsavel,
      this.escola,
      this.periodo,
      this.poloId});

  Aluno.empty(
      {this.id = 0,
      this.nome = '',
      this.rg = '',
      this.cpf = '',
      this.dataNascimento,
      this.peso = '',
      this.faixa = 0,
      this.endereco = '',
      this.bairro = '',
      this.cidade = '',
      this.celular = '',
      this.responsavel = '',
      this.parentesco = 0,
      this.rgResponsavel = '',
      this.cpfResponsavel = '',
      this.escola = '',
      this.periodo = '',
      this.poloId});

  factory Aluno.fromJson(Map<String, dynamic> json) => Aluno(
      id: json['id'],
      nome: json['nome'],
      rg: json['rg'],
      cpf: json['cpf'],
      dataNascimento: json['dataNascimento'] != null
          ? DateTime.parse(json['dataNascimento'])
          : null,
      peso: json['peso'],
      faixa: (json['faixa'] ?? 0),
      endereco: json['endereco'],
      bairro: json['bairro'],
      cidade: json['cidade'],
      celular: json['celular'],
      responsavel: json['responsavel'],
      parentesco: (json['parentesco'] ?? 0),
      rgResponsavel: json['rgResponsavel'],
      cpfResponsavel: json['cpfResponsavel'],
      escola: json['escola'],
      periodo: json['periodo'],
      poloId: json['poloId']);

  Map<String, dynamic> toJson() => {
        "id": id,
        "nome": nome,
        "rg": rg,
        "cpf": cpf,
        "dataNascimento": dataNascimento?.toIso8601String(),
        "peso": peso,
        "faixa": faixa,
        "endereco": endereco,
        "bairro": bairro,
        "cidade": cidade,
        "celular": celular,
        "responsavel": responsavel,
        "parentesco": parentesco,
        "rgResponsavel": rgResponsavel,
        "cpfResponsavel": cpfResponsavel,
        "escola": escola,
        "periodo": periodo,
        "poloId": poloId
      };

  Aluno copyWith({
    int? id,
    String? nome,
    String? rg,
    String? cpf,
    DateTime? dataNascimento,
    String? peso,
    int? faixa,
    String? endereco,
    String? bairro,
    String? cidade,
    String? celular,
    String? responsavel,
    int? parentesco,
    String? rgResponsavel,
    String? cpfResponsavel,
    String? escola,
    String? periodo,
    int? poloId,
  }) {
    return Aluno(
      id: id ?? this.id,
      nome: nome ?? this.nome,
      rg: rg ?? this.rg,
      cpf: cpf ?? this.cpf,
      dataNascimento: dataNascimento ?? this.dataNascimento,
      peso: peso ?? this.peso,
      faixa: faixa ?? this.faixa,
      endereco: endereco ?? this.endereco,
      bairro: bairro ?? this.bairro,
      cidade: cidade ?? this.cidade,
      celular: celular ?? this.celular,
      responsavel: responsavel ?? this.responsavel,
      parentesco: parentesco ?? this.parentesco,
      rgResponsavel: rgResponsavel ?? this.rgResponsavel,
      cpfResponsavel: cpfResponsavel ?? this.cpfResponsavel,
      escola: escola ?? this.escola,
      periodo: periodo ?? this.periodo,
      poloId: poloId ?? this.poloId,
    );
  }
}
