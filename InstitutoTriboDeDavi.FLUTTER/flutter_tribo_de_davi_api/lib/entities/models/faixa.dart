enum Faixa {
  branca,
  cinza,
  amarela,
  laranja,
  verde,
  azul,
  roxa,
  marrom,
  preta;

  // IDs numéricos associados às faixas
  int get id {
    switch (this) {
      case Faixa.branca:
        return 0;
      case Faixa.cinza:
        return 1;
      case Faixa.amarela:
        return 2;
      case Faixa.laranja:
        return 3;
      case Faixa.verde:
        return 4;
      case Faixa.azul:
        return 5;
      case Faixa.roxa:
        return 6;
      case Faixa.marrom:
        return 7;
      case Faixa.preta:
        return 8;
    }
  }

  // Descrição textual das faixas
  String get descricao {
    switch (this) {
      case Faixa.branca:
        return 'Faixa Branca';
      case Faixa.cinza:
        return 'Cinza';
      case Faixa.amarela:
        return 'Amarela';
      case Faixa.laranja:
        return 'Laranja';
      case Faixa.verde:
        return 'Verde';
      case Faixa.azul:
        return 'Faixa Azul';
      case Faixa.roxa:
        return 'Roxa';
      case Faixa.marrom:
        return 'Marrom';
      case Faixa.preta:
        return 'Preta';
    }
  }

  // Caminho da imagem associada à faixa
  String get imagem {
    switch (this) {
      case Faixa.branca:
        return 'lib/assets/images/faixa_branca.png';
      case Faixa.cinza:
        return 'lib/assets/images/faixa_cinza.png';
      case Faixa.amarela:
        return 'lib/assets/images/faixa_amarela.png';
      case Faixa.laranja:
        return 'lib/assets/images/faixa_laranja.png';
      case Faixa.verde:
        return 'lib/assets/images/faixa_verde.png';
      case Faixa.azul:
        return 'lib/assets/images/faixa_azul.png';
      case Faixa.roxa:
        return 'lib/assets/images/faixa_roxa.png';
      case Faixa.marrom:
        return 'lib/assets/images/faixa_marrom.png';
      case Faixa.preta:
        return 'lib/assets/images/faixa_preta.png';
    }
  }

  // Métodos auxiliares para conversão

  /// Obtém uma faixa a partir de seu ID numérico
  static Faixa fromId(int id) {
    return Faixa.values
        .firstWhere((faixa) => faixa.id == id, orElse: () => Faixa.branca);
  }

  /// Obtém uma faixa a partir de sua descrição textual
  static Faixa fromDescricao(String descricao) {
    return Faixa.values.firstWhere(
        (faixa) => faixa.descricao.toLowerCase() == descricao.toLowerCase(),
        orElse: () => Faixa.branca);
  }

  /// Lista todas as faixas como um mapa para dropdowns ou listas
  static List<Map<String, dynamic>> get listaFaixas {
    return Faixa.values.map((faixa) {
      return {
        'label': faixa.descricao,
        'value': faixa.id,
        'imagem': faixa.imagem
      };
    }).toList();
  }
}
