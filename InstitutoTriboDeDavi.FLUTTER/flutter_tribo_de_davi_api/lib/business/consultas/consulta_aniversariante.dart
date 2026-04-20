import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aniversariante.dart';
import 'package:intl/intl.dart';

class AniversariantesPage extends StatefulWidget {
  const AniversariantesPage({super.key});

  @override
  State<AniversariantesPage> createState() => _AniversariantesPageState();
}

class _AniversariantesPageState extends State<AniversariantesPage> {
  late int _mesSelecionado;
  late Future<List<Aniversariante>> _future;

  static const List<String> _meses = [
    'Janeiro',
    'Fevereiro',
    'Março',
    'Abril',
    'Maio',
    'Junho',
    'Julho',
    'Agosto',
    'Setembro',
    'Outubro',
    'Novembro',
    'Dezembro',
  ];

  @override
  void initState() {
    super.initState();
    _mesSelecionado = DateTime.now().month;
    _future = _fetchAniversariantes(_mesSelecionado);
  }

  Future<List<Aniversariante>> _fetchAniversariantes(int mes) async {
    return ApiHandler<Aniversariante>(
      baseUri: ApiRoutes.entity("aniversariante/aniversariantes/$mes"),
      fromJson: (json) => Aniversariante.fromJson(json),
    ).getDataUrl();
  }

  void _onMesChanged(int? novoMes) {
    if (novoMes == null || novoMes == _mesSelecionado) return;
    setState(() {
      _mesSelecionado = novoMes;
      _future = _fetchAniversariantes(_mesSelecionado);
    });
  }

  bool _isAniversarianteDoDia(DateTime? dataNascimento) {
    if (dataNascimento == null) return false;
    final agora = DateTime.now().toUtc().add(const Duration(hours: -3));
    return dataNascimento.day == agora.day &&
        dataNascimento.month == agora.month;
  }

