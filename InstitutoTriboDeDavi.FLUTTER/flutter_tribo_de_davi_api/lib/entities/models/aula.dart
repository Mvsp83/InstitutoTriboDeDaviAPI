class Aula {
  final int id;
  final int poloId;
  final DateTime data;
  final String horaInicio;
  final String horaFim;
  late bool presencaSalva;

  Aula(
      {required this.id,
      required this.poloId,
      required this.data,
      required this.horaInicio,
      required this.horaFim,
      this.presencaSalva = true});

  Aula.empty()
      : id = 0,
        poloId = 0,
        data = DateTime(1970, 1, 1),
        horaInicio = '',
        horaFim = '',
        presencaSalva = true;

  factory Aula.fromJson(Map<String, dynamic> json) => Aula(
      id: json['id'],
      poloId: json['poloId'],
      data: DateTime.parse(json['data']),
      horaInicio: json['horaInicio'],
      horaFim: json['horaFim'],
      presencaSalva: json['presencaSalva']);

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'poloId': poloId,
      'data': data.toIso8601String(),
      'horaInicio': horaInicio,
      'horaFim': horaFim,
      'presencaSalva': presencaSalva
    };
  }
}
