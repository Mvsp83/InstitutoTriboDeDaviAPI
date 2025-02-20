import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_base.dart';
import 'package:flutter_tribo_de_davi_api/api/api_routes.dart';
import 'package:flutter_tribo_de_davi_api/api/api_theme.dart';
import 'package:flutter_tribo_de_davi_api/entities/aluno/aluno_edit.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/aluno.dart';
import 'package:flutter_tribo_de_davi_api/entities/models/faixa.dart';
import 'package:intl/intl.dart';

class AlunoPage extends StatefulWidget {
  const AlunoPage({super.key});

  @override
  State<AlunoPage> createState() => _AlunoPageState();
}

class _AlunoPageState extends State<AlunoPage> {
  final ApiHandler<Aluno> apiHandler = ApiHandler<Aluno>(
    baseUri: ApiRoutes.entity("aluno"),
    fromJson: (json) => Aluno.fromJson(json),
  );
  List<Aluno> data = [];
  List<Aluno> filteredData = [];
  bool isLoading = false;
  String searchQuery = '';

  @override
  void initState() {
    super.initState();
    getAlunos();
  }

  Future<void> getAlunos() async {
    setState(() => isLoading = true);
    try {
      final alunos = await apiHandler.getData();
      alunos.sort((a, b) => a.nome!.compareTo(b.nome!));
      setState(() {
        data = alunos;
        filteredData = List.from(alunos);
        isLoading = false;
      });
    } catch (e) {
      setState(() => isLoading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Erro ao carregar alunos: $e')),
      );
    }
  }

  void _filterAlunos(String query) {
    setState(() {
      searchQuery = query;
      if (query.isEmpty) {
        filteredData = List.from(data);
      } else {
        filteredData = data.where((aluno) {
          final faixaNome =
              aluno.faixa != null ? Faixa.fromId(aluno.faixa).descricao : '';
          return aluno.nome!.toLowerCase().contains(query.toLowerCase()) ||
              faixaNome.toLowerCase().contains(query.toLowerCase());
        }).toList();
      }
    });
  }

  Widget _buildAlunoCard(Aluno aluno) {
    return GestureDetector(
      onTap: () async {
        final alunoAtualizado = await Navigator.push<Aluno>(
          context,
          MaterialPageRoute(
            builder: (context) => AlunoUpdatePage(aluno: aluno),
          ),
        );

        if (alunoAtualizado != null) {
          setState(() {
            final index = data.indexWhere((a) => a.id == alunoAtualizado.id);
            if (index != -1) {
              data[index] = alunoAtualizado;
              data.sort((a, b) => a.nome!.compareTo(b.nome!));
              _filterAlunos(searchQuery);
            }
          });
        }
      },
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 8),
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: AppTheme.primaryColor,
          borderRadius: BorderRadius.circular(15),
          border: Border.all(color: AppTheme.borderColor, width: 2),
          boxShadow: [
            BoxShadow(
              color: AppTheme.primaryColor.withOpacity(0.4),
              spreadRadius: 1,
              blurRadius: 8,
              offset: const Offset(2, 4),
            ),
          ],
        ),
        child: Row(
          children: [
            Container(
              width: 70,
              height: 70,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                image: DecorationImage(
                  image: AssetImage(
                    aluno.faixa != null
                        ? Faixa.fromId(aluno.faixa).imagem
                        : 'lib/assets/images/faixa_default.png',
                  ),
                  fit: BoxFit.fill,
                ),
              ),
            ),
            const SizedBox(width: 15),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    aluno.nome!,
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 18,
                      color: AppTheme.textColor,
                    ),
                  ),
                  Text(
                    'Data de Nasc: ${DateFormat('dd/MM/yyyy').format(aluno.dataNascimento!)}',
                    style: const TextStyle(
                      fontSize: 14,
                      color: AppTheme.textColor,
                    ),
                  ),
                  Text(
                    'Responsável: ${aluno.responsavel}',
                    style: const TextStyle(
                      fontSize: 14,
                      color: AppTheme.textColor,
                    ),
                  ),
                  Text(
                    'Fone: ${aluno.celular}',
                    style: const TextStyle(
                      fontSize: 14,
                      color: AppTheme.textColor,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.backgroundColor,
      appBar: AppBar(
        title: const Text("Gestão de Alunos"),
        centerTitle: true,
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: AppTheme.textColor,
        elevation: 4,
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: TextField(
              decoration: InputDecoration(
                hintText: "Buscar por nome ou faixa...",
                prefixIcon: const Icon(Icons.search, color: AppTheme.iconColor),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
                focusedBorder: OutlineInputBorder(
                  borderSide: const BorderSide(color: AppTheme.primaryColor),
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              onChanged: _filterAlunos,
            ),
          ),
          Expanded(
            child: isLoading
                ? const Center(
                    child: CircularProgressIndicator(color: Colors.teal))
                : filteredData.isEmpty
                    ? const Center(
                        child: Text(
                          'Nenhum aluno encontrado.',
                          style: TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                              color: Colors.redAccent),
                        ),
                      )
                    : ListView.builder(
                        itemCount: filteredData.length,
                        itemBuilder: (context, index) {
                          return _buildAlunoCard(filteredData[index]);
                        },
                      ),
          ),
        ],
      ),
    );
  }
}
