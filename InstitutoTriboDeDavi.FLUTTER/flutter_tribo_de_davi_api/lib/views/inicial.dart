import 'package:flutter_tribo_de_davi_api/widgets/action_card.dart';
import 'dart:math';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_tribo_de_davi_api/api/auth_service.dart';
import 'package:flutter_tribo_de_davi_api/business/consultas/consulta_informacoes.dart';
import 'package:flutter_tribo_de_davi_api/entities/aluno/aluno_pendentes.dart';
import 'package:flutter_tribo_de_davi_api/entities/biblia/biblia_find.dart';
import 'package:flutter_tribo_de_davi_api/entities/aula/aula_main.dart';
import 'package:flutter_tribo_de_davi_api/business/info_main.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/views/cadastros.dart';

class HomePage extends StatefulWidget {
  final String userName;

  const HomePage({super.key, required this.userName});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  Bible? _bible;
  String _selectedVerse = "";
  bool _isAdmin = false;

  @override
  void initState() {
    super.initState();
    _loadBibleXml();
    _loadPoloName();
    _loadRole();
  }

  String _poloName = 'Carregando polo...';

  // A role vem do token; usada só para esconder áreas que a API
  // bloquearia de qualquer forma (a defesa real fica no servidor)
  Future<void> _loadRole() async {
    final isAdmin = await AuthService.isAdministrador();
    if (!mounted) return;
    setState(() => _isAdmin = isAdmin);
  }

  Future<void> _loadPoloName() async {
    final poloName = await AuthService.getPoloName();
    if (!mounted) return;
    setState(() {
      _poloName = poloName ?? 'Polo não encontrado';
    });
  }

  Future<void> _loadBibleXml() async {
    try {
      String xmlString =
          await rootBundle.loadString('lib/assets/biblia/nvi.min.xml');
      Bible bible = Bible.fromXml(xmlString);

      setState(() {
        _bible = bible;
        _drawRandomVerse();
      });
    } catch (error) {
      debugPrint('Erro ao carregar a Bíblia: $error');
    }
  }

  void _drawRandomVerse() {
    if (_bible == null) return;

    final book = _bible!.getRandomBook();
    final chapter = book.getRandomChapter();

    // Capítulos curtos (ex.: Salmo 117 tem 2 versículos) mostram o que houver
    final total = chapter.verses.length;
    if (total == 0) return;

    final quantidade = total >= 3 ? 3 : total;
    final maxInicio = total - quantidade;
    final inicio = maxInicio > 0 ? Random().nextInt(maxInicio + 1) : 0;
    final versos = chapter.verses.sublist(inicio, inicio + quantidade);

    setState(() {
      _selectedVerse =
          '${book.name} - Cap.: ${chapter.number} - Vers.: ${versos.first.number} : ${versos.last.number}\n'
          '\n'
          '${versos.map((v) => '${v.number} - ${v.text}').join('\n')}';
    });
  }

  // Apaga o token e volta ao login limpando a pilha de navegação
  void _logout() {
    AuthService.logout();
  }

  void _confirmLogout() {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          backgroundColor: AppTheme.surfaceColor,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
            side: const BorderSide(color: AppTheme.borderColor, width: 0.5),
          ),
          title: const Text(
            'Confirmar Saída',
            style: TextStyle(
              color: AppTheme.textColor,
              fontSize: 16,
              fontWeight: FontWeight.w500,
            ),
          ),
          content: const Text(
            'Tem certeza de que deseja sair?',
            style: TextStyle(
              color: AppTheme.textMutedColor,
              fontSize: 14,
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text(
                'Cancelar',
                style: TextStyle(
                  color: AppTheme.textMutedColor,
                  fontSize: 14,
                ),
              ),
            ),
            ElevatedButton(
              onPressed: () {
                Navigator.of(context).pop();
                _logout();
              },
              style: AppTheme.elevatedButtonStyle,
              child: const Text('SAIR', style: AppTheme.buttonTextStyle),
            ),
          ],
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            const Text(
              "Instituto Tribo de Davi",
              style: TextStyle(
                color: AppTheme.textColor,
                fontSize: 16,
                fontWeight: FontWeight.w500,
              ),
            ),
            Text(
              _poloName,
              style: const TextStyle(
                color: AppTheme.accentColor,
                fontSize: 13,
                fontWeight: FontWeight.w400,
              ),
            ),
          ],
        ),
        centerTitle: true,
        backgroundColor: AppTheme.surfaceColor,
        foregroundColor: AppTheme.textColor,
        elevation: 0,
        toolbarHeight: kToolbarHeight,
        actions: [
          IconButton(
            icon: const Icon(Icons.exit_to_app, color: AppTheme.accentColor),
            onPressed: _confirmLogout,
            tooltip: 'Sair',
          ),
        ],
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(1),
          child: Container(color: AppTheme.borderColor, height: 0.5),
        ),
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                padding: const EdgeInsets.all(16.0),
                margin: const EdgeInsets.only(bottom: 20.0),
                decoration: AppTheme.cardDecoration,
                child: Row(
                  children: [
                    ClipOval(
                      child: Image.asset(
                        'lib/assets/images/logo.png',
                        width: 60,
                        height: 60,
                        fit: BoxFit.cover,
                      ),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Text(
                        'Bem-vindo(a), ${widget.userName}!',
                        style:
                            const TextStyle(color: AppTheme.textColor, fontSize: 24),
                      ),
                    ),
                    //),
                  ],
                ),
              ),
              Container(
                padding: const EdgeInsets.all(16.0),
                margin: const EdgeInsets.only(bottom: 20.0),
                decoration: AppTheme.cardDecoration,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Medite na Palavra',
                      style:
                          TextStyle(color: AppTheme.accentColor, fontSize: 24),
                    ),
                    const SizedBox(height: 10),
                    Text(
                      _selectedVerse,
                      style: const TextStyle(color: AppTheme.textColor, fontSize: 14),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 20),
              ActionCard(
                title: "Alunos Novos",
                icon: Icons.person_add_sharp,
                color: AppTheme.primaryColor,
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                        builder: (context) => const AlunoPendentesPage()),
                  );
                },
              ),
              const SizedBox(height: 20),
              Column(
                children: [
                  ActionCard(
                    title: "Aulas",
                    icon: Icons.diversity_1_sharp,
                    color: AppTheme.primaryColor,
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                            builder: (context) => const AulaPage()),
                      );
                    },
                  ),
                  const SizedBox(height: 20),
                  ActionCard(
                    title: "Consultas",
                    icon: Icons.manage_search_sharp,
                    color: AppTheme.primaryColor,
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                            builder: (context) => const InfoPage()),
                      );
                    },
                  ),
                  const SizedBox(height: 20),
                  // Gerenciamento de cadastros é área de administrador
                  if (_isAdmin) ...[
                    ActionCard(
                      title: "Cadastros",
                      icon: Icons.portrait_sharp,
                      color: AppTheme.primaryColor,
                      onPressed: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                              builder: (context) => const CadastrosPage()),
                        );
                      },
                    ),
                    const SizedBox(height: 20),
                  ],
                  ActionCard(
                    title: "Informações",
                    icon: Icons.info_outline_rounded,
                    color: AppTheme.primaryColor,
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                            builder: (context) => const DashboardPage()),
                      );
                    },
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

}
