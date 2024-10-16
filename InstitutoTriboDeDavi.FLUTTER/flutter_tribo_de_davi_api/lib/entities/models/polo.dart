class Polo {
  final int id;
  final String nome;

  const Polo({required this.id, required this.nome});

  const Polo.empty({this.id = 0, this.nome = ''});

  factory Polo.fromJson(Map<String, dynamic> json) =>
      Polo(id: json['id'], nome: json['nome']);

  Map<String, dynamic> toJson() => {"id": id, "nome": nome};
}