  bool _jaOcorreu(DateTime? dataNascimento) {
    if (dataNascimento == null) return false;
    final agora = DateTime.now().toUtc().add(const Duration(hours: -3));
    // Compara só dia e mês dentro do mês selecionado
    final aniversarioEsteAno = DateTime(
      agora.year,
      dataNascimento.month,
      dataNascimento.day,
    );
    // Já ocorreu se o aniversário deste ano é antes de hoje (sem incluir hoje)
    return aniversarioEsteAno.isBefore(
      DateTime(agora.year, agora.month, agora.day),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.primaryColor,
      appBar: AppBar(
        title: const Text(
          "Aniversariantes",
          style: TextStyle(fontSize: 16, fontWeight: FontWeight.w500),
        ),
        centerTitle: true,
        backgroundColor: AppTheme.surfaceColor,
        foregroundColor: AppTheme.textColor,
        elevation: 0,
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(1),
          child: Container(color: AppTheme.borderColor, height: 0.5),
        ),
      ),
      body: Column(
        children: [
          // ── Seletor de mês ──────────────────────────────────────────────
          Container(
            color: AppTheme.surfaceColor,
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
            child: DropdownButtonFormField<int>(
              value: _mesSelecionado,
              dropdownColor: AppTheme.surfaceColor,
              iconEnabledColor: AppTheme.accentColor,
              style: const TextStyle(color: AppTheme.textColor, fontSize: 14),
              decoration: InputDecoration(
                labelText: 'Mês',
                labelStyle: const TextStyle(
                  color: AppTheme.textMutedColor,
                  fontSize: 14,
                ),
                floatingLabelStyle: const TextStyle(
                  color: AppTheme.accentColor,
                  fontSize: 12,
                  fontWeight: FontWeight.w500,
                ),
                prefixIcon: const Icon(
                  Icons.calendar_month_outlined,
                  color: AppTheme.accentColor,
                  size: 20,
                ),
                filled: true,
                fillColor: AppTheme.surfaceColor,
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                  borderSide:
                      const BorderSide(color: AppTheme.borderColor, width: 0.5),
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(10),
                  borderSide:
                      const BorderSide(color: AppTheme.accentColor, width: 1),
                ),
                contentPadding:
                    const EdgeInsets.symmetric(vertical: 16, horizontal: 16),
              ),
              items: List.generate(
                12,
                (i) => DropdownMenuItem<int>(
                  value: i + 1,
                  child: Text(
                    _meses[i],
                    style: const TextStyle(
                        color: AppTheme.textColor, fontSize: 14),
                  ),
                ),
              ),
              onChanged: _onMesChanged,
            ),
          ),

          // ── Lista de aniversariantes ────────────────────────────────────
          Expanded(
            child: FutureBuilder<List<Aniversariante>>(
              future: _future,
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(
                    child: CircularProgressIndicator(
                      color: AppTheme.accentColor,
                    ),
                  );
                }

                if (snapshot.hasError) {
                  return Center(
                    child: Text(
                      "Erro: ${snapshot.error}",
                      style: const TextStyle(color: Colors.redAccent),
                    ),
                  );
                }

                final lista = snapshot.data ?? [];

                if (lista.isEmpty) {
                  return Center(
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        const Icon(
                          Icons.cake_outlined,
                          color: AppTheme.textMutedColor,
                          size: 48,
                        ),
                        const SizedBox(height: 12),
                        Text(
                          "Nenhum aniversariante em\n${_meses[_mesSelecionado - 1]}",
                          textAlign: TextAlign.center,
                          style: const TextStyle(
                            color: AppTheme.textMutedColor,
                            fontSize: 14,
                          ),
                        ),
                      ],
                    ),
                  );
                }

                return ListView.builder(
                  padding:
                      const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
                  itemCount: lista.length,
                  itemBuilder: (context, index) {
                    final aniversariante = lista[index];
                    final isHoje =
                        _isAniversarianteDoDia(aniversariante.dataNascimento);
                    final jaComemorado =
                        _jaOcorreu(aniversariante.dataNascimento);

                    return Container(
                      margin: const EdgeInsets.symmetric(vertical: 6),
                      padding: const EdgeInsets.symmetric(
                          vertical: 14, horizontal: 16),
                      decoration: BoxDecoration(
                        color: isHoje
                            ? Colors.green.withOpacity(0.15)
                            : jaComemorado
                                ? AppTheme
                                    .primaryColor // preto/fundo escuro = já passou
                                : AppTheme
                                    .surfaceColor, // cinza = ainda vai ocorrer
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(
                          color: isHoje
                              ? Colors.green
                              : jaComemorado
                                  ? AppTheme.borderColor
                                      .withOpacity(0.3) // borda mais apagada
                                  : AppTheme.borderColor,
                          width: isHoje ? 1 : 0.5,
                        ),
                      ),
                      child: Row(
                        children: [
                          // Ícone de bolo ou check
                          Icon(
                              isHoje
                                  ? Icons.cake
                                  : jaComemorado
                                      ? Icons.check_circle_outline
                                      : Icons.cake_outlined,
                              color: isHoje
                                  ? Colors.green
                                  : jaComemorado
                                      ? Colors.green
                                      : AppTheme.accentColor,
                              size: 22),
                          const SizedBox(width: 14),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  aniversariante.nome,
                                  style: TextStyle(
                                    fontSize: 15,
                                    fontWeight: FontWeight.w500,
                                    color: jaComemorado
                                        ? AppTheme.textMutedColor
                                        : AppTheme.textColor,
                                  ),
                                ),
                                const SizedBox(height: 3),
                                Text(
                                  aniversariante.dataNascimento != null
                                      ? DateFormat('dd/MM').format(
                                          aniversariante.dataNascimento!)
                                      : 'Data não informada',
                                  style: const TextStyle(
                                    fontSize: 12,
                                    color: AppTheme.textMutedColor,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          // Badge "Hoje"
                          if (isHoje)
                            Container(
                              padding: const EdgeInsets.symmetric(
                                  horizontal: 8, vertical: 3),
                              decoration: BoxDecoration(
                                color: Colors.green.withOpacity(0.2),
                                borderRadius: BorderRadius.circular(6),
                                border:
                                    Border.all(color: Colors.green, width: 0.5),
                              ),
                              child: const Text(
                                'Hoje',
                                style: TextStyle(
                                  color: Colors.green,
                                  fontSize: 11,
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            ),
                        ],
                      ),
                    );
                  },
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}
