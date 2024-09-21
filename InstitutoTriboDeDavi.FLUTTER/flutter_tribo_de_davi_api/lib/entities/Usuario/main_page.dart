import 'package:flutter/material.dart';
import 'package:flutter_tribo_de_davi_api/api/api_handler.dart';
import 'package:flutter_tribo_de_davi_api/entities/Usuario/add_page.dart';
import 'package:flutter_tribo_de_davi_api/entities/Usuario/edit_page.dart';
import 'package:flutter_tribo_de_davi_api/entities/Usuario/find_user.dart';
import 'package:flutter_tribo_de_davi_api/models/model.dart';

class MainPage extends StatefulWidget {
  const MainPage({super.key});

  @override
  State<MainPage> createState() => _MainPageState();
}

class _MainPageState extends State<MainPage> {
  ApiHandler apiHandler = ApiHandler();
  late List<Usuario> data = [];

  void getData() async {
    data = await apiHandler.getUsuarioData();
    setState(() {});
  }

  void deleteUsuario(int id) async {
    await apiHandler.deleteUsuario(id: id);
    setState(() {});
  }

  @override
  void initState() {
    getData();
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Instituto Tribo de Davi API"),
        centerTitle: true,
        backgroundColor: Colors.black87,
        foregroundColor: Colors.white,
        elevation: 4, // Adicionando sombra na AppBar para profundidade
        shadowColor: Colors.black54,
      ),
      bottomNavigationBar: MaterialButton(
        color: Colors.teal,
        textColor: Colors.yellow,
        padding: const EdgeInsets.all(20),
        onPressed: getData,
        child: const Text('Refresh'),
      ),
      floatingActionButton: Column(
        mainAxisAlignment: MainAxisAlignment.end,
        children: [
          FloatingActionButton(
            heroTag: 1,
            backgroundColor: Colors.teal,
            foregroundColor: Colors.white,
            onPressed: () {
              Navigator.push(context,
                  MaterialPageRoute(builder: (context) => const FindUsuario()));
            },
            child: const Icon(Icons.search),
          ),
          const SizedBox(
            height: 10,
          ),
          FloatingActionButton(
            heroTag: 2,
            backgroundColor: Colors.teal,
            foregroundColor: Colors.white,
            onPressed: () {
              Navigator.push(context,
                  MaterialPageRoute(builder: (context) => const AddUsuario()));
            },
            child: const Icon(Icons.add),
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(
            8.0), // Adicionando espaçamento ao redor da lista
        child: Column(
          children: [
            Expanded(
              // Isso garante que a ListView ocupe o espaço disponível
              child: data.isEmpty
                  ? Center(
                      child: CircularProgressIndicator(
                        color: Colors.teal, // Indicador de carregamento
                      ),
                    )
                  : ListView.builder(
                      itemCount: data.length,
                      itemBuilder: (BuildContext context, int index) {
                        return Card(
                          elevation:
                              5, // Aumentando a sombra para destacar o item
                          margin: const EdgeInsets.symmetric(
                              vertical: 8), // Espaçamento entre os itens
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(
                                15), // Bordas arredondadas
                          ),
                          child: ListTile(
                            onTap: () {
                              Navigator.push(
                                  context,
                                  MaterialPageRoute(
                                      builder: (context) =>
                                          EditPage(usuario: data[index])));
                            },
                            leading: CircleAvatar(
                              backgroundColor:
                                  Colors.teal, // Cor de fundo do avatar
                              child: Text(
                                "${data[index].id}",
                                style: const TextStyle(
                                    color:
                                        Colors.white), // Texto branco no avatar
                              ),
                            ),
                            title: Text(
                              data[index].email,
                              style: const TextStyle(
                                fontWeight: FontWeight.bold, // Texto em negrito
                                fontSize: 16, // Aumentando o tamanho da fonte
                              ),
                            ),
                            subtitle: Text(
                              data[index].login,
                              style: TextStyle(
                                  color: Colors
                                      .grey[600]), // Mudando a cor do subtítulo
                            ),
                            trailing: IconButton(
                              icon: const Icon(Icons.delete_outline),
                              onPressed: () {
                                deleteUsuario(data[index].id);
                              },
                            ), // Ícone ao final do item
                          ),
                        );
                      },
                    ),
            ),
          ],
        ),
      ),
    );
  }

  //@override
  // Widget build(BuildContext context) {
  //   return Scaffold(
  //     appBar: AppBar(
  //       title: const Text("Instituto Tribo de Davi API"),
  //       centerTitle: true,
  //       backgroundColor: Colors.black87,
  //       foregroundColor: Colors.white,
  //     ),
  //     bottomNavigationBar: MaterialButton(
  //       color: Colors.teal,
  //       textColor: Colors.yellow,
  //       padding: const EdgeInsets.all(20),
  //       onPressed: getData,
  //       child: const Text('Refresh'),
  //     ),
  //     floatingActionButton: Column(
  //       mainAxisAlignment: MainAxisAlignment.end,
  //       children: [
  //         FloatingActionButton(
  //           heroTag: 1,
  //           backgroundColor: Colors.teal,
  //           foregroundColor: Colors.white,
  //           onPressed: () {
  //             Navigator.push(context,
  //                 MaterialPageRoute(builder: (context) => const FindUsuario()));
  //           },
  //           child: const Icon(Icons.search),
  //         ),
  //         const SizedBox(
  //           height: 10,
  //         ),
  //         FloatingActionButton(
  //           heroTag: 2,
  //           backgroundColor: Colors.teal,
  //           foregroundColor: Colors.white,
  //           onPressed: () {
  //             Navigator.push(
  //                 context,
  //                 MaterialPageRoute(
  //                   builder: (context) => const AddUsuario(),
  //                 ));
  //           },
  //           child: const Icon(Icons.add),
  //         ),
  //       ],
  //     ),
  //     body: Padding(
  //       padding: const EdgeInsets.all(
  //           8.0), // Adicionando espaçamento ao redor da lista
  //       child: Column(
  //         children: [
  //           Expanded(
  //             // Isso garante que a ListView ocupe o espaço disponível
  //             child: ListView.builder(
  //               itemCount: data.length,
  //               itemBuilder: (BuildContext context, int index) {
  //                 return Card(
  //                   elevation: 3, // Sombra para destacar o item
  //                   margin: const EdgeInsets.symmetric(
  //                       vertical: 8), // Espaçamento entre os itens
  //                   shape: RoundedRectangleBorder(
  //                     borderRadius:
  //                         BorderRadius.circular(10), // Bordas arredondadas
  //                   ),
  //                   child: ListTile(
  //                     onTap: () {
  //                       Navigator.push(
  //                           context,
  //                           MaterialPageRoute(
  //                               builder: (context) =>
  //                                   EditPage(usuario: data[index])));
  //                     },
  //                     leading: CircleAvatar(
  //                       backgroundColor: Colors.teal, // Cor de fundo do avatar
  //                       child: Text(
  //                         "${data[index].id}",
  //                         style: const TextStyle(
  //                             color: Colors.white), // Texto branco no avatar
  //                       ),
  //                     ),
  //                     title: Text(
  //                       data[index].email,
  //                       style: const TextStyle(
  //                         fontWeight: FontWeight.bold, // Texto em negrito
  //                         fontSize: 16, // Aumentando o tamanho da fonte
  //                       ),
  //                     ),
  //                     subtitle: Text(
  //                       data[index].login,
  //                       style: TextStyle(
  //                           color:
  //                               Colors.grey[600]), // Mudando a cor do subtítulo
  //                     ),
  //                     trailing: IconButton(
  //                       icon: const Icon(Icons.delete_outline),
  //                       onPressed: () {
  //                         deleteUsuario(data[index].id);
  //                       },
  //                     ), // Ícone ao final do item
  //                   ),
  //                 );
  //               },
  //             ),
  //           ),
  //         ],
  //       ),
  //     ),
  //   );
  // }
}

// @override
  // Widget build(BuildContext context) {
  //   return Scaffold(
  //     appBar: AppBar(
  //       title: const Text("Instituto Tribo de Davi API"),
  //       centerTitle: true,
  //       backgroundColor: Colors.black87,
  //       foregroundColor: Colors.white,             
  //     ),
  //     bottomNavigationBar: MaterialButton(
  //       color: Colors.teal,
  //       textColor: Colors.yellow,
  //       padding: const EdgeInsets.all(20),
  //       onPressed: getData,
  //       child: const Text('Refresh'),
  //       ),
  //     body: Column(children: [
  //       ListView.builder(
  //         shrinkWrap: true,
  //         itemCount: data.length,
  //         itemBuilder: (BuildContext context, int index){
  //           return ListTile(
  //             leading: Text("${data[index].id}"),
  //             title: Text(data[index].email),
  //             subtitle: Text(data[index].login),
  //           );
  //         },
  //       )
  //     ],),
  //   );
  // }