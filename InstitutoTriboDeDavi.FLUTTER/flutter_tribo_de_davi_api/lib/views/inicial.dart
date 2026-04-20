import 'dart:math';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/business/consultas/consulta_informacoes.dart';
import 'package:flutter_tribo_de_davi_api/entities/biblia/biblia_find.dart';
import 'package:flutter_tribo_de_davi_api/entities/aula/aula_main.dart';
import 'package:flutter_tribo_de_davi_api/business/info_main.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/views/cadastros.dart';
import 'package:flutter_tribo_de_davi_api/views/login.dart';

class HomePage extends StatefulWidget {
  final String userName;

  const HomePage({super.key, required this.userName});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  Bible? _bible;
  String _selectedVerse = "";

  @override
  void initState() {
    super.initState();
    _loadBibleXml();
    _loadPoloName();
  }

  String _poloName = 'Carregando polo...';

  Future<void> _loadPoloName() async {
    final poloName = await ApiHandler.getPoloName();
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
      print('Erro ao carregar a Bíblia: $error');
    }
  }

  void _drawRandomVerse() {
    if (_bible != null) {
      final book = _bible!.getRandomBook();
      final chapter = book.getRandomChapter();

      final randomIndex = Random().nextInt(chapter.verses.length - 2);

      final verse1 = chapter.verses[randomIndex];
      final verse2 = chapter.verses[randomIndex + 1];
      final verse3 = chapter.verses[randomIndex + 2];

      setState(() {
        _selectedVerse =
            '${book.name} - Cap.: ${chapter.number} - Vers.: ${verse1.number} : ${verse3.number}\n'
            '\n'
            '${verse1.number} - ${verse1.text}\n'
            '${verse2.number} - ${verse2.text}\n'
            '${verse3.number} - ${verse3.text}';
      });
    }
  }

  void _logout() {
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(builder: (context) => const LoginPage()),
    );
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
                            TextStyle(color: AppTheme.textColor, fontSize: 24),
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
                    Text(
                      'Medite na Palavra',
                      style:
                          TextStyle(color: AppTheme.accentColor, fontSize: 24),
                    ),
                    const SizedBox(height: 10),
                    Text(
                      _selectedVerse,
                      style: TextStyle(color: AppTheme.textColor, fontSize: 14),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 20),
              Column(
                children: [
                  _buildActionCard(
                    context,
                    title: "Aulas",
                    icon: Icons.check_rounded,
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
                  _buildActionCard(
                    context,
                    title: "Consultas",
                    icon: Icons.dashboard,
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
                  _buildActionCard(
                    context,
                    title: "Cadastros",
                    icon: Icons.person,
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
                  _buildActionCard(
                    context,
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

  Widget _buildActionCard(BuildContext context,
      {required String title,
      required IconData icon,
      required Color color,
      required VoidCallback onPressed}) {
    return GestureDetector(
      onTap: onPressed,
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 24, horizontal: 16),
        decoration: AppTheme.cardDecoration.copyWith(
          color: AppTheme.primaryColor,
          border: Border.all(color: AppTheme.borderColor, width: 2),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(icon, size: 40, color: AppTheme.iconColor),
            const SizedBox(width: 15),
            Text(
              title,
              style: AppTheme.bodyTextStyle.copyWith(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppTheme.textColor,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
