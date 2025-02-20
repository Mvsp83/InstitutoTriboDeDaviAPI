enum Faixa {
  branca,
  branca1,
  branca2,
  branca3,
  branca4,
  cinza,
  cinza1,
  cinza2,
  cinza3,
  cinza4,
  amarela,
  amarela1,
  amarela2,
  amarela3,
  amarela4,
  laranja,
  laranja1,
  laranja2,
  laranja3,
  laranja4,
  verde,
  verde1,
  verde2,
  verde3,
  verde4,
  azul,
  azul1,
  azul2,
  azul3,
  azul4,
  roxa,
  roxa1,
  roxa2,
  roxa3,
  roxa4,
  marrom,
  marrom1,
  marrom2,
  marrom3,
  marrom4,
  preta;

  int get id {
    switch (this) {
      case Faixa.branca:
        return 0;
      case Faixa.branca1:
        return 1;
      case Faixa.branca2:
        return 2;
      case Faixa.branca3:
        return 3;
      case Faixa.branca4:
        return 4;
      case Faixa.cinza:
        return 5;
      case Faixa.cinza1:
        return 6;
      case Faixa.cinza2:
        return 7;
      case Faixa.cinza3:
        return 8;
      case Faixa.cinza4:
        return 9;
      case Faixa.amarela:
        return 10;
      case Faixa.amarela1:
        return 11;
      case Faixa.amarela2:
        return 12;
      case Faixa.amarela3:
        return 13;
      case Faixa.amarela4:
        return 14;
      case Faixa.laranja:
        return 15;
      case Faixa.laranja1:
        return 16;
      case Faixa.laranja2:
        return 17;
      case Faixa.laranja3:
        return 18;
      case Faixa.laranja4:
        return 19;
      case Faixa.verde:
        return 20;
      case Faixa.verde1:
        return 21;
      case Faixa.verde2:
        return 22;
      case Faixa.verde3:
        return 23;
      case Faixa.verde4:
        return 24;
      case Faixa.azul:
        return 25;
      case Faixa.azul1:
        return 26;
      case Faixa.azul2:
        return 27;
      case Faixa.azul3:
        return 28;
      case Faixa.azul4:
        return 29;
      case Faixa.roxa:
        return 30;
      case Faixa.roxa1:
        return 31;
      case Faixa.roxa2:
        return 32;
      case Faixa.roxa3:
        return 33;
      case Faixa.roxa4:
        return 34;
      case Faixa.marrom:
        return 35;
      case Faixa.marrom1:
        return 36;
      case Faixa.marrom2:
        return 37;
      case Faixa.marrom3:
        return 38;
      case Faixa.marrom4:
        return 39;
      case Faixa.preta:
        return 40;
    }
  }

  String get descricao {
    switch (this) {
      case Faixa.branca:
        return 'Faixa Branca';
      case Faixa.branca1:
        return 'Faixa Branca 1g';
      case Faixa.branca2:
        return 'Faixa Branca 2g';
      case Faixa.branca3:
        return 'Faixa Branca 3g';
      case Faixa.branca4:
        return 'Faixa Branca 4g';
      case Faixa.cinza:
        return 'Faixa Cinza';
      case Faixa.cinza1:
        return 'Faixa Cinza 1g';
      case Faixa.cinza2:
        return 'Faixa Cinza 2g';
      case Faixa.cinza3:
        return 'Faixa Cinza 3g';
      case Faixa.cinza4:
        return 'Faixa Cinza 4g';
      case Faixa.amarela:
        return 'Faixa Amarela';
      case Faixa.amarela1:
        return 'Faixa Amarela 1g';
      case Faixa.amarela2:
        return 'Faixa Amarela 2g';
      case Faixa.amarela3:
        return 'Faixa Amarela 3g';
      case Faixa.amarela4:
        return 'Faixa Amarela 4g';
      case Faixa.laranja:
        return 'Faixa Laranja';
      case Faixa.laranja1:
        return 'Faixa Laranja 1g';
      case Faixa.laranja2:
        return 'Faixa Laranja 2g';
      case Faixa.laranja3:
        return 'Faixa Laranja 3g';
      case Faixa.laranja4:
        return 'Faixa Laranja 4g';
      case Faixa.verde:
        return 'Faixa Verde';
      case Faixa.verde1:
        return 'Faixa Verde 1g';
      case Faixa.verde2:
        return 'Faixa Verde 2g';
      case Faixa.verde3:
        return 'Faixa Verde 3g';
      case Faixa.verde4:
        return 'Faixa Verde 4g';
      case Faixa.azul:
        return 'Faixa Azul';
      case Faixa.azul1:
        return 'Faixa Azul 1g';
      case Faixa.azul2:
        return 'Faixa Azul 2g';
      case Faixa.azul3:
        return 'Faixa Azul 3g';
      case Faixa.azul4:
        return 'Faixa Azul 4g';
      case Faixa.roxa:
        return 'Faixa Roxa';
      case Faixa.roxa1:
        return 'Faixa Roxa 1g';
      case Faixa.roxa2:
        return 'Faixa Roxa 2g';
      case Faixa.roxa3:
        return 'Faixa Roxa 3g';
      case Faixa.roxa4:
        return 'Faixa Roxa 4g';
      case Faixa.marrom:
        return 'Faixa Marrom';
      case Faixa.marrom1:
        return 'Faixa Marrom 1g';
      case Faixa.marrom2:
        return 'Faixa Marrom 2g';
      case Faixa.marrom3:
        return 'Faixa Marrom 3g';
      case Faixa.marrom4:
        return 'Faixa Marrom 4g';
      case Faixa.preta:
        return 'Faixa Preta';
    }
  }

  String get imagem {
    switch (this) {
      case Faixa.branca:
        return 'lib/assets/images/faixa_branca.png';
      case Faixa.branca1:
        return 'lib/assets/images/faixa_branca_1.png';
      case Faixa.branca2:
        return 'lib/assets/images/faixa_branca_2.png';
      case Faixa.branca3:
        return 'lib/assets/images/faixa_branca_3.png';
      case Faixa.branca4:
        return 'lib/assets/images/faixa_branca_4.png';
      case Faixa.cinza:
        return 'lib/assets/images/faixa_cinza.png';
      case Faixa.cinza1:
        return 'lib/assets/images/faixa_cinza_1.png';
      case Faixa.cinza2:
        return 'lib/assets/images/faixa_cinza_2.png';
      case Faixa.cinza3:
        return 'lib/assets/images/faixa_cinza_3.png';
      case Faixa.cinza4:
        return 'lib/assets/images/faixa_cinza_4.png';
      case Faixa.amarela:
        return 'lib/assets/images/faixa_amarela.png';
      case Faixa.amarela1:
        return 'lib/assets/images/faixa_amarela_1.png';
      case Faixa.amarela2:
        return 'lib/assets/images/faixa_amarela_2.png';
      case Faixa.amarela3:
        return 'lib/assets/images/faixa_amarela_3.png';
      case Faixa.amarela4:
        return 'lib/assets/images/faixa_amarela_4.png';
      case Faixa.laranja:
        return 'lib/assets/images/faixa_laranja.png';
      case Faixa.laranja1:
        return 'lib/assets/images/faixa_laranja_1.png';
      case Faixa.laranja2:
        return 'lib/assets/images/faixa_laranja_2.png';
      case Faixa.laranja3:
        return 'lib/assets/images/faixa_laranja_3.png';
      case Faixa.laranja4:
        return 'lib/assets/images/faixa_laranja_4.png';
      case Faixa.verde:
        return 'lib/assets/images/faixa_verde.png';
      case Faixa.verde1:
        return 'lib/assets/images/faixa_verde_1.png';
      case Faixa.verde2:
        return 'lib/assets/images/faixa_verde_2.png';
      case Faixa.verde3:
        return 'lib/assets/images/faixa_verde_3.png';
      case Faixa.verde4:
        return 'lib/assets/images/faixa_verde_4.png';
      case Faixa.azul:
        return 'lib/assets/images/faixa_azul.png';
      case Faixa.azul1:
        return 'lib/assets/images/faixa_azul_1.png';
      case Faixa.azul2:
        return 'lib/assets/images/faixa_azul_2.png';
      case Faixa.azul3:
        return 'lib/assets/images/faixa_azul_3.png';
      case Faixa.azul4:
        return 'lib/assets/images/faixa_azul_4.png';
      case Faixa.roxa:
        return 'lib/assets/images/faixa_roxa.png';
      case Faixa.roxa1:
        return 'lib/assets/images/faixa_roxa_1.png';
      case Faixa.roxa2:
        return 'lib/assets/images/faixa_roxa_2.png';
      case Faixa.roxa3:
        return 'lib/assets/images/faixa_roxa_3.png';
      case Faixa.roxa4:
        return 'lib/assets/images/faixa_roxa_4.png';
      case Faixa.marrom:
        return 'lib/assets/images/faixa_marrom.png';
      case Faixa.marrom1:
        return 'lib/assets/images/faixa_marrom_1.png';
      case Faixa.marrom2:
        return 'lib/assets/images/faixa_marrom_2.png';
      case Faixa.marrom3:
        return 'lib/assets/images/faixa_marrom_3.png';
      case Faixa.marrom4:
        return 'lib/assets/images/faixa_marrom_4.png';
      case Faixa.preta:
        return 'lib/assets/images/faixa_preta.png';
    }
  }

  static Faixa fromId(int id) {
    return Faixa.values
        .firstWhere((faixa) => faixa.id == id, orElse: () => Faixa.branca);
  }

  static Faixa fromDescricao(String descricao) {
    return Faixa.values.firstWhere(
        (faixa) => faixa.descricao.toLowerCase() == descricao.toLowerCase(),
        orElse: () => Faixa.branca);
  }

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
